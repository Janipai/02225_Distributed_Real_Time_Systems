#include <iostream>
#include <fstream>
#include <sstream>
#include <vector>
#include <algorithm>
#include <map>

using namespace std;

struct Task {
    string id;
    int wcet;           
    int bcet;           
    int period;         
    int deadline;       
    int priority;       
    int remaining_time; 
    int release_time; 
    int response_time; 
};

// Function to read tasks from a CSV file
void initialize_tasks(vector<Task>& tasks, const string& filename) {
    
    ifstream file(filename);
    string line;
    
    //skip first row
    getline(file, line); 

    while (getline(file, line)) {
        stringstream ss(line);
        string token;
        Task task;
        getline(ss, token, ',');
        task.id = token;
        getline(ss, token, ',');
        task.wcet = stoi(token);
        getline(ss, token, ',');
        task.bcet = stoi(token);
        getline(ss, token, ',');
        task.period = stoi(token);
        getline(ss, token, ',');
        task.deadline = stoi(token);
        getline(ss, token, ',');
        task.priority = stoi(token);
        task.remaining_time = task.wcet; // Initialize remaining time to WCET
        task.release_time = 0;           // Initial release time
        task.response_time = -1;         // Initially, response time is unknown
        tasks.push_back(task);
    }
}

// Function to get ready tasks at the current time
vector<Task*> get_ready_tasks(vector<Task>& tasks, int currentTime) {
    vector<Task*> ready_tasks;
    for (auto& task : tasks) {
        if (currentTime % task.period == 0) {
            task.release_time = currentTime; // Update release time
            task.remaining_time = task.wcet; // Reset remaining time for new period
        }
        if (task.release_time <= currentTime && task.remaining_time > 0) {
            ready_tasks.push_back(&task);
        }
    }
    return ready_tasks;
}

// Function to select the highest priority task from the ready list
Task* highest_priority_task(const vector<Task*>& ready_tasks) {
    return *max_element(ready_tasks.begin(), ready_tasks.end(),
        [](const Task* a, const Task* b) {
            return a->priority < b->priority;
        });
}

// Main simulation function
void simulate(vector<Task>& tasks, int simulation_time) {
    int currentTime = 0;
    map<string, int> wcrt; // Store worst-case response times

    while (currentTime <= simulation_time) {
        auto ready_tasks = get_ready_tasks(tasks, currentTime);
        if (!ready_tasks.empty()) {
            Task* current_task = highest_priority_task(ready_tasks);
            int delta = 1; // Advance time by 1 unit
            currentTime += delta;
            current_task->remaining_time -= delta;

            if (current_task->remaining_time <= 0) {
                current_task->response_time = currentTime - current_task->release_time;
                if (wcrt.find(current_task->id) == wcrt.end() || wcrt[current_task->id] < current_task->response_time) {
                    wcrt[current_task->id] = current_task->response_time; // Update WCRT
                }
                cout << "Task " << current_task->id << " completed with response time " << current_task->response_time << endl;
            }
        } else {
            currentTime += 1; // No tasks are ready, just advance time
        }
    }

    // Output worst-case response times (WCRT) for all tasks
    cout << "\nWorst-Case Response Times (WCRT):" << endl;
    for (const auto& task : tasks) {
        cout << "Task " << task.id << ": " << wcrt[task.id] << endl;
    }
}

int main() {
    vector<Task> tasks;
    initialize_tasks(tasks, "data/exercise-TC1.csv");

    int simulation_time = 100; // Set simulation time
    simulate(tasks, simulation_time);

    return 0;
}