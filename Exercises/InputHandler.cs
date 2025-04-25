namespace _02225;

public class InputHandler
{
    public CompleteCase Cc = new CompleteCase(new Architecture(), new Budget(), new TaskList());
        
    public InputHandler(String arcchitectureFilePath, string budgetFilePath, string tasksFilePath)
    {
        
        Console.WriteLine("Starting InputHandler...");
        Console.WriteLine("Loading architecture data...");
        LoadArchitectureData(arcchitectureFilePath);
        Console.WriteLine("Loading budget data...");
        LoadBudgetData(budgetFilePath);
        Console.WriteLine("Loading tasks data...");
        LoadTaskData(tasksFilePath);
        Console.WriteLine("Done loading, showing result...");
        Cc.PrintComepleteCase();
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
                Cc.GetArchitecture().AddCore(tokens[0], double.Parse(tokens[1]), tokens[2]);
                
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
                Cc.GetBudget().AddComponent(
                    tokens[0], tokens[1], int.Parse(tokens[2]), 
                    int.Parse(tokens[3]), Cc.GetArchitecture().GetCoreFromId(tokens[4]));

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
                Cc.GetTaskList().AddTask(
                    tokens[0], int.Parse(tokens[1]), int.Parse(tokens[2]),
                    Cc.GetBudget().GetComponentFromId(tokens[3]), int.Parse(tokens[4]));

            }
        }
        
    }
}