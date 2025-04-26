namespace _02225.Datastructure;

public class TaskList
{
    List<ProjectTask> tasks = new List<ProjectTask>();

    public TaskList()
    {
        
    }

    public ProjectTask addTask(string taskName, int wcet, int period, Component componentID, string priority)
    {
        ProjectTask newTask = new ProjectTask(taskName, wcet, period, componentID, priority);
        tasks.Add(newTask);
        return newTask;
    }
    
    public void printTasks()
    {
        Console.WriteLine("=== Tasks in Task List ===");
        foreach (var task in tasks)
        {
            task.printTask();
        }
    }
    
    public List<ProjectTask> getTasks()
    {
        return tasks;
    }
    
}