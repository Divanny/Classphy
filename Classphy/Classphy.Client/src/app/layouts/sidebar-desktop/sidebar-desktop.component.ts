import { Component, OnInit } from '@angular/core';
import { ImportsModule } from '../../imports';
import { MenuItem } from 'primeng/api';
import { ThemeService } from '../../services/theme.service';
import { Store } from '@ngrx/store';
import { logout } from '../../store/auth/auth.actions';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { SidebarItemComponent } from '../sidebar-item/sidebar-item.component';
import { AuthState } from '../../store/auth/auth.state';

@Component({
  selector: 'sidebar-desktop',
  templateUrl: './sidebar-desktop.component.html',
  standalone: true,
  imports: [ImportsModule, SidebarItemComponent],
  providers: [],
})
export class SidebarDesktopComponent implements OnInit {
  sidebarVisible: boolean = false;
  isDarkMode: boolean = true;
  sidebarItems: MenuItem[] | undefined;
  user: any;
  profileMenuItems: MenuItem[] = [];
  perfiles!: { [key: string]: number };

  constructor(
    private themeService: ThemeService,
    private messageService: MessageService,
    private store: Store,
    private router: Router
  ) {
    this.perfiles = {
      Administrador: 1,
      Profesor: 2
    }
    this.isDarkMode = localStorage.getItem('isDarkMode') === 'true';
    this.themeService.toggleTheme(this.isDarkMode);
  }

  toggleTheme(): void {
    this.profileMenuItems[0].icon = this.isDarkMode
      ? 'pi pi-sun'
      : 'pi pi-moon';
    this.isDarkMode = !this.isDarkMode;
    localStorage.setItem('isDarkMode', this.isDarkMode.toString());
    this.themeService.toggleTheme(this.isDarkMode);
  }

  ngOnInit() {
    this.profileMenuItems = [
      {
        label: 'Cambiar tema',
        icon: this.isDarkMode ? 'pi pi-sun' : 'pi pi-moon',
        command: () => this.toggleTheme(),
      },
      {
        label: 'Cerrar sesión',
        icon: 'pi pi-sign-out',
        command: () => this.logout(),
      },
    ];
    this.store.select((state: any) => state.auth).subscribe((authState: AuthState) => {
      this.user = authState.user;
    });
    this.sidebarItems = this.getStaticSidebarItems();
  }

  getStaticSidebarItems(): MenuItem[] {
    return [
      {
        label: 'Inicio',
        icon: 'pi pi-home',
        routerLink: '/',
      },
      {
        label: 'Calificaciones',
        icon: 'pi pi-star',
        routerLink: '/grades',
        visible: this.user?.idPerfil === this.perfiles['Profesor'],
      },
      { 
        label: 'Asistencia', 
        icon: 'pi pi-calendar', 
        routerLink: '/attendance',
        visible: this.user?.idPerfil === this.perfiles['Profesor'],
      },
      { 
        label: 'Usuarios', 
        icon: 'pi pi-user', 
        routerLink: '/users',
        visible: this.user?.idPerfil === this.perfiles['Administrador'],
      },
      { 
        label: 'Asignaturas', 
        icon: 'pi pi-book', 
        routerLink: '/subjects',
        visible: this.user?.idPerfil === this.perfiles['Profesor'],
      },
      {
        label: 'Estudiantes',
        icon: 'pi pi-graduation-cap',
        routerLink: '/students',
        visible: this.user?.idPerfil === this.perfiles['Profesor'],
      },
    ];
  }

  logout() {
    this.store.dispatch(logout());
    this.messageService.add({
      severity: 'info',
      summary: 'Sesión cerrada',
      detail: 'Hasta luego 👋'
    });
    this.router.navigate(['/sign-in']);
  }

  getFirstLastName(nombres: string, apellidos: string): string {
    const firstName = nombres.split(' ')[0];
    const lastName = apellidos.split(' ')[0];
    return `${firstName} ${lastName}`;
  }
}
