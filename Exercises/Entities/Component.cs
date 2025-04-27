namespace _02225.Entities;

public class Component(string componentId, string scheduler, int budget, int period, Core? coreId)
{
    private string ComponentId { get; set; } = componentId;
    private string Scheduler { get; set; } = scheduler;
    private int Budget { get; set; } = budget;
    private int Period { get; set; } = period;
    private Core? Core { get; set; } = coreId;
    
    private List<ProjectTask> _childTasks  = new List<ProjectTask>();
    
    public void AddChildTask(ProjectTask newTask)
    {
        _childTasks.Add(newTask);
    }

    public List<ProjectTask> GetChildTasks()
    {
        return _childTasks;
    }

    public void PrintChildTasks()
    {
        Console.WriteLine("=== Child Tasks in Component ===");
        foreach (var task in _childTasks)
        {
            task.PrintTask();
        }
    }

    public string PrintComponent()
    {
        return $"Component ID: {ComponentId}, Scheduler: {Scheduler}, Budget Total: {Budget}, Period: {Period}, Core ID: {Core.Get()}";
    }
    public string Get()
    {
        return ComponentId;
    }

    public int GetPeriod()
    {
        return Period;
    }

    public Core? GetCore()
    {
        return Core;
    }

    public string GetScheduler()
    {
        return Scheduler;
    }
}