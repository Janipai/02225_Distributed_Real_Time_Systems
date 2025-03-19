using System;
using System.Collections.Generic;
using System.Linq;

namespace DefaultNamespace
{
    public class RTA
    {
        public static Dictionary<string, int> PerformRTA(List<Job> taskSet)
        {
            taskSet = taskSet.OrderBy(j => j.Priority).ToList();
            
            // Store worst-case response times
            Dictionary<string, int> wcrt = new();

            foreach (Job task in taskSet)
            {
                int R = task.WCET;
                int prevR;

                while (R != prevR){
                    prevR = R;
                    int interference = 0;

                    // Compute interference from higher-priority tasks
                    foreach (Job higherPriorityTask in taskSet.Where(j => j.Priority < task.Priority))
                    {
                        interference += (int)Math.Ceiling((double)R / higherPriorityTask.Period) * higherPriorityTask.WCET;
                    }

                    R = task.WCET + interference;

                    if (R > task.Deadline)
                    {
                        Console.WriteLine($"Task {task.Task} is UNSCHEDULABLE (WCRT: {R} > Deadline: {task.Deadline})");
                        return null;
                    }
                }

                wcrt[task.Task] = R; // Store final WCRT for the task
            }

            return wcrt;
        }

        public static void PrintWCRT(Dictionary<string, int> wcrt)
        {
            if (wcrt == null)
            {
                Console.WriteLine("The task set is UNSCHEDULABLE.");
                return;
            }

            Console.WriteLine("\nWorst-Case Response Times (WCRT):");
            foreach (var entry in wcrt)
            {
                Console.WriteLine($"Task {entry.Key}: WCRT = {entry.Value}");
            }
        }
    }
}
