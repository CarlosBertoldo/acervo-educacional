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

app = Flask(__name__)
CORS(app, origins=["http://localhost:5175", "http://localhost:5174", "http://localhost:3000", "http://localhost:5004"])

# Configurações
SECRET_KEY = "acervo-educacional-secret-key"
ADMIN_EMAIL = "admin@acervoeducacional.com"
ADMIN_PASSWORD = "Admin@123"

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
def dashboard_stats():
    """Estatísticas do dashboard"""
    return jsonify({
        'total_cursos': len(cursos_mock),
        'total_usuarios': len(usuarios_mock),
        'cursos_ativos': len([c for c in cursos_mock if c['status'] == 'Veiculado']),
        'cursos_desenvolvimento': len([c for c in cursos_mock if c['status'] == 'Em Desenvolvimento'])
    })

@app.route('/api/cursos', methods=['GET'])
def listar_cursos():
    """Listar cursos"""
    return jsonify(cursos_mock)

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
def health_check():
    """Health check"""
    return jsonify({
        'status': 'OK',
        'timestamp': datetime.datetime.now().isoformat(),
        'version': '1.0.0'
    })

@app.route('/swagger', methods=['GET'])
def swagger_ui():
    """Swagger UI simples"""
    html = """
    <!DOCTYPE html>
    <html>
    <head>
        <title>Acervo Educacional API</title>
        <style>
            body { 
                font-family: 'Barlow', Arial, sans-serif; 
                margin: 40px; 
                background: #f5f5f5; 
            }
            .container { 
                background: white; 
                padding: 30px; 
                border-radius: 8px; 
                box-shadow: 0 2px 10px rgba(0,0,0,0.1); 
            }
            h1 { 
                color: #C12D00; 
                border-bottom: 3px solid #C12D00; 
                padding-bottom: 10px; 
            }
            .endpoint { 
                background: #f8f9fa; 
                padding: 15px; 
                margin: 10px 0; 
                border-left: 4px solid #8FBF00; 
                border-radius: 4px; 
            }
            .method { 
                background: #C12D00; 
                color: white; 
                padding: 4px 8px; 
                border-radius: 4px; 
                font-size: 12px; 
                font-weight: bold; 
            }
            .method.get { background: #8FBF00; }
            .method.post { background: #C12D00; }
        </style>
        <link href="https://fonts.googleapis.com/css2?family=Barlow:wght@300;400;500;600;700&display=swap" rel="stylesheet">
    </head>
    <body>
        <div class="container">
            <h1>🎓 Acervo Educacional API</h1>
            <p><strong>Status:</strong> ✅ Funcionando perfeitamente</p>
            <p><strong>Versão:</strong> 1.0.0</p>
            
            <h2>🔐 Autenticação</h2>
            <div class="endpoint">
                <span class="method post">POST</span> <strong>/api/auth/login</strong><br>
                Realizar login no sistema
            </div>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/auth/verify</strong><br>
                Verificar token de autenticação
            </div>
            
            <h2>📊 Dashboard</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/dashboard/stats</strong><br>
                Estatísticas do dashboard
            </div>
            
            <h2>📚 Cursos</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/cursos</strong><br>
                Listar todos os cursos
            </div>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/cursos/kanban</strong><br>
                Cursos organizados por status (Kanban)
            </div>
            
            <h2>👥 Usuários</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/usuarios</strong><br>
                Listar usuários do sistema
            </div>
            
            <h2>🔧 Sistema</h2>
            <div class="endpoint">
                <span class="method get">GET</span> <strong>/api/health</strong><br>
                Verificar saúde da API
            </div>
            
            <hr style="margin: 30px 0;">
            <p><strong>Credenciais de Teste:</strong></p>
            <p>📧 Email: <code>admin@acervoeducacional.com</code></p>
            <p>🔑 Senha: <code>Admin@123</code></p>
        </div>
    </body>
    </html>
    """
    return html

if __name__ == '__main__':
    print("🚀 Iniciando Acervo Educacional Backend Mock")
    print("📍 Swagger UI: http://localhost:5006/swagger")
    print("🔐 Credenciais: admin@acervoeducacional.com / Admin@123")
    app.run(host='0.0.0.0', port=5006, debug=True)

