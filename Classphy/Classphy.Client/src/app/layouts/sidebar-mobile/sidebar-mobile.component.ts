import { Component, OnInit } from '@angular/core';
import { ImportsModule } from '../../imports';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-sidebar-mobile',
  templateUrl: './sidebar-mobile.component.html',
  standalone: true,
  imports: [ImportsModule],
})
export class SideBarMobileComponent implements OnInit {
  sidebarVisible: boolean = false;
  isDarkMode: boolean = true;
  sidebarItems: MenuItem[] | undefined;

  ngOnInit() {
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
}
