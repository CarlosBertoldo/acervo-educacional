#!/usr/bin/env python3
"""
Backend Mock Funcional para Acervo Educacional
Demonstração completa de login e navegação
"""

from flask import Flask, request, jsonify
from flask_cors import CORS
import jwt
import datetime
import json
import logging
import sys
from functools import wraps

app = Flask(__name__)
CORS(app, origins=["http://localhost:5175", "http://localhost:5174", "http://localhost:3000", "http://localhost:5004"])

# Configurações
SECRET_KEY = "acervo-educacional-secret-key"
ADMIN_EMAIL = "admin@acervoeducacional.com"
ADMIN_PASSWORD = "Admin@123"

# Configuração de Logs Estruturados
class StructuredLogger:
    def __init__(self):
        self.logger = logging.getLogger('acervo_educacional')
        self.logger.setLevel(logging.INFO)
        
        # Handler para console
        console_handler = logging.StreamHandler(sys.stdout)
        console_handler.setLevel(logging.INFO)
        
        # Formatter JSON estruturado
        formatter = logging.Formatter(
            '{"timestamp": "%(asctime)s", "level": "%(levelname)s", "message": "%(message)s", "module": "%(name)s"}'
        )
        console_handler.setFormatter(formatter)
        
        self.logger.addHandler(console_handler)
    
    def info(self, message, **kwargs):
        log_data = {"message": message, **kwargs}
        self.logger.info(json.dumps(log_data))
    
    def error(self, message, **kwargs):
        log_data = {"message": message, **kwargs}
        self.logger.error(json.dumps(log_data))
    
    def warning(self, message, **kwargs):
        log_data = {"message": message, **kwargs}
        self.logger.warning(json.dumps(log_data))
    
    def debug(self, message, **kwargs):
        log_data = {"message": message, **kwargs}
        self.logger.debug(json.dumps(log_data))

# Instância global do logger
structured_logger = StructuredLogger()

# Decorator para log de requisições
def log_request(f):
    @wraps(f)
    def decorated_function(*args, **kwargs):
        start_time = datetime.datetime.now()
        
        # Log da requisição
        structured_logger.info("Request received", 
                             endpoint=request.endpoint,
                             method=request.method,
                             ip=request.remote_addr,
                             user_agent=request.headers.get('User-Agent', 'Unknown'))
        
        try:
            result = f(*args, **kwargs)
            
            # Log de sucesso
            end_time = datetime.datetime.now()
            duration = (end_time - start_time).total_seconds()
            
            structured_logger.info("Request completed successfully",
                                 endpoint=request.endpoint,
                                 method=request.method,
                                 duration_seconds=duration,
                                 status_code=getattr(result, 'status_code', 200))
            
            return result
            
        except Exception as e:
            # Log de erro
            end_time = datetime.datetime.now()
            duration = (end_time - start_time).total_seconds()
            
            structured_logger.error("Request failed",
                                  endpoint=request.endpoint,
                                  method=request.method,
                                  duration_seconds=duration,
                                  error=str(e),
                                  error_type=type(e).__name__)
            raise
    
    return decorated_function

# Sistema de Cache Simples em Memória
class SimpleCache:
    def __init__(self):
        self.cache = {}
        self.timestamps = {}
        self.default_ttl = 300  # 5 minutos em segundos
    
    def get(self, key: str):
        """Recupera um valor do cache"""
        if key not in self.cache:
            return None
        
        # Verificar se o cache expirou
        if self._is_expired(key):
            self.delete(key)
            return None
        
        logger.info("Cache hit", key=key)
        return self.cache[key]
    
    def set(self, key: str, value, ttl: int = None):
        """Armazena um valor no cache"""
        if ttl is None:
            ttl = self.default_ttl
        
        self.cache[key] = value
        self.timestamps[key] = {
            'created': datetime.datetime.now(),
            'ttl': ttl
        }
        
        logger.info("Cache set", key=key, ttl=ttl)
    
    def delete(self, key: str):
        """Remove um valor do cache"""
        if key in self.cache:
            del self.cache[key]
            del self.timestamps[key]
            logger.info("Cache deleted", key=key)
    
    def clear(self):
        """Limpa todo o cache"""
        self.cache.clear()
        self.timestamps.clear()
        logger.info("Cache cleared")
    
    def _is_expired(self, key: str) -> bool:
        """Verifica se uma entrada do cache expirou"""
        if key not in self.timestamps:
            return True
        
        timestamp_info = self.timestamps[key]
        created = timestamp_info['created']
        ttl = timestamp_info['ttl']
        
        return (datetime.datetime.now() - created).total_seconds() > ttl
    
    def get_stats(self):
        """Retorna estatísticas do cache"""
        total_keys = len(self.cache)
        expired_keys = sum(1 for key in self.cache.keys() if self._is_expired(key))
        
        return {
            'total_keys': total_keys,
            'active_keys': total_keys - expired_keys,
            'expired_keys': expired_keys
        }

