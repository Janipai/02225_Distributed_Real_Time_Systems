namespace _02225;

public class Program
{
    static void Main()
    {
        string part = "data/Project/1-tiny-test-case/";
        String architectureFilePath =  part + "architecture.csv";
        String budgetFilePath = part + "budgets.csv";
        String tasksFilePath = part + "tasks.csv";
        
        InputHandler IH = new InputHandler(architectureFilePath, budgetFilePath, tasksFilePath);
        CompleteCase CC = IH.CC;
        
        VerifyInitialization(CC);
        RunTestSimulation(CC);
        
        /*List<Task> tasks = new List<Task>();
        string filePath = "data/Exercise/exercise-TC1.csv";

        VSS vss = new VSS();
        
        vss.InitializeTasks(tasks, filePath);

        Console.WriteLine("\n=== Task Statistics ===");
        foreach (var task in tasks)
        {
            Console.WriteLine($"Task {task.Id}: WCET={task.Wcet}, BCET={task.Bcet}, Period={task.Period}, Deadline={task.Deadline}, Priority={task.Priority}");
        }

        int simulationTime = 100; // Set simulation time
        Console.WriteLine("\n=== Running Simulation ===");
        vss.Simulate(tasks, simulationTime);

        // Run Response-Time Analysis
        vss.ResponseTimeAnalysis(tasks);*/
    }
    
    static void VerifyInitialization(CompleteCase CC)
    {
        Console.WriteLine("\n=== Initialization Verification ===");
        
        // Check cores
        var core = CC.GetArchitecture().getCoreFromID("Core_1");
        Console.WriteLine($"Core loaded: {core != null} (Speed={core?.speed})");
        
        // Check component
        var component = CC.GetBudget().getComponentFromID("Camera_Sensor");
        Console.WriteLine($"Component created: {component != null}");
        Console.WriteLine($"Scheduler type: {component?.Scheduler?.GetType().Name}");
        
        // Check tasks
        Console.WriteLine($"Task count: {component?.Tasks.Count}");
        Console.WriteLine($"Task0 WCET (adjusted for core speed): {component?.Tasks[0].Wcet}");
    }
    
    static void RunTestSimulation(CompleteCase CC)
    {
        var component = CC.GetBudget().getComponentFromID("Camera_Sensor");
        var tasks = component.Tasks;
        int simulationTime = 10; // Adjust based on your task periods

        Console.WriteLine("\n=== Simulation Results ===");
    
        for (int t = 0; t < simulationTime; t++)
        {
            // Get the next task to run (respecting hierarchy/BDR)
            var task = component.Scheduler.GetNextTask(t);
        
            if (task != null)
            {
                task.RemainingTime--;
                Console.WriteLine($"Time {t}: Running {task.Id} (Remaining: {task.RemainingTime})");
            
                // Check completion
                if (task.RemainingTime <= 0)
                {
                    task.ResponseTime = t - task.ReleaseTime;
                    Console.WriteLine($"  {task.Id} completed! Response: {task.ResponseTime}");
                
                    // Reset for next period (if periodic)
                    task.ReleaseTime += task.Period;
                    task.RemainingTime = task.Wcet;
                }
            }
            else
            {
                Console.WriteLine($"Time {t}: IDLE (No task scheduled)");
            }
        }
    }
}