import { useState, useEffect } from "react"
import Login from "./Login"
import Dashboard from "./Dashboard"

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false)

  useEffect(() => {
    // Verificar se há token no localStorage
    const token = localStorage.getItem("token")
    if (token) {
      setIsLoggedIn(true)
    }
  }, [])

  // Escutar mudanças no localStorage (para logout)
  useEffect(() => {
    const handleStorageChange = () => {
      const token = localStorage.getItem("token")
      setIsLoggedIn(!!token)
    }

    window.addEventListener("storage", handleStorageChange)
    
    // Verificar periodicamente (para logout em outras abas)
    const interval = setInterval(() => {
      const token = localStorage.getItem("token")
      setIsLoggedIn(!!token)
    }, 1000)

    return () => {
      window.removeEventListener("storage", handleStorageChange)
      clearInterval(interval)
    }
  }, [])

  return (
    <div style={{ 
      minHeight: "100vh", 
      backgroundColor: "#f8f9fa",
      fontFamily: "Arial, sans-serif"
    }}>
      {isLoggedIn ? <Dashboard /> : <Login />}
    </div>
  )
}

export default App
