using _02225.Entities;

namespace _02225;

public class MainController
{
    private CompleteCase _cc;
    private Scheduler _scheduler;
    private AnalysisTool _analysisTool;
    
    
    public MainController(CompleteCase cc)
    {
        this._cc = cc;
        this._scheduler = new Scheduler(cc);
        this._analysisTool = new AnalysisTool(cc);
        
    }
    
    public void Start()
    {
        Console.WriteLine("Starting MainController...");
        _scheduler.RunScheduler();
        
        Console.WriteLine("\nRunning Analysis Tool...");
        _analysisTool.RunAnalysis();
    }
    
    
}