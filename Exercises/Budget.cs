namespace _02225;

public class Budget
{
    private List<Component?> _components = new List<Component?>();
    
    public Budget()
    {
        
    }

    public void AddComponent(string budgetId, string scheduler, int budget, int period, Core? coreId)
    {
        //Component newComponent = new Component(budgetID, scheduler, budget, period, coreID);
        Component? newComponent = new Component 
        {
            ComponentId = budgetId,
            SchedulerType = scheduler,
            Budget = budget,
            Period = period,
            Core = coreId,
            Bdr = new BoundedDelayResource((double)budget/period, period-budget)
        };
        newComponent.InitializeScheduler();
        _components.Add(newComponent);
    }
    
    public void PrintBudget()
    {
        Console.WriteLine("=== Budget ===");
        foreach (var component in _components)
        {
            Console.WriteLine(component?.PrintComponent());
        }
    }
    
    public Component? GetComponentFromId(string id)
    {
        return _components.FirstOrDefault(component => component?.Get() == id);
    }
}