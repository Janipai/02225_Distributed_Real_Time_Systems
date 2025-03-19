namespace DefaultNamespace;

public class Job
{
    public string Task { get; set; }
    public int WCET { get; set; }
    public int BCET { get; set; }
    public int Period { get; set; }
    public int Deadline { get; set; }
    public int Priority { get; set; }
    public int Release { get; set; }
    public int Time { get; set; }
    public int Response { get; set; }
    public bool IsFinished { get; set; } = false;
}