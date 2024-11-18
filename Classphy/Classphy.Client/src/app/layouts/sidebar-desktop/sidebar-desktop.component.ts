import { Component, OnInit } from '@angular/core';
import { ImportsModule } from '../../imports';
import { MenuItem } from 'primeng/api';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'sidebar-desktop',
  templateUrl: './sidebar-desktop.component.html',
  standalone: true,
  imports: [ImportsModule],
})
export class SidebarDesktopComponent implements OnInit {
  sidebarVisible: boolean = false;
  isDarkMode: boolean = true;
  sidebarItems: MenuItem[] | undefined;
  user = { nombres: 'Juan', apellidos: 'Pérez' };
  profileMenuItems: MenuItem[] = [];

  constructor(private themeService: ThemeService) {}

  toggleTheme(): void {
    this.isDarkMode = !this.isDarkMode;
    this.themeService.toggleTheme(this.isDarkMode);
  }

  ngOnInit() {
    this.sidebarItems = this.getStaticSidebarItems();
    this.profileMenuItems = [
      {
        label: 'Cambiar tema',
        icon: this.isDarkMode ? 'pi pi-sun' : 'pi pi-moon',
        command: () => this.toggleDarkMode(),
      },
      {
        label: 'Cerrar sesión',
        icon: 'pi pi-sign-out',
        command: () => this.logout(),
      },
    ];
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
        icon: 'pi pi-search',
        routerLink: '/Consultas',
      },
      { label: 'Asistencia', icon: 'pi pi-calendar', routerLink: '/Reportes' },
      { label: 'Usuarios', icon: 'pi pi-user', routerLink: '/users' },
      { label: 'Materias', icon: 'pi pi-book', routerLink: '/subjects' },
      {
        label: 'Estudiantes',
        icon: 'pi pi-graduation-cap',
        routerLink: '/students',
      },
    ];
  }
  toggleDarkMode() {
    this.isDarkMode = !this.isDarkMode;
    this.profileMenuItems[0].icon = this.isDarkMode
      ? 'pi pi-sun'
      : 'pi pi-moon';
    document.documentElement.classList.toggle('p-dark', this.isDarkMode);
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    localStorage.removeItem('sessionExpireTime');
    console.log('Sesión cerrada');
  }

  getFirstLastName(nombres: string, apellidos: string): string {
    const firstName = nombres.split(' ')[0];
    const lastName = apellidos.split(' ')[0];
    return `${firstName} ${lastName}`;
  }
}
