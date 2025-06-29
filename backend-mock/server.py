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
CORS(app, origins=["http://localhost:5175", "http://localhost:5174", "http://localhost:5176", "http://localhost:3000", "http://localhost:5004"])

# Configurações
SECRET_KEY = "acervo-educacional-secret-key"

# Configuração de Logs Estruturados
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[
        logging.StreamHandler(sys.stdout)
    ]
)

logger = logging.getLogger(__name__)

# Sistema de Cache Simples em Memória
class SimpleCache:
    def __init__(self):
        self.cache = {}
        self.timestamps = {}
    
    def get(self, key):
        if key in self.cache:
            timestamp = self.timestamps.get(key, 0)
            if datetime.datetime.now().timestamp() - timestamp < self.ttl.get(key, 300):
                logger.info(f"Cache HIT para chave: {key}")
                return self.cache[key]
            else:
                logger.info(f"Cache EXPIRED para chave: {key}")
                self.delete(key)
        logger.info(f"Cache MISS para chave: {key}")
        return None
    
    def set(self, key, value, ttl=300):
        self.cache[key] = value
        self.timestamps[key] = datetime.datetime.now().timestamp()
        if not hasattr(self, 'ttl'):
            self.ttl = {}
        self.ttl[key] = ttl
        logger.info(f"Cache SET para chave: {key}, TTL: {ttl}s")
    
    def delete(self, key):
        if key in self.cache:
            del self.cache[key]
        if key in self.timestamps:
            del self.timestamps[key]
        if hasattr(self, 'ttl') and key in self.ttl:
            del self.ttl[key]
    
    def get_stats(self):
        return {
            'total_keys': len(self.cache),
            'cache_size_bytes': sys.getsizeof(self.cache),
            'oldest_entry': min(self.timestamps.values()) if self.timestamps else None
        }

cache = SimpleCache()

# Decorator para logs estruturados
def log_request(f):
    @wraps(f)
    def decorated_function(*args, **kwargs):
        start_time = datetime.datetime.now()
        
        # Log da requisição
        logger.info("REQUEST", extra={
            'method': request.method,
            'url': request.url,
            'remote_addr': request.remote_addr,
            'user_agent': request.headers.get('User-Agent', ''),
            'timestamp': start_time.isoformat()
        })
        
        try:
            result = f(*args, **kwargs)
            
            # Log da resposta
            end_time = datetime.datetime.now()
            duration = (end_time - start_time).total_seconds()
            
            logger.info("RESPONSE", extra={
                'method': request.method,
                'url': request.url,
                'status_code': getattr(result, 'status_code', 200),
                'duration_seconds': duration,
                'timestamp': end_time.isoformat()
            })
            
            return result
            
        except Exception as e:
            # Log do erro
            end_time = datetime.datetime.now()
            duration = (end_time - start_time).total_seconds()
            
            logger.error("ERROR", extra={
                'method': request.method,
                'url': request.url,
                'error': str(e),
                'duration_seconds': duration,
                'timestamp': end_time.isoformat()
            })
            
            raise
    
    return decorated_function

# Dados mock
usuarios_mock = [
    {
        'id': 1,
        'email': 'admin@acervoeducacional.com',
        'nome': 'Administrador',
        'senha': 'Admin@123',
        'is_admin': True
    }
]

cursos_mock = [
    {
        'id': 1,
        'titulo': 'Curso de Python',
        'categoria': 'Programação',
        'status': 'Veiculado'
    },
    {
        'id': 2,
        'titulo': 'Curso de React',
        'categoria': 'Frontend',
        'status': 'Em Desenvolvimento'
    },
    {
        'id': 3,
        'titulo': 'Curso de Docker',
        'categoria': 'DevOps',
        'status': 'Backlog'
    }
]

