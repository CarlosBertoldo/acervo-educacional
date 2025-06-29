import { useState } from "react"

function Login() {
  const [email, setEmail] = useState("admin@acervoeducacional.com")
  const [password, setPassword] = useState("Admin@123")
  const [message, setMessage] = useState("")
  const [loading, setLoading] = useState(false)

  const handleLogin = async (e) => {
    e.preventDefault()
    setLoading(true)
    setMessage("")
    
    try {
      const response = await fetch("http://localhost:5007/api/auth/login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email: email,
          password: password
        } )
      })
      
      console.log("Response status:", response.status)
      const data = await response.json()
      console.log("Response data:", data)
      
      if (response.ok && data.success) {
        setMessage("✅ Login realizado com sucesso! Redirecionando...")
        localStorage.setItem("token", data.accessToken || data.token)
        setTimeout(() => window.location.reload(), 1000)
      } else {
        setMessage("❌ " + (data.message || "Erro no login"))
      }
    } catch (error) {
      console.error("Erro:", error)
      setMessage("❌ Erro ao conectar com o servidor: " + error.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div style={{ 
      maxWidth: "400px", 
      margin: "50px auto", 
      padding: "20px",
      border: "1px solid #ddd",
      borderRadius: "8px",
      backgroundColor: "white",
      boxShadow: "0 2px 10px rgba(0,0,0,0.1)"
    }}>
      <h2 style={{ textAlign: "center", color: "#C12D00", marginBottom: "10px" }}>
        🎓 Acervo Educacional
      </h2>
      <p style={{ textAlign: "center", marginBottom: "30px", color: "#666" }}>
        Ferreira Costa
      </p>
      
      <form onSubmit={handleLogin}>
        <div style={{ marginBottom: "15px" }}>
          <label style={{ display: "block", marginBottom: "5px", fontWeight: "bold" }}>
            Email:
          </label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            placeholder="admin@acervoeducacional.com"
            style={{
              width: "100%",
              padding: "12px",
              border: "1px solid #ddd",
              borderRadius: "4px",
              fontSize: "14px"
            }}
            required
          />
        </div>
        
        <div style={{ marginBottom: "20px" }}>
          <label style={{ display: "block", marginBottom: "5px", fontWeight: "bold" }}>
            Senha:
          </label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Admin@123"
            style={{
              width: "100%",
              padding: "12px",
              border: "1px solid #ddd",
              borderRadius: "4px",
              fontSize: "14px"
            }}
            required
          />
        </div>
        
        <button
          type="submit"
          disabled={loading}
          style={{
            width: "100%",
            padding: "12px",
            backgroundColor: loading ? "#ccc" : "#C12D00",
            color: "white",
            border: "none",
            borderRadius: "4px",
            cursor: loading ? "not-allowed" : "pointer",
            fontSize: "16px",
            fontWeight: "bold"
          }}
        >
          {loading ? "Entrando..." : "Entrar"}
        </button>
      </form>
      
      {message && (
        <div style={{ 
          marginTop: "15px", 
          padding: "12px", 
          textAlign: "center",
          backgroundColor: message.includes("✅") ? "#d4edda" : "#f8d7da",
          border: "1px solid " + (message.includes("✅") ? "#c3e6cb" : "#f5c6cb"),
          borderRadius: "4px",
          fontSize: "14px"
        }}>
          {message}
        </div>
      )}
      
      <div style={{ 
        marginTop: "20px", 
        padding: "12px", 
        backgroundColor: "#e7f3ff",
        borderRadius: "4px",
        fontSize: "12px"
      }}>
        <strong>🔑 Credenciais de teste:</strong><br/>
        📧 admin@acervoeducacional.com<br/>
        🔒 Admin@123
      </div>
    </div>
  )
}

export default Login