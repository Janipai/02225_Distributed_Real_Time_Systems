using _02225.Scheduling_policies;

namespace _02225;

public class Component
{
    public string ComponentId { get; set; } = string.Empty;
    public string SchedulerType { get; set; } = string.Empty;
    public int Budget { get; set; } = 0;
    public int Period { get; set; } = 0;
    public Core? Core { get; set; } = new Core(string.Empty, 0.0, string.Empty);

    public BoundedDelayResource Bdr { get; set; }
    public IScheduler Scheduler { get; set; }
    public List<Task> Tasks { get; } = new List<Task>();
    
    public void InitializeScheduler()
    {
        Scheduler = SchedulerType switch
        {
            "RM" => new FpsScheduler(),
            "EDF" => new EdfScheduler(),
            _ => throw new ArgumentException($"Unknown scheduler type: {SchedulerType}")
        };
    }
    public string PrintComponent()
    {
        return $"Component ID: {ComponentId}, SchedulerType: {SchedulerType}, Budget Total: {Budget}, Period: {Period}, Core ID: {Core?.Get()}";
    }
    public string Get()
    {
        return ComponentId;
    }
}