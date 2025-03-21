# Real-Time Scheduling Simulator

This repository contains a C# implementation of a real-time scheduling simulator that demonstrates two key concepts in real-time systems:

- **Rate Monotonic Scheduling (RMS):** A priority-based scheduling algorithm where tasks with shorter periods are given higher priority.
- **Response-Time Analysis (RTA):** A method to compute the worst-case response times (WCRT) of tasks to verify whether a set of tasks can meet their deadlines under the given scheduling policy.

## Overview

The tool simulates RMS by reading a set of tasks from a CSV file and executing them over a fixed simulation time. During the simulation, it dynamically handles task releases, preemptions, and completions. After simulation, the Response-Time Analysis (RTA) is performed to evaluate the schedulability of the task set by computing the worst-case response time for each task.

## Repository Structure

- **VSS.cs:** Main source file containing the implementation of task initialization, simulation, and RTA.
- **data/exercise-TC1.csv:** Example CSV file containing task definitions. Each task includes parameters such as WCET, BCET, Period, Deadline, etc.
- **README.md:** This documentation file.

## How to Run

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.0 or later)

### Steps

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/janipai/02225_Distributed_Real_Time-Systems.git
   cd 02225_Distributed_Real_Time-Systems
   
2 **Prepare the CSV Data File:**
    Ensure that the CSV file (exercise-TC1.csv) is located in the data folder as referenced in the code.

3 **Build and Run the Simulator:**
Open a terminal in the repository directory and, cd into ```02225_Distributed_Real_Time_Systems/Exercises``` and run:
```bash
    dotnet build
```
and then run:
```bash
    dotnet run
```

