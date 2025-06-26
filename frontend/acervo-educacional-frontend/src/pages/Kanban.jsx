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
  Folder
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
        setCursos(response.data);
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

  // Filtrar cursos por termo de busca
  const filteredCursos = cursos.filter(curso =>
    curso.nomeCurso.toLowerCase().includes(searchTerm.toLowerCase()) ||
    curso.codigoCurso.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (curso.descricaoAcademia && curso.descricaoAcademia.toLowerCase().includes(searchTerm.toLowerCase()))
  );

  // Agrupar cursos por status
  const cursosPorStatus = {
    [STATUS_CURSO.BACKLOG]: filteredCursos.filter(c => c.status === STATUS_CURSO.BACKLOG),
    [STATUS_CURSO.EM_DESENVOLVIMENTO]: filteredCursos.filter(c => c.status === STATUS_CURSO.EM_DESENVOLVIMENTO),
    [STATUS_CURSO.VEICULADO]: filteredCursos.filter(c => c.status === STATUS_CURSO.VEICULADO)
  };

  // Mover curso para outro status
  const moverCurso = async (cursoId, novoStatus) => {
    try {
      const response = await apiService.updateStatusCurso(cursoId, novoStatus);
      
      if (response.success) {
        // Atualizar estado local
        setCursos(prev => prev.map(curso => 
          curso.id === cursoId 
            ? { ...curso, status: novoStatus, updatedAt: new Date().toISOString() }
            : curso
        ));
      } else {
        setError(response.message || MESSAGES.ERROR.GENERIC);
      }
    } catch (error) {
      console.error('Erro ao mover curso:', error);
      setError(error.message || MESSAGES.ERROR.GENERIC);
    }
  };

  // Abrir modal de arquivos
  const abrirArquivos = (curso) => {
    setCursoSelecionado(curso);
    setArquivosModalOpen(true);
  };

  // Fechar modal de arquivos
  const fecharArquivos = () => {
    setArquivosModalOpen(false);
    setCursoSelecionado(null);
  };

  // Componente do card do curso
  const CursoCard = ({ curso }) => {
    const statusConfig = STATUS_COLORS[curso.status];
    
    return (
      <Card className="mb-3 hover:shadow-md transition-shadow cursor-pointer">
        <CardHeader className="pb-3">
          <div className="flex items-start justify-between">
            <div className="flex-1">
              <div className="flex items-center gap-2 mb-1">
                <Badge variant="outline" className="text-xs">
                  {curso.codigoCurso}
                </Badge>
                <Badge 
                  variant="secondary" 
                  className={`text-xs ${statusConfig.badge}`}
                >
                  {ORIGEM_LABELS[curso.origem]}
                </Badge>
              </div>
              <CardTitle className="text-sm font-medium leading-tight">
                {truncateText(curso.nomeCurso, 60)}
              </CardTitle>
            </div>
            
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                  <MoreVertical className="h-4 w-4" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onClick={() => console.log('Ver detalhes', curso.id)}>
                  <FileText className="mr-2 h-4 w-4" />
                  Ver Detalhes
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => abrirArquivos(curso)}>
                  <Folder className="mr-2 h-4 w-4" />
                  Gerenciar Arquivos
                </DropdownMenuItem>
                {curso.status !== STATUS_CURSO.BACKLOG && (
                  <DropdownMenuItem 
                    onClick={() => moverCurso(curso.id, STATUS_CURSO.BACKLOG)}
                  >
                    Mover para Backlog
                  </DropdownMenuItem>
                )}
                {curso.status !== STATUS_CURSO.EM_DESENVOLVIMENTO && (
                  <DropdownMenuItem 
                    onClick={() => moverCurso(curso.id, STATUS_CURSO.EM_DESENVOLVIMENTO)}
                  >
                    Mover para Em Desenvolvimento
                  </DropdownMenuItem>
                )}
                {curso.status !== STATUS_CURSO.VEICULADO && (
                  <DropdownMenuItem 
                    onClick={() => moverCurso(curso.id, STATUS_CURSO.VEICULADO)}
                  >
                    Mover para Veiculado
                  </DropdownMenuItem>
                )}
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </CardHeader>
        
        <CardContent className="pt-0">
          {curso.descricaoAcademia && (
            <p className="text-xs text-gray-600 mb-3">
              {truncateText(curso.descricaoAcademia, 80)}
            </p>
          )}
          
          <div className="space-y-2">
            {curso.tipoAmbiente && (
              <div className="flex items-center gap-1 text-xs text-gray-500">
                <Building className="h-3 w-3" />
                <span>{curso.tipoAmbiente}</span>
              </div>
            )}
            
            {curso.dataInicioOperacao && (
              <div className="flex items-center gap-1 text-xs text-gray-500">
                <Calendar className="h-3 w-3" />
                <span>{formatDate(curso.dataInicioOperacao)}</span>
              </div>
            )}
            
            {curso.criadoPorNome && (
              <div className="flex items-center gap-1 text-xs text-gray-500">
                <User className="h-3 w-3" />
                <span>{curso.criadoPorNome}</span>
              </div>
            )}
          </div>
          
          {curso.totalArquivos > 0 && (
            <div className="mt-3 pt-3 border-t">
              <div className="flex items-center justify-between text-xs">
                <span className="text-gray-500">Arquivos:</span>
                <Badge variant="outline" className="text-xs">
                  {curso.totalArquivos}
                </Badge>
              </div>
              
              {Object.keys(curso.arquivosPorCategoria).length > 0 && (
                <div className="mt-2 flex flex-wrap gap-1">
                  {Object.entries(curso.arquivosPorCategoria).map(([categoria, count]) => (
                    <Badge 
                      key={categoria} 
                      variant="secondary" 
                      className="text-xs px-1 py-0"
                    >
                      {CATEGORIA_LABELS[categoria]}: {count}
                    </Badge>
                  ))}
                </div>
              )}
            </div>
          )}
        </CardContent>
      </Card>
    );
  };

  // Componente da coluna do Kanban
  const KanbanColumn = ({ status, cursos, title }) => {
    const statusConfig = STATUS_COLORS[status];
    
    return (
      <div className="flex-1 min-w-80">
        <div className={`rounded-lg border-2 ${statusConfig.border} bg-white h-full`}>
          <div className={`p-4 ${statusConfig.bg} rounded-t-lg border-b`}>
            <div className="flex items-center justify-between">
              <h3 className={`font-semibold ${statusConfig.text}`}>
                {title}
              </h3>
              <Badge variant="secondary" className="ml-2">
                {cursos.length}
              </Badge>
            </div>
          </div>
          
          <div className="p-4 max-h-[calc(100vh-300px)] overflow-y-auto">
            {cursos.length === 0 ? (
              <div className="text-center py-8 text-gray-500">
                <p className="text-sm">Nenhum curso nesta coluna</p>
              </div>
            ) : (
              cursos.map(curso => (
                <CursoCard key={curso.id} curso={curso} />
              ))
            )}
          </div>
        </div>
      </div>
    );
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="flex items-center space-x-2">
          <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
          <span className="text-gray-600">Carregando cursos...</span>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Cabeçalho */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Kanban de Cursos</h1>
          <p className="text-gray-600 mt-1">
            Gerencie o fluxo de desenvolvimento dos cursos
          </p>
        </div>
        
        <div className="flex items-center gap-3">
          <Button
            variant="outline"
            size="sm"
            onClick={handleRefresh}
            disabled={refreshing}
          >
            <RefreshCw className={`h-4 w-4 mr-2 ${refreshing ? 'animate-spin' : ''}`} />
            Atualizar
          </Button>
          
          <Button>
            <Plus className="h-4 w-4 mr-2" />
            Novo Curso
          </Button>
        </div>
      </div>

      {/* Filtros */}
      <div className="flex items-center gap-4">
        <div className="relative flex-1 max-w-md">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-gray-400" />
          <Input
            placeholder="Buscar cursos..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        
        <Button variant="outline" size="sm">
          <Filter className="h-4 w-4 mr-2" />
          Filtros
        </Button>
      </div>

      {/* Alertas */}
      {error && (
        <Alert variant="destructive">
          <AlertDescription>{error}</AlertDescription>
        </Alert>
      )}

      {/* Kanban Board */}
      <div className="flex gap-6 overflow-x-auto pb-4">
        <KanbanColumn
          status={STATUS_CURSO.BACKLOG}
          cursos={cursosPorStatus[STATUS_CURSO.BACKLOG]}
          title={STATUS_LABELS[STATUS_CURSO.BACKLOG]}
        />
        
        <KanbanColumn
          status={STATUS_CURSO.EM_DESENVOLVIMENTO}
          cursos={cursosPorStatus[STATUS_CURSO.EM_DESENVOLVIMENTO]}
          title={STATUS_LABELS[STATUS_CURSO.EM_DESENVOLVIMENTO]}
        />
        
        <KanbanColumn
          status={STATUS_CURSO.VEICULADO}
          cursos={cursosPorStatus[STATUS_CURSO.VEICULADO]}
          title={STATUS_LABELS[STATUS_CURSO.VEICULADO]}
        />
      </div>

      {/* Estatísticas */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-8">
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Total de Cursos</p>
                <p className="text-2xl font-bold">{filteredCursos.length}</p>
              </div>
              <div className="h-12 w-12 bg-blue-100 rounded-full flex items-center justify-center">
                <FileText className="h-6 w-6 text-blue-600" />
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Em Desenvolvimento</p>
                <p className="text-2xl font-bold">
                  {cursosPorStatus[STATUS_CURSO.EM_DESENVOLVIMENTO].length}
                </p>
              </div>
              <div className="h-12 w-12 bg-yellow-100 rounded-full flex items-center justify-center">
                <RefreshCw className="h-6 w-6 text-yellow-600" />
              </div>
            </div>
          </CardContent>
        </Card>
        
        <Card>
          <CardContent className="p-4">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Veiculados</p>
                <p className="text-2xl font-bold">
                  {cursosPorStatus[STATUS_CURSO.VEICULADO].length}
                </p>
              </div>
              <div className="h-12 w-12 bg-green-100 rounded-full flex items-center justify-center">
                <FileText className="h-6 w-6 text-green-600" />
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Modal de Arquivos */}
      <ArquivosModal
        isOpen={arquivosModalOpen}
        onClose={fecharArquivos}
        curso={cursoSelecionado}
      />
    </div>
  );
};

export default Kanban;

