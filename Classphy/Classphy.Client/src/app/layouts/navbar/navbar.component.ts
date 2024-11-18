import { Component } from '@angular/core';
import { ImportsModule } from '../../imports';
import { MenuItem } from 'primeng/api';
import { SideBarMobileComponent } from '../sidebar-mobile/sidebar-mobile.component';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  standalone: true,
  imports: [
    ImportsModule,
    SideBarMobileComponent
  ]
})
export class NavbarComponent {
  user = { nombres: 'Juan', apellidos: 'Pérez' };  // Datos estáticos para el nombre del usuario.
  isDarkMode = false;
  profileMenuItems: MenuItem[] ;

  constructor() {
    this.profileMenuItems = [
      {
        label: 'Cambiar tema',
        icon: this.isDarkMode ? 'pi pi-sun' : 'pi pi-moon',
        command: () => this.toggleDarkMode()
      },
      {
        label: 'Cerrar sesión',
        icon: 'pi pi-sign-out',
        command: () => this.logout()
      }
    ];
  }

  toggleDarkMode() {
    this.isDarkMode = !this.isDarkMode;
    this.profileMenuItems[0].icon = this.isDarkMode ? 'pi pi-sun' : 'pi pi-moon';
    document.documentElement.classList.toggle('p-dark', this.isDarkMode);
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    localStorage.removeItem('sessionExpireTime');
    console.log('Sesión cerrada');
    // Navegación simulada; en tu proyecto puedes usar Router para redirigir a la página de login.
  }

  getFirstLastName(nombres: string, apellidos: string): string {
    const firstName = nombres.split(" ")[0];
    const lastName = apellidos.split(" ")[0];
    return `${firstName} ${lastName}`;
  }
}
