import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { 
  BookOpen, 
  Users, 
  FileText, 
  TrendingUp,
  Calendar,
  Activity,
  Loader2,
  ArrowRight,
  Plus,
  BarChart3,
  Clock
} from 'lucide-react';
import apiService from '../services/api';
import { ROUTES, MESSAGES } from '../constants';
import { formatDate, formatDateTime } from '../utils';

const Dashboard = () => {
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadDashboardStats();
  }, []);

  const loadDashboardStats = async () => {
    try {
      setError('');
      const response = await apiService.getDashboardStats();
      
      if (response.success) {
        setStats(response.data);
      } else {
        setError(response.message || MESSAGES.ERROR.GENERIC);
      }
    } catch (error) {
      console.error('Erro ao carregar estatísticas:', error);
      setError(error.message || MESSAGES.ERROR.NETWORK);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin" style={{ color: 'var(--fc-primary)' }} />
          <span style={{ color: 'var(--fc-gray)' }}>Carregando estatísticas...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold" style={{ color: 'var(--fc-gray-dark)' }}>
            Dashboard
          </h1>
          <p style={{ color: 'var(--fc-gray)' }}>
            Visão geral do sistema de gerenciamento de cursos
          </p>
        </div>
        <Button className="fc-btn fc-btn-primary">
          <Plus className="h-4 w-4 mr-2" />
          Novo Curso
        </Button>
      </div>

      {/* Error Alert */}
      {error && (
        <Alert style={{ borderColor: 'var(--fc-primary)', backgroundColor: 'rgba(193, 45, 0, 0.1)' }}>
          <AlertDescription style={{ color: 'var(--fc-primary)' }}>
            {error}
          </AlertDescription>
        </Alert>
      )}

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        {/* Total de Cursos */}
        <Card className="fc-card fc-transition">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium" style={{ color: 'var(--fc-gray)' }}>
              Total de Cursos
            </CardTitle>
            <BookOpen className="h-4 w-4" style={{ color: 'var(--fc-primary)' }} />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold" style={{ color: 'var(--fc-gray-dark)' }}>
              {stats?.totalCursos || 0}
            </div>
            <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
              +2 novos este mês
            </p>
          </CardContent>
        </Card>

        {/* Usuários Ativos */}
        <Card className="fc-card fc-transition">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium" style={{ color: 'var(--fc-gray)' }}>
              Usuários Ativos
            </CardTitle>
            <Users className="h-4 w-4" style={{ color: 'var(--fc-green)' }} />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold" style={{ color: 'var(--fc-gray-dark)' }}>
              {stats?.usuariosAtivos || 0}
            </div>
            <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
              +12% desde o último mês
            </p>
          </CardContent>
        </Card>

        {/* Arquivos */}
        <Card className="fc-card fc-transition">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium" style={{ color: 'var(--fc-gray)' }}>
              Arquivos
            </CardTitle>
            <FileText className="h-4 w-4" style={{ color: 'var(--fc-yellow)' }} />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold" style={{ color: 'var(--fc-gray-dark)' }}>
              {stats?.totalArquivos || 0}
            </div>
            <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
              {stats?.tamanhoTotal || '0 MB'} total
            </p>
          </CardContent>
        </Card>

        {/* Taxa de Conclusão */}
        <Card className="fc-card fc-transition">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium" style={{ color: 'var(--fc-gray)' }}>
              Taxa de Conclusão
            </CardTitle>
            <TrendingUp className="h-4 w-4" style={{ color: 'var(--fc-green)' }} />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold" style={{ color: 'var(--fc-gray-dark)' }}>
              {stats?.taxaConclusao || '0%'}
            </div>
            <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
              +5% desde o último mês
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Charts and Activity */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Gráfico de Atividade */}
        <Card className="fc-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2" style={{ color: 'var(--fc-gray-dark)' }}>
              <BarChart3 className="h-5 w-5" style={{ color: 'var(--fc-primary)' }} />
              Atividade dos Últimos 7 Dias
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="h-64 flex items-center justify-center" style={{ backgroundColor: 'var(--fc-background)', borderRadius: 'var(--fc-border-radius)' }}>
              <div className="text-center">
                <Activity className="h-12 w-12 mx-auto mb-2" style={{ color: 'var(--fc-gray-light)' }} />
                <p style={{ color: 'var(--fc-gray)' }}>Gráfico em desenvolvimento</p>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Atividades Recentes */}
        <Card className="fc-card">
          <CardHeader>
            <CardTitle className="flex items-center gap-2" style={{ color: 'var(--fc-gray-dark)' }}>
              <Clock className="h-5 w-5" style={{ color: 'var(--fc-primary)' }} />
              Atividades Recentes
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              {/* Atividade 1 */}
              <div className="flex items-start space-x-3">
                <div className="w-2 h-2 rounded-full mt-2" style={{ backgroundColor: 'var(--fc-green)' }}></div>
                <div className="flex-1">
                  <p className="text-sm font-medium" style={{ color: 'var(--fc-gray-dark)' }}>
                    Novo curso "React Avançado" criado
                  </p>
                  <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                    Há 2 horas
                  </p>
                </div>
              </div>

              {/* Atividade 2 */}
              <div className="flex items-start space-x-3">
                <div className="w-2 h-2 rounded-full mt-2" style={{ backgroundColor: 'var(--fc-yellow)' }}></div>
                <div className="flex-1">
                  <p className="text-sm font-medium" style={{ color: 'var(--fc-gray-dark)' }}>
                    5 novos usuários registrados
                  </p>
                  <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                    Há 4 horas
                  </p>
                </div>
              </div>

              {/* Atividade 3 */}
              <div className="flex items-start space-x-3">
                <div className="w-2 h-2 rounded-full mt-2" style={{ backgroundColor: 'var(--fc-primary)' }}></div>
                <div className="flex-1">
                  <p className="text-sm font-medium" style={{ color: 'var(--fc-gray-dark)' }}>
                    Backup do sistema realizado
                  </p>
                  <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                    Há 6 horas
                  </p>
                </div>
              </div>

              {/* Atividade 4 */}
              <div className="flex items-start space-x-3">
                <div className="w-2 h-2 rounded-full mt-2" style={{ backgroundColor: 'var(--fc-green)' }}></div>
                <div className="flex-1">
                  <p className="text-sm font-medium" style={{ color: 'var(--fc-gray-dark)' }}>
                    Curso "JavaScript Básico" atualizado
                  </p>
                  <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                    Ontem
                  </p>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Quick Actions */}
      <Card className="fc-card">
        <CardHeader>
          <CardTitle style={{ color: 'var(--fc-gray-dark)' }}>
            Ações Rápidas
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <Link to={ROUTES.KANBAN}>
              <Button variant="outline" className="w-full fc-btn fc-btn-secondary h-auto p-4">
                <div className="text-center">
                  <Activity className="h-6 w-6 mx-auto mb-2" />
                  <div className="font-medium">Kanban</div>
                  <div className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                    Gerenciar fluxo de cursos
                  </div>
                </div>
              </Button>
            </Link>

            <Link to={ROUTES.CURSOS}>
              <Button variant="outline" className="w-full fc-btn fc-btn-secondary h-auto p-4">
                <div className="text-center">
                  <BookOpen className="h-6 w-6 mx-auto mb-2" />
                  <div className="font-medium">Cursos</div>
                  <div className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                    Visualizar todos os cursos
                  </div>
                </div>
              </Button>
            </Link>

            <Link to={ROUTES.USUARIOS}>
              <Button variant="outline" className="w-full fc-btn fc-btn-secondary h-auto p-4">
                <div className="text-center">
                  <Users className="h-6 w-6 mx-auto mb-2" />
                  <div className="font-medium">Usuários</div>
                  <div className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                    Gerenciar usuários
                  </div>
                </div>
              </Button>
            </Link>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default Dashboard;

