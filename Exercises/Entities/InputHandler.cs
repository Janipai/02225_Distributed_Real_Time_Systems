namespace _02225.Entities;

public class InputHandler
{
    private CompleteCase _cc = new CompleteCase(new Architecture(), new Budget(), new TaskList());
        
    public InputHandler(String architectureFilePath, string budgetFilePath, string tasksFilePath)
    {
        
        Console.WriteLine("Starting InputHandler...");
        Console.WriteLine("Loading architecture data...");
        LoadArchitectureData(architectureFilePath);
        Console.WriteLine("Loading budget data...");
        LoadBudgetData(budgetFilePath);
        Console.WriteLine("Loading tasks data...");
        LoadTaskData(tasksFilePath);
        Console.WriteLine("Done loading, showing result...");
        _cc.PrintCompleteCase();
    }


    public void LoadArchitectureData(string filePath)
    {
        // Load architecture data from the specified file
        using (var reader = new StreamReader(filePath))
        {
            bool skipFirstLine = true;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                    continue;
                }
                var tokens = line.Split(',');
                _cc.GetArchitecture().AddCore(tokens[0], double.Parse(tokens[1]), tokens[2]);
                
            }
        }
    }

    

    public void LoadBudgetData(string filePath)
    {
        // Load budget data from the specified file
        using (var reader = new StreamReader(filePath))
        {
            bool skipFirstLine = true;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                    continue;
                }
                var tokens = line.Split(',');
                _cc.GetBudget().AddComponent(
                    tokens[0], tokens[1], int.Parse(tokens[2]), 
                    int.Parse(tokens[3]), _cc.GetArchitecture().GetCoreFromId(tokens[4]));

            }
        }
        
    }

    public void LoadTaskData(string filePath)
    {
        // Load task data from the specified file
        using (var reader = new StreamReader(filePath))
        {
            bool skipFirstLine = true;
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                    continue;
                }
                var tokens = line.Split(',');
                _cc.GetTaskList().AddTask(
                    tokens[0], int.Parse(tokens[1]), int.Parse(tokens[2]),
                    _cc.GetBudget().GetComponentFromId(tokens[3]), int.Parse(tokens[4]));

            }
        }
        
    }
}