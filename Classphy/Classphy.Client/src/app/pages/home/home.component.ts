import { Component, OnInit } from '@angular/core';
import { ImportsModule } from '../../imports';
import { CardModule } from 'primeng/card';
import { ButtonModule } from 'primeng/button';
import { Router } from '@angular/router';
import { AuthState } from '../../store/auth/auth.state';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  standalone: true,
  imports: [
    ImportsModule,
    CardModule,
    ButtonModule,
  ]
})
export class HomeComponent implements OnInit {
  welcomeMessage: string | undefined;
  features: { title: string; description: string; icon: string; route: string }[] | undefined;
  user: any;
  perfiles!: { [key: string]: number };
  administrador: boolean | undefined;

  constructor(private store: Store, private router: Router) {
    this.perfiles = {
      Administrador: 1,
      Profesor: 2,
    };
  }

  ngOnInit() {
    this.store.select((state: any) => state.auth).subscribe((authState: AuthState) => {
      this.user = authState.user;
      if (this.user) {
        this.administrador = this.user.idPerfil === this.perfiles['Administrador'];
        this.initializeContent();
      }
    });
  }

  initializeContent() {
    if (this.administrador) {
      this.welcomeMessage = "Bienvenido Administrador";
      this.features = [
        { title: 'Gestión de Usuarios', description: 'Administra usuarios y asigna roles.', icon: 'pi pi-users', route: '/users' }
      ];
    } else {
      this.welcomeMessage = "Bienvenido a Classphy, tu sistema de gestión educativa.";
      this.features = [
        { title: 'Calificaciones', description: 'Gestiona y visualiza las calificaciones de los estudiantes.', icon: 'pi pi-star', route: '/grades' },
        { title: 'Asistencia', description: 'Realiza el seguimiento de la asistencia de los estudiantes.', icon: 'pi pi-calendar', route: '/attendance' },
        { title: 'Gestión de Asignaturas', description: 'Administra todas las asignaturas de manera eficiente.', icon: 'pi pi-book', route: '/subjects' },
        { title: 'Gestión de Estudiantes', description: 'Mantén un registro actualizado de todos los estudiantes.', icon: 'pi pi-graduation-cap', route: '/students' }
      ];
    }
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
  }
}