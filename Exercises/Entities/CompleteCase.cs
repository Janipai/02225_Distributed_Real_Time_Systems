namespace _02225.Entities;

public class CompleteCase
{
    private readonly Architecture _architecture;
    private readonly Budget _budget;
    private readonly TaskList _taskList;
    private int _simulationTime;

    public CompleteCase(Architecture architecture, Budget budget, TaskList taskList, int simulationTime)
    {
        this._architecture = architecture;
        this._budget = budget;
        this._taskList = taskList;
        this._simulationTime = simulationTime;
    }

    public Architecture GetArchitecture()
    {
        return _architecture;
    }
    
    public Budget GetBudget()
    {
        return _budget;
    }

    public TaskList GetTaskList()
    {
        return _taskList;
    }
    
    public void PrintCompleteCase()
    {
        Console.WriteLine("=== Complete Case ===");
        _architecture.PrintCores();
        _budget.PrintBudget();
        _taskList.PrintTasks();
        
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