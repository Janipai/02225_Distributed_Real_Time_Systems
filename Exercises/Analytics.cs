using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using _02225;
using _02225.Entities;

public class AnalysisTool
{
    private CompleteCase caseData;
    private Dictionary<Component, (double alpha, double delta)> compBDRInterfaces;
    private Dictionary<Component, (double theta, double pi, double delta)> compEDPInterfaces;
    private Dictionary<Component, (double theta, double pi)> compPRMInterfaces;
    private ResourceModels resourceModels;

    public AnalysisTool(CompleteCase caseData)
    {
        this.caseData = caseData;
        this.compBDRInterfaces = new Dictionary<Component, (double alpha, double delta)>();
        this.compEDPInterfaces = new Dictionary<Component, (double theta, double pi, double delta)>();
        this.compPRMInterfaces = new Dictionary<Component, (double theta, double pi)>();
        this.resourceModels = new ResourceModels();
    }

    private void ComputeAllInterfaces(Component comp, string model)
    {
            // Get all tasks belonging to this component
            List<ProjectTask> tasks = GetTasksForComponent(comp);
            
            switch (model?.ToUpper())  // Add null check and case insensitivity
            {
                case "PRM":
                    var (t, p) = resourceModels.ComputePRMInterface(tasks);
                    compPRMInterfaces[comp] = (t, p);
                    Console.WriteLine($"Component {comp.ComponentId}: PRM (Θ={t:F2}, Π={p:F2})");
                    
                    // Check each core for schedulability using its components' interfaces
                    CoreSchedulabilityPRM(compPRMInterfaces);
                    break;
        
                case "EDF":  // Standardize to EDF instead of EDP
                    var (theta, pi, d) = resourceModels.ComputeEDPInterface(tasks);
                    compEDPInterfaces[comp] = (theta, pi, d);
                    Console.WriteLine($"Component {comp.ComponentId}: EDP (Θ={theta:F2}, Π={pi:F2}, Δ={d:F2})");
                    
                    // Check each core for schedulability using its components' interfaces
                    CoreSchedulabilityEDF(compEDPInterfaces);
                    break;
        
                default: // BDR
                    var (a, b) = resourceModels.ComputeBDRInterface(tasks);
                    compBDRInterfaces[comp] = (a, b);
                    Console.WriteLine($"Component {comp.ComponentId}: BDR (α={a:F2}, Δ={b:F2})");
                    
                    // Check each core for schedulability using its components' interfaces
                    CoreSchedulabilityBDR(compBDRInterfaces);
                    break;
            }
        
    }
    
    private void CoreSchedulabilityPRM(Dictionary<Component, (double theta, double pi)> compInterface)
    {
        Console.WriteLine("\nCore Schedulability (PRM Model):");

        foreach (Core core in caseData.Architecture.GetCores())
        {
            var comps = compInterface.Keys.Where(c => c.Core == core).ToList();
            if (comps.Count == 0)
            {
                Console.WriteLine($"Core {core.CoreId}: No components assigned (trivially schedulable).");
                continue;
            }

            double totalUtil = comps.Sum(c =>
            {
                var (theta, pi) = compInterface[c];
                return theta / pi;
            });

            bool schedulable = totalUtil <= 1.0 + 1e-9;
            Console.WriteLine($"Core {core.CoreId}: {(schedulable ? "Schedulable" : "Not Schedulable")} (total Θ/Π = {totalUtil:F2})");
        }
    }
    
    private void CoreSchedulabilityEDF(Dictionary<Component, (double theta, double pi, double delta)> compEDFInterfaces)
    {
        Console.WriteLine("\nCore Schedulability (EDF Model):");
        
        foreach (Core core in caseData.Architecture.GetCores())
        {
            // Get all EDF components on this core
            var componentsOnCore = compEDFInterfaces.Keys
                .Where(comp => comp.Core == core)
                .ToList();

            if (componentsOnCore.Count == 0)
            {
                Console.WriteLine($"Core {core.CoreId}: No EDF components assigned");
                continue;
            }

            // For EDF, we check total utilization (Θ/Π for each component)
            double totalUtilization = componentsOnCore.Sum(comp => 
            {
                var (theta, pi, _) = compEDFInterfaces[comp];
                return theta / pi;
            });

            bool schedulable = totalUtilization <= 1.0 + 1e-9;

            Console.WriteLine(
                $"Core {core.CoreId}: " +
                (schedulable ? "✓ Schedulable" : "✗ Not Schedulable") +
                $" (Total U = {totalUtilization:F2})"
            );
        }

        // EDF-specific WCRT analysis
        ComputeWcrtPerTaskEDF(compEDFInterfaces);
    }

