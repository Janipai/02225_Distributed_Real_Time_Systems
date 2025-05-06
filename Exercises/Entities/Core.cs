namespace _02225.Entities;

    public class Core
    {
        public string CoreId { get; set; }
        public readonly double Speed;
        public readonly string Scheduler;

        public Core(string coreId, double speed, string scheduler)
        {
            this.CoreId = coreId;
            this.Speed = speed;
            this.Scheduler = scheduler;
        }

        public string PrintCore() => $"Core ID: {CoreId}, Speed: {Speed} multiplier, Scheduler: {Scheduler}";
        public string Get() => CoreId;
        public string GetScheduler() => Scheduler;
        public double GetSpeed() => Speed;
    }
