import { Routes, Route, Navigate } from 'react-router-dom';

// Import the real pages
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
// Import the placeholder dashboard
import DashboardPage from './pages/DashboardPage';

// We will create this component in the next step
// import ProjectDetailPage from './pages/ProjectDetailPage';

function App() {

  // A simple component to show while we build the real one
  const ProjectDetailPage = () => <h1>Project Detail Page (To be built)</h1>

  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/dashboard" element={<DashboardPage />} /> {/* <-- This now points to your placeholder */}
      <Route path="/project/:projectId" element={<ProjectDetailPage />} />
      <Route path="*" element={<Navigate to="/login" />} />
    </Routes>
  );
}

export default App;