using System;
using System.Collections.Generic;
using System.Linq;
using _02225.Datastructure;

namespace _02225
{
    public class RTA
    {
        private CompleteCase cc;

        public RTA(CompleteCase cc)
        {
            this.cc = cc;
        }

        public void runRTA()
        {
            Console.WriteLine("Response Time Analysis");
            Console.WriteLine("Scheduling Algorithm: RateMonotonic");

            // Retrieve tasks (using private getters)
            List<ProjectTask> tasks = cc.GetTaskList().getTasks();
            var sorted = tasks.OrderBy(t => t.getPriority()).ToList();

            // Compute hyperperiod (LCM of all periods) and utilization
            long hyperperiod = sorted.Select(t => (long)t.getPeriod())
                                      .Aggregate(1L, (acc, p) => Lcm(acc, p));
            double utilization = sorted.Sum(t => (double)t.getWcet() / t.getPeriod());
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
                int Ci = ti.getWcet();
                int Di = ti.getPeriod();
                double R = Ci;
                bool schedulable = true;

                // Iteratively compute response time
                while (true)
                {
                    double previousR = R;
                    double interference = 0;

                    foreach (var hj in sorted.Where(tj => tj.getPriority() < ti.getPriority()))
                    {
                        interference += Math.Ceiling(previousR / hj.getPeriod()) * hj.getWcet();
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

                results.Add((ti.getName(), R, Di, schedulable));
            }

            // Reprint overall schedulability
            Console.WriteLine();
            Console.WriteLine($"Overall Schedulable: {allSchedulable}");
            Console.WriteLine();

            // Print each task's result
            foreach (var (Name, Wcrt, Deadline, Sched) in results)
            {
                string statusSym = Sched ? "√" : "X";
                Console.WriteLine($"{Name.PadRight(6)} {Wcrt,5:F1} {Deadline,11} {statusSym,6}");
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
