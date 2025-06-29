/**
 * Testes para SecurityUtils
 * Acervo Educacional - Ferreira Costa
 */

import SecurityUtils from '../SecurityUtils';

// Mock do DOMPurify
jest.mock('dompurify', () => ({
  sanitize: jest.fn((input) => {
    // Simular comportamento básico do DOMPurify
    if (typeof input !== 'string') return '';
    return input.replace(/<script[^>]*>.*?<\/script>/gi, '');
  })
}));

describe('SecurityUtils', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('sanitizeInput', () => {
    test('deve sanitizar string normal', () => {
      const input = 'Texto normal';
      const result = SecurityUtils.sanitizeInput(input);
      expect(result).toBe('Texto normal');
    });

    test('deve retornar string vazia para input null', () => {
      const result = SecurityUtils.sanitizeInput(null);
      expect(result).toBe('');
    });

    test('deve retornar string vazia para input undefined', () => {
      const result = SecurityUtils.sanitizeInput(undefined);
      expect(result).toBe('');
    });

    test('deve retornar string vazia para input não-string', () => {
      const result = SecurityUtils.sanitizeInput(123);
      expect(result).toBe('');
    });

    test('deve remover scripts maliciosos', () => {
      const input = 'Texto <script>alert("xss")</script> normal';
      const result = SecurityUtils.sanitizeInput(input);
      expect(result).toBe('Texto  normal');
    });
  });

  describe('sanitizeHTML', () => {
    test('deve sanitizar HTML básico', () => {
      const input = '<p>Parágrafo <strong>forte</strong></p>';
      const result = SecurityUtils.sanitizeHTML(input);
      expect(result).toBe('<p>Parágrafo <strong>forte</strong></p>');
    });

    test('deve retornar string vazia para input inválido', () => {
      expect(SecurityUtils.sanitizeHTML(null)).toBe('');
      expect(SecurityUtils.sanitizeHTML(undefined)).toBe('');
      expect(SecurityUtils.sanitizeHTML(123)).toBe('');
    });
  });

  describe('isValidEmail', () => {
    test('deve validar email correto', () => {
      const validEmails = [
        'admin@acervoeducacional.com',
        'user@example.com',
        'test.email@domain.co.uk',
        'user+tag@example.org'
      ];

      validEmails.forEach(email => {
        expect(SecurityUtils.isValidEmail(email)).toBe(true);
      });
    });

    test('deve rejeitar email inválido', () => {
      const invalidEmails = [
        'email-sem-arroba',
        '@domain.com',
        'user@',
        'user@domain',
        '',
        null,
        undefined,
        123
      ];

      invalidEmails.forEach(email => {
        expect(SecurityUtils.isValidEmail(email)).toBe(false);
      });
    });

    test('deve trimar espaços do email', () => {
      expect(SecurityUtils.isValidEmail('  admin@acervoeducacional.com  ')).toBe(true);
    });
  });

  describe('validatePassword', () => {
    test('deve validar senha correta', () => {
      const result = SecurityUtils.validatePassword('Admin@123');
      expect(result.isValid).toBe(true);
      expect(result.errors).toHaveLength(0);
    });

    test('deve rejeitar senha muito curta', () => {
      const result = SecurityUtils.validatePassword('123');
      expect(result.isValid).toBe(false);
      expect(result.errors).toContain('Senha deve ter pelo menos 6 caracteres');
    });

    test('deve rejeitar senha muito longa', () => {
      const longPassword = 'a'.repeat(129);
      const result = SecurityUtils.validatePassword(longPassword);
      expect(result.isValid).toBe(false);
      expect(result.errors).toContain('Senha deve ter no máximo 128 caracteres');
    });

    test('deve rejeitar senha null/undefined', () => {
      expect(SecurityUtils.validatePassword(null).isValid).toBe(false);
      expect(SecurityUtils.validatePassword(undefined).isValid).toBe(false);
      expect(SecurityUtils.validatePassword('').isValid).toBe(false);
    });

    test('deve rejeitar input não-string', () => {
      const result = SecurityUtils.validatePassword(123);
      expect(result.isValid).toBe(false);
      expect(result.errors).toContain('Senha é obrigatória');
    });
  });

  describe('removeSpecialChars', () => {
    test('deve remover caracteres especiais perigosos', () => {
      const input = 'Texto<>com"caracteres\'&especiais';
      const result = SecurityUtils.removeSpecialChars(input);
      expect(result).toBe('Textocomcaracteresespeciais');
    });

    test('deve manter texto normal', () => {
      const input = 'Texto normal sem caracteres especiais';
      const result = SecurityUtils.removeSpecialChars(input);
      expect(result).toBe('Texto normal sem caracteres especiais');
    });

    test('deve retornar string vazia para input inválido', () => {
      expect(SecurityUtils.removeSpecialChars(null)).toBe('');
      expect(SecurityUtils.removeSpecialChars(undefined)).toBe('');
      expect(SecurityUtils.removeSpecialChars(123)).toBe('');
    });
  });

  describe('escapeHTML', () => {
    test('deve escapar caracteres HTML', () => {
      const input = '<script>alert("xss")</script>';
      const result = SecurityUtils.escapeHTML(input);
      expect(result).toBe('&lt;script&gt;alert(&quot;xss&quot;)&lt;&#x2F;script&gt;');
    });

    test('deve escapar todos os caracteres perigosos', () => {
      const input = '&<>"\'\/';
      const result = SecurityUtils.escapeHTML(input);
      expect(result).toBe('&amp;&lt;&gt;&quot;&#x27;&#x2F;');
    });

    test('deve retornar string vazia para input inválido', () => {
      expect(SecurityUtils.escapeHTML(null)).toBe('');
      expect(SecurityUtils.escapeHTML(undefined)).toBe('');
      expect(SecurityUtils.escapeHTML(123)).toBe('');
    });
  });

  describe('isValidJWTFormat', () => {
    test('deve validar formato JWT correto', () => {
      const validJWT = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c';
      expect(SecurityUtils.isValidJWTFormat(validJWT)).toBe(true);
    });

    test('deve rejeitar formato JWT inválido', () => {
      const invalidJWTs = [
        'token-sem-pontos',
        'apenas.dois',
        'um.ponto',
        '',
        null,
        undefined,
        123
      ];

      invalidJWTs.forEach(token => {
        expect(SecurityUtils.isValidJWTFormat(token)).toBe(false);
      });
    });

    test('deve rejeitar token com partes vazias', () => {
      const tokenWithEmptyPart = 'header..signature';
      expect(SecurityUtils.isValidJWTFormat(tokenWithEmptyPart)).toBe(false);
    });
  });

  describe('cleanFormData', () => {
    test('deve limpar dados do formulário', () => {
      const formData = {
        email: '  admin@acervoeducacional.com  ',
        password: '  Admin@123  ',
        name: 'João Silva'
      };

      const result = SecurityUtils.cleanFormData(formData);
      
      expect(result.email).toBe('admin@acervoeducacional.com');
      expect(result.password).toBe('Admin@123');
      expect(result.name).toBe('João Silva');
    });

    test('deve manter valores não-string', () => {
      const formData = {
        email: 'admin@acervoeducacional.com',
        age: 25,
        active: true,
        data: null
      };

      const result = SecurityUtils.cleanFormData(formData);
      
      expect(result.email).toBe('admin@acervoeducacional.com');
      expect(result.age).toBe(25);
      expect(result.active).toBe(true);
      expect(result.data).toBe(null);
    });

    test('deve retornar objeto vazio para input inválido', () => {
      expect(SecurityUtils.cleanFormData(null)).toEqual({});
      expect(SecurityUtils.cleanFormData(undefined)).toEqual({});
      expect(SecurityUtils.cleanFormData('string')).toEqual({});
      expect(SecurityUtils.cleanFormData(123)).toEqual({});
    });

    test('deve sanitizar strings no formulário', () => {
      const formData = {
        comment: 'Comentário <script>alert("xss")</script> normal'
      };

      const result = SecurityUtils.cleanFormData(formData);
      expect(result.comment).toBe('Comentário  normal');
    });
  });

  describe('default export', () => {
    test('deve exportar todas as funções', () => {
      expect(typeof SecurityUtils.sanitizeInput).toBe('function');
      expect(typeof SecurityUtils.sanitizeHTML).toBe('function');
      expect(typeof SecurityUtils.isValidEmail).toBe('function');
      expect(typeof SecurityUtils.validatePassword).toBe('function');
      expect(typeof SecurityUtils.removeSpecialChars).toBe('function');
      expect(typeof SecurityUtils.escapeHTML).toBe('function');
      expect(typeof SecurityUtils.isValidJWTFormat).toBe('function');
      expect(typeof SecurityUtils.cleanFormData).toBe('function');
    });
  });
});