# Instância global do cache
cache = SimpleCache()

# Decorator para log de requisições
def log_request(f):
    @wraps(f)
    def decorated_function(*args, **kwargs):
        start_time = datetime.datetime.now()
        
        # Log da requisição
        logger.info("Request received", 
                   endpoint=request.endpoint,
                   method=request.method,
                   ip=request.remote_addr,
                   user_agent=request.headers.get('User-Agent', 'Unknown'))
        
        try:
            result = f(*args, **kwargs)
            
            # Log de sucesso
            duration = (datetime.datetime.now() - start_time).total_seconds()
            logger.info("Request completed successfully",
                       endpoint=request.endpoint,
                       method=request.method,
                       duration_seconds=duration,
                       status_code=getattr(result, 'status_code', 200))
            
            return result
            
        except Exception as e:
            # Log de erro
            duration = (datetime.datetime.now() - start_time).total_seconds()
            logger.error("Request failed",
                        endpoint=request.endpoint,
                        method=request.method,
                        duration_seconds=duration,
                        error=str(e))
            raise
    
    return decorated_function

# Dados mock
usuarios_mock = [
    {
        "id": 1,
        "email": "admin@acervoeducacional.com",
        "nome": "Administrador",
        "is_admin": True
    }
]

cursos_mock = [
    {"id": 1, "titulo": "Curso de Python", "status": "Veiculado", "categoria": "Programação"},
    {"id": 2, "titulo": "Curso de React", "status": "Em Desenvolvimento", "categoria": "Frontend"},
    {"id": 3, "titulo": "Curso de Docker", "status": "Backlog", "categoria": "DevOps"},
]

@app.route('/api/auth/login', methods=['POST'])
@app.route('/api/v1/auth/login', methods=['POST'])
@log_request
def login():
    """Endpoint de autenticação"""
    try:
        data = request.get_json()
        email = data.get('email', '').strip()
        password = data.get('password', '').strip()
        
        print(f"Tentativa de login: {email}")
        
        # Validar credenciais
        if email == ADMIN_EMAIL and password == ADMIN_PASSWORD:
            # Gerar token JWT
            payload = {
                'user_id': 1,
                'email': email,
                'nome': 'Administrador',
                'is_admin': True,
                'exp': datetime.datetime.utcnow() + datetime.timedelta(hours=24)
            }
            
            token = jwt.encode(payload, SECRET_KEY, algorithm='HS256')
            
            return jsonify({
                'success': True,
                'message': 'Login realizado com sucesso',
                'data': {
                    'accessToken': token,
                    'refreshToken': token,  # Usando o mesmo token para simplificar
                    'usuario': {
                        'id': 1,
                        'email': email,
                        'nome': 'Administrador',
                        'is_admin': True
                    }
                },
                'accessToken': token,
                'refreshToken': token,
                'token': token,
                'user': {
                    'id': 1,
                    'email': email,
                    'nome': 'Administrador',
                    'is_admin': True
                }
            }), 200
        else:
            return jsonify({
                'success': False,
                'message': 'Credenciais inválidas'
            }), 401
            
    except Exception as e:
        print(f"Erro no login: {str(e)}")
        return jsonify({
            'success': False,
            'message': 'Erro interno do servidor'
        }), 500

@app.route('/api/auth/verify', methods=['GET'])
@app.route('/api/v1/auth/verify', methods=['GET'])
@app.route('/api/v1/auth/me', methods=['GET'])
@log_request
def verify_token():
    """Verificar token de autenticação"""
    try:
        auth_header = request.headers.get('Authorization')
        if not auth_header or not auth_header.startswith('Bearer '):
            return jsonify({'valid': False, 'message': 'Token não fornecido'}), 401
        
        token = auth_header.split(' ')[1]
        payload = jwt.decode(token, SECRET_KEY, algorithms=['HS256'])
        
        return jsonify({
            'valid': True,
            'user': {
                'id': payload['user_id'],
                'email': payload['email'],
                'nome': payload['nome'],
                'is_admin': payload['is_admin']
            }
        }), 200
        
    except jwt.ExpiredSignatureError:
        return jsonify({'valid': False, 'message': 'Token expirado'}), 401
    except jwt.InvalidTokenError:
        return jsonify({'valid': False, 'message': 'Token inválido'}), 401

