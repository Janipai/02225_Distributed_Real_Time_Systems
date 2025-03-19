using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DefaultNamespace
{
    public class VSS
    {
        private int n = 0; // number of cycles
        public int currentTime = 0;
        private List<Job> jobList = new();
        private Dictionary<string, int> wcResponseTime = new(); // Store WCRT by task name

        public void InitializePriorities()
        {
            jobList = jobList.OrderBy(job => job.Period).ToList();

            int priorityLevel = 1;
            foreach (Job job in jobList)
            {
                job.Priority = priorityLevel;
                priorityLevel++;
            }
        }

        public void InitializeJobs(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(',');

                    Job job = new Job
                    {
                        Task = values[0],
                        WCET = int.Parse(values[1]),
                        BCET = int.Parse(values[2]),
                        Period = int.Parse(values[3]),
                        Deadline = int.Parse(values[4]),
                        Priority = int.Parse(values[5]),
                        Release = 0,
                        Time = int.Parse(values[1]), // WCET as initial execution time
                        IsFinished = false
                    };

                    n++;
                    jobList.Add(job);
                }
            }
            InitializePriorities(); // Assign priorities after loading jobs
        }

        public int AdvanceTime()
        {
            int nextRelease = jobList
                .Where(j => j.Release > currentTime && !j.IsFinished)
                .Select(j => j.Release)
                .DefaultIfEmpty(int.MaxValue)
                .Min();

            return Math.Max(1, nextRelease - currentTime);
        }

        public List<Job> GetReadyJobs()
        {
            return jobList.Where(job => job.Release <= currentTime && !job.IsFinished).ToList();
        }

        public void VVS()
        {
            while (currentTime <= n && jobList.Any(job => !job.IsFinished))
            {
                List<Job> readyList = GetReadyJobs();
                Job currentJob = readyList.OrderBy(job => job.Priority).FirstOrDefault();

                if (currentJob != null)
                {
                    int delta = AdvanceTime();
                    currentTime += delta;
                    currentJob.Time -= delta;

                    if (currentTime > currentJob.Deadline)
                    {
                        Console.WriteLine($"Job {currentJob.Task} missed its deadline at time {currentTime}");
                    }

                    if (currentJob.Time <= 0) // Task completed
                    {
                        int responseTime = currentTime - currentJob.Release;

                        // ✅ FIX: Use currentJob.Task (string) instead of currentJob (Job)
                        if (wcResponseTime.ContainsKey(currentJob.Task))
                        {
                            wcResponseTime[currentJob.Task] = Math.Max(wcResponseTime[currentJob.Task], responseTime);
                        }
                        else
                        {
                            wcResponseTime[currentJob.Task] = responseTime;
                        }

                        currentJob.IsFinished = true;
                    }
                }
                else
                {
                    currentTime += AdvanceTime();
                }
            }

            PrintResults();
        }

        private void PrintResults()
        {
            Console.WriteLine("\nWorst-Case Response Times (WCRT) per Task:");
            foreach (var entry in wcResponseTime)
            {
                Console.WriteLine($"Task {entry.Key}: WCRT = {entry.Value}");
            }
        }

		public List<Job> GetJobs()
    	{
    	    return jobList;
    	}
    }
}
