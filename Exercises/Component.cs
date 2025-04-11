namespace _02225;

public class Component
{
    private string ComponentID { get; set; } = string.Empty;
    private string Scheduler { get; set; } = string.Empty;
    private int Budget { get; set; } = 0;
    private int Period { get; set; } = 0;
    private Core core { get; set; } = new Core(string.Empty, 0.0, string.Empty);

    public Component(string componentId, string scheduler, int budget, int period, Core coreID)
    {
        ComponentID = componentId;
        Scheduler = scheduler;
        Budget = budget;
        Period = period;
        core = coreID;
        
    }
    
    public string printComponent()
    {
        return $"Component ID: {ComponentID}, Scheduler: {Scheduler}, Budget Total: {Budget}, Period: {Period}, Core ID: {core.get()}";
    }
    public string get()
    {
        return ComponentID;
    }
}