import { Component, OnInit } from '@angular/core';
import { ImportsModule } from '../../imports';
import { MenuItem } from 'primeng/api';
import { SidebarItemComponent } from '../sidebar-item/sidebar-item.component';
import { AuthState } from '../../store/auth/auth.state';
import { Store } from '@ngrx/store';

@Component({
  selector: 'app-sidebar-mobile',
  templateUrl: './sidebar-mobile.component.html',
  standalone: true,
  imports: [ImportsModule, SidebarItemComponent],
})
export class SideBarMobileComponent implements OnInit {
  sidebarVisible: boolean = false;
  isDarkMode: boolean = true;
  sidebarItems: MenuItem[] | undefined;
  user: any;
  perfiles!: { [key: string]: number };

  constructor(private store: Store) {
    this.perfiles = {
      Administrador: 1,
      Profesor: 2,
    };
  }

  ngOnInit() {
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
}
