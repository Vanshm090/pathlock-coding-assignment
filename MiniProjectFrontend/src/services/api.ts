import axios from 'axios';

// --- IMPORTANT ---
// Get the port from your backend terminal (the one running 'dotnet run')
const API_PORT = 5189; // <-- CHANGE THIS TO YOUR PORT
// -----------------

const api = axios.create({
  baseURL: `http://localhost:${API_PORT}/api`,
});

// This is an "interceptor". It's a function that runs
// BEFORE every single API request is sent.
api.interceptors.request.use(
  (config) => {
    // Get the token from localStorage
    const token = localStorage.getItem('jwt_token');
    
    // If the token exists, add it to the 'Authorization' header
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default api;