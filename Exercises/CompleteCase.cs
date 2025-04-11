namespace _02225;

public class CompleteCase
{
    private Architecture Architecture;
    private Budget Budget;
    private TaskList TaskList;

    public CompleteCase(Architecture architecture, Budget budget, TaskList taskList)
    {
        this.Architecture = architecture;
        this.Budget = budget;
        this.TaskList = taskList;
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
    
    public void printComepleteCase()
    {
        Console.WriteLine("=== Complete Case ===");
        Architecture.PrintCores();
        Budget.printBudget();
        TaskList.printTasks();
        
    }
    
}