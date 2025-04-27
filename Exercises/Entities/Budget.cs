namespace _02225.Entities;

public class Budget
{
    List<Component> components = new List<Component>();
    
    public Budget()
    {
        
    }

    public void AddComponent(string budgetId, string scheduler, int budget, int period, Core? coreId)
    {
        Component newComponent = new Component(budgetId, scheduler, budget, period, coreId);
        components.Add(newComponent);
    }
    
    public void PrintBudget()
    {
        Console.WriteLine("=== Budget ===");
        foreach (var component in components)
        {
            Console.WriteLine(component.PrintComponent());
            component.PrintComponent();
        }
    }
    
    public Component? GetComponentFromId(string id)
    {
        return components.FirstOrDefault(component => component.Get() == id);
    }
    
    public List<Component> GetComponents()
    {
        return components;
    }
    
}