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
  Plus
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
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando dashboard...</span>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <Alert variant="destructive">
        <AlertDescription>{error}</AlertDescription>
      </Alert>
    );
  }

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
          <p className="text-gray-600 mt-1">
            Visão geral do sistema de acervo educacional
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <Button asChild>
            <Link to={ROUTES.KANBAN}>
              <Plus className="h-4 w-4 mr-2" />
              Novo Curso
            </Link>
          </Button>
        </div>
      </div>

      {/* Cards de estatísticas principais */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total de Cursos</CardTitle>
            <BookOpen className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats?.totalCursos || 0}</div>
            <p className="text-xs text-muted-foreground">
              Todos os cursos cadastrados
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total de Arquivos</CardTitle>
            <FileText className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats?.totalArquivos || 0}</div>
            <p className="text-xs text-muted-foreground">
              {stats?.tamanhoTotalFormatado || '0 B'} de armazenamento
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Usuários Ativos</CardTitle>
            <Users className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{stats?.usuariosAtivos || 0}</div>
            <p className="text-xs text-muted-foreground">
              Usuários com acesso ao sistema
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Última Sincronização</CardTitle>
            <TrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-sm font-bold">
              {stats?.ultimaSincronizacao 
                ? formatDate(stats.ultimaSincronizacao)
                : 'Nunca'
              }
            </div>
            <p className="text-xs text-muted-foreground">
              Sincronização com Senior
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Gráficos de distribuição */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Cursos por Status */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Activity className="h-5 w-5" />
              Cursos por Status
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {stats?.cursosPorStatus && Object.entries(stats.cursosPorStatus).map(([status, count]) => (
                <div key={status} className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className={`w-3 h-3 rounded-full ${
                      status === 'Backlog' ? 'bg-gray-400' :
                      status === 'EmDesenvolvimento' ? 'bg-blue-500' :
                      'bg-green-500'
                    }`} />
                    <span className="text-sm">
                      {status === 'EmDesenvolvimento' ? 'Em Desenvolvimento' : status}
                    </span>
                  </div>
                  <span className="font-semibold">{count}</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Cursos por Origem */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <BookOpen className="h-5 w-5" />
              Cursos por Origem
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {stats?.cursosPorOrigem && Object.entries(stats.cursosPorOrigem).map(([origem, count]) => (
                <div key={origem} className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <div className={`w-3 h-3 rounded-full ${
                      origem === 'Manual' ? 'bg-blue-500' : 'bg-purple-500'
                    }`} />
                    <span className="text-sm">{origem}</span>
                  </div>
                  <span className="font-semibold">{count}</span>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Atividades Recentes */}
      <Card>
        <CardHeader className="flex flex-row items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <Calendar className="h-5 w-5" />
            Atividades Recentes
          </CardTitle>
          <Button variant="outline" size="sm" asChild>
            <Link to={ROUTES.LOGS}>
              Ver Todos
              <ArrowRight className="h-4 w-4 ml-2" />
            </Link>
          </Button>
        </CardHeader>
        <CardContent>
          {stats?.atividadesRecentes && stats.atividadesRecentes.length > 0 ? (
            <div className="space-y-4">
              {stats.atividadesRecentes.slice(0, 5).map((atividade) => (
                <div key={atividade.id} className="flex items-start gap-3 p-3 rounded-lg bg-gray-50">
                  <div className="w-2 h-2 bg-blue-500 rounded-full mt-2" />
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-gray-900">
                      {atividade.usuarioNome}
                    </p>
                    <p className="text-sm text-gray-600">
                      {atividade.descricao}
                    </p>
                    {(atividade.cursoNome || atividade.arquivoNome) && (
                      <p className="text-xs text-gray-500 mt-1">
                        {atividade.cursoNome && `Curso: ${atividade.cursoNome}`}
                        {atividade.arquivoNome && ` • Arquivo: ${atividade.arquivoNome}`}
                      </p>
                    )}
                  </div>
                  <div className="text-xs text-gray-500">
                    {formatDateTime(atividade.createdAt)}
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="text-center py-8 text-gray-500">
              <Activity className="h-12 w-12 mx-auto mb-4 text-gray-300" />
              <p>Nenhuma atividade recente</p>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Links rápidos */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card className="hover:shadow-md transition-shadow cursor-pointer">
          <CardContent className="p-6">
            <Link to={ROUTES.KANBAN} className="flex items-center justify-between">
              <div>
                <h3 className="font-semibold text-gray-900">Kanban</h3>
                <p className="text-sm text-gray-600">Gerencie o fluxo de cursos</p>
              </div>
              <ArrowRight className="h-5 w-5 text-gray-400" />
            </Link>
          </CardContent>
        </Card>

        <Card className="hover:shadow-md transition-shadow cursor-pointer">
          <CardContent className="p-6">
            <Link to={ROUTES.CURSOS} className="flex items-center justify-between">
              <div>
                <h3 className="font-semibold text-gray-900">Cursos</h3>
                <p className="text-sm text-gray-600">Lista completa de cursos</p>
              </div>
              <ArrowRight className="h-5 w-5 text-gray-400" />
            </Link>
          </CardContent>
        </Card>

        <Card className="hover:shadow-md transition-shadow cursor-pointer">
          <CardContent className="p-6">
            <Link to={ROUTES.USUARIOS} className="flex items-center justify-between">
              <div>
                <h3 className="font-semibold text-gray-900">Usuários</h3>
                <p className="text-sm text-gray-600">Gerenciar usuários</p>
              </div>
              <ArrowRight className="h-5 w-5 text-gray-400" />
            </Link>
          </CardContent>
        </Card>
      </div>
    </div>
  );
};

export default Dashboard;

