namespace _02225.Entities;

public class Architecture
{
    
    List<Core> cores = new List<Core>();
    
    public void AddCore(string coreId, double speed, string scheduler)
    {
        Core newCore = new Core(coreId, speed, scheduler);
        cores.Add(newCore);
    }

    public void PrintCores()
    {
        Console.WriteLine("=== Cores in Architecture ===");
        foreach (var core in cores)
        {
            Console.WriteLine(core.PrintCore());
        }
    }
    
    public Core? GetCoreFromId(string id)
    {
        return cores.FirstOrDefault(core => core.Get() == id);
    }
}