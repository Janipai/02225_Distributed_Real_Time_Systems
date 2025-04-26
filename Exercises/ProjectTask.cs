namespace _02225;

public class ProjectTask
{
    private string TaskName { get; set; }
    private int Wcet { get; set; }
    private int Period { get; set; }
    private Component _componentId;
    private int Priority { get; set; } = 0;
    
    
    public ProjectTask(string taskName, int wcet, int period, Component componentId, int priority)
    {
        this.TaskName = taskName;
        this.Wcet = wcet;
        this.Period = period;
        this._componentId = componentId;
        this.Priority = priority;
    }
    
    public void PrintTask()
    {
        Console.WriteLine($"Task Name: {TaskName}, WCET: {Wcet}, Period: {Period}, Component ID: {_componentId.Get()}, Priority: {Priority}");
    }
}