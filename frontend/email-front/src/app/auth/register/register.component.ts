import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth';
import { RegisterRequest, TokenResponse } from '../../core/models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.registerForm = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
      role: ['']
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }
    return null;
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request: RegisterRequest = {
      userName: this.registerForm.value.userName,
      email: this.registerForm.value.email,
      password: this.registerForm.value.password,
      role: this.registerForm.value.role || undefined
    };

    this.authService.register(request).subscribe({
      next: (response: TokenResponse) => {
        this.authService.saveToken(response.accessToken);
        this.successMessage = 'Cadastro realizado com sucesso! Redirecionando...';
        this.isLoading = false;
        setTimeout(() => {
          this.router.navigate(['/emails']);
        }, 1500);
      },
      error: (error: any) => {
        console.error('Erro ao cadastrar:', error);
        if (error.status === 0) {
          this.errorMessage = 'Não foi possível conectar à API. Verifique se a API está rodando em http://localhost:5041';
        } else if (error.status === 400) {
          this.errorMessage = error.error || 'Dados inválidos. Verifique o formulário.';
        } else {
          this.errorMessage = error.error?.message || error.message || 'Erro ao cadastrar. Tente novamente.';
        }
        this.isLoading = false;
      }
    });
  }
}

