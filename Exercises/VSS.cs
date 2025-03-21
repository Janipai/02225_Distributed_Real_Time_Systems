using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Task
{
    public string Id { get; set; } = string.Empty;
    public int Wcet { get; set; }          
    public int Bcet { get; set; }          
    public int Period { get; set; }        
    public int Deadline { get; set; }      
    public int Priority { get; set; }      
    public int RemainingTime { get; set; } 
    public int ReleaseTime { get; set; }   
    public int ResponseTime { get; set; }  

    public Task()
    {
        ResponseTime = -1;  // Initially unknown
    }
}

class Program
{
    static void InitializeTasks(List<Task> tasks, string filename)
    {
        using (var reader = new StreamReader(filename))
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
                if (tokens.Length < 6) continue;

                Task task = new Task
                {
                    Id = tokens[0],
                    Wcet = int.Parse(tokens[1]),
                    Bcet = int.Parse(tokens[2]),
                    Period = int.Parse(tokens[3]),
                    Deadline = int.Parse(tokens[4]),
                    Priority = 0, // Set dynamically based on RMS rule
                    RemainingTime = int.Parse(tokens[1]),
                    ReleaseTime = 0
                };

                tasks.Add(task);
            }
        }

        // Sort tasks based on RMS: shorter period → higher priority
        tasks.Sort((a, b) => a.Period.CompareTo(b.Period));

        // Assign priority dynamically (lower number = higher priority)
        for (int i = 0; i < tasks.Count; i++)
        {
            tasks[i].Priority = tasks.Count - i;  // Highest priority = highest number
        }
    }

    // Function to get ready tasks at the current time
    static List<Task> GetReadyTasks(List<Task> tasks, int currentTime)
    {
        List<Task> readyTasks = new List<Task>();

        foreach (var task in tasks)
        {
            if (currentTime % task.Period == 0)
            {
                task.ReleaseTime = currentTime;
                task.RemainingTime = task.Wcet;
            }
            if (task.ReleaseTime <= currentTime && task.RemainingTime > 0)
            {
                readyTasks.Add(task);
            }
        }
        return readyTasks;
    }

    // Function to select the highest priority task from the ready list
    static Task HighestPriorityTask(List<Task> readyTasks)
    {
        return readyTasks.OrderByDescending(task => task.Priority).FirstOrDefault();
    }

    // Improved AdvanceTime function to skip idle intervals
    static int AdvanceTime(List<Task> tasks, int currentTime)
    {
        int nextReleaseTime = tasks
            .Where(t => currentTime % t.Period != 0)
            .Select(t => t.Period * (currentTime / t.Period + 1))
            .Where(t => t > currentTime)
            .DefaultIfEmpty(int.MaxValue)
            .Min();

        return nextReleaseTime == int.MaxValue ? 1 : Math.Min(1, nextReleaseTime - currentTime);
        //return 1; // Change this to skip idle time: 
    }

    // Main simulation function (Very Simple Simulator - VSS)
    static void Simulate(List<Task> tasks, int simulationTime)
    {
        int currentTime = 0;
        Dictionary<string, int> wcrt = new Dictionary<string, int>();

        while (currentTime <= simulationTime)
        {
            var readyTasks = GetReadyTasks(tasks, currentTime);
            int delta = AdvanceTime(tasks, currentTime);

            if (readyTasks.Any())
            {
                Task currentTask = HighestPriorityTask(readyTasks);
                
                // Preempt lower-priority tasks
                foreach (var task in readyTasks)
                {
                    if (task.Priority > currentTask.Priority && task.RemainingTime > 0)
                    {
                        currentTask = task; // Preempt with higher-priority task
                    }
                }

                currentTask.RemainingTime -= delta;
                currentTime += delta;

                if (currentTask.RemainingTime <= 0)
                {
                    currentTask.ResponseTime = currentTime - currentTask.ReleaseTime;
                    if (!wcrt.ContainsKey(currentTask.Id) || wcrt[currentTask.Id] < currentTask.ResponseTime)
                    {
                        wcrt[currentTask.Id] = currentTask.ResponseTime;
                    }
                    Console.WriteLine($"Task {currentTask.Id} completed with response time {currentTask.ResponseTime}");
                }
            }
            else
            {
                currentTime += delta;
            }
        }

        // Output WCRT
        Console.WriteLine("\nWorst-Case Response Times (WCRT) from Simulation:");
        foreach (var task in tasks)
        {
            Console.WriteLine($"Task {task.Id}: {(wcrt.ContainsKey(task.Id) ? wcrt[task.Id].ToString() : "N/A")}");
        }
    }

    // Response-Time Analysis (RTA) Function
    static void ResponseTimeAnalysis(List<Task> tasks)
    {
        tasks = tasks.OrderByDescending(t => t.Priority).ToList(); // Sort tasks by priority

        Console.WriteLine("\n=== Running Response-Time Analysis (RTA) ===");

        bool allSchedulable = true;
        List<string> unschedulableTasks = new List<string>();

        foreach (var task in tasks)
        {
            int responseTime = task.Wcet;
            int previousResponseTime;

            do
            {
                previousResponseTime = responseTime;
                responseTime = task.Wcet;

                foreach (var higherPriorityTask in tasks.Where(t => t.Priority > task.Priority))
                {
                    responseTime += (int)Math.Ceiling((double)previousResponseTime / higherPriorityTask.Period) * higherPriorityTask.Wcet;
                }

                if (responseTime > task.Deadline)
                {
                    allSchedulable = false;
                    unschedulableTasks.Add(task.Id);
                    break; // Continue analyzing other tasks
                }
            }
            while (responseTime != previousResponseTime);

            task.ResponseTime = responseTime;
            Console.WriteLine($"Task {task.Id} WCRT: {task.ResponseTime} (Deadline: {task.Deadline})");
        }

        if (allSchedulable)
        {
            Console.WriteLine("\nAll tasks are SCHEDULABLE.");
        }
        else
        {
            Console.WriteLine("\nThe following tasks are UNSCHEDULABLE:");
            foreach (var taskId in unschedulableTasks)
            {
                Console.WriteLine($"- Task {taskId}");
            }
        }
    }

    static void Main()
    {
        List<Task> tasks = new List<Task>();
        string filePath = "data/exercise-TC1.csv";
        InitializeTasks(tasks, filePath);

        Console.WriteLine("\n=== Task Statistics ===");
        foreach (var task in tasks)
        {
            Console.WriteLine($"Task {task.Id}: WCET={task.Wcet}, BCET={task.Bcet}, Period={task.Period}, Deadline={task.Deadline}, Priority={task.Priority}");
        }

        int simulationTime = 100; // Set simulation time
        Console.WriteLine("\n=== Running Simulation ===");
        Simulate(tasks, simulationTime);

        // Run Response-Time Analysis
        ResponseTimeAnalysis(tasks);
    }
}
