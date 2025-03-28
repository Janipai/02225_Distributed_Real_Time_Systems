namespace _02225;

public class Task
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