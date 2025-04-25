namespace _02225;

public class Architecture
{
    private readonly List<Core?> _cores = new List<Core?>();
    
    public void AddCore(string coreId, double speed, string scheduler)
    {
        Core? newCore = new Core(coreId, speed, scheduler);
        _cores.Add(newCore);
    }

    public Architecture()
    {
        
    }
    public void PrintCores()
    {
        Console.WriteLine("=== Cores in Architecture ===");
        foreach (var core in _cores)
        {
            Console.WriteLine(core?.PrintCore());
        }
    }
    
    public Core? GetCoreFromId(string id)
    {
        foreach (var core in _cores)
        {
            if (core?.Get() == id)
            {
                return core;
            }
        }
        return null;
    }
    
    
}