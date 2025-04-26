namespace _02225;

public class BoundedDelayResource
{
    public double Alpha { get; }
    public int Delta { get; }
    
    public BoundedDelayResource(double alpha, int delta) 
    {
        Alpha = alpha;
        Delta = delta;
    }
    
    public int GetSupply(int time) => 
        Math.Max(0, (int)(Alpha * (time - Delta)));
}