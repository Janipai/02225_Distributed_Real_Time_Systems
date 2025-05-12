using System;
using System.Collections.Generic;
using System.Linq;
using _02225.Entities;
public class AnalysisTool
{
    private CompleteCase caseData;

    public AnalysisTool(CompleteCase caseData)
    {
        this.caseData = caseData;
    }

    // Entry point to run the analysis and print results
    public void RunAnalysis()
    {
        // Store computed interfaces for each component (to use for core analysis)
        var compInterfaces = new Dictionary<Component, (double alpha, double delta)>();

        Console.WriteLine("Component BDR Interfaces:");
        foreach (Component comp in caseData.Budget.GetComponents())
        {
            // Get all tasks belonging to this component
            List<ProjectTask> tasks = GetTasksForComponent(comp);
            if (tasks.Count == 0)
            {
                // No tasks in this component
                compInterfaces[comp] = (0.0, 0.0);
                Console.WriteLine($"Component {comp.ComponentId}: (α, Δ) = (0, 0)");
                continue;
            }

            // Determine scheduling policy for this component (EDF or RM)
            bool useEDF = true;
            if (comp.Scheduler != null)
            {
                // Assume SchedulingPolicy is a string like "EDF" or "RM"
                useEDF = comp.Scheduler.ToUpper() == "EDF";
            }
            // (If no policy is explicitly set, default to EDF as it demands the most resource.)

            // Compute the minimal interface (α, Δ) for this component
            var (alpha, delta) = ComputeComponentInterface(tasks, useEDF);
            compInterfaces[comp] = (alpha, delta);

            // Print the component’s interface, formatted to two decimal places
            Console.WriteLine($"Component {comp.ComponentId}: (α, Δ) = ({alpha:F2}, {delta:F2})");
            
            // ---- PRM CHECK: assuming a fixed period Π for PRM ----
            double prmPeriod = tasks.Min(t => t.Period);  // conservative
            double prmBudget = alpha * prmPeriod;

            bool prmSchedulable = IsSchedulableWithPRM(tasks, prmPeriod, prmBudget);
            Console.WriteLine($"  PRM (Π={prmPeriod}, Θ={prmBudget:F2}): " +
                              (prmSchedulable ? "Schedulable" : "Not Schedulable"));
            
            // --- EDP CHECK ---
            bool edpSchedulable = IsDemandSchedulable(tasks, 1.0, 0.0); // EDF on full processor
            Console.WriteLine($"  EDP (full EDF test): " + (edpSchedulable ? "Schedulable" : "Not Schedulable"));
        }

        Console.WriteLine(); // blank line
        Console.WriteLine("Core Schedulability:");
        // Check each core for schedulability using its components' interfaces
        foreach (Core core in caseData.Architecture.GetCores())
        {
            // Find all components assigned to this core
            var componentsOnCore = compInterfaces.Keys
                                       .Where(comp => comp.Core == core)
                                       .ToList();
            if (componentsOnCore.Count == 0)
            {
                Console.WriteLine($"Core {core.CoreId} (\"{core.CoreId}\"): No components assigned (trivially schedulable).");
                continue;
            }

            // Sum the availability factors of all components on this core
            double totalAlpha = componentsOnCore.Sum(comp => compInterfaces[comp].alpha);
            bool schedulable = totalAlpha <= 1.0 + 1e-9;  // allow tiny numerical tolerance

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
        ComputeWcrtPerTask(compInterfaces);
        
        Console.WriteLine("\nSummary Comparison (per Component):");
        Console.WriteLine("Component | BDR (α,Δ) | PRM (Θ/Φ) | EDP Sched | EDP Util");
        
        foreach (var comp in compInterfaces.Keys)
        {
            var (alpha, delta) = compInterfaces[comp];
            var tasks = GetTasksForComponent(comp);
            var (phi, theta) = ComputeComponentPRMInterface(tasks);
            bool edpSchedulable = IsDemandSchedulable(tasks, 1.0, 0.0);
            var edpUtil = CalculateUtilizationForEDPTasks(tasks);

            Console.WriteLine($"{comp.ComponentId,-10} | (α={alpha:F2}, Δ={delta:F2}) | Θ={theta:F2}/Φ={phi:F2} | {(edpSchedulable ? "Yes" : "No"),-10} | {edpUtil:F2}");
        }
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

    // Helper: Compute the demand bound function for a task set at time t
    private double ComputeDBF(List<ProjectTask> tasks, double t)
    {
        double demand = 0.0;
        foreach (ProjectTask task in tasks)
        {
            // Get task parameters
            double C = task.Wcet;               // worst-case execution time
            double T = task.Period;
            double D = task.Period;  // use period if no explicit deadline

            if (t < D)
            {
                // No job of this task has deadline <= t
                continue;
            }
            // Number of jobs with deadlines <= t = floor((t + T - D) / T)
            double count = Math.Floor((t + T - D) / T);
            if (count < 0) count = 0;
            demand += count * C;
        }
        return demand;
    }

    // Helper: Compute the minimal (α, Δ) interface for a component’s task set
    private (double alpha, double delta) ComputeComponentInterface(List<ProjectTask> tasks, bool useEDF)
    {
        // Start with the total utilization as a lower bound for α
        double totalUtil = tasks.Sum(task => task.Wcet / task.Period);
        double alphaMin = Math.Min(totalUtil, 1.0);
        if (alphaMin < 0) alphaMin = 0;  // just in case (shouldn’t happen, tasks have positive execution times)

        // If the task set is not even schedulable on a full processor, we handle that as a special case.
        // Check schedulability on a full core (α=1, Δ=0) by basic demand criterion:
        if (!IsDemandSchedulable(tasks, 1.0, 0.0))
        {
            // If tasks miss deadlines even on a full core, we cannot find a valid (α, Δ).
            // Here we simply return full resource and note a large delay as a placeholder.
            return (1.0, 0.0);
        }

        // Binary search for minimal alpha in [alphaMin, 1] that yields a feasible Δ
        double lo = alphaMin;
        double hi = 1.0;
        double bestAlpha = 1.0;
        double bestDelta = 0.0;

        // Tolerance for binary search (stop when interval is very small)
        const double eps = 1e-3;
        while (hi - lo > eps)
        {
            double mid = (lo + hi) / 2.0;
            double requiredDelta = ComputeRequiredDelay(tasks, mid);
            if (requiredDelta >= 0)
            {
                // Feasible: supply mid*CPU with delay = requiredDelta meets demand
                bestAlpha = mid;
                bestDelta = requiredDelta;
                // Try to reduce alpha further
                hi = mid;
            }
            else
            {
                // Not feasible: increase alpha
                lo = mid;
            }
        }

        // After search, bestAlpha/bestDelta holds the minimal interface found
        // We round Δ up to the next higher non-negative value if needed (it should be >=0 already if feasible).
        if (bestDelta < 0) bestDelta = 0.0;
        return (bestAlpha, bestDelta);
    }

    private (double phi, double theta) ComputeComponentPRMInterface(List<ProjectTask> tasks)
    {
        // 1. Choose a conservative resource period Φ (e.g., minimum task period)
        double phi = tasks.Min(t => t.Period);

        // 2. Set search bounds for budget Θ: from total utilization * Φ up to Φ
        double util = tasks.Sum(t => (double)t.Wcet / t.Period);
        double lo = Math.Max(0.1, util * phi);  // start just above 0, or util*phi
        double hi = phi;

        // 3. Binary search for the minimal Θ such that PRM is schedulable
        double bestTheta = hi;
        const double eps = 1e-3;

        while (hi - lo > eps)
        {
            double mid = (lo + hi) / 2.0;
            bool schedulable = IsSchedulableWithPRM(tasks, phi, mid);
            if (schedulable)
            {
                bestTheta = mid;
                hi = mid;
            }
            else
            {
                lo = mid;
            }
        }

        return (phi, bestTheta);
    }
    private double CalculateUtilizationForEDPTasks(List<ProjectTask> tasks)
    {
        double utilization = 0.0;
        
        foreach (ProjectTask task in tasks)
        {
            utilization += (double)task.Wcet/(double)task.Period; //sum(c_i/t_i)
        }

        return utilization;
    }
    
    // Helper: Determine if a task set is schedulable on a resource with given α and Δ (using demand bound test)
    private bool IsDemandSchedulable(List<ProjectTask> tasks, double alpha, double delta)
    {
        // We need to verify that for all t, SBF(α,Δ)(t) >= DBF(t).
        // SBF(α,Δ)(t) for bounded-delay resource is 0 for t < Δ, and α * (t - Δ) for t >= Δ.
        // So we must check DBF(t) <= α*(t - Δ) for all t >= Δ (and trivially satisfied for t < Δ if no demand before Δ).
        // It suffices to check at critical points (task deadlines).
        double horizon = ComputeAnalysisHorizon(tasks);
        // Check at t = Δ as well (just after supply starts)
        double startT = delta;
        if (startT < 0) startT = 0;
        // We iterate over relevant time points (all tasks' deadlines up to horizon)
        foreach (double t in GetCriticalTimes(tasks, horizon))
        {
            if (t < delta) continue; // before resource starts supplying, no supply (and ideally no demand due to no deadlines <= t)
            double demand = ComputeDBF(tasks, t);
            double supply = alpha * (t - delta);
            if (demand > supply + 1e-9)
            {
                // demand exceeds supply at time t, not schedulable
                return false;
            }
        }
        return true;
    }

    // Helper: Compute the minimum required delay Δ for a given α such that DBF <= SBF(α, Δ)
    private double ComputeRequiredDelay(List<ProjectTask> tasks, double alpha)
    {
        // If α is 0 (no resource provided), tasks can't be served (return -∞ as impossible)
        if (alpha <= 0) return double.NegativeInfinity;

        double horizon = ComputeAnalysisHorizon(tasks);
        double minOffset = double.PositiveInfinity;
        // We want Δ such that for all t: α*(t - Δ) >= DBF(t), or equivalently Δ <= t - (DBF(t)/α).
        // So we find the minimal right-hand side across all t in [0, horizon].
        foreach (double t in GetCriticalTimes(tasks, horizon))
        {
            double dbf = ComputeDBF(tasks, t);
            // Rearranged condition: Δ <= t - dbf/α
            double rhs = t - (dbf / alpha);
            if (rhs < minOffset)
            {
                minOffset = rhs;
            }
        }
        // The maximum needed Δ would be the negative of minOffset if minOffset is negative.
        // Actually, if minOffset is positive, we can choose Δ = minOffset (the smallest t - dbf/α).
        // If minOffset is negative, it means even at t = 0, demand vs supply condition fails (which usually implies α is too low).
        // Return minOffset as the feasible Δ (if minOffset < 0, it indicates infeasible).
        return minOffset;
    }

    // Helper: Determine a safe analysis horizon (time limit) to check for demand vs supply
    private double ComputeAnalysisHorizon(List<ProjectTask> tasks)
    {
        // A conservative choice is the hyperperiod (LCM of all periods) plus the maximum deadline.
        // This covers all possible release alignments&#8203;:contentReference[oaicite:9]{index=9}.
        long lcm = 1;
        long maxDeadline = 0;
        foreach (ProjectTask task in tasks)
        {
            long T = task.Period;
            lcm = Lcm(lcm, T);
            long D = task.Period;
            if (D > maxDeadline) maxDeadline = D;
        }
        long horizon = lcm + maxDeadline;
        // Limit horizon to avoid extreme values (if LCM is huge, we rely on utilization and demand pattern analysis).
        // For safety, cap at some large value if needed (e.g., 1000 * max period).
        long cap = 1000 * tasks.Max(task => (long)task.Period);
        if (horizon > cap) horizon = cap;
        return (double)horizon;
    }

    // Helper: Collect all critical time points (deadlines of jobs) up to the given horizon
    private IEnumerable<double> GetCriticalTimes(List<ProjectTask> tasks, double horizon)
    {
        var timePoints = new SortedSet<double>();
        timePoints.Add(0);
        foreach (ProjectTask task in tasks)
        {
            double T = task.Period;
            double D = task.Period;
            // Generate deadlines for jobs of this task up to horizon
            if (T <= 0) continue;
            for (double t = D; t <= horizon; t += T)
            {
                timePoints.Add(Math.Round(t, 6));  // round to avoid floating precision issues
            }
        }
        return timePoints;
    }
    
    private bool IsSchedulableWithPRM(List<ProjectTask> tasks, double Pi, double Theta)
    {
        double horizon = ComputeAnalysisHorizon(tasks);
        foreach (double t in GetCriticalTimes(tasks, horizon))
        {
            double dbf = ComputeDBF(tasks, t);
            double sbf = ComputePRMSupply(t, Pi, Theta);
            if (dbf > sbf + 1e-9) return false;
        }
        return true;
    }

    private double ComputePRMSupply(double t, double Pi, double Theta)
    {
        if (t < Pi - Theta) return 0;

        double n = Math.Floor((t - (Pi - Theta)) / Pi);
        double rem = t - (Pi - Theta) - n * Pi;

        return n * Theta + Math.Min(rem, Theta);
    }


    // Helper: Compute least common multiple of two numbers
    private long Lcm(long a, long b)
    {
        return a / Gcd(a, b) * b;
    }
    private long Gcd(long a, long b)
    {
        return b == 0 ? Math.Abs(a) : Gcd(b, a % b);
    }
}
