import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { 
  Plus, 
  Search, 
  Filter, 
  MoreVertical, 
  Calendar, 
  User, 
  FileText,
  Building,
  Loader2,
  RefreshCw,
  Folder,
  Clock,
  CheckCircle,
  AlertCircle
} from 'lucide-react';
import { 
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import ArquivosModal from '../components/ArquivosModal';
import apiService from '../services/api';
import { 
  STATUS_CURSO, 
  STATUS_LABELS, 
  STATUS_COLORS, 
  ORIGEM_LABELS,
  CATEGORIA_LABELS,
  MESSAGES 
} from '../constants';
import { formatDate, truncateText } from '../utils';

const Kanban = () => {
  const [cursos, setCursos] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [refreshing, setRefreshing] = useState(false);
  
  // Estados para o modal de arquivos
  const [arquivosModalOpen, setArquivosModalOpen] = useState(false);
  const [cursoSelecionado, setCursoSelecionado] = useState(null);

  // Carregar cursos do Kanban
  const loadCursos = async () => {
    try {
      setError('');
      const response = await apiService.getCursosKanban();
      
      if (response.success) {
        setCursos(response.data || []);
      } else {
        setError(response.message || MESSAGES.ERROR.GENERIC);
      }
    } catch (error) {
      console.error('Erro ao carregar cursos:', error);
      setError(error.message || MESSAGES.ERROR.NETWORK);
    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  };

  useEffect(() => {
    loadCursos();
  }, []);

  const handleRefresh = () => {
    setRefreshing(true);
    loadCursos();
  };

  const handleOpenArquivos = (curso) => {
    setCursoSelecionado(curso);
    setArquivosModalOpen(true);
  };

  // Filtrar cursos por termo de busca
  const filteredCursos = cursos.filter(curso =>
    curso.titulo?.toLowerCase().includes(searchTerm.toLowerCase()) ||
    curso.descricao?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  // Agrupar cursos por status
  const cursosPorStatus = {
    [STATUS_CURSO.BACKLOG]: filteredCursos.filter(c => c.status === STATUS_CURSO.BACKLOG),
    [STATUS_CURSO.EM_DESENVOLVIMENTO]: filteredCursos.filter(c => c.status === STATUS_CURSO.EM_DESENVOLVIMENTO),
    [STATUS_CURSO.VEICULADO]: filteredCursos.filter(c => c.status === STATUS_CURSO.VEICULADO)
  };

  const getStatusIcon = (status) => {
    switch (status) {
      case STATUS_CURSO.BACKLOG:
        return <Clock className="h-4 w-4" />;
      case STATUS_CURSO.EM_DESENVOLVIMENTO:
        return <AlertCircle className="h-4 w-4" />;
      case STATUS_CURSO.VEICULADO:
        return <CheckCircle className="h-4 w-4" />;
      default:
        return <Clock className="h-4 w-4" />;
    }
  };

  const getStatusColor = (status) => {
    switch (status) {
      case STATUS_CURSO.BACKLOG:
        return 'var(--fc-gray)';
      case STATUS_CURSO.EM_DESENVOLVIMENTO:
        return 'var(--fc-yellow)';
      case STATUS_CURSO.VEICULADO:
        return 'var(--fc-green)';
      default:
        return 'var(--fc-gray)';
    }
  };

  const KanbanColumn = ({ status, title, cursos, icon }) => (
    <div className="flex-1 min-w-80">
      <Card className="fc-kanban-column h-full">
        <CardHeader className="pb-3">
          <CardTitle className="flex items-center justify-between">
            <div className="flex items-center gap-2" style={{ color: 'var(--fc-gray-dark)' }}>
              {icon}
              <span>{title}</span>
              <Badge 
                variant="secondary" 
                className="ml-2"
                style={{ 
                  backgroundColor: getStatusColor(status), 
                  color: 'var(--fc-white)',
                  fontSize: '0.75rem'
                }}
              >
                {cursos.length}
              </Badge>
            </div>
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          {cursos.length === 0 ? (
            <div className="text-center py-8" style={{ color: 'var(--fc-gray)' }}>
              <Folder className="h-12 w-12 mx-auto mb-2" style={{ color: 'var(--fc-gray-light)' }} />
              <p>Nenhum curso nesta coluna</p>
            </div>
          ) : (
            cursos.map((curso) => (
              <Card key={curso.id} className="fc-card fc-transition cursor-pointer hover:shadow-md">
                <CardContent className="p-4">
                  <div className="space-y-3">
                    {/* Título e Menu */}
                    <div className="flex items-start justify-between">
                      <h4 className="font-medium text-sm leading-tight" style={{ color: 'var(--fc-gray-dark)' }}>
                        {truncateText(curso.titulo, 60)}
                      </h4>
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button variant="ghost" size="sm" className="h-6 w-6 p-0">
                            <MoreVertical className="h-3 w-3" style={{ color: 'var(--fc-gray)' }} />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuItem onClick={() => handleOpenArquivos(curso)}>
                            <FileText className="h-4 w-4 mr-2" />
                            Ver Arquivos
                          </DropdownMenuItem>
                          <DropdownMenuItem>
                            <Calendar className="h-4 w-4 mr-2" />
                            Agendar
                          </DropdownMenuItem>
                        </DropdownMenuContent>
                      </DropdownMenu>
                    </div>

                    {/* Descrição */}
                    {curso.descricao && (
                      <p className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                        {truncateText(curso.descricao, 100)}
                      </p>
                    )}

                    {/* Metadados */}
                    <div className="space-y-2">
                      {curso.categoria && (
                        <div className="flex items-center gap-1">
                          <Building className="h-3 w-3" style={{ color: 'var(--fc-gray)' }} />
                          <span className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                            {CATEGORIA_LABELS[curso.categoria] || curso.categoria}
                          </span>
                        </div>
                      )}
                      
                      {curso.origem && (
                        <div className="flex items-center gap-1">
                          <User className="h-3 w-3" style={{ color: 'var(--fc-gray)' }} />
                          <span className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                            {ORIGEM_LABELS[curso.origem] || curso.origem}
                          </span>
                        </div>
                      )}

                      {curso.dataAtualizacao && (
                        <div className="flex items-center gap-1">
                          <Calendar className="h-3 w-3" style={{ color: 'var(--fc-gray)' }} />
                          <span className="text-xs" style={{ color: 'var(--fc-gray)' }}>
                            {formatDate(curso.dataAtualizacao)}
                          </span>
                        </div>
                      )}
                    </div>

                    {/* Tags */}
                    {curso.tags && curso.tags.length > 0 && (
                      <div className="flex flex-wrap gap-1">
                        {curso.tags.slice(0, 3).map((tag, index) => (
                          <Badge 
                            key={index} 
                            variant="outline" 
                            className="text-xs px-2 py-0"
                            style={{ 
                              borderColor: 'var(--fc-primary)', 
                              color: 'var(--fc-primary)',
                              fontSize: '0.65rem'
                            }}
                          >
                            {tag}
                          </Badge>
                        ))}
                        {curso.tags.length > 3 && (
                          <Badge 
                            variant="outline" 
                            className="text-xs px-2 py-0"
                            style={{ 
                              borderColor: 'var(--fc-gray)', 
                              color: 'var(--fc-gray)',
                              fontSize: '0.65rem'
                            }}
                          >
                            +{curso.tags.length - 3}
                          </Badge>
                        )}
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>
            ))
          )}
        </CardContent>
      </Card>
    </div>
  );

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin" style={{ color: 'var(--fc-primary)' }} />
          <span style={{ color: 'var(--fc-gray)' }}>Carregando cursos...</span>
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
            Kanban de Cursos
          </h1>
          <p style={{ color: 'var(--fc-gray)' }}>
            Gerencie o fluxo de desenvolvimento dos cursos
          </p>
        </div>
        <div className="flex items-center gap-2">
          <Button 
            variant="outline" 
            size="sm" 
            onClick={handleRefresh}
            disabled={refreshing}
            className="fc-btn fc-btn-secondary"
          >
            <RefreshCw className={`h-4 w-4 mr-2 ${refreshing ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
          <Button className="fc-btn fc-btn-success">
            <Plus className="h-4 w-4 mr-2" />
            Novo Curso
          </Button>
        </div>
      </div>

      {/* Filtros */}
      <Card className="fc-card">
        <CardContent className="p-4">
          <div className="flex items-center gap-4">
            <div className="flex-1 relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4" style={{ color: 'var(--fc-gray)' }} />
              <Input
                placeholder="Buscar cursos..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="fc-input pl-10"
              />
            </div>
            <Button variant="outline" className="fc-btn fc-btn-secondary">
              <Filter className="h-4 w-4 mr-2" />
              Filtros
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Error Alert */}
      {error && (
        <Alert style={{ borderColor: 'var(--fc-primary)', backgroundColor: 'rgba(193, 45, 0, 0.1)' }}>
          <AlertDescription style={{ color: 'var(--fc-primary)' }}>
            {error}
          </AlertDescription>
        </Alert>
      )}

      {/* Kanban Board */}
      <div className="flex gap-6 overflow-x-auto pb-4">
        <KanbanColumn
          status={STATUS_CURSO.BACKLOG}
          title="Backlog"
          cursos={cursosPorStatus[STATUS_CURSO.BACKLOG]}
          icon={getStatusIcon(STATUS_CURSO.BACKLOG)}
        />
        <KanbanColumn
          status={STATUS_CURSO.EM_DESENVOLVIMENTO}
          title="Em Desenvolvimento"
          cursos={cursosPorStatus[STATUS_CURSO.EM_DESENVOLVIMENTO]}
          icon={getStatusIcon(STATUS_CURSO.EM_DESENVOLVIMENTO)}
        />
        <KanbanColumn
          status={STATUS_CURSO.VEICULADO}
          title="Veiculado"
          cursos={cursosPorStatus[STATUS_CURSO.VEICULADO]}
          icon={getStatusIcon(STATUS_CURSO.VEICULADO)}
        />
      </div>

      {/* Estatísticas */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        <Card className="fc-card">
          <CardContent className="p-4 text-center">
            <div className="text-2xl font-bold" style={{ color: 'var(--fc-gray-dark)' }}>
              {cursosPorStatus[STATUS_CURSO.BACKLOG].length}
            </div>
            <p className="text-sm" style={{ color: 'var(--fc-gray)' }}>
              Total de Cursos
            </p>
          </CardContent>
        </Card>
        
        <Card className="fc-card">
          <CardContent className="p-4 text-center">
            <div className="text-2xl font-bold" style={{ color: 'var(--fc-yellow)' }}>
              {cursosPorStatus[STATUS_CURSO.EM_DESENVOLVIMENTO].length}
            </div>
            <p className="text-sm" style={{ color: 'var(--fc-gray)' }}>
              Em Desenvolvimento
            </p>
          </CardContent>
        </Card>
        
        <Card className="fc-card">
          <CardContent className="p-4 text-center">
            <div className="text-2xl font-bold" style={{ color: 'var(--fc-green)' }}>
              {cursosPorStatus[STATUS_CURSO.VEICULADO].length}
            </div>
            <p className="text-sm" style={{ color: 'var(--fc-gray)' }}>
              Veiculados
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Modal de Arquivos */}
      {arquivosModalOpen && cursoSelecionado && (
        <ArquivosModal
          isOpen={arquivosModalOpen}
          onClose={() => setArquivosModalOpen(false)}
          curso={cursoSelecionado}
        />
      )}
    </div>
  );
};

export default Kanban;

