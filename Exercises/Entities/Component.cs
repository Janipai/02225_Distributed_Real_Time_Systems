namespace _02225.Entities;

public class Component
{
    public string ComponentId { get; set; }
    public string Scheduler { get; set; }
    public int Budget { get; set; }
    public int Period { get; set; }
    public Core? Core { get; set; }
    
    private readonly List<ProjectTask> _childTasks = new();
    public IReadOnlyList<ProjectTask>  ChildTasks  => _childTasks.AsReadOnly();
    
    public Component(string componentId,
        string scheduler,
        int    budget,
        int    period,
        Core   core)
    {
        ComponentId = componentId;
        Scheduler   = scheduler;
        Budget      = budget;
        Period      = period;
        Core        = core;
    }
    
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