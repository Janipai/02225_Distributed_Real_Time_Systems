namespace _02225;

public class Core
{
    private string CoreId { get; set; } = string.Empty;
    public double Speed { get; set; } = 0.0;
    private string Scheduler { get; set; } = string.Empty;
    
    
    public Core(string coreId, double speed, string scheduler)
    {
        this.CoreId = coreId;
        this.Speed = speed;
        this.Scheduler = scheduler;
    }
    
    public string PrintCore()
    {
        return $"Core ID: {CoreId}, Speed: {Speed} multiplier, Scheduler: {Scheduler}";
    }
    public string Get()
    {
        return CoreId;
    }
}