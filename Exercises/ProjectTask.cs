namespace _02225;

public class ProjectTask
{
    private string taskName { get; set; } = string.Empty;
    private int wcet { get; set; } = 0;
    private int period { get; set; } = 0;
    private Component componentID  = new Component(string.Empty, string.Empty, 0, 0, new Core(string.Empty, 0.0, string.Empty));
    private int priority { get; set; } = 0;
    
    
    public ProjectTask(string taskName, int wcet, int period, Component componentID, int priority)
    {
        this.taskName = taskName;
        this.wcet = wcet;
        this.period = period;
        this.componentID = componentID;
        this.priority = priority;
    }
    
    public void printTask()
    {
        Console.WriteLine($"Task Name: {taskName}, WCET: {wcet}, Period: {period}, Component ID: {componentID.get()}, Priority: {priority}");
    }
}