import { useState, useEffect } from "react"

function Dashboard() {
  const [stats, setStats] = useState(null)
  const [cursos, setCursos] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchDashboardData()
    fetchCursos()
  }, [])

  const fetchDashboardData = async () => {
    try {
      const response = await fetch("http://localhost:5007/api/dashboard/stats" )
      const data = await response.json()
      setStats(data)
    } catch (error) {
      console.error("Erro ao buscar estatÃ­sticas:", error)
    }
  }

  const fetchCursos = async () => {
    try {
      const response = await fetch("http://localhost:5007/api/cursos" )
      const data = await response.json()
      setCursos(data.data || data)
      setLoading(false)
    } catch (error) {
      console.error("Erro ao buscar cursos:", error)
      setLoading(false)
    }
  }

  const handleLogout = () => {
    localStorage.removeItem("token")
    window.location.reload()
  }

  if (loading) {
    return (
      <div style={{ textAlign: "center", padding: "50px" }}>
        <h2>Carregando...</h2>
      </div>
    )
  }

  return (
    <div style={{ minHeight: "100vh", backgroundColor: "#f8f9fa" }}>
      {/* Header */}
      <header style={{
        backgroundColor: "#C12D00",
        color: "white",
        padding: "15px 0",
        marginBottom: "30px"
      }}>
        <div style={{
          maxWidth: "1200px",
          margin: "0 auto",
          padding: "0 20px",
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center"
        }}>
          <h1 style={{ margin: 0 }}>í¾“ Acervo Educacional - Ferreira Costa</h1>
          <button
            onClick={handleLogout}
            style={{
              backgroundColor: "rgba(255,255,255,0.2)",
              color: "white",
              border: "1px solid white",
              padding: "8px 16px",
              borderRadius: "4px",
              cursor: "pointer"
            }}
          >
            Sair
          </button>
        </div>
      </header>

      <div style={{ maxWidth: "1200px", margin: "0 auto", padding: "0 20px" }}>
        {/* EstatÃ­sticas */}
        {stats && (
          <div style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fit, minmax(250px, 1fr))",
            gap: "20px",
            marginBottom: "30px"
          }}>
            <div style={{
              backgroundColor: "white",
              padding: "20px",
              borderRadius: "8px",
              boxShadow: "0 2px 10px rgba(0,0,0,0.1)",
              textAlign: "center"
            }}>
              <h3 style={{ color: "#C12D00", margin: "0 0 10px 0" }}>Total de Cursos</h3>
              <p style={{ fontSize: "32px", fontWeight: "bold", margin: 0, color: "#333" }}>
                {stats.total_cursos}
              </p>
            </div>

            <div style={{
              backgroundColor: "white",
              padding: "20px",
              borderRadius: "8px",
              boxShadow: "0 2px 10px rgba(0,0,0,0.1)",
              textAlign: "center"
            }}>
              <h3 style={{ color: "#8FBF00", margin: "0 0 10px 0" }}>Cursos Ativos</h3>
              <p style={{ fontSize: "32px", fontWeight: "bold", margin: 0, color: "#333" }}>
                {stats.cursos_ativos}
              </p>
            </div>

            <div style={{
              backgroundColor: "white",
              padding: "20px",
              borderRadius: "8px",
              boxShadow: "0 2px 10px rgba(0,0,0,0.1)",
              textAlign: "center"
            }}>
              <h3 style={{ color: "#007bff", margin: "0 0 10px 0" }}>Em Desenvolvimento</h3>
              <p style={{ fontSize: "32px", fontWeight: "bold", margin: 0, color: "#333" }}>
                {stats.cursos_desenvolvimento}
              </p>
            </div>

            <div style={{
              backgroundColor: "white",
              padding: "20px",
              borderRadius: "8px",
              boxShadow: "0 2px 10px rgba(0,0,0,0.1)",
              textAlign: "center"
            }}>
              <h3 style={{ color: "#6c757d", margin: "0 0 10px 0" }}>Total UsuÃ¡rios</h3>
              <p style={{ fontSize: "32px", fontWeight: "bold", margin: 0, color: "#333" }}>
                {stats.total_usuarios}
              </p>
            </div>
          </div>
        )}

        {/* Lista de Cursos */}
        <div style={{
          backgroundColor: "white",
          padding: "20px",
          borderRadius: "8px",
          boxShadow: "0 2px 10px rgba(0,0,0,0.1)"
        }}>
          <h2 style={{ color: "#C12D00", marginBottom: "20px" }}>í³š Cursos DisponÃ­veis</h2>
          
          {cursos.length > 0 ? (
            <div style={{ display: "grid", gap: "15px" }}>
              {cursos.map((curso) => (
                <div
                  key={curso.id}
                  style={{
                    border: "1px solid #e0e0e0",
                    borderRadius: "6px",
                    padding: "15px",
                    display: "flex",
                    justifyContent: "space-between",
                    alignItems: "center"
                  }}
                >
                  <div>
                    <h4 style={{ margin: "0 0 5px 0", color: "#333" }}>
                      {curso.titulo}
                    </h4>
                    <p style={{ margin: 0, color: "#666", fontSize: "14px" }}>
                      í³‚ {curso.categoria}
                    </p>
                  </div>
                  <span
                    style={{
                      padding: "4px 12px",
                      borderRadius: "20px",
                      fontSize: "12px",
                      fontWeight: "bold",
                      backgroundColor: 
                        curso.status === "Veiculado" ? "#d4edda" :
                        curso.status === "Em Desenvolvimento" ? "#fff3cd" : "#f8d7da",
                      color:
                        curso.status === "Veiculado" ? "#155724" :
                        curso.status === "Em Desenvolvimento" ? "#856404" : "#721c24"
                    }}
                  >
                    {curso.status}
                  </span>
                </div>
              ))}
            </div>
          ) : (
            <p style={{ textAlign: "center", color: "#666" }}>
              Nenhum curso encontrado.
            </p>
          )}
        </div>

        {/* Footer */}
        <footer style={{
          textAlign: "center",
          padding: "30px 0",
          color: "#666",
          borderTop: "1px solid #e0e0e0",
          marginTop: "40px"
        }}>
          <p>í¿¢ Ferreira Costa - Sistema de Acervo Educacional</p>
          <p style={{ fontSize: "12px" }}>
            Backend: http://localhost:5007 | 
            Frontend: http://localhost:5173 | 
            Swagger: http://localhost:5007/swagger
          </p>
        </footer>
      </div>
    </div>
   )
}

export default Dashboard
