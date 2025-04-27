namespace _02225.Entities;

public class ProjectTask
{
    private string TaskName { get; set; }
    private int Wcet { get; set; }
    private int Period { get; set; }
    private Component _componentId;
    private int Priority { get; set; }
    
    public ProjectTask(string taskName, int wcet, int period, Component componentId, string priority)
    {
        this.TaskName = taskName;
        this.Wcet = wcet;
        this.Period = period;
        this._componentId = componentId;
        
        if(componentId.GetScheduler() == "RM")
        {
            this.Priority = int.Parse(priority);
        }
        else
        {
            this.Priority = 0;
        }
    }
    
    public void PrintTask()
    {
        Console.WriteLine($"Task Name: {TaskName}, WCET: {Wcet}, Period: {Period}, Component ID: {_componentId.Get()}, Priority: {Priority}");
    }
    
    public int GetPriority()
    {
        return Priority;
    }
    public int GetPeriod()
    {
        return Period;
    }
    
    public int GetWcet()
    {
        return Wcet;
    }
    
    public string GetName()
    {
        return TaskName;
    }
}