# 🔐 Credenciais de Login - Acervo Educacional

## 👤 **Usuário Administrador Padrão**

### 📧 **Email:**
```
admin@acervo.com
```

### 🔑 **Senha:**
```
Admin123!
```

## ✅ **Correções Aplicadas:**

### 🔧 **Problemas Corrigidos:**
1. **Rotas da API** - Adicionado `/v1/` nas rotas do frontend
2. **Estrutura de resposta** - Corrigido para `{ accessToken, refreshToken, usuario }`
3. **Claims JWT** - Ajustado para usar `"sub"`, `"admin"` etc.
4. **Injeção de dependência** - AuthService registrado corretamente

### 🎯 **Como testar:**
1. Acesse o sistema
2. Use as credenciais acima
3. O login deve funcionar normalmente agora

### 📝 **Observações:**
- Todos os usuários são administradores por padrão
- O sistema cria automaticamente o usuário admin na primeira execução
- As senhas são criptografadas com BCrypt

---
**Sistema Acervo Educacional - Login Funcional** ✅