@app.route('/api/dashboard/stats', methods=['GET'])
@log_request
def dashboard_stats():
    """Estatísticas do dashboard com cache"""
    cache_key = "dashboard_stats"
    
    # Tentar recuperar do cache primeiro
    cached_stats = cache.get(cache_key)
    if cached_stats:
        return jsonify(cached_stats)
    
    # Se não estiver no cache, calcular as estatísticas
    stats = {
        'total_cursos': len(cursos_mock),
        'total_usuarios': len(usuarios_mock),
        'cursos_ativos': len([c for c in cursos_mock if c['status'] == 'Veiculado']),
        'cursos_desenvolvimento': len([c for c in cursos_mock if c['status'] == 'Em Desenvolvimento']),
        'cache_info': 'Dados calculados e armazenados em cache',
        'timestamp': datetime.datetime.now().isoformat()
    }
    
    # Armazenar no cache por 2 minutos (120 segundos)
    cache.set(cache_key, stats, ttl=120)
    
    return jsonify(stats)

@app.route('/api/cursos', methods=['GET'])
@log_request
def listar_cursos():
    """Listar cursos com paginação"""
    # Parâmetros de paginação
    page = request.args.get('page', 1, type=int)
    per_page = request.args.get('per_page', 10, type=int)
    search = request.args.get('search', '', type=str)
    
    # Validar parâmetros
    if page < 1:
        page = 1
    if per_page < 1 or per_page > 100:
        per_page = 10
    
    # Filtrar cursos se houver busca
    filtered_cursos = cursos_mock
    if search:
        search_lower = search.lower()
        filtered_cursos = [
            curso for curso in cursos_mock 
            if search_lower in curso['titulo'].lower() or 
               search_lower in curso['categoria'].lower()
        ]
    
    # Calcular paginação
    total = len(filtered_cursos)
    start = (page - 1) * per_page
    end = start + per_page
    cursos_paginated = filtered_cursos[start:end]
    
    # Calcular informações de paginação
    total_pages = (total + per_page - 1) // per_page
    has_next = page < total_pages
    has_prev = page > 1
    
    return jsonify({
        'data': cursos_paginated,
        'pagination': {
            'page': page,
            'per_page': per_page,
            'total': total,
            'total_pages': total_pages,
            'has_next': has_next,
            'has_prev': has_prev,
            'next_page': page + 1 if has_next else None,
            'prev_page': page - 1 if has_prev else None
        },
        'search': search
    })

@app.route('/api/cursos/kanban', methods=['GET'])
def cursos_kanban():
    """Cursos organizados por status para Kanban"""
    kanban = {
        'backlog': [c for c in cursos_mock if c['status'] == 'Backlog'],
        'em_desenvolvimento': [c for c in cursos_mock if c['status'] == 'Em Desenvolvimento'],
        'veiculado': [c for c in cursos_mock if c['status'] == 'Veiculado']
    }
    return jsonify(kanban)

@app.route('/api/usuarios', methods=['GET'])
def listar_usuarios():
    """Listar usuários"""
    return jsonify(usuarios_mock)

@app.route('/api/health', methods=['GET'])
@log_request
def health_check():
    """Health check com informações detalhadas"""
    try:
        # Verificar status do cache
        cache_stats = cache.get_stats()
        
        # Informações do sistema
        health_info = {
            'status': 'healthy',
            'timestamp': datetime.datetime.now().isoformat(),
            'version': '1.0.0',
            'environment': 'development',
            'services': {
                'api': 'healthy',
                'cache': 'healthy',
                'logging': 'healthy'
            },
            'cache_stats': cache_stats,
            'endpoints': {
                'auth': 'active',
                'dashboard': 'active',
                'cursos': 'active',
                'usuarios': 'active'
            },
            'uptime_seconds': (datetime.datetime.now() - datetime.datetime.now().replace(hour=0, minute=0, second=0, microsecond=0)).total_seconds()
        }
        
        return jsonify(health_info), 200
        
    except Exception as e:
        logger.error("Health check failed", error=str(e))
        return jsonify({
            'status': 'unhealthy',
            'timestamp': datetime.datetime.now().isoformat(),
            'error': 'Health check failed',
            'version': '1.0.0'
        }), 500
    @app.route('/swagger', methods=['GET'])
