using _02225.Entities;

namespace _02225;

public class MainController
{
    private CompleteCase _cc;
    private Rta _rta;
    private Scheduler _scheduler;
    
    
    public MainController(CompleteCase cc)
    {
        this._cc = cc;
        this._rta = new Rta(cc);
        this._scheduler = new Scheduler(cc);
        
    }
    
    public void start()
    {
        Console.WriteLine("Starting MainController...");
        _rta.RunRta();
        _scheduler.RunScheduler();
    }
    
    
}