namespace _02225.Datastructure
{

    public class Core
    {
        private string coreID { get; set; } = string.Empty;
        private double speed = 0.0;
        private string scheduler = string.Empty;

        public Core(string coreID, double speed, string scheduler)
        {
            this.coreID = coreID;
            this.speed = speed;
            this.scheduler = scheduler;
        }

        public string printCore() => $"Core ID: {coreID}, Speed: {speed} multiplier, Scheduler: {scheduler}";
        public string get() => coreID;
        public string GetScheduler() => scheduler;
        public double getSpeed() => speed;
    }
}