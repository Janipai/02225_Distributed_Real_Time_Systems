namespace _02225;

public class ProjectTask : Task
{
    private string TaskName { get; set; } = string.Empty;
    private int Wcet { get; set; } = 0;
    private int period { get; set; } = 0;
    //private Component componentID  = new Component(string.Empty, string.Empty, 0, 0, new Core(string.Empty, 0.0, string.Empty));
    public Component AssignedComponent { get; set; }
    private int priority { get; set; } = 0;
    
    public List<Task> Tasks { get; } = new List<Task>(); // Stores both Task and ProjectTask

    //public void AddTask(ProjectTask task)
    //{
     //   Tasks.Add(task);
     //   Scheduler?.AddTask(task); 
    //}
    public ProjectTask(string taskName, int wcet, int period, Component? component, int priority)//Component componentID, )
    {
        //this.taskName = taskName;
        //this.wcet = wcet;
        //this.period = period;
        //this.componentID = componentID;
        //this.priority = priority;
        
        //component.Tasks.Add(this);
        //component.Scheduler?.AddTask(this);
        
        Id = taskName;
        Wcet = wcet;
        Period = period;
        Deadline = period; 
        Priority = priority;
        RemainingTime = wcet;
        ReleaseTime = 0;
        ResponseTime = -1; 

        // ProjectTask-specific
        AssignedComponent = component ?? throw new ArgumentNullException(nameof(component));

        // Automatic system integration
        RegisterWithComponent();
    }
    
    private void RegisterWithComponent()
    {
        // 1. Add to component's task list
        AssignedComponent.Tasks.Add(this);

        // 2. Register with component's scheduler if exists
        if (AssignedComponent.Scheduler != null)
        {
            AssignedComponent.Scheduler.AddTask(this);
        }
        else
        {
            Console.WriteLine($"[WARNING] Task {Id} created before component scheduler initialization");
        }

        // 3. Apply core speed factor
        if (AssignedComponent.Core != null)
        {
            Wcet = (int)Math.Ceiling(Wcet / AssignedComponent.Core.Speed);
        }
    }
    
    public void PrintTask()
    {
        Console.WriteLine($"Task Name: {TaskName}, WCET: {Wcet}, Period: {period}, Component ID: {AssignedComponent.Get()}, Priority: {priority}");
    }
}