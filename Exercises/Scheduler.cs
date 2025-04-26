using System;
using System.Collections.Generic;
using System.Linq;
using _02225.Datastructure;

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
        private readonly CompleteCase cc;

        public Scheduler(CompleteCase cc)
        {
            this.cc = cc;
        }

        public void runScheduler()
        {
            Console.WriteLine("Running Scheduler...");
            int simTime = cc.getSimulationTime();

            foreach (var comp in cc.GetBudget().getComponents())
            {
                var compId = comp.get();
                var core = comp.getCore();
                var speed = core.getSpeed();
                var tasks = comp.getChildTasks();
                var policy = comp.getScheduler();

                // Store response times per task
                var respStore = tasks.ToDictionary(t => t, t => new List<double>());

                // Display component header with core speed
                Console.WriteLine($"\n=== Component {compId} on Core '{core.get()}' [Speed={speed:F2}] ({policy}) ===");

                // Simulate according to component-level scheduler
                if (policy.Equals("EDF", StringComparison.OrdinalIgnoreCase))
                    SimulateEDF(tasks, simTime, respStore, speed);
                else
                    SimulateFPS(tasks, simTime, respStore, speed);

                // Determine overall component schedulability
                bool compSched = respStore.All(kv => kv.Value.All(rt => rt <= kv.Key.getPeriod()));

                // Display results
                Console.WriteLine($"Component Schedulable: {(compSched ? "Yes" : "No")}");
                Console.WriteLine("Task   | Schedulable | Avg RT | Max RT");
                Console.WriteLine("--------------------------------------");

                foreach (var task in tasks)
                {
                    var rts = respStore[task];
                    double avg = rts.Any() ? rts.Average() : 0.0;
                    double max = rts.Any() ? rts.Max() : 0.0;
                    bool sched = rts.All(rt => rt <= task.getPeriod());

                    Console.WriteLine(
                        $"{task.getName(),-6} | {(sched ? "Yes" : "No"),-11} | {avg,6:F1} | {max,6:F1}");
                }
            }
        }

        private void SimulateFPS(List<ProjectTask> tasks, int simTime, Dictionary<ProjectTask, List<double>> respStore, double speed)
        {
            var jobs = new List<Job>();
            foreach (var t in tasks)
            {
                double wcetAdj = t.getWcet() / speed;
                for (int r = 0; r < simTime; r += t.getPeriod())
                {
                    jobs.Add(new Job {
                        Task = t,
                        ReleaseTime = r,
                        Deadline = r + t.getPeriod(),
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
                var job = ready.OrderBy(j => j.Task.getPriority()).First();
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
                Console.WriteLine($"[DEBUG] {t.getName()} recorded {respStore[t].Count} samples");

        }

        private void SimulateEDF(List<ProjectTask> tasks, int simTime, Dictionary<ProjectTask, List<double>> respStore, double speed)
        {
            var jobs = new List<Job>();
            foreach (var t in tasks)
            {
                double wcetAdj = t.getWcet() / speed;
                for (int r = 0; r < simTime; r += t.getPeriod())
                {
                    jobs.Add(new Job {
                        Task = t,
                        ReleaseTime = r,
                        Deadline = r + t.getPeriod(),
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
                Console.WriteLine($"[DEBUG] {t.getName()} recorded {respStore[t].Count} samples");
        }
        

    }
}
