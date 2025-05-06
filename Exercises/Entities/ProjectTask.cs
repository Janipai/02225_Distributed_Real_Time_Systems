namespace _02225.Entities;

public class ProjectTask
{
    public string TaskName { get; set; }
    public int Wcet { get; set; }
    public  int Period { get; set; }
    public Component ComponentId;
    private int Priority { get; set; }

    public double Wcrt { get; set; } = 0.0;   // set by AnalysisTool
    public bool MissesDeadline => Wcrt > Period;
        
    
    public ProjectTask(string taskName, int wcet, int period, Component componentId, string priority)
    {
        this.TaskName = taskName;
        this.Wcet = wcet;
        this.Period = period;
        this.ComponentId = componentId;
        
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
        Console.WriteLine($"Task Name: {TaskName}, WCET: {Wcet}, Period: {Period}, Component ID: {ComponentId.Get()}, Priority: {Priority}");
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