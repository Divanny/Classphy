import { Component } from '@angular/core';
import { ImportsModule } from '../../imports';
import { ApiService } from '../../services/api.service';
import { MessageService } from 'primeng/api';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { login } from '../../store/auth/auth.actions';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  standalone: true,
  imports: [ImportsModule],
  providers: [MessageService],
})
export class SignInComponent {
  signInForm: FormGroup;

  constructor(
    private apiService: ApiService,
    private messageService: MessageService,
    private store: Store,
    private router: Router
  ) {
    this.signInForm = new FormGroup({
      username: new FormControl<string | null>(null, Validators.required),
      password: new FormControl<string | null>(null, Validators.required),
    });
  }

  errors: { [key: string]: string } = {};

  async signIn(e: Event) {
    e.preventDefault();
    this.errors = {};

    if (this.signInForm.invalid) {
      if (this.signInForm.controls['username'].hasError('required')) {
        this.errors['username'] = 'El nombre de usuario es requerido';
      }
      if (this.signInForm.controls['password'].hasError('required')) {
        this.errors['password'] = 'La contraseña es requerida';
      }
      return;
    }

    try {
      const response = await this.apiService.api.post('/account', {
        username: this.signInForm.value.username,
        password: this.signInForm.value.password,
      });

      if (response.data.success) {
        const { data, token } = response.data;
        this.messageService.add({
          severity: 'success',
          summary: 'Inicio de sesión exitoso',
          detail: `Bienvenido, ${data.nombreUsuario}.`,
        });
        this.store.dispatch(login({ user: data, token }));
        this.router.navigate(['/']);
      } else {
        this.messageService.add({
          severity: 'warn',
          summary: 'Error al iniciar sesión',
          detail: 'Usuario o contraseña incorrectos',
        });
      }
    } catch (error: any) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error al iniciar sesión',
        detail: error.message,
      });
    }
  }
}
