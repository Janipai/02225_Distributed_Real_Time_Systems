using System.Net.Mime;

namespace _02225;

public class Program
{
    static void Main()
    {
        
        string easy = "data/Project/1-tiny-test-case/";
        string small = "data/Project/2-small-test-case/";
        string medium = "data/Project/3-medium-test-case/";
        String architectureFilePath =  medium + "architecture.csv";
        String budgetFilePath = medium + "budgets.csv";
        String tasksFilePath = medium + "tasks.csv";
        int simulationTime = 100000;
        InputHandler IH = new InputHandler(architectureFilePath, budgetFilePath, tasksFilePath, simulationTime);
        MainController mc = new MainController(IH.CC);
        mc.start();


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