import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import Layout from './components/Layout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Kanban from './pages/Kanban';
import { Loader2 } from 'lucide-react';
import { ROUTES } from './constants';
import './App.css';

// Componente para rotas protegidas
const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando...</span>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.LOGIN} replace />;
  }

  return <Layout>{children}</Layout>;
};

// Componente para rotas públicas (quando já logado, redireciona)
const PublicRoute = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando...</span>
        </div>
      </div>
    );
  }

  if (isAuthenticated) {
    return <Navigate to={ROUTES.DASHBOARD} replace />;
  }

  return children;
};

// Componente principal da aplicação
const AppContent = () => {
  return (
    <Routes>
      {/* Rotas públicas */}
      <Route 
        path={ROUTES.LOGIN} 
        element={
          <PublicRoute>
            <Login />
          </PublicRoute>
        } 
      />

      {/* Rotas protegidas */}
      <Route 
        path={ROUTES.DASHBOARD} 
        element={
          <ProtectedRoute>
            <Dashboard />
          </ProtectedRoute>
        } 
      />
      
      <Route 
        path={ROUTES.KANBAN} 
        element={
          <ProtectedRoute>
            <Kanban />
          </ProtectedRoute>
        } 
      />

      {/* Páginas temporárias para outras rotas */}
      <Route 
        path={ROUTES.CURSOS} 
        element={
          <ProtectedRoute>
            <div className="text-center py-12">
              <h2 className="text-2xl font-bold text-gray-900 mb-4">Cursos</h2>
              <p className="text-gray-600">Página em desenvolvimento</p>
            </div>
          </ProtectedRoute>
        } 
      />

      <Route 
        path={ROUTES.USUARIOS} 
        element={
          <ProtectedRoute>
            <div className="text-center py-12">
              <h2 className="text-2xl font-bold text-gray-900 mb-4">Usuários</h2>
              <p className="text-gray-600">Página em desenvolvimento</p>
            </div>
          </ProtectedRoute>
        } 
      />

      <Route 
        path={ROUTES.LOGS} 
        element={
          <ProtectedRoute>
            <div className="text-center py-12">
              <h2 className="text-2xl font-bold text-gray-900 mb-4">Logs</h2>
              <p className="text-gray-600">Página em desenvolvimento</p>
            </div>
          </ProtectedRoute>
        } 
      />

      <Route 
        path={ROUTES.CONFIGURACOES} 
        element={
          <ProtectedRoute>
            <div className="text-center py-12">
              <h2 className="text-2xl font-bold text-gray-900 mb-4">Configurações</h2>
              <p className="text-gray-600">Página em desenvolvimento</p>
            </div>
          </ProtectedRoute>
        } 
      />

      {/* Rota raiz - redireciona para dashboard se logado, senão para login */}
      <Route 
        path="/" 
        element={<Navigate to={ROUTES.DASHBOARD} replace />} 
      />

      {/* Rota 404 */}
      <Route 
        path="*" 
        element={
          <div className="min-h-screen flex items-center justify-center bg-gray-50">
            <div className="text-center">
              <h1 className="text-4xl font-bold text-gray-900 mb-4">404</h1>
              <p className="text-gray-600 mb-4">Página não encontrada</p>
              <a 
                href={ROUTES.DASHBOARD}
                className="text-blue-600 hover:text-blue-500 underline"
              >
                Voltar ao Dashboard
              </a>
            </div>
          </div>
        } 
      />
    </Routes>
  );
};

// Componente App principal
function App() {
  return (
    <Router>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </Router>
  );
}

export default App;
