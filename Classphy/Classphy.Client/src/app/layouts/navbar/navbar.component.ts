import { Component } from '@angular/core';
import { ImportsModule } from '../../imports';
import { MenuItem } from 'primeng/api';
import { SideBarMobileComponent } from '../sidebar-mobile/sidebar-mobile.component';
import { Store } from '@ngrx/store';
import { logout } from '../../store/auth/auth.actions';
import { MessageService } from 'primeng/api';
import { Router } from '@angular/router';
import { AuthState } from '../../store/auth/auth.state';
import { ThemeService } from '../../services/theme.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  standalone: true,
  imports: [
    ImportsModule,
    SideBarMobileComponent
  ],
  providers: []
})
export class NavbarComponent {
  user: any;
  isDarkMode = false;
  profileMenuItems: MenuItem[] ;

  constructor(
    private messageService: MessageService,
    private store: Store,
    private router: Router,
    private themeService: ThemeService
  ) {
    this.isDarkMode = localStorage.getItem('isDarkMode') === 'true';
    this.themeService.toggleTheme(this.isDarkMode);
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
    this.store.select((state: any) => state.auth).subscribe((authState: AuthState) => {
      this.user = authState.user;
    });
  }

  toggleDarkMode() {
    this.isDarkMode = !this.isDarkMode;
    localStorage.setItem('isDarkMode', this.isDarkMode.toString());
    this.themeService.toggleTheme(this.isDarkMode);
    this.profileMenuItems[0].icon = this.isDarkMode ? 'pi pi-sun' : 'pi pi-moon';
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
    const firstName = nombres.split(" ")[0];
    const lastName = apellidos.split(" ")[0];
    return `${firstName} ${lastName}`;
  }
}
