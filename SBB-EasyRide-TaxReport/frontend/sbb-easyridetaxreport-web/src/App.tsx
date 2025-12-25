import { useEffect, useState } from 'react'

function App() {
  const [health, setHealth] = useState<string>('Loading...')

  useEffect(() => {
    fetch('https://localhost:7145/api/health')
      .then((res) => res.json())
      .then((data) => setHealth(data.status))
      .catch((err) => setHealth('Error: ' + err))
  }, [])

  return (
    <div style={{ padding: 20 }}>
      <h1>Backend Health Check</h1>
      <p>Status HTTPS: {health}</p>
    </div>
  )
}

export default App
