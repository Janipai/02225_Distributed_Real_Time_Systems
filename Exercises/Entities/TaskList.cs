namespace _02225.Entities;

public class TaskList
{
    List<ProjectTask> _tasks = new List<ProjectTask>();

    public TaskList()
    {
        
    }

    public ProjectTask AddTask(string taskName, int wcet, int period, Component componentId, string priority)
    {
        ProjectTask newTask = new ProjectTask(taskName, wcet, period, componentId, priority);
        _tasks.Add(newTask);
        return newTask;
    }
    
    public void PrintTasks()
    {
        Console.WriteLine("=== Tasks in Task List ===");
        foreach (var task in _tasks)
        {
            task.PrintTask();
        }
    }

    public List<ProjectTask> GetTasks()
    {
        return _tasks;
    }
    
}