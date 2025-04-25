namespace _02225;

public class CompleteCase
{
    private readonly Architecture _architecture;
    private readonly Budget _budget;
    private readonly TaskList _taskList;

    public CompleteCase(Architecture architecture, Budget budget, TaskList taskList)
    {
        this._architecture = architecture;
        this._budget = budget;
        this._taskList = taskList;
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
    
    public void PrintComepleteCase()
    {
        Console.WriteLine("=== Complete Case ===");
        _architecture.PrintCores();
        _budget.PrintBudget();
        _taskList.PrintTasks();
        
    }
    
}