namespace DefaultNamespace;

using Simulation;
using System;
using System.Collections;
public class Program
{
    public static void Main(string[] args)
    {
        // Configurations
        string filePath = @"C:\Users\phra\DTU\02225_Distributed_Real_Time_Systems\Exercises\data\exercise-TC1.csv";
        int simulationTime = 4;
        
        // Get tasks and ensure correct value
        List<Task> tasks = TaskLoader.GetTasks(filePath);
        foreach (var task in tasks)
        {
            Console.WriteLine($"Task: {task.Name}, WCET: {task.WCET}, BCET: {task.BCET}, Period: {task.Period}, Deadline: {task.Deadline}, Priority: {task.Priority}");
        }
        
        // Start simulation
        Simulator simulator = new Simulator(tasks, simulationTime);
        List<int> WCResponseTimes = simulator.Start();
        
        Console.WriteLine("Simulation Response Times:");
        foreach (var rt in WCResponseTimes)
        {
            Console.WriteLine(rt);
        }
    }
}