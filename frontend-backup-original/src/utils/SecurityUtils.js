/**
 * Utilitários de Segurança para o Acervo Educacional
 * Proteção contra XSS e sanitização de dados
 */

import DOMPurify from 'dompurify';

/**
 * Sanitiza uma string removendo scripts maliciosos e tags perigosas
 * @param {string} input - String a ser sanitizada
 * @returns {string} String sanitizada
 */
export const sanitizeInput = (input) => {
  if (!input || typeof input !== 'string') {
    return '';
  }
  
  // Configuração básica do DOMPurify para remover scripts
  return DOMPurify.sanitize(input, {
    ALLOWED_TAGS: ['b', 'i', 'em', 'strong', 'p', 'br'],
    ALLOWED_ATTR: []
  });
};

/**
 * Sanitiza HTML permitindo apenas tags seguras
 * @param {string} html - HTML a ser sanitizado
 * @returns {string} HTML sanitizado
 */
export const sanitizeHTML = (html) => {
  if (!html || typeof html !== 'string') {
    return '';
  }
  
  return DOMPurify.sanitize(html, {
    ALLOWED_TAGS: ['p', 'br', 'strong', 'em', 'b', 'i', 'u', 'ul', 'ol', 'li'],
    ALLOWED_ATTR: ['class']
  });
};

/**
 * Valida se um email tem formato válido
 * @param {string} email - Email a ser validado
 * @returns {boolean} True se válido
 */
export const isValidEmail = (email) => {
  if (!email || typeof email !== 'string') {
    return false;
  }
  
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email.trim());
};

/**
 * Valida se uma senha atende aos critérios mínimos
 * @param {string} password - Senha a ser validada
 * @returns {object} Objeto com resultado da validação
 */
export const validatePassword = (password) => {
  if (!password || typeof password !== 'string') {
    return {
      isValid: false,
      errors: ['Senha é obrigatória']
    };
  }
  
  const errors = [];
  
  if (password.length < 6) {
    errors.push('Senha deve ter pelo menos 6 caracteres');
  }
  
  if (password.length > 128) {
    errors.push('Senha deve ter no máximo 128 caracteres');
  }
  
  return {
    isValid: errors.length === 0,
    errors
  };
};

/**
 * Remove caracteres especiais perigosos de uma string
 * @param {string} input - String a ser limpa
 * @returns {string} String limpa
 */
export const removeSpecialChars = (input) => {
  if (!input || typeof input !== 'string') {
    return '';
  }
  
  // Remove caracteres que podem ser usados em ataques
  return input.replace(/[<>\"'&]/g, '');
};

/**
 * Escapa caracteres especiais para uso seguro em HTML
 * @param {string} input - String a ser escapada
 * @returns {string} String escapada
 */
export const escapeHTML = (input) => {
  if (!input || typeof input !== 'string') {
    return '';
  }
  
  const escapeMap = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#x27;',
    '/': '&#x2F;'
  };
  
  return input.replace(/[&<>"'/]/g, (char) => escapeMap[char]);
};

/**
 * Valida se um token JWT tem formato básico válido
 * @param {string} token - Token a ser validado
 * @returns {boolean} True se tem formato válido
 */
export const isValidJWTFormat = (token) => {
  if (!token || typeof token !== 'string') {
    return false;
  }
  
  // JWT deve ter 3 partes separadas por pontos
  const parts = token.split('.');
  return parts.length === 3 && parts.every(part => part.length > 0);
};

/**
 * Limpa dados de formulário removendo espaços e sanitizando
 * @param {object} formData - Dados do formulário
 * @returns {object} Dados limpos
 */
export const cleanFormData = (formData) => {
  if (!formData || typeof formData !== 'object') {
    return {};
  }
  
  const cleaned = {};
  
  for (const [key, value] of Object.entries(formData)) {
    if (typeof value === 'string') {
      cleaned[key] = sanitizeInput(value.trim());
    } else {
      cleaned[key] = value;
    }
  }
  
  return cleaned;
};

export default {
  sanitizeInput,
  sanitizeHTML,
  isValidEmail,
  validatePassword,
  removeSpecialChars,
  escapeHTML,
  isValidJWTFormat,
  cleanFormData
};