def swagger_ui():
    """Swagger UI com documentação completa"""
    html = """
    <!DOCTYPE html>
    <html>
    <head>
        <title>Acervo Educacional API - Documentação</title>
        <style>
            body { 
                font-family: 'Barlow', Arial, sans-serif; 
                margin: 0; 
                background: #f5f5f5; 
                line-height: 1.6;
            }
            .container { 
                max-width: 1200px;
                margin: 0 auto;
                background: white; 
                padding: 40px; 
                box-shadow: 0 2px 20px rgba(0,0,0,0.1); 
            }
            h1 { 
                color: #C12D00; 
                border-bottom: 3px solid #C12D00; 
                padding-bottom: 15px; 
                margin-bottom: 30px;
            }
            h2 {
                color: #8FBF00;
                margin-top: 30px;
                margin-bottom: 15px;
            }
            .endpoint { 
                background: #f8f9fa; 
                padding: 20px; 
                margin: 15px 0; 
                border-left: 4px solid #8FBF00; 
                border-radius: 6px; 
            }
            .method { 
                background: #C12D00; 
                color: white; 
                padding: 6px 12px; 
                border-radius: 4px; 
                font-size: 12px; 
                font-weight: bold; 
                margin-right: 10px;
            }
            .method.get { background: #8FBF00; }
            .method.post { background: #C12D00; }
            .params {
                background: #e9ecef;
                padding: 10px;
                border-radius: 4px;
                margin: 10px 0;
                font-family: monospace;
                font-size: 14px;
            }
            .response {
                background: #d4edda;
                padding: 10px;
                border-radius: 4px;
                margin: 10px 0;
                font-family: monospace;
                font-size: 12px;
                border-left: 4px solid #28a745;
            }
            .description {
                color: #666;
                margin: 10px 0;
            }
            .status {
                display: inline-block;
                background: #28a745;
                color: white;
                padding: 2px 6px;
                border-radius: 3px;
                font-size: 11px;
                margin-left: 10px;
            }
            .auth-required {
                background: #fff3cd;
                color: #856404;
                padding: 5px 10px;
                border-radius: 4px;
                font-size: 12px;
                margin: 5px 0;
            }
            .cache-info {
                background: #cce5ff;
                color: #004085;
                padding: 5px 10px;
                border-radius: 4px;
                font-size: 12px;
                margin: 5px 0;
            }
        </style>
        <link href="https://fonts.googleapis.com/css2?family=Barlow:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    </head>
    <body>
        <div class="container">
            <h1>🎓 Acervo Educacional API</h1>
            <p><strong>Versão:</strong> 1.0.0 | <strong>Base URL:</strong> http://localhost:5005/api</p>
            <p class="description">API para gerenciamento do sistema de acervo educacional da Ferreira Costa.</p>
            
            <h2>🔐 Autenticação</h2>
            <div class="endpoint">
                <span class="method post">POST</span> <strong>/api/auth/login</strong> <span class="status">✅ Ativo</span>
                <div class="description">Realiza autenticação no sistema</div>
                <div class="params">
                    <strong>Body (JSON):</strong><br>
                    {<br>
                    &nbsp;&nbsp;"email": "admin@acervoeducacional.com",<br>
                    &nbsp;&nbsp;"password": "Admin@123"<br>
                    }
                </div>
                <div class="response">
                    <strong>Resposta (200):</strong><br>
                    {<br>
                    &nbsp;&nbsp;"success": true,<br>
                    &nbsp;&nbsp;"message": "Login realizado com sucesso",<br>
                    &nbsp;&nbsp;"data": {<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"accessToken": "jwt_token_here",<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"refreshToken": "jwt_token_here",<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"usuario": { ... }<br>
                    &nbsp;&nbsp;}<br>
                    }
                </div>
            </div>

            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/auth/verify</strong> <span class="status">✅ Ativo</span>
                <div class="auth-required">🔒 Requer autenticação: Bearer Token</div>
                <div class="description">Verifica validade do token de autenticação</div>
                <div class="params">
                    <strong>Headers:</strong><br>
                    Authorization: Bearer {token}
                </div>
            </div>

            <h2>📊 Dashboard</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/dashboard/stats</strong> <span class="status">✅ Ativo</span>
                <div class="cache-info">💾 Cache ativo: 2 minutos</div>
                <div class="description">Retorna estatísticas do dashboard</div>
                <div class="response">
                    <strong>Resposta (200):</strong><br>
                    {<br>
                    &nbsp;&nbsp;"total_cursos": 3,<br>
                    &nbsp;&nbsp;"total_usuarios": 2,<br>
                    &nbsp;&nbsp;"cursos_ativos": 1,<br>
                    &nbsp;&nbsp;"cursos_desenvolvimento": 1,<br>
                    &nbsp;&nbsp;"cache_info": "...",<br>
                    &nbsp;&nbsp;"timestamp": "2025-01-01T12:00:00"<br>
                    }
                </div>
            </div>

            <h2>📚 Cursos</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/cursos</strong> <span class="status">✅ Ativo</span>
                <div class="description">Lista cursos com paginação e busca</div>
                <div class="params">
                    <strong>Query Parameters (opcionais):</strong><br>
                    • page: Número da página (padrão: 1)<br>
                    • per_page: Itens por página (padrão: 10, máx: 100)<br>
                    • search: Termo de busca (título ou categoria)
                </div>
                <div class="response">
                    <strong>Exemplo:</strong> /api/cursos?page=1&per_page=5&search=react<br>
                    <strong>Resposta (200):</strong><br>
                    {<br>
                    &nbsp;&nbsp;"data": [...],<br>
                    &nbsp;&nbsp;"pagination": {<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"page": 1,<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"per_page": 5,<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"total": 3,<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"total_pages": 1,<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"has_next": false,<br>
                    &nbsp;&nbsp;&nbsp;&nbsp;"has_prev": false<br>
                    &nbsp;&nbsp;}<br>
                    }
                </div>
            </div>

            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/cursos/kanban</strong> <span class="status">✅ Ativo</span>
                <div class="description">Cursos organizados por status para visualização Kanban</div>
            </div>

            <h2>👥 Usuários</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/usuarios</strong> <span class="status">✅ Ativo</span>
                <div class="description">Lista usuários do sistema</div>
            </div>

            <h2>🔧 Sistema</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/health</strong> <span class="status">✅ Ativo</span>
                <div class="description">Verifica status de saúde da API</div>
                <div class="response">
                    <strong>Resposta (200):</strong><br>
                    {<br>
                    &nbsp;&nbsp;"status": "healthy",<br>
                    &nbsp;&nbsp;"timestamp": "2025-01-01T12:00:00",<br>
                    &nbsp;&nbsp;"version": "1.0.0",<br>
                    &nbsp;&nbsp;"cache_stats": { ... }<br>
                    }
                </div>
            </div>

            <h2>📝 Notas Importantes</h2>
            <ul>
                <li><strong>Autenticação:</strong> Use o token JWT no header Authorization</li>
                <li><strong>Cache:</strong> Estatísticas do dashboard são cacheadas por 2 minutos</li>
                <li><strong>Paginação:</strong> Todas as listagens suportam paginação</li>
                <li><strong>Logs:</strong> Todas as requisições são logadas em formato estruturado</li>
                <li><strong>CORS:</strong> Configurado para aceitar requisições do frontend</li>
            </ul>

            <h2>🔗 Links Úteis</h2>
            <ul>
                <li><a href="/api/health" target="_blank">Health Check</a></li>
                <li><a href="/api/dashboard/stats" target="_blank">Dashboard Stats</a></li>
                <li><a href="/api/cursos" target="_blank">Lista de Cursos</a></li>
            </ul>
            
            <hr style="margin: 30px 0;">
            <p style="text-align: center; color: #666; font-size: 14px;">
                🏢 <strong>Ferreira Costa</strong> - Sistema Acervo Educacional<br>
                Desenvolvido com Flask + React + TypeScript
            </p>
        </div>
    </body>
    </html>
    """
    return htmlckend Mock")
    print("📍 Swagger UI: http://localhost:5005/swagger")
    print("🔐 Credenciais: admin@acervoeducacional.com / Admin@123")
    app.run(host='0.0.0.0', port=5005, debug=True)

