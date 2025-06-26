import React, { createContext, useContext, useState, useEffect } from 'react';
import apiService from '../services/api';

const AuthContext = createContext();

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth deve ser usado dentro de um AuthProvider');
  }
  return context;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  // Verificar se há token salvo e validar
  useEffect(() => {
    const initAuth = async () => {
      const token = localStorage.getItem('token');
      const savedUser = localStorage.getItem('user');

      if (token && savedUser) {
        try {
          // Verificar se o token ainda é válido
          const response = await apiService.getCurrentUser();
          if (response.success) {
            setUser(response.data);
            setIsAuthenticated(true);
          } else {
            // Token inválido, limpar dados
            clearAuth();
          }
        } catch (error) {
          console.error('Erro ao validar token:', error);
          clearAuth();
        }
      }
      setLoading(false);
    };

    initAuth();
  }, []);

  const login = async (email, password) => {
    try {
      setLoading(true);
      const response = await apiService.login(email, password);
      
      if (response.success) {
        const { token, refreshToken, user: userData } = response.data;
        
        // Salvar dados no localStorage
        localStorage.setItem('token', token);
        localStorage.setItem('refreshToken', refreshToken);
        localStorage.setItem('user', JSON.stringify(userData));
        
        // Atualizar estado
        setUser(userData);
        setIsAuthenticated(true);
        
        return { success: true };
      } else {
        return { success: false, message: response.message };
      }
    } catch (error) {
      console.error('Erro no login:', error);
      return { success: false, message: error.message || 'Erro interno do servidor' };
    } finally {
      setLoading(false);
    }
  };

  const logout = async () => {
    try {
      await apiService.logout();
    } catch (error) {
      console.error('Erro no logout:', error);
    } finally {
      clearAuth();
    }
  };

  const clearAuth = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    setUser(null);
    setIsAuthenticated(false);
  };

  const forgotPassword = async (email) => {
    try {
      const response = await apiService.forgotPassword(email);
      return { success: response.success, message: response.message };
    } catch (error) {
      console.error('Erro ao solicitar recuperação de senha:', error);
      return { success: false, message: error.message || 'Erro interno do servidor' };
    }
  };

  const resetPassword = async (token, newPassword, confirmPassword) => {
    try {
      const response = await apiService.resetPassword(token, newPassword, confirmPassword);
      return { success: response.success, message: response.message };
    } catch (error) {
      console.error('Erro ao redefinir senha:', error);
      return { success: false, message: error.message || 'Erro interno do servidor' };
    }
  };

  const changePassword = async (currentPassword, newPassword, confirmPassword) => {
    try {
      const response = await apiService.changePassword(currentPassword, newPassword, confirmPassword);
      return { success: response.success, message: response.message };
    } catch (error) {
      console.error('Erro ao alterar senha:', error);
      return { success: false, message: error.message || 'Erro interno do servidor' };
    }
  };

  const refreshUserData = async () => {
    try {
      const response = await apiService.getCurrentUser();
      if (response.success) {
        setUser(response.data);
        localStorage.setItem('user', JSON.stringify(response.data));
      }
    } catch (error) {
      console.error('Erro ao atualizar dados do usuário:', error);
    }
  };

  const value = {
    user,
    loading,
    isAuthenticated,
    login,
    logout,
    forgotPassword,
    resetPassword,
    changePassword,
    refreshUserData,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

