import heapq

class Task:
    def __init__(self, name, period, bcet, wcet):
        self.name = name
        self.period = period
        self.bcet = bcet
        self.wcet = wcet
        self.priority = 1 / period  # Rate Monotonic priority
        self.release_time = 0
        self.execution_time = 0
        self.response_time = 0

    def __lt__(self, other):
        # Higher priority tasks come first
        return self.priority > other.priority

def parse_input(file_path):
    tasks = []
    with open(file_path, 'r') as file:
        for line in file:
            name, period, bcet, wcet = line.strip().split(',')
            tasks.append(Task(name, int(period), int(bcet), int(wcet)))
    return tasks

def simulate(tasks, sim_time):
    # Initialize task release times and ready queue
    for task in tasks:
        task.release_time = 0
    ready_queue = []
    current_time = 0

    # Initialize worst-case response times
    wcrt = {task.name: 0 for task in tasks}

    while current_time < sim_time:
        # Check for task releases
        for task in tasks:
            if current_time % task.period == 0:
                task.release_time = current_time
                task.execution_time = 0
                heapq.heappush(ready_queue, task)

        if ready_queue:
            # Select the highest-priority task
            current_task = heapq.heappop(ready_queue)
            print(f"Time {current_time}: Executing {current_task.name}")

            # Execute the task for one time unit
            current_task.execution_time += 1
            current_time += 1

            # Check if the task has completed
            if current_task.execution_time == current_task.wcet:
                # Compute response time
                response_time = current_time - current_task.release_time
                if response_time > wcrt[current_task.name]:
                    wcrt[current_task.name] = response_time
                print(f"Task {current_task.name} completed at time {current_time}")
            else:
                # Re-add the task to the ready queue
                heapq.heappush(ready_queue, current_task)
        else:
            # No tasks to execute, advance time
            current_time += 1

    return wcrt

if __name__ == "__main__":
    tasks = parse_input("tasks.txt")
    sim_time = 100  # Simulation time
    wcrt = simulate(tasks, sim_time)
    print("Worst-Case Response Times (WCRT):")
    for task_name, response_time in wcrt.items():
        print(f"{task_name}: {response_time}")