class Task:
    def __init__(self, name, period, bcet, wcet, priority):
        self.name = name
        self.period = period
        self.bcet = bcet
        self.wcet = wcet
        self.priority = priority

def parse_input(file_path):
    tasks = []
    with open(file_path, 'r') as file:
        for line in file:
            name, period, bcet, wcet, priority = line.strip().split(',')
            tasks.append(Task(name, int(period), int(bcet), int(wcet), int(priority)))
    return tasks

def response_time_analysis(tasks):
    for task in sorted(tasks, key=lambda x: x.priority, reverse=True):
        R = task.wcet
        while True:
            interference = 0
            for hp_task in [t for t in tasks if t.priority > task.priority]:
                interference += ((R + hp_task.period - 1) // hp_task.period) * hp_task.wcet
            new_R = task.wcet + interference
            if new_R == R:
                break
            R = new_R
        if R > task.period:
            print(f"Task {task.name} is not schedulable (R={R}, D={task.period})")
            return False
    print("All tasks are schedulable.")
    return True

def simulate(tasks):
    # Implement the simulator here
    pass

if __name__ == "__main__":
    tasks = parse_input("tasks.txt")
    if response_time_analysis(tasks):
        simulate(tasks)