    private void CoreSchedulabilityBDR(Dictionary<Component, (double alpha, double delta)> compInterface)
    {
        Console.WriteLine(); // blank line
        Console.WriteLine("Core Schedulability:");
        
        // Check each core for schedulability using its components' interfaces
        foreach (Core core in caseData.Architecture.GetCores())
        {
            // Find all components assigned to this core
            var componentsOnCore = compInterface.Keys
                .Where(comp => comp.Core == core)
                .ToList();
            if (componentsOnCore.Count == 0)
            {
                Console.WriteLine(
                    $"Core {core.CoreId} (\"{core.CoreId}\"): No components assigned (trivially schedulable).");
                continue;
            }

            // Sum the availability factors of all components on this core
            double totalAlpha = componentsOnCore.Sum(comp => compInterface[comp].alpha);
            bool schedulable = totalAlpha <= 1.0 + 1e-9; // allow tiny numerical tolerance

            // (If needed, further demand/supply analysis per core could be done here. 
            // For EDF scheduling on the core, total utilization <= 1 is a necessary and 
            // sufficient condition for schedulability&#8203;:contentReference[oaicite:8]{index=8} given the interfaces were computed correctly.)

            Console.WriteLine(
                $"Core {core.CoreId} (\"{core.CoreId}\"): " +
                (schedulable ? "Schedulable" : "Not Schedulable") +
                $" (total α = {totalAlpha:F2})"
            );
        }

        // per-task analysis Worst-Case Response Time (WCRT) analysis
        ComputeWcrtPerTask(compInterface);
    }
    
    // Helper: Compute WCRT for each task in the system
    private void ComputeWcrtPerTask(Dictionary<Component, (double alpha, double delta)> compIfs)
    {
        Console.WriteLine("\n Worst-Case Response Times (WCRT) per task:");
        foreach (Component comp in caseData.Budget.GetComponents())
        {
            List<ProjectTask> tasks = GetTasksForComponent(comp);
            if (tasks.Count == 0)
            {
                Console.WriteLine($"Component {comp.ComponentId}: No tasks to analyze.");
                continue;
            }
            
            if (!compIfs.ContainsKey(comp))
                Console.WriteLine($"Missing interface for component: {comp.ComponentId}, on core: {comp.GetCore().CoreId}");

            var (alpha, delta) = compIfs[comp];
            Console.WriteLine($"\nComponent {comp.ComponentId}  (α={alpha:F2}, Δ={delta:F2})  [{comp.Scheduler}]");

            // Order tasks for deterministic output
            tasks = comp.Scheduler.Equals("RM", StringComparison.OrdinalIgnoreCase)
                        ? tasks.OrderBy(t => t.GetPriority()).ToList()
                        : tasks.OrderBy(t => t.Period).ToList();
            
            foreach (var t in tasks)
            {
                // Compute WCRT for this task
                double wcrt = 0.0;
                if (comp.Scheduler.Equals("RM", StringComparison.OrdinalIgnoreCase))
                {
                    // RM analysis
                    wcrt = ComputeWcrtRm(t, tasks, alpha, delta);
                }
                else
                {
                    // EDF analysis
                    wcrt = ComputeWcrtEdf(t, tasks, alpha, delta);
                }
                
                // Store the WCRT in the task object
                t.Wcrt = wcrt;
                
                // Print the result
                string status = (wcrt > t.Period) ? "MISS deadline!" : "meets deadline";
                Console.WriteLine($"Task {t.TaskName}: WCRT = {wcrt:F2}, Deadline = {t.Period}, " +
                                  $"{status} (α={alpha:F2}, Δ={delta:F2})");
            }
        }
    }

    // ---------- EDF WCRT (Guan & Yi style, scaled by α,Δ) -------------------
    private double ComputeWcrtEdf(ProjectTask task, List<ProjectTask> all, double alpha, double delta)
    {
        // Identify tasks with equal or shorter deadlines (EDF interference set)
        var interferers = all.Where(t => t != task && t.Period <= task.Period).ToList();
        double Rprev = -1, R = task.Wcet;
        for (int iter = 0; iter < 1000 && Math.Abs(R - Rprev) > 1e-6; iter++)
        {
            Rprev = R;
            double demand = task.Wcet +
                            interferers.Sum(t => Math.Ceiling(R / t.Period) * t.Wcet);
            R = delta + demand / alpha;
            if (R > task.Period) break;  // miss
        }
        return R;
    }

