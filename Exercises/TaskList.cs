namespace _02225;

public class TaskList
{
    private List<ProjectTask> tasks = new List<ProjectTask>();

    public TaskList()
    {
        
    }

    public void addTask(string taskName, int wcet, int period, Component componentID, int priority)
    {
        ProjectTask newTask = new ProjectTask(taskName, wcet, period, componentID, priority);
        tasks.Add(newTask);
    }
    
    public void printTasks()
    {
        Console.WriteLine("=== Tasks in Task List ===");
        foreach (var task in tasks)
        {
            task.printTask();
        }
    }
    
}