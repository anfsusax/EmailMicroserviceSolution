// Modelos TypeScript para autenticação

export interface GenerateTokenRequest {
  userName: string;
  email?: string;
  role?: string;
}

export interface TokenResponse {
  accessToken: string;
  expiresAtUtc: string;
}

