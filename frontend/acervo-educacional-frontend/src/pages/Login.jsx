import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Loader2, Eye, EyeOff, GraduationCap } from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import { validateEmail } from '../utils';
import { ROUTES, MESSAGES } from '../constants';

const Login = () => {
  const navigate = useNavigate();
  const { login, loading } = useAuth();
  
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  });
  
  const [errors, setErrors] = useState({});
  const [showPassword, setShowPassword] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Limpar erro do campo quando usuário começar a digitar
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }));
    }
  };

  const validateForm = () => {
    const newErrors = {};

    if (!formData.email) {
      newErrors.email = 'Email é obrigatório';
    } else if (!validateEmail(formData.email)) {
      newErrors.email = 'Email deve ter um formato válido';
    }

    if (!formData.password) {
      newErrors.password = 'Senha é obrigatória';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);
    
    try {
      const result = await login(formData.email, formData.password);
      
      if (result.success) {
        navigate(ROUTES.DASHBOARD);
      } else {
        setErrors({ general: result.message || MESSAGES.ERROR.GENERIC });
      }
    } catch (error) {
      setErrors({ general: error.message || MESSAGES.ERROR.GENERIC });
    } finally {
      setIsSubmitting(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center" style={{ backgroundColor: 'var(--fc-background)' }}>
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin" style={{ color: 'var(--fc-primary)' }} />
          <span style={{ color: 'var(--fc-gray)' }}>Carregando...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex items-center justify-center p-4" style={{ backgroundColor: 'var(--fc-background)' }}>
      <div className="w-full max-w-md">
        {/* Logo e título */}
        <div className="text-center mb-8">
          <div className="flex justify-center mb-4">
            <div className="p-3 rounded-full" style={{ backgroundColor: 'var(--fc-primary)' }}>
              <GraduationCap className="h-8 w-8" style={{ color: 'var(--fc-white)' }} />
            </div>
          </div>
          <h1 className="text-3xl font-bold mb-2" style={{ color: 'var(--fc-gray-dark)' }}>
            Acervo Educacional
          </h1>
          <p style={{ color: 'var(--fc-gray)' }}>
            Sistema de Gerenciamento de Cursos
          </p>
        </div>

        {/* Card de login */}
        <Card className="fc-card">
          <CardHeader className="text-center">
            <CardTitle className="text-2xl" style={{ color: 'var(--fc-gray-dark)' }}>
              Entrar
            </CardTitle>
            <CardDescription style={{ color: 'var(--fc-gray)' }}>
              Digite suas credenciais para acessar o sistema
            </CardDescription>
          </CardHeader>
          
          <CardContent>
            {errors.general && (
              <Alert className="mb-4" style={{ borderColor: 'var(--fc-primary)', backgroundColor: 'rgba(193, 45, 0, 0.1)' }}>
                <AlertDescription style={{ color: 'var(--fc-primary)' }}>
                  {errors.general}
                </AlertDescription>
              </Alert>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="email" style={{ color: 'var(--fc-gray-dark)' }}>
                  Email
                </Label>
                <Input
                  id="email"
                  name="email"
                  type="email"
                  placeholder="seu@email.com"
                  value={formData.email}
                  onChange={handleChange}
                  className={`fc-input ${errors.email ? 'border-red-500' : ''}`}
                  disabled={isSubmitting}
                />
                {errors.email && (
                  <p className="text-sm" style={{ color: 'var(--fc-primary)' }}>
                    {errors.email}
                  </p>
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="password" style={{ color: 'var(--fc-gray-dark)' }}>
                  Senha
                </Label>
                <div className="relative">
                  <Input
                    id="password"
                    name="password"
                    type={showPassword ? 'text' : 'password'}
                    placeholder="Digite sua senha"
                    value={formData.password}
                    onChange={handleChange}
                    className={`fc-input pr-10 ${errors.password ? 'border-red-500' : ''}`}
                    disabled={isSubmitting}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 transform -translate-y-1/2"
                    style={{ color: 'var(--fc-gray)' }}
                  >
                    {showPassword ? <EyeOff className="h-4 w-4" /> : <Eye className="h-4 w-4" />}
                  </button>
                </div>
                {errors.password && (
                  <p className="text-sm" style={{ color: 'var(--fc-primary)' }}>
                    {errors.password}
                  </p>
                )}
              </div>

              <div className="flex items-center justify-between">
                <Link 
                  to="/forgot-password" 
                  className="text-sm hover:underline"
                  style={{ color: 'var(--fc-primary)' }}
                >
                  Esqueceu sua senha?
                </Link>
              </div>

              <Button
                type="submit"
                className="w-full fc-btn fc-btn-primary"
                disabled={isSubmitting}
              >
                {isSubmitting ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Entrando...
                  </>
                ) : (
                  'Entrar'
                )}
              </Button>
            </form>
          </CardContent>
        </Card>

        {/* Footer */}
        <div className="text-center mt-6 text-sm" style={{ color: 'var(--fc-gray)' }}>
          <p>Sistema desenvolvido para gerenciamento de cursos educacionais</p>
          <p className="mt-1">Todos os usuários são administradores</p>
        </div>
      </div>
    </div>
  );
};

export default Login;

