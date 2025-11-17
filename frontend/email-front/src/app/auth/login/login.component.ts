import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth';
import { LoginRequest } from '../../core/models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const request: LoginRequest = {
      userName: this.loginForm.value.userName,
      password: this.loginForm.value.password
    };

    this.authService.login(request).subscribe({
      next: (response) => {
        this.authService.saveToken(response.accessToken);
        this.isLoading = false;
        this.router.navigate(['/emails']);
      },
      error: (error) => {
        console.error('Erro ao fazer login:', error);
        if (error.status === 0) {
          this.errorMessage = 'Não foi possível conectar à API. Verifique se a API está rodando em http://localhost:5041';
        } else if (error.status === 401) {
          this.errorMessage = error.error || 'Credenciais inválidas. Verifique seu usuário e senha.';
        } else if (error.status === 400) {
          this.errorMessage = error.error || 'Dados inválidos. Verifique o formulário.';
        } else {
          this.errorMessage = error.error?.message || error.message || 'Erro ao fazer login. Tente novamente.';
        }
        this.isLoading = false;
      }
    });
  }
}

