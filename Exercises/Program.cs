namespace _02225;

public class Program
{
    static void Main()
    {
        string part = "data/Project/1-tiny-test-case/";
        string architectureFilePath =  part + "architecture.csv";
        string budgetFilePath = part + "budgets.csv";
        string tasksFilePath = part + "tasks.csv";
        InputHandler ih = new InputHandler(architectureFilePath, budgetFilePath, tasksFilePath);
        
        
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
}