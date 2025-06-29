import React, { useEffect, useRef, useState } from 'react';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { Card, CardContent } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { 
  Play, 
  Pause, 
  Volume2, 
  VolumeX, 
  Maximize, 
  Shield, 
  Lock,
  Eye,
  EyeOff
} from 'lucide-react';

const ProtectedVideoPlayer = ({ 
  src, 
  title, 
  allowDownload = false,
  allowSeek = true,
  allowSpeedControl = true,
  watermarkText = '',
  restrictedDomain = null,
  onPlay = () => {},
  onPause = () => {},
  onTimeUpdate = () => {},
  onEnded = () => {}
}) => {
  const videoRef = useRef(null);
  const containerRef = useRef(null);
  const overlayRef = useRef(null);
  
  const [isPlaying, setIsPlaying] = useState(false);
  const [isMuted, setIsMuted] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);
  const [volume, setVolume] = useState(1);
  const [isFullscreen, setIsFullscreen] = useState(false);
  const [showControls, setShowControls] = useState(true);
  const [protectionActive, setProtectionActive] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const video = videoRef.current;
    if (!video) return;

    // Configurações de proteção
    video.controlsList = 'nodownload noremoteplayback';
    video.disablePictureInPicture = true;
    video.preload = 'metadata';

    // Event listeners para proteção
    const handleContextMenu = (e) => {
      e.preventDefault();
      return false;
    };

    const handleKeyDown = (e) => {
      // Bloquear teclas de atalho comuns
      const blockedKeys = [
        'F12', // DevTools
        'PrintScreen', // Print Screen
        'Insert', // Insert
      ];

      const blockedCombinations = [
        { ctrl: true, key: 's' }, // Ctrl+S (Save)
        { ctrl: true, key: 'S' },
        { ctrl: true, key: 'p' }, // Ctrl+P (Print)
        { ctrl: true, key: 'P' },
        { ctrl: true, key: 'u' }, // Ctrl+U (View Source)
        { ctrl: true, key: 'U' },
        { ctrl: true, shift: true, key: 'I' }, // Ctrl+Shift+I (DevTools)
        { ctrl: true, shift: true, key: 'i' },
        { ctrl: true, shift: true, key: 'J' }, // Ctrl+Shift+J (Console)
        { ctrl: true, shift: true, key: 'j' },
        { ctrl: true, shift: true, key: 'C' }, // Ctrl+Shift+C (Inspect)
        { ctrl: true, shift: true, key: 'c' },
        { alt: true, key: 'Tab' }, // Alt+Tab
        { alt: true, key: 'F4' }, // Alt+F4
      ];

      if (blockedKeys.includes(e.key)) {
        e.preventDefault();
        return false;
      }

      for (const combo of blockedCombinations) {
        if (
          (combo.ctrl && e.ctrlKey) &&
          (combo.shift ? e.shiftKey : !e.shiftKey) &&
          (combo.alt ? e.altKey : !e.altKey) &&
          e.key === combo.key
        ) {
          e.preventDefault();
          return false;
        }
      }
    };

    const handleSelectStart = (e) => {
      e.preventDefault();
      return false;
    };

    const handleDragStart = (e) => {
      e.preventDefault();
      return false;
    };

    // Detectar tentativas de captura de tela
    const handleVisibilityChange = () => {
      if (document.hidden && isPlaying) {
        video.pause();
        setIsPlaying(false);
      }
    };

    // Detectar DevTools
    const detectDevTools = () => {
      const threshold = 160;
      if (
        window.outerHeight - window.innerHeight > threshold ||
        window.outerWidth - window.innerWidth > threshold
      ) {
        video.pause();
        setIsPlaying(false);
        setError('Ferramentas de desenvolvedor detectadas. Reprodução pausada por segurança.');
      }
    };

    // Event listeners do vídeo
    const handleLoadedMetadata = () => {
      setDuration(video.duration);
    };

    const handleTimeUpdate = () => {
      setCurrentTime(video.currentTime);
      onTimeUpdate(video.currentTime);
    };

    const handlePlay = () => {
      setIsPlaying(true);
      onPlay();
    };

    const handlePause = () => {
      setIsPlaying(false);
      onPause();
    };

    const handleEnded = () => {
      setIsPlaying(false);
      onEnded();
    };

    const handleVolumeChange = () => {
      setVolume(video.volume);
      setIsMuted(video.muted);
    };

    // Adicionar event listeners
    document.addEventListener('contextmenu', handleContextMenu);
    document.addEventListener('keydown', handleKeyDown);
    document.addEventListener('selectstart', handleSelectStart);
    document.addEventListener('dragstart', handleDragStart);
    document.addEventListener('visibilitychange', handleVisibilityChange);

    video.addEventListener('loadedmetadata', handleLoadedMetadata);
    video.addEventListener('timeupdate', handleTimeUpdate);
    video.addEventListener('play', handlePlay);
    video.addEventListener('pause', handlePause);
    video.addEventListener('ended', handleEnded);
    video.addEventListener('volumechange', handleVolumeChange);

    // Detectar DevTools periodicamente
    const devToolsInterval = setInterval(detectDevTools, 1000);

    // Cleanup
    return () => {
      document.removeEventListener('contextmenu', handleContextMenu);
      document.removeEventListener('keydown', handleKeyDown);
      document.removeEventListener('selectstart', handleSelectStart);
      document.removeEventListener('dragstart', handleDragStart);
      document.removeEventListener('visibilitychange', handleVisibilityChange);

      if (video) {
        video.removeEventListener('loadedmetadata', handleLoadedMetadata);
        video.removeEventListener('timeupdate', handleTimeUpdate);
        video.removeEventListener('play', handlePlay);
        video.removeEventListener('pause', handlePause);
        video.removeEventListener('ended', handleEnded);
        video.removeEventListener('volumechange', handleVolumeChange);
      }

      clearInterval(devToolsInterval);
    };
  }, [isPlaying, onPlay, onPause, onTimeUpdate, onEnded]);

  // Verificar domínio restrito
  useEffect(() => {
    if (restrictedDomain) {
      const currentDomain = window.location.hostname;
      if (!currentDomain.includes(restrictedDomain)) {
        setError(`Este conteúdo só pode ser reproduzido em: ${restrictedDomain}`);
        return;
      }
    }
  }, [restrictedDomain]);

  const togglePlay = () => {
    const video = videoRef.current;
    if (!video) return;

    if (isPlaying) {
      video.pause();
    } else {
      video.play().catch(err => {
        console.error('Erro ao reproduzir vídeo:', err);
        setError('Erro ao reproduzir o vídeo. Tente novamente.');
      });
    }
  };

  const toggleMute = () => {
    const video = videoRef.current;
    if (!video) return;

    video.muted = !video.muted;
  };

  const handleSeek = (e) => {
    if (!allowSeek) return;
    
    const video = videoRef.current;
    if (!video) return;

    const rect = e.currentTarget.getBoundingClientRect();
    const pos = (e.clientX - rect.left) / rect.width;
    video.currentTime = pos * duration;
  };

  const handleVolumeChange = (e) => {
    const video = videoRef.current;
    if (!video) return;

    const newVolume = parseFloat(e.target.value);
    video.volume = newVolume;
    setVolume(newVolume);
  };

  const toggleFullscreen = () => {
    const container = containerRef.current;
    if (!container) return;

    if (!isFullscreen) {
      if (container.requestFullscreen) {
        container.requestFullscreen();
      } else if (container.webkitRequestFullscreen) {
        container.webkitRequestFullscreen();
      } else if (container.msRequestFullscreen) {
        container.msRequestFullscreen();
      }
    } else {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      } else if (document.webkitExitFullscreen) {
        document.webkitExitFullscreen();
      } else if (document.msExitFullscreen) {
        document.msExitFullscreen();
      }
    }
  };

  const formatTime = (time) => {
    const minutes = Math.floor(time / 60);
    const seconds = Math.floor(time % 60);
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  };

  if (error) {
    return (
      <Card className="w-full">
        <CardContent className="p-6">
          <Alert variant="destructive">
            <Shield className="h-4 w-4" />
            <AlertDescription>{error}</AlertDescription>
          </Alert>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card className="w-full overflow-hidden">
      <CardContent className="p-0">
        <div 
          ref={containerRef}
          className="relative bg-black group"
          onMouseEnter={() => setShowControls(true)}
          onMouseLeave={() => setShowControls(false)}
        >
          {/* Overlay de proteção */}
          <div 
            ref={overlayRef}
            className="absolute inset-0 z-10 pointer-events-none"
            style={{
              background: 'transparent',
              userSelect: 'none',
              WebkitUserSelect: 'none',
              MozUserSelect: 'none',
              msUserSelect: 'none'
            }}
          />

          {/* Watermark */}
          {watermarkText && (
            <div className="absolute top-4 right-4 z-20 bg-black bg-opacity-50 text-white text-xs px-2 py-1 rounded pointer-events-none">
              {watermarkText}
            </div>
          )}

          {/* Indicador de proteção */}
          <div className="absolute top-4 left-4 z-20 flex items-center gap-2">
            <Badge variant="secondary" className="bg-red-600 text-white">
              <Lock className="h-3 w-3 mr-1" />
              Protegido
            </Badge>
            {!allowDownload && (
              <Badge variant="secondary" className="bg-orange-600 text-white">
                <EyeOff className="h-3 w-3 mr-1" />
                Sem Download
              </Badge>
            )}
          </div>

          {/* Vídeo */}
          <video
            ref={videoRef}
            src={src}
            className="w-full h-auto"
            style={{
              userSelect: 'none',
              WebkitUserSelect: 'none',
              MozUserSelect: 'none',
              msUserSelect: 'none'
            }}
            onContextMenu={(e) => e.preventDefault()}
            onSelectStart={(e) => e.preventDefault()}
            onDragStart={(e) => e.preventDefault()}
            disablePictureInPicture
            controlsList="nodownload noremoteplayback"
            playsInline
          />

          {/* Controles customizados */}
          <div 
            className={`absolute bottom-0 left-0 right-0 bg-gradient-to-t from-black to-transparent p-4 transition-opacity duration-300 ${
              showControls ? 'opacity-100' : 'opacity-0'
            }`}
          >
            {/* Barra de progresso */}
            <div 
              className="w-full h-2 bg-gray-600 rounded-full mb-4 cursor-pointer"
              onClick={handleSeek}
            >
              <div 
                className="h-full bg-blue-500 rounded-full transition-all duration-100"
                style={{ width: `${(currentTime / duration) * 100}%` }}
              />
            </div>

            {/* Controles */}
            <div className="flex items-center justify-between text-white">
              <div className="flex items-center gap-4">
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={togglePlay}
                  className="text-white hover:bg-white hover:bg-opacity-20"
                >
                  {isPlaying ? <Pause className="h-5 w-5" /> : <Play className="h-5 w-5" />}
                </Button>

                <div className="flex items-center gap-2">
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={toggleMute}
                    className="text-white hover:bg-white hover:bg-opacity-20"
                  >
                    {isMuted ? <VolumeX className="h-4 w-4" /> : <Volume2 className="h-4 w-4" />}
                  </Button>
                  
                  <input
                    type="range"
                    min="0"
                    max="1"
                    step="0.1"
                    value={isMuted ? 0 : volume}
                    onChange={handleVolumeChange}
                    className="w-20 h-1 bg-gray-600 rounded-lg appearance-none cursor-pointer"
                  />
                </div>

                <span className="text-sm">
                  {formatTime(currentTime)} / {formatTime(duration)}
                </span>
              </div>

              <div className="flex items-center gap-2">
                {title && (
                  <span className="text-sm text-gray-300 mr-4">{title}</span>
                )}
                
                <Button
                  variant="ghost"
                  size="sm"
                  onClick={toggleFullscreen}
                  className="text-white hover:bg-white hover:bg-opacity-20"
                >
                  <Maximize className="h-4 w-4" />
                </Button>
              </div>
            </div>
          </div>
        </div>
      </CardContent>
    </Card>
  );
};

export default ProtectedVideoPlayer;

