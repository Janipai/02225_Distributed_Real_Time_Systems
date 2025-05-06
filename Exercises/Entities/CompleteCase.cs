namespace _02225.Entities;

public class CompleteCase
{
    public readonly Architecture Architecture;
    public readonly Budget Budget;
    public readonly TaskList TaskList;
    private int _simulationTime;

    public CompleteCase(Architecture architecture, Budget budget, TaskList taskList, int simulationTime)
    {
        this.Architecture = architecture;
        this.Budget = budget;
        this.TaskList = taskList;
        this._simulationTime = simulationTime;
    }

    public Architecture GetArchitecture()
    {
        return Architecture;
    }
    
    public Budget GetBudget()
    {
        return Budget;
    }

    public TaskList GetTaskList()
    {
        return TaskList;
    }
    
    public void PrintCompleteCase()
    {
        Console.WriteLine("=== Complete Case ===");
        Architecture.PrintCores();
        Budget.PrintBudget();
        TaskList.PrintTasks();
        
    }

    public int GetSimulationTime()
    {
        return _simulationTime;
    }

    public void SetSimulationTime(int simulationTime)
    {
        this._simulationTime = simulationTime;
    }
    
}