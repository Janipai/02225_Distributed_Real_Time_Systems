namespace _02225.Datastructure;

public class ProjectTask
{
    private string taskName { get; set; } = string.Empty;
    private int wcet { get; set; } = 0;
    private int period { get; set; } = 0;
    private Component componentID  = new Component(string.Empty, string.Empty, 0, 0, new Core(string.Empty, 0.0, string.Empty));
    private int priority { get; set; } = 0;
    
    
    public ProjectTask(string taskName, int wcet, int period, Component componentID, string priority)
    {
        this.taskName = taskName;
        this.wcet = wcet;
        this.period = period;
        this.componentID = componentID;
        
        if(componentID.getScheduler() == "RM")
        {
            this.priority = int.Parse(priority);
        }
        else
        {
            this.priority = 0;
        }
    }

    
    public void printTask()
    {
        Console.WriteLine($"Task Name: {taskName}, WCET: {wcet}, Period: {period}, Component ID: {componentID.get()}, Priority: {priority}");
    }
    
    public int getPriority()
    {
        return priority;
    }
    public int getPeriod()
    {
        return period;
    }
    
    public int getWcet()
    {
        return wcet;
    }
    
    public string getName()
    {
        return taskName;
    }
}