namespace _02225.Entities;

    public class Core
    {
        private string CoreId { get; set; }
        private readonly double _speed;
        private readonly string _scheduler;

        public Core(string coreId, double speed, string scheduler)
        {
            this.CoreId = coreId;
            this._speed = speed;
            this._scheduler = scheduler;
        }

        public string PrintCore() => $"Core ID: {CoreId}, Speed: {_speed} multiplier, Scheduler: {_scheduler}";
        public string Get() => CoreId;
        public string GetScheduler() => _scheduler;
        public double GetSpeed() => _speed;
    }
