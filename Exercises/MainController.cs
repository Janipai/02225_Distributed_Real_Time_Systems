using _02225.Datastructure;

namespace _02225;

public class MainController
{
    private CompleteCase cc;
    private RTA _rta;
    private Scheduler _scheduler;
    
    
    public MainController(CompleteCase cc)
    {
        this.cc = cc;
        this._rta = new RTA(cc);
        this._scheduler = new Scheduler(cc);
        
    }
    
    public void start()
    {
        Console.WriteLine("Starting MainController...");
        _rta.runRTA();
        _scheduler.runScheduler();
    }
    
    
}