    // ---------- Rate‑Monotonic WCRT (scaled by α,Δ) -------------------------
    private double ComputeWcrtRm(ProjectTask task, List<ProjectTask> all, double alpha, double delta)
    {
        var hp = all.Where(t => t.GetPriority() < task.GetPriority()).ToList();
        double Rprev = -1, R = task.Wcet;
        for (int iter = 0; iter < 1000 && Math.Abs(R - Rprev) > 1e-6; iter++)
        {
            Rprev = R;
            double demand = task.Wcet +
                            hp.Sum(t => Math.Ceiling(R / t.Period) * t.Wcet);
            R = delta + demand / alpha;
            if (R > task.Period) break;
        }
        return R;
    }
    // Helper: retrieve all tasks that belong to a given component
    private List<ProjectTask> GetTasksForComponent(Component comp)
    {
        // Assuming each ProjectTask has a property ComponentId linking it to its component
        return caseData.TaskList.GetTasks()
            .Where(task => task.ComponentId == comp)
            .ToList();
    }

    private void ComputeWcrtPerTaskEDF(Dictionary<Component, (double theta, double pi, double delta)> compEDFInterfaces)
    {
        Console.WriteLine("\nEDF Worst-Case Response Times:");
        
        foreach (var comp in compEDFInterfaces.Keys)
        {
            var tasks = GetTasksForComponent(comp);
            if (tasks.Count == 0) continue;

            var (theta, pi, delta) = compEDFInterfaces[comp];
            Console.WriteLine($"\nComponent {comp.ComponentId} (Θ={theta:F2}, Π={pi:F2}, Δ={delta:F2})");

            foreach (var task in tasks.OrderBy(t => t.Period)) // EDF orders by deadline (period)
            {
                double wcrt = ComputeWcrtEdf(task, tasks, theta, pi, delta);
                task.Wcrt = wcrt;
                
                string status = wcrt <= task.Period ? "✓ Meets deadline" : "✗ Misses deadline";
                Console.WriteLine($"Task {task.TaskName}: WCRT = {wcrt:F2}, Period = {task.Period}, {status}");
            }
        }
    }

    private double ComputeWcrtEdf(ProjectTask task, List<ProjectTask> tasks, double theta, double pi, double delta)
    {
        // EDF interference set - tasks with shorter or equal periods
        var interferers = tasks.Where(t => t != task && t.Period <= task.Period).ToList();
        
        double R = task.Wcet;
        for (int i = 0; i < 100; i++) // Max iterations
        {
            double demand = task.Wcet + interferers.Sum(t => Math.Ceiling(R / t.Period) * t.Wcet);
            
            // Account for EDP resource supply: supply(t) = theta*floor((t-delta)/pi)
            double newR = delta + (demand / theta) * pi;
            
            if (Math.Abs(newR - R) < 1e-6) break;
            R = newR;
            
            if (R > task.Period * 2) break; // Safety check
        }
        
        return R;
    }
    
    private void CompareResourceUtilization()
    {
        Console.WriteLine("\n--- Resource Utilization Summary ---");

        foreach (Core core in caseData.Architecture.GetCores())
        {
            double bdrUtil = compBDRInterfaces
                .Where(kvp => kvp.Key.Core == core)
                .Sum(kvp => kvp.Value.alpha);

            double edpUtil = compEDPInterfaces
                .Where(kvp => kvp.Key.Core == core)
                .Sum(kvp => kvp.Value.theta / kvp.Value.pi);

            double prmUtil = compPRMInterfaces
                .Where(kvp => kvp.Key.Core == core)
                .Sum(kvp => kvp.Value.theta / kvp.Value.pi);

            Console.WriteLine($"Core {core.CoreId} → BDR: {bdrUtil:F2}, EDP: {edpUtil:F2}, PRM: {prmUtil:F2}");
        }
    }

    // Entry point to run the analysis and print results
    public void RunAnalysis()
    {
        Console.WriteLine("Component interfaces: ");
        foreach (Component comp in caseData.Budget.GetComponents())
        {
            string model = comp.GetScheduler();
            ComputeAllInterfaces(comp, model);
        }

        CompareResourceUtilization();
    }
}