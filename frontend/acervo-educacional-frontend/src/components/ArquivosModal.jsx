import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Alert, AlertDescription } from '@/components/ui/alert';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
} from '@/components/ui/dialog';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Switch } from '@/components/ui/switch';
import {
  Upload,
  File,
  Download,
  Share2,
  Trash2,
  Eye,
  MoreVertical,
  Loader2,
  X,
  Plus,
  FileText,
  Video,
  Music,
  Image as ImageIcon,
  Folder
} from 'lucide-react';
import ProtectedMediaViewer from './ProtectedMediaViewer';
import apiService from '../services/api';
import { 
  CATEGORIA_ARQUIVO, 
  CATEGORIA_LABELS, 
  CATEGORIA_ICONS,
  TODOS_TIPOS_PERMITIDOS,
  MAX_FILE_SIZE,
  MESSAGES 
} from '../constants';
import { formatFileSize, validateFile } from '../utils';

const ArquivosModal = ({ isOpen, onClose, curso }) => {
  const [arquivos, setArquivos] = useState([]);
  const [loading, setLoading] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState('');
  const [activeTab, setActiveTab] = useState('todos');
  const [showUploadForm, setShowUploadForm] = useState(false);
  
  // Form de upload
  const [uploadForm, setUploadForm] = useState({
    arquivo: null,
    nome: '',
    descricao: '',
    categoria: CATEGORIA_ARQUIVO.OUTROS,
    isPublico: false,
    permiteDownload: true,
    permiteCompartilhamento: true
  });

  // Estados para visualizador de mídia
  const [showViewer, setShowViewer] = useState(false);
  const [selectedFile, setSelectedFile] = useState(null);
  const [protectionSettings, setProtectionSettings] = useState({
    allowDownload: true,
    allowSeek: true,
    allowSpeedControl: true,
    allowPrint: false,
    watermarkText: '',
    restrictedDomain: null
  });

  useEffect(() => {
    if (isOpen && curso) {
      loadArquivos();
    }
  }, [isOpen, curso]);

  const loadArquivos = async () => {
    try {
      setLoading(true);
      setError('');
      
      const response = await apiService.getArquivos(curso.id);
      
      if (response.success) {
        setArquivos(response.data);
      } else {
        setError(response.message || MESSAGES.ERROR.GENERIC);
      }
    } catch (error) {
      console.error('Erro ao carregar arquivos:', error);
      setError(error.message || MESSAGES.ERROR.NETWORK);
    } finally {
      setLoading(false);
    }
  };

  const handleFileSelect = (event) => {
    const file = event.target.files[0];
    if (!file) return;

    const validation = validateFile(file);
    if (!validation.valid) {
      setError(validation.message);
      return;
    }

    setUploadForm(prev => ({
      ...prev,
      arquivo: file,
      nome: file.name.split('.').slice(0, -1).join('.')
    }));
    setError('');
  };

  const handleUpload = async () => {
    if (!uploadForm.arquivo) {
      setError('Selecione um arquivo');
      return;
    }

    if (!uploadForm.nome.trim()) {
      setError('Nome do arquivo é obrigatório');
      return;
    }

    try {
      setUploading(true);
      setError('');

      const formData = new FormData();
      formData.append('arquivo', uploadForm.arquivo);
      formData.append('nome', uploadForm.nome);
      formData.append('descricao', uploadForm.descricao);
      formData.append('categoria', uploadForm.categoria);
      formData.append('isPublico', uploadForm.isPublico);
      formData.append('permiteDownload', uploadForm.permiteDownload);
      formData.append('permiteCompartilhamento', uploadForm.permiteCompartilhamento);

      const response = await apiService.uploadArquivo(curso.id, formData);
      
      if (response.success) {
        setArquivos(prev => [...prev, response.data]);
        setShowUploadForm(false);
        resetUploadForm();
      } else {
        setError(response.message || MESSAGES.ERROR.GENERIC);
      }
    } catch (error) {
      console.error('Erro no upload:', error);
      setError(error.message || MESSAGES.ERROR.NETWORK);
    } finally {
      setUploading(false);
    }
  };

  const handleDelete = async (arquivoId) => {
    if (!confirm('Tem certeza que deseja excluir este arquivo?')) {
      return;
    }

    try {
      const response = await apiService.deleteArquivo(arquivoId);
      
      if (response.success) {
        setArquivos(prev => prev.filter(a => a.id !== arquivoId));
      } else {
        setError(response.message || MESSAGES.ERROR.GENERIC);
      }
    } catch (error) {
      console.error('Erro ao excluir arquivo:', error);
      setError(error.message || MESSAGES.ERROR.GENERIC);
    }
  };

  const handleDownload = async (arquivo) => {
    try {
      const response = await apiService.getDownloadUrl(arquivo.id);
      
      if (response.success) {
        window.open(response.data, '_blank');
      } else {
        setError(response.message || MESSAGES.ERROR.GENERIC);
      }
    } catch (error) {
      console.error('Erro ao baixar arquivo:', error);
      setError(error.message || MESSAGES.ERROR.GENERIC);
    }
  };

  const handleView = (arquivo) => {
    setSelectedFile(arquivo);
    setProtectionSettings({
      allowDownload: arquivo.permiteDownload,
      allowSeek: true,
      allowSpeedControl: true,
      allowPrint: false,
      watermarkText: curso?.nomeCurso || '',
      restrictedDomain: null
    });
    setShowViewer(true);
  };

  const closeViewer = () => {
    setShowViewer(false);
    setSelectedFile(null);
  };

  const resetUploadForm = () => {
    setUploadForm({
      arquivo: null,
      nome: '',
      descricao: '',
      categoria: CATEGORIA_ARQUIVO.OUTROS,
      isPublico: false,
      permiteDownload: true,
      permiteCompartilhamento: true
    });
  };

  const getFileIcon = (tipoMime) => {
    if (tipoMime?.startsWith('video/')) return Video;
    if (tipoMime?.startsWith('audio/')) return Music;
    if (tipoMime?.startsWith('image/')) return ImageIcon;
    return FileText;
  };

  const arquivosPorCategoria = arquivos.reduce((acc, arquivo) => {
    const categoria = arquivo.categoria || 'Outros';
    if (!acc[categoria]) acc[categoria] = [];
    acc[categoria].push(arquivo);
    return acc;
  }, {});

  const ArquivoCard = ({ arquivo }) => {
    const IconComponent = getFileIcon(arquivo.tipoMime);
    
    return (
      <Card className="hover:shadow-md transition-shadow">
        <CardContent className="p-4">
          <div className="flex items-start justify-between mb-3">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <IconComponent className="h-5 w-5 text-blue-600" />
              </div>
              <div className="flex-1 min-w-0">
                <h4 className="font-medium text-sm truncate">{arquivo.nome}</h4>
                <p className="text-xs text-gray-500">
                  {formatFileSize(arquivo.tamanhoBytes)}
                </p>
              </div>
            </div>
            
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                  <MoreVertical className="h-4 w-4" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuItem onClick={() => handleDownload(arquivo)}>
                  <Download className="mr-2 h-4 w-4" />
                  Download
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => handleView(arquivo)}>
                  <Eye className="mr-2 h-4 w-4" />
                  Visualizar
                </DropdownMenuItem>
                <DropdownMenuItem>
                  <Share2 className="mr-2 h-4 w-4" />
                  Compartilhar
                </DropdownMenuItem>
                <DropdownMenuItem 
                  onClick={() => handleDelete(arquivo.id)}
                  className="text-red-600"
                >
                  <Trash2 className="mr-2 h-4 w-4" />
                  Excluir
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
          
          {arquivo.descricao && (
            <p className="text-xs text-gray-600 mb-2">{arquivo.descricao}</p>
          )}
          
          <div className="flex items-center gap-2">
            <Badge variant="outline" className="text-xs">
              {CATEGORIA_LABELS[arquivo.categoria]}
            </Badge>
            {arquivo.isPublico && (
              <Badge variant="secondary" className="text-xs">
                Público
              </Badge>
            )}
          </div>
        </CardContent>
      </Card>
    );
  };

  if (!isOpen) return null;

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-hidden">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Folder className="h-5 w-5" />
            Arquivos do Curso: {curso?.nomeCurso}
          </DialogTitle>
          <DialogDescription>
            Gerencie os arquivos educacionais deste curso
          </DialogDescription>
        </DialogHeader>

        <div className="flex-1 overflow-hidden">
          {error && (
            <Alert variant="destructive" className="mb-4">
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <div className="flex items-center justify-between mb-4">
            <div className="flex items-center gap-2">
              <Badge variant="outline">
                {arquivos.length} arquivo{arquivos.length !== 1 ? 's' : ''}
              </Badge>
            </div>
            
            <Button 
              onClick={() => setShowUploadForm(true)}
              disabled={uploading}
            >
              <Plus className="h-4 w-4 mr-2" />
              Adicionar Arquivo
            </Button>
          </div>

          {showUploadForm && (
            <Card className="mb-4">
              <CardHeader>
                <CardTitle className="text-lg flex items-center justify-between">
                  Upload de Arquivo
                  <Button 
                    variant="ghost" 
                    size="sm"
                    onClick={() => setShowUploadForm(false)}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <Label htmlFor="arquivo">Arquivo</Label>
                  <Input
                    id="arquivo"
                    type="file"
                    onChange={handleFileSelect}
                    accept={TODOS_TIPOS_PERMITIDOS.map(ext => `.${ext}`).join(',')}
                  />
                  <p className="text-xs text-gray-500 mt-1">
                    Tamanho máximo: {formatFileSize(MAX_FILE_SIZE)}
                  </p>
                </div>

                <div>
                  <Label htmlFor="nome">Nome do Arquivo</Label>
                  <Input
                    id="nome"
                    value={uploadForm.nome}
                    onChange={(e) => setUploadForm(prev => ({ ...prev, nome: e.target.value }))}
                    placeholder="Nome do arquivo"
                  />
                </div>

                <div>
                  <Label htmlFor="categoria">Categoria</Label>
                  <Select 
                    value={uploadForm.categoria} 
                    onValueChange={(value) => setUploadForm(prev => ({ ...prev, categoria: value }))}
                  >
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {Object.entries(CATEGORIA_LABELS).map(([key, label]) => (
                        <SelectItem key={key} value={key}>
                          {label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>

                <div>
                  <Label htmlFor="descricao">Descrição (opcional)</Label>
                  <Textarea
                    id="descricao"
                    value={uploadForm.descricao}
                    onChange={(e) => setUploadForm(prev => ({ ...prev, descricao: e.target.value }))}
                    placeholder="Descrição do arquivo"
                    rows={3}
                  />
                </div>

                <div className="space-y-3">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="isPublico">Arquivo público</Label>
                    <Switch
                      id="isPublico"
                      checked={uploadForm.isPublico}
                      onCheckedChange={(checked) => setUploadForm(prev => ({ ...prev, isPublico: checked }))}
                    />
                  </div>

                  <div className="flex items-center justify-between">
                    <Label htmlFor="permiteDownload">Permite download</Label>
                    <Switch
                      id="permiteDownload"
                      checked={uploadForm.permiteDownload}
                      onCheckedChange={(checked) => setUploadForm(prev => ({ ...prev, permiteDownload: checked }))}
                    />
                  </div>

                  <div className="flex items-center justify-between">
                    <Label htmlFor="permiteCompartilhamento">Permite compartilhamento</Label>
                    <Switch
                      id="permiteCompartilhamento"
                      checked={uploadForm.permiteCompartilhamento}
                      onCheckedChange={(checked) => setUploadForm(prev => ({ ...prev, permiteCompartilhamento: checked }))}
                    />
                  </div>
                </div>

                <div className="flex gap-2">
                  <Button 
                    onClick={handleUpload}
                    disabled={uploading || !uploadForm.arquivo}
                  >
                    {uploading ? (
                      <>
                        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                        Enviando...
                      </>
                    ) : (
                      <>
                        <Upload className="mr-2 h-4 w-4" />
                        Enviar Arquivo
                      </>
                    )}
                  </Button>
                  
                  <Button 
                    variant="outline" 
                    onClick={() => setShowUploadForm(false)}
                  >
                    Cancelar
                  </Button>
                </div>
              </CardContent>
            </Card>
          )}

          <Tabs value={activeTab} onValueChange={setActiveTab}>
            <TabsList className="grid w-full grid-cols-4">
              <TabsTrigger value="todos">Todos</TabsTrigger>
              <TabsTrigger value="videos">Vídeos</TabsTrigger>
              <TabsTrigger value="documentos">Documentos</TabsTrigger>
              <TabsTrigger value="outros">Outros</TabsTrigger>
            </TabsList>

            <TabsContent value="todos" className="mt-4">
              {loading ? (
                <div className="flex items-center justify-center py-8">
                  <Loader2 className="h-6 w-6 animate-spin text-blue-600" />
                  <span className="ml-2">Carregando arquivos...</span>
                </div>
              ) : arquivos.length === 0 ? (
                <div className="text-center py-8 text-gray-500">
                  <File className="h-12 w-12 mx-auto mb-4 text-gray-300" />
                  <p>Nenhum arquivo encontrado</p>
                  <p className="text-sm">Clique em "Adicionar Arquivo" para começar</p>
                </div>
              ) : (
                <div className="space-y-4 max-h-96 overflow-y-auto">
                  {Object.entries(arquivosPorCategoria).map(([categoria, arquivosCategoria]) => (
                    <div key={categoria}>
                      <h4 className="font-medium text-sm text-gray-700 mb-2">
                        {CATEGORIA_LABELS[categoria] || categoria} ({arquivosCategoria.length})
                      </h4>
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                        {arquivosCategoria.map(arquivo => (
                          <ArquivoCard key={arquivo.id} arquivo={arquivo} />
                        ))}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </TabsContent>

            <TabsContent value="videos" className="mt-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                {arquivos
                  .filter(a => a.tipoMime?.startsWith('video/'))
                  .map(arquivo => (
                    <ArquivoCard key={arquivo.id} arquivo={arquivo} />
                  ))}
              </div>
            </TabsContent>

            <TabsContent value="documentos" className="mt-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                {arquivos
                  .filter(a => a.tipoMime?.includes('pdf') || a.tipoMime?.includes('document') || a.tipoMime?.includes('presentation'))
                  .map(arquivo => (
                    <ArquivoCard key={arquivo.id} arquivo={arquivo} />
                  ))}
              </div>
            </TabsContent>

            <TabsContent value="outros" className="mt-4">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                {arquivos
                  .filter(a => !a.tipoMime?.startsWith('video/') && 
                              !a.tipoMime?.includes('pdf') && 
                              !a.tipoMime?.includes('document') && 
                              !a.tipoMime?.includes('presentation'))
                  .map(arquivo => (
                    <ArquivoCard key={arquivo.id} arquivo={arquivo} />
                  ))}
              </div>
            </TabsContent>
          </Tabs>
        </div>
      </DialogContent>

      {/* Modal do Visualizador de Mídia */}
      <Dialog open={showViewer} onOpenChange={closeViewer}>
        <DialogContent className="max-w-6xl max-h-[95vh] overflow-hidden">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Eye className="h-5 w-5" />
              {selectedFile?.nome || 'Visualizar Arquivo'}
            </DialogTitle>
            <DialogDescription>
              Visualização protegida do arquivo
            </DialogDescription>
          </DialogHeader>

          <div className="flex-1 overflow-auto">
            {selectedFile && (
              <ProtectedMediaViewer
                arquivo={selectedFile}
                protectionSettings={protectionSettings}
                onPlay={() => console.log('Play:', selectedFile.nome)}
                onPause={() => console.log('Pause:', selectedFile.nome)}
                onTimeUpdate={(time) => console.log('Time update:', time)}
                onEnded={() => console.log('Ended:', selectedFile.nome)}
                onView={() => console.log('View:', selectedFile.nome)}
                onDownload={() => handleDownload(selectedFile)}
              />
            )}
          </div>
        </DialogContent>
      </Dialog>
    </Dialog>
  );
};

export default ArquivosModal;

