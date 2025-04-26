namespace _02225;

public class Component(string componentId, string scheduler, int budget, int period, Core coreId)
{
    private string ComponentId { get; set; } = componentId;
    private string Scheduler { get; set; } = scheduler;
    private int Budget { get; set; } = budget;
    private int Period { get; set; } = period;
    private Core Core { get; set; } = coreId;

    public string PrintComponent()
    {
        return $"Component ID: {ComponentId}, Scheduler: {Scheduler}, Budget Total: {Budget}, Period: {Period}, Core ID: {Core.Get()}";
    }
    public string Get()
    {
        return ComponentId;
    }
}