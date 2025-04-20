namespace _02225.Scheduling_policies;

using System.Collections.Generic;
using System.Linq;

public class EDFScheduler : IScheduler
{
    private readonly List<Task> _tasks = new();
    private readonly PriorityQueue<Task, int> _readyQueue = new();

    public void AddTask(Task task)
    {
        _tasks.Add(task);
        if (task.RemainingTime <= 0) 
        {
            task.RemainingTime = task.Wcet; // Reset if adding a new instance
        }
        UpdateReadyQueue(task.ReleaseTime); // Initial update
    }

    public Task GetNextTask(int currentTime)
    {
        UpdateReadyQueue(currentTime);
        return _readyQueue.TryDequeue(out var task, out _) ? task : null;
    }

    public bool IsSchedulable(List<Task> tasks)
    {
        double utilization = tasks.Sum(t => 
        {
            double denominator = t.Deadline > 0 ? t.Deadline : t.Period;
            return (double)t.Wcet / denominator;
        });
        return utilization <= 1.0;
    }

    public List<Task> GetTaskList() => _tasks;

    private void UpdateReadyQueue(int currentTime)
    {
        _readyQueue.Clear();
        foreach (var task in _tasks.Where(t => 
                     t.RemainingTime > 0 && 
                     t.ReleaseTime <= currentTime))
        {
            int absoluteDeadline = task.ReleaseTime + 
                                   (task.Deadline > 0 ? task.Deadline : task.Period);
            _readyQueue.Enqueue(task, absoluteDeadline);
        }
    }
}