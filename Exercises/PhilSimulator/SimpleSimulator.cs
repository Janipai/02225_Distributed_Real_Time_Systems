namespace Simulation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    
    // Input: A set of tasks stored in a file and the simulation time 
    // Output: A list of worst-case response times observed during the simulation  
    public class Simulator
    {
        public List<Task> Tasks { get; set;  }
        private int simulationTime;
        
        public Simulator(List<Task> tasks, int simulationTime)
        {
            Tasks = tasks;
            this.simulationTime = simulationTime;
        }
        public List<int> Start()
        {
            // Setup 
            int cycles = Tasks.Count;
            int currentTime = 0;
            
            Console.WriteLine("Starting Simulator");
            Console.WriteLine($"Configurations: cycles: {cycles}, currentTime: {currentTime}");
            
            while (currentTime <= cycles && Tasks.Any(t => t.RemainingTime > 0))
            {
                // Ready tasks whose release time is <= current time and are unfinished.
                var readyTasks = Tasks.Where(t => t.ReleaseTime <= currentTime && t.RemainingTime > 0).ToList();
                Console.WriteLine($"Tasks that are ready");
                foreach (var task in readyTasks)
                {
                    Console.WriteLine(task.Name);
                }
                
                if (readyTasks.Any())
                {
                    // Assuming that a lower Priority number means a higher priority.
                    var currentTask = readyTasks.OrderBy(t => t.Priority).First();
                    Console.WriteLine($"Current Task: {currentTask}");
                    int delta = 1;
                    currentTime += delta;
                    currentTask.RemainingTime -= delta;

                    // If the task is finished, record its response time.
                    if (currentTask.RemainingTime <= 0)
                    {
                        currentTask.ResponseTime = currentTime - currentTask.ReleaseTime;
                    }
                }
                else
                {
                    // If no task is ready, simply advance time.
                    currentTime++;
                }
            }

            // Return the response times of the tasks (could be worst-case if you run multiple iterations).
            return Tasks.Select(t => t.ResponseTime).ToList();
        }
    }

    public class Task
    {   
        public string Name { get; set; }
        public int WCET { get; set; }
        public int BCET { get; set; }
        public int Period { get; set; }
        public int Deadline { get; set; }
        public int Priority { get; set; }
        
        public int RemainingTime { get; set; }
        public int ReleaseTime { get; set; }
        public int ResponseTime { get; set; }
    }

    public static class TaskLoader
    {
        public static List<Task> GetTasks(string filePath)
        {
            var tasks = new List<Task>();

            using (var reader = new StreamReader(filePath))
            {
                // Read and ignore the header
                string header = reader.ReadLine();
                string line;
                
                while ((line = reader.ReadLine()) != null)
                {
                    var taskAttributes = line.Split(',');

                    if (taskAttributes.Length >= 6)
                    {
                        tasks.Add(new Task
                        {
                            Name = taskAttributes[0],
                            WCET = int.Parse(taskAttributes[1]),
                            BCET = int.Parse(taskAttributes[2]),
                            Period = int.Parse(taskAttributes[3]),
                            Deadline = int.Parse(taskAttributes[4]),
                            Priority = int.Parse(taskAttributes[5]),
                            RemainingTime = int.Parse(taskAttributes[1])
                        });
                    }
                }
            }
            return tasks;   
        }
    }
}