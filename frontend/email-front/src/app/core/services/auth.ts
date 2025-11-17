import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RegisterRequest, LoginRequest, TokenResponse, UserResponse, GenerateTokenRequest } from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/Auth`;

  constructor(private http: HttpClient) { }

  /**
   * Registra um novo usuário
   * @param request Dados do usuário (userName, email, password, role opcional)
   * @returns Observable com o token e data de expiração
   */
  register(request: RegisterRequest): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/register`, request);
  }

  /**
   * Faz login com credenciais reais
   * @param request Credenciais (userName, password)
   * @returns Observable com o token e data de expiração
   */
  login(request: LoginRequest): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/login`, request);
  }

  /**
   * Obtém dados do usuário logado
   * @returns Observable com os dados do usuário
   */
  getCurrentUser(): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.apiUrl}/me`);
  }

  /**
   * Gera um token JWT (método antigo - deprecated)
   * @param request Dados do usuário (userName, email opcional, role opcional)
   * @returns Observable com o token e data de expiração
   * @deprecated Use register() ou login() em vez disso
   */
  generateToken(request: GenerateTokenRequest): Observable<TokenResponse> {
    return this.http.post<TokenResponse>(`${this.apiUrl}/token`, request);
  }

  /**
   * Salva o token no localStorage
   * @param token Token JWT recebido da API
   */
  saveToken(token: string): void {
    localStorage.setItem('auth_token', token);
  }

  /**
   * Recupera o token do localStorage
   * @returns Token JWT ou null se não existir
   */
  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  /**
   * Remove o token do localStorage (logout)
   */
  clearToken(): void {
    localStorage.removeItem('auth_token');
  }

  /**
   * Verifica se o usuário está autenticado
   * @returns true se existe token, false caso contrário
   */
  isAuthenticated(): boolean {
    return this.getToken() !== null;
  }
}
