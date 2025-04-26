namespace _02225;

public class TaskList
{
    List<ProjectTask> _tasks = new List<ProjectTask>();

    public TaskList()
    {
        
    }

    public void AddTask(string taskName, int wcet, int period, Component componentId, int priority)
    {
        ProjectTask newTask = new ProjectTask(taskName, wcet, period, componentId, priority);
        _tasks.Add(newTask);
    }
    
    public void PrintTasks()
    {
        Console.WriteLine("=== Tasks in Task List ===");
        foreach (var task in _tasks)
        {
            task.PrintTask();
        }
    }
    
}