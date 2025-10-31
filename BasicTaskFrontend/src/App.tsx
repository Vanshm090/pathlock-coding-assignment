import { useState, useEffect } from 'react';
import axios from 'axios';
import './App.css'; // We'll create this file next for styling

// --- 1. Define the Types ---

// This must match the TaskItem class in your C# backend
interface TaskItem {
  id: string; // GUID in C# is a string in JSON
  description: string;
  isCompleted: boolean;
}

// This is the base URL of your running backend API
const API_URL = "http://localhost:5000/api/tasks";

function App() {
  // --- 2. State Management (React Hooks) ---
  const [tasks, setTasks] = useState<TaskItem[]>([]);
  const [newTaskDesc, setNewTaskDesc] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // --- 3. Fetch Initial Data ---
  // This useEffect hook runs once when the component loads
  useEffect(() => {
    fetchTasks();
  }, []);

  const fetchTasks = async () => {
    try {
      setLoading(true);
      setError(null);
      const response = await axios.get<TaskItem[]>(API_URL);
      setTasks(response.data);
    } catch (err) {
      setError("Failed to fetch tasks. Make sure the backend API is running.");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // --- 4. API Functions ---

  // Add a new task [POST]
  const handleAddTask = async (e: React.FormEvent) => {
    e.preventDefault(); // Prevent page reload on form submit
    if (!newTaskDesc.trim()) return; // Don't add empty tasks

    try {
      const response = await axios.post<TaskItem>(API_URL, {
        description: newTaskDesc,
      });
      // Add the new task to the list in state
      setTasks([...tasks, response.data]);
      setNewTaskDesc(""); // Clear the input box
    } catch (err) {
      setError("Failed to add task.");
    }
  };

  // Toggle completion status [PUT]
  const handleToggleComplete = async (taskToToggle: TaskItem) => {
    try {
      // Create the updated task object
      const updatedTask = {
        ...taskToToggle,
        isCompleted: !taskToToggle.isCompleted,
      };

      // Send the PUT request
      await axios.put(`${API_URL}/${taskToToggle.id}`, {
        description: updatedTask.description,
        isCompleted: updatedTask.isCompleted,
      });

      // Update the state
      setTasks(
        tasks.map((task) =>
          task.id === taskToToggle.id ? updatedTask : task
        )
      );
    } catch (err) {
      setError("Failed to update task.");
    }
  };

  // Delete a task [DELETE]
  const handleDeleteTask = async (id: string) => {
    try {
      await axios.delete(`${API_URL}/${id}`);
      // Filter out the deleted task from state
      setTasks(tasks.filter((task) => task.id !== id));
    } catch (err) {
      setError("Failed to delete task.");
    }
  };

  // --- 5. Render the UI (JSX) ---
  return (
    <div className="app-container">
      <h1>Basic Task Manager</h1>
      <p>A simple React + C# Full-Stack App</p>

      {/* --- Add Task Form --- */}
      <form className="add-task-form" onSubmit={handleAddTask}>
        <input
          type="text"
          value={newTaskDesc}
          onChange={(e) => setNewTaskDesc(e.target.value)}
          placeholder="Enter a new task..."
        />
        <button type="submit">Add Task</button>
      </form>

      {/* --- Loading and Error Display --- */}
      {loading && <div className="loading">Loading tasks...</div>}
      {error && <div className="error">{error}</div>}

      {/* --- Task List --- */}
      <ul className="task-list">
        {tasks.map((task) => (
          <li key={task.id} className={task.isCompleted ? 'completed' : ''}>
            <span
              className="task-description"
              onClick={() => handleToggleComplete(task)}
            >
              {task.description}
            </span>
            <button
              className="delete-btn"
              onClick={() => handleDeleteTask(task.id)}
            >
              &times;
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}

export default App;