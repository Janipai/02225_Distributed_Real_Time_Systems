namespace _02225.Scheduling_policies;

using System.Collections.Generic;
using System.Linq;

public class FpsScheduler : IScheduler
{
    private readonly List<Task> _tasks = new();

    public void AddTask(Task task)
    {
        if (_tasks.Contains(task)) return;
        _tasks.Add(task);
        // RM priority assignment
        _tasks.Sort((a, b) => a.Period.CompareTo(b.Period));
        UpdatePriorities();
    }

    public Task? GetNextTask(int currentTime)
    {
        var next = _tasks.Where(t => 
                t.RemainingTime > 0 && 
                t.ReleaseTime <= currentTime)
            .OrderBy(t => t.Period)
            .FirstOrDefault();

        if (next is ProjectTask pt)
        {
            pt.AssignedComponent.Bdr.GetSupply(currentTime); // Use BDR if needed
        }
        return next;
    }

    public bool IsSchedulable(List<Task> tasks)
    {
        var sortedTasks = tasks.OrderBy(t => t.Period).ToList();
        
        for (var i = 0; i < sortedTasks.Count; i++)
        {
            var responseTime = sortedTasks[i].Wcet;
            int prevResponseTime;
            var iterations = 0;
            const int maxIterations = 1000;

            do
            {
                prevResponseTime = responseTime;
                responseTime = sortedTasks[i].Wcet + sortedTasks
                    .Take(i)
                    .Sum(hp => (int)Math.Ceiling((double)prevResponseTime / hp.Period) * hp.Wcet);
                
                if (responseTime > sortedTasks[i].Deadline) 
                    return false;
                    
                if (iterations++ > maxIterations)
                    throw new Exception("RTA failed to converge");
                    
            } while (responseTime != prevResponseTime);
        }
        return true;
    }

    public List<Task> GetTaskList() => _tasks;

    private void UpdatePriorities()
    {
        for (var i = 0; i < _tasks.Count; i++)
        {
            _tasks[i].Priority = _tasks.Count - i; // Higher number = higher priority
        }
    }
}