namespace _02225.Datastructure;

public class Architecture
{
    private List<Core> cores = new List<Core>();
    
    public void AddCore(string coreID, double speed, string scheduler)
    {
        Core newCore = new Core(coreID, speed, scheduler);
        cores.Add(newCore);
    }

    public Architecture()
    {
        
    }
    public void PrintCores()
    {
        Console.WriteLine("=== Cores in Architecture ===");
        foreach (var core in cores)
        {
            Console.WriteLine(core.printCore());
        }
    }
    
    public Core getCoreFromID(string ID)
    {
        foreach (var core in cores)
        {
            if (core.get() == ID)
            {
                return core;
            }
        }
        return null;
    }
}