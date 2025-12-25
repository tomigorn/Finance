import { useEffect, useState } from "react";
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "./authConfig";

function App() {
  const { instance, accounts } = useMsal();
  const [health, setHealth] = useState<string>("Loading...");
  const [user, setUser] = useState<string | null>(null);

  // Backend health check
  useEffect(() => {
  fetch('/api/health') 
    .then((res) => res.json())
    .then((data) => setHealth(data.status))
    .catch((err) => setHealth('Error: ' + err))
}, []);


  // Login function
  const handleLogin = () => {
    instance
      .loginPopup(loginRequest)
      .then((response) => {
        setUser(response.account?.username || null);
        console.log("Login successful", response);
      })
      .catch((err) => {
        console.error("Login failed", err);
        alert("Login failed: " + err);
      });
  };

  return (
    <div style={{ padding: 20 }}>
      <h1>Backend Health Check</h1>
      <p>Status HTTPS: {health}</p>

      <h2>Microsoft Login</h2>
      {user ? (
        <p>Logged in as: {user}</p>
      ) : (
        <button onClick={handleLogin}>Login with Microsoft</button>
      )}
    </div>
  );
}

export default App;
