import { useEffect, useState } from "react";
import { useMsal } from "@azure/msal-react";
import { loginRequest } from "./authConfig";

function App() {
  const { instance, accounts } = useMsal();
  const isLoggedIn = accounts.length > 0;
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

  const handleLogout = async () => {
    await instance.logoutPopup({
      postLogoutRedirectUri: "https://localhost:5173",
    });
  };


  return (
    <div style={{ padding: 20 }}>
      <h1>Microsoft Login</h1>

      {isLoggedIn ? (
        <>
          <p>Logged in as: {accounts[0].username}</p>
          <button onClick={handleLogout}>Logout</button>
        </>
      ) : (
        <>
          <p>Not logged in</p>
          <button onClick={handleLogin}>Login</button>
        </>
      )}
    </div>
  );
}

export default App;
