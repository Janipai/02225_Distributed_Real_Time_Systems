using _02225.Scheduling_policies;

namespace _02225;

public class Component
{
    public string ComponentID { get; set; } = string.Empty;
    public string SchedulerType { get; set; } = string.Empty;
    public int Budget { get; set; } = 0;
    public int Period { get; set; } = 0;
    public Core core { get; set; } = new Core(string.Empty, 0.0, string.Empty);

    public BoundedDelayResource BDR { get; set; }
    public IScheduler Scheduler { get; set; }
    public List<Task> Tasks { get; } = new List<Task>();
    
    public void InitializeScheduler()
    {
        Scheduler = SchedulerType switch
        {
            "RM" => new FPSScheduler(),
            "EDF" => new EDFScheduler(),
            _ => throw new ArgumentException($"Unknown scheduler type: {SchedulerType}")
        };
    }
    public string printComponent()
    {
        return $"Component ID: {ComponentID}, SchedulerType: {SchedulerType}, Budget Total: {Budget}, Period: {Period}, Core ID: {core.get()}";
    }
    public string get()
    {
        return ComponentID;
    }
}