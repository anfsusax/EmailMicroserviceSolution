// Modelos TypeScript para autenticação

// Request para registro de novo usuário
export interface RegisterRequest {
  userName: string;
  email: string;
  password: string;
  role?: string;
}

// Request para login
export interface LoginRequest {
  userName: string;
  password: string;
}

// Response do token
export interface TokenResponse {
  accessToken: string;
  expiresAtUtc: string;
}

// Response do usuário
export interface UserResponse {
  id: string;
  userName: string;
  email: string;
  role?: string;
  createdAt: string;
}

// Mantido para compatibilidade (deprecated)
export interface GenerateTokenRequest {
  userName: string;
  email?: string;
  role?: string;
}

