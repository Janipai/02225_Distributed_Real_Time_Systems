namespace _02225;

public class Budget
{
    private List<Component> components = new List<Component>();
    
    public Budget()
    {
        
    }

    public void addComponent(string budgetID, string scheduler, int budget, int period, Core coreID)
    {
        //Component newComponent = new Component(budgetID, scheduler, budget, period, coreID);
        Component newComponent = new Component 
        {
            ComponentID = budgetID,
            SchedulerType = scheduler,
            Budget = budget,
            Period = period,
            core = coreID,
            BDR = new BoundedDelayResource((double)budget/period, period-budget)
        };
        newComponent.InitializeScheduler();
        components.Add(newComponent);
    }
    
    public void printBudget()
    {
        Console.WriteLine("=== Budget ===");
        foreach (var component in components)
        {
            Console.WriteLine(component.printComponent());
        }
    }
    
    public Component getComponentFromID(string ID)
    {
        foreach (var component in components)
        {
            if (component.get() == ID)
            {
                return component;
            }
        }
        return null;
    }
}