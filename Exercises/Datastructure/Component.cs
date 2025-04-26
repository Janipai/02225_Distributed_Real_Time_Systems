namespace _02225.Datastructure;

public class Component
{
    private string ComponentID { get; set; } = string.Empty;
    private string Scheduler { get; set; } = string.Empty;
    private int Budget { get; set; } = 0;
    private int Period { get; set; } = 0;
    private Core core { get; set; } = new Core(string.Empty, 0.0, string.Empty);
    
    private List<ProjectTask> childTasks  = new List<ProjectTask>();

    public Component(string componentId, string scheduler, int budget, int period, Core coreID)
    {
        ComponentID = componentId;
        Scheduler = scheduler;
        Budget = budget;
        Period = period;
        core = coreID;
        
    }
    public void addChildTask(ProjectTask newTask)
    {
        childTasks.Add(newTask);
    }
    
    public List<ProjectTask> getChildTasks()
    {
        return childTasks;
    }
    
    public void printChildTasks()
    {
        Console.WriteLine("=== Child Tasks in Component ===");
        foreach (var task in childTasks)
        {
            task.printTask();
        }
    }
    
    public string printComponent()
    {
        return $"Component ID: {ComponentID}, Scheduler: {Scheduler}, Budget Total: {Budget}, Period: {Period}, Core ID: {core.get()}";
    }
    public string get()
    {
        return ComponentID;
    }
    
    public int getPeriod()
    {
        return Period;
    }
    
    public Core getCore() 
    {
        return core;
    }

    public string getScheduler()
    {
        return Scheduler;
    }
}