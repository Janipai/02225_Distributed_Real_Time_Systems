namespace _02225.Entities;

public class Core
{
    private string CoreId { get; set; }
    private double Speed { get; set; }
    private string Scheduler { get; set; }


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