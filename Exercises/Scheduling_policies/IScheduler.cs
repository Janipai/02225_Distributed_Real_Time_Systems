namespace _02225.Scheduling_policies;

public interface IScheduler
{
    Task GetNextTask(int currentTime);
    void AddTask(Task task);
    bool IsSchedulable(List<Task> tasks);
}