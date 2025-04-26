namespace _02225.Datastructure;

public class CompleteCase
{
    private Architecture Architecture;
    private Budget Budget;
    private TaskList TaskList;
    private int simulationTime;

    public CompleteCase(Architecture architecture, Budget budget, TaskList taskList, int simulationTime)
    {
        this.Architecture = architecture;
        this.Budget = budget;
        this.TaskList = taskList;
        this.simulationTime = simulationTime;
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
    
    public int getSimulationTime()
    {
        return simulationTime;
    }
    
    public void setSimulationTime(int simulationTime)
    {
        this.simulationTime = simulationTime;
    }
    
}