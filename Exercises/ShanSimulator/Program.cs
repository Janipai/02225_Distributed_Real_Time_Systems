namespace DefaultNamespace;

public class Program
{
    static void Main(string[] args)
    {
        VSS scheduler = new VSS();
        
        string filePath = @"../data/exercise-TC1.csv";
        
        scheduler.InitializeJobs(filePath);
        scheduler.VVS();
        
        //Dictionary<string, int> wcrt = RTA.PerformRTA(scheduler.GetJobs());
        //RTA.PrintWCRT(wcrt);
    }
}