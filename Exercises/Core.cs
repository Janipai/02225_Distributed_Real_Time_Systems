namespace _02225;

public class Core
{
    private string coreID { get; set; } = string.Empty;
    private double speed { get; set; } = 0.0;
    private string scheduler { get; set; } = string.Empty;
    
    
    public Core(string coreID, double speed, string scheduler)
    {
        this.coreID = coreID;
        this.speed = speed;
        this.scheduler = scheduler;
    }
    
    public string printCore()
    {
        return $"Core ID: {coreID}, Speed: {speed} multiplier, Scheduler: {scheduler}";
    }
    public string get()
    {
        return coreID;
    }
}