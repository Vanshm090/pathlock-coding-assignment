import { useNavigate } from 'react-router-dom';

const DashboardPage = () => {
  const navigate = useNavigate();

  // Simple logout logic for the placeholder
  const handleLogout = () => {
    localStorage.removeItem('jwt_token');
    navigate('/login');
  };

  return (
    <div style={{ padding: '2rem' }}>
      <h1>Dashboard Page (To be built)</h1>
      <p>Login was successful. This page is not yet implemented.</p>
      <button 
        onClick={handleLogout} 
        style={{ 
          padding: '0.5em 1em', 
          backgroundColor: '#535bf2', 
          color: 'white', 
          border: 'none', 
          borderRadius: '8px', 
          cursor: 'pointer' 
        }}
      >
        Logout
      </button>
    </div>
  );
};

export default DashboardPage;