@app.route('/api/auth/login', methods=['POST'])
@app.route('/api/v1/auth/login', methods=['POST'])
@log_request
def login():
    """Realizar login no sistema"""
    try:
        data = request.get_json()
        
        if not data:
            return jsonify({
                'success': False,
                'message': 'Dados não fornecidos'
            }), 400
        
        email = data.get('email', '').strip().lower()
        senha = data.get('password', '')
        
        print(f"Tentativa de login: {email}")
        
        # Verificar credenciais
        usuario = next((u for u in usuarios_mock if u['email'].lower() == email and u['senha'] == senha), None)
        
        if usuario:
            # Gerar token JWT
            payload = {
                'user_id': usuario['id'],
                'email': usuario['email'],
                'is_admin': usuario['is_admin'],
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
            .status { 
                background: #28a745; 
                color: white; 
                padding: 4px 8px; 
                border-radius: 3px; 
                font-size: 11px; 
                margin-left: 10px;
            }
            .description { 
                color: #666; 
                margin-top: 8px; 
                font-style: italic;
            }
            .credentials {
                background: #e7f3ff;
                border: 1px solid #b3d9ff;
                padding: 15px;
                border-radius: 5px;
                margin: 20px 0;
            }
            .links {
                background: #f0f8f0;
                border: 1px solid #c3e6c3;
                padding: 15px;
                border-radius: 5px;
                margin: 20px 0;
            }
            .links a {
                color: #8FBF00;
                text-decoration: none;
                margin-right: 15px;
            }
            .links a:hover {
                text-decoration: underline;
            }
        </style>
    </head>
    <body>
        <div class="container">
            <h1>🎓 Acervo Educacional API</h1>
            <p><strong>Versão:</strong> 1.0.0 | <strong>Base URL:</strong> http://localhost:5005/api</p>
            <p class="description">API para gerenciamento do sistema de acervo educacional da Ferreira Costa.</p>
            
            <h2>🔐 Autenticação</h2>
            <div class="endpoint">
                <span class="method post">POST</span> <strong>/api/auth/login</strong> <span class="status">✅ Ativo</span>
                <div class="description">Realizar login no sistema</div>
            </div>
            
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/auth/verify</strong> <span class="status">✅ Ativo</span>
                <div class="description">Verificar token de autenticação</div>
            </div>
            
            <h2>📊 Dashboard</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/dashboard/stats</strong> <span class="status">✅ Ativo</span>
                <div class="description">Estatísticas do dashboard</div>
            </div>
            
            <h2>📚 Cursos</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/cursos</strong> <span class="status">✅ Ativo</span>
                <div class="description">Listar todos os cursos</div>
            </div>
            
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/cursos/kanban</strong> <span class="status">✅ Ativo</span>
                <div class="description">Cursos organizados por status (Kanban)</div>
            </div>
            
            <h2>👥 Usuários</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/usuarios</strong> <span class="status">✅ Ativo</span>
                <div class="description">Listar usuários do sistema</div>
            </div>
            
            <h2>🔧 Sistema</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/health</strong> <span class="status">✅ Ativo</span>
                <div class="description">Verificar saúde da API</div>
            </div>
            
            <div class="credentials">
                <h3>🔑 Credenciais de Teste:</h3>
                <p>📧 <strong>Email:</strong> admin@acervoeducacional.com</p>
                <p>🔒 <strong>Senha:</strong> Admin@123</p>
            </div>
            
            <h2>📝 Notas Importantes</h2>
            <ul>
                <li><strong>Autenticação:</strong> Use Bearer token no header Authorization</li>
                <li><strong>CORS:</strong> Configurado para localhost:5175, 5174, 3000, 5004</li>
                <li><strong>Cache:</strong> Estatísticas do dashboard são cacheadas por 2 minutos</li>
                <li><strong>Paginação:</strong> Todas as listagens suportam paginação</li>
                <li><strong>Logs:</strong> Todas as requisições são logadas em formato estruturado</li>
            </ul>
            
            <div class="links">
                <h3>🔗 Links Úteis:</h3>
                <a href="/api/health" target="_blank">Health Check</a>
                <a href="/api/dashboard/stats" target="_blank">Dashboard Stats</a>
                <a href="/api/cursos" target="_blank">Lista de Cursos</a>
                <a href="/api/usuarios" target="_blank">Lista de Usuários</a>
            </div>
        </div>
    </body>
    </html>
    """
    return html

if __name__ == '__main__':
    print("🚀 Iniciando Backend Mock")
    print("📍 Swagger UI: http://localhost:5005/swagger")
    print("🔐 Credenciais: admin@acervoeducacional.com / Admin@123")
    app.run(host='0.0.0.0', port=5007, debug=True)

