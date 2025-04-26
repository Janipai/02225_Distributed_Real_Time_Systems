namespace _02225.Datastructure;

public class Budget
{
    List<Component> components = new List<Component>();
    
    public Budget()
    {
        
    }

    public void addComponent(string budgetID, string scheduler, int budget, int period, Core coreID)
    {
        Component newComponent = new Component(budgetID, scheduler, budget, period, coreID);
        components.Add(newComponent);
    }
    
    public void printBudget()
    {
        Console.WriteLine("=== Budget ===");
        foreach (var component in components)
        {
            Console.WriteLine(component.printComponent());
            component.printChildTasks();
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
    
    public List<Component> getComponents()
    {
        return components;
    }
}