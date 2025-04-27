using System;
using System.Collections.Generic;
using System.Linq;
using _02225.Entities;

namespace _02225
{
    public class Rta
    {
        private CompleteCase _cc;

        public Rta(CompleteCase cc)
        {
            this._cc = cc;
        }

        public void RunRta()
        {
            Console.WriteLine("Response Time Analysis");
            Console.WriteLine("Scheduling Algorithm: RateMonotonic");

            // Retrieve tasks (using private getters)
            List<ProjectTask> tasks = _cc.GetTaskList().GetTasks();
            var sorted = tasks.OrderBy(t => t.GetPriority()).ToList();

            // Compute hyperperiod (LCM of all periods) and utilization
            long hyperperiod = sorted.Select(t => (long)t.GetPeriod())
                                      .Aggregate(1L, (acc, p) => Lcm(acc, p));
            double utilization = sorted.Sum(t => (double)t.GetWcet() / t.GetPeriod());
            bool allSchedulable = true;

            Console.WriteLine($"Schedulable: {allSchedulable}");  // Will update after analysis
            Console.WriteLine($"Hyperperiod: {hyperperiod}");
            Console.WriteLine($"Utilization: {utilization:F2}");
            Console.WriteLine("Status: ( \u2713 = schedulable, X = not schedulable)");
            Console.WriteLine();

            // Table header
            Console.WriteLine("Task   WCRT    Deadline    Status");
            Console.WriteLine("---------------------------------");

            // Collect results
            var results = new List<(string Name, double Wcrt, int Deadline, bool Sched)>();

            foreach (var ti in sorted)
            {
                int Ci = ti.GetWcet();
                int Di = ti.GetPeriod();
                double R = Ci;
                bool schedulable = true;

                // Iteratively compute response time
                while (true)
                {
                    double previousR = R;
                    double interference = 0;

                    foreach (var hj in sorted.Where(tj => tj.GetPriority() < ti.GetPriority()))
                    {
                        interference += Math.Ceiling(previousR / hj.GetPeriod()) * hj.GetWcet();
                    }

                    R = Ci + interference;
                    if (R > Di)
                    {
                        schedulable = false;
                        break;
                    }

                    if (Math.Abs(R - previousR) < 1e-6)
                        break;
                }

                if (!schedulable)
                    allSchedulable = false;

                results.Add((ti.GetName(), R, Di, schedulable));
            }

            // Reprint overall schedulability
            Console.WriteLine();
            Console.WriteLine($"Overall Schedulable: {allSchedulable}");
            Console.WriteLine();

            // Print each task's result
            foreach (var (name, wcrt, deadline, sched) in results)
            {
                string statusSym = sched ? "√" : "X";
                Console.WriteLine($"{name.PadRight(6)} {wcrt,5:F1} {deadline,11} {statusSym,6}");
            }
        }

        // Utility methods for hyperperiod
        private long Gcd(long a, long b)
        {
            while (b != 0)
            {
                var t = b;
                b = a % b;
                a = t;
            }
            return a;
        }

        private long Lcm(long a, long b)
        {
            return a / Gcd(a, b) * b;
        }
    }
}