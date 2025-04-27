using System;
using System.Collections.Generic;
using System.Linq;
using _02225.Entities;

namespace _02225
{
    // Internal helper to track each job
    class Job
    {
        public ProjectTask Task;
        public double ReleaseTime;
        public double Deadline;
        public double RemainingTime;
    }

    public class Scheduler
    {
        private readonly CompleteCase _cc;

        public Scheduler(CompleteCase cc)
        {
            this._cc = cc;
        }

        public void RunScheduler()
        {
            Console.WriteLine("Running Scheduler...");
            int simTime = _cc.GetSimulationTime();

            foreach (var comp in _cc.GetBudget().GetComponents())
            {
                var compId = comp.Get();
                var core = comp.GetCore();
                var speed = core.GetSpeed();
                var tasks = comp.GetChildTasks();
                var policy = comp.GetScheduler();

                // Store response times per task
                var respStore = tasks.ToDictionary(t => t, t => new List<double>());

                // Display component header with core speed
                Console.WriteLine($"\n=== Component {compId} on Core '{core.Get()}' [Speed={speed:F2}] ({policy}) ===");

                // Simulate according to component-level scheduler
                if (policy.Equals("EDF", StringComparison.OrdinalIgnoreCase))
                    SimulateEdf(tasks, simTime, respStore, speed);
                else
                    SimulateFPS(tasks, simTime, respStore, speed);

                // Determine overall component schedulability
                bool compSched = respStore.All(kv => kv.Value.All(rt => rt <= kv.Key.GetPeriod()));

                // Display results
                Console.WriteLine($"Component Schedulable: {(compSched ? "Yes" : "No")}");
                Console.WriteLine("Task   | Schedulable | Avg RT | Max RT");
                Console.WriteLine("--------------------------------------");

                foreach (var task in tasks)
                {
                    var rts = respStore[task];
                    double avg = rts.Any() ? rts.Average() : 0.0;
                    double max = rts.Any() ? rts.Max() : 0.0;
                    bool sched = rts.All(rt => rt <= task.GetPeriod());

                    Console.WriteLine(
                        $"{task.GetName(),-6} | {(sched ? "Yes" : "No"),-11} | {avg,6:F1} | {max,6:F1}");
                }
            }
        }

        private void SimulateFPS(List<ProjectTask> tasks, int simTime, Dictionary<ProjectTask, List<double>> respStore, double speed)
        {
            var jobs = new List<Job>();
            foreach (var t in tasks)
            {
                double wcetAdj = t.GetWcet() / speed;
                for (int r = 0; r < simTime; r += t.GetPeriod())
                {
                    jobs.Add(new Job {
                        Task = t,
                        ReleaseTime = r,
                        Deadline = r + t.GetPeriod(),
                        RemainingTime = wcetAdj
                    });
                }
            }
            
            Console.WriteLine($"[DEBUG] Created {jobs.Count} total jobs for {tasks.Count} tasks.");


            double now = 0;
            var pending = new List<Job>(jobs);

            // Run until all jobs are completed or time expires
            while (now < simTime && pending.Any())
            {
                // Find all jobs that have been released
                var ready = pending.Where(j => j.ReleaseTime <= now).ToList();
                if (!ready.Any())
                {
                    // No ready jobs => fast-forward to next release
                    double nextRelease = pending.Min(j => j.ReleaseTime);
                    now = nextRelease;
                    continue;
                }

                // Select highest-priority job (lower priority value = higher priority)
                var job = ready.OrderBy(j => j.Task.GetPriority()).First();
                double toFinish = job.RemainingTime;
                double nextRel = pending.Where(j => j.ReleaseTime > now)
                                         .DefaultIfEmpty()
                                         .Min(j => j?.ReleaseTime ?? simTime);
                double delta = Math.Min(toFinish, nextRel - now);

                now += delta;
                job.RemainingTime -= delta;

                if (job.RemainingTime <= 0)
                {
                    respStore[job.Task].Add(now - job.ReleaseTime);
                    pending.Remove(job);
                }
            }
            foreach (var t in tasks)
                Console.WriteLine($"[DEBUG] {t.GetName()} recorded {respStore[t].Count} samples");

        }

        private void SimulateEdf(List<ProjectTask> tasks, int simTime, Dictionary<ProjectTask, List<double>> respStore, double speed)
        {
            var jobs = new List<Job>();
            foreach (var t in tasks)
            {
                double wcetAdj = t.GetWcet() / speed;
                for (int r = 0; r < simTime; r += t.GetPeriod())
                {
                    jobs.Add(new Job {
                        Task = t,
                        ReleaseTime = r,
                        Deadline = r + t.GetPeriod(),
                        RemainingTime = wcetAdj
                    });
                }
            }
            
            Console.WriteLine($"[DEBUG] Created {jobs.Count} total jobs for {tasks.Count} tasks.");


            double now = 0;
            var pending = new List<Job>(jobs);

            // Run until all jobs are completed or time expires
            while (now < simTime && pending.Any())
            {
                // Find all jobs that have been released
                var ready = pending.Where(j => j.ReleaseTime <= now).ToList();
                if (!ready.Any())
                {
                    // No ready jobs => fast-forward to next release
                    double nextRelease = pending.Min(j => j.ReleaseTime);
                    now = nextRelease;
                    continue;
                }

                // Select earliest-deadline job
                var job = ready.OrderBy(j => j.Deadline).First();
                double toFinish = job.RemainingTime;
                double nextRel = pending.Where(j => j.ReleaseTime > now)
                                         .DefaultIfEmpty()
                                         .Min(j => j?.ReleaseTime ?? simTime);
                double delta = Math.Min(toFinish, nextRel - now);

                now += delta;
                job.RemainingTime -= delta;

                if (job.RemainingTime <= 0)
                {
                    respStore[job.Task].Add(now - job.ReleaseTime);
                    pending.Remove(job);
                }
            }
            foreach (var t in tasks)
                Console.WriteLine($"[DEBUG] {t.GetName()} recorded {respStore[t].Count} samples");
        }
        

    }
}