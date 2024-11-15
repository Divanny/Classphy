import { Component, OnInit } from '@angular/core';
import { ImportsModule } from '../../imports';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-sidebar-mobile',
  templateUrl: './sidebar-mobile.component.html',
  standalone: true,
  imports: [
    ImportsModule
  ]
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
        label: "Inicio",
        icon: "pi pi-home",
        routerLink: "/"
      },
      {
        label: "Procesos Nómina",
        items: [
          { label: "Precarga", icon: "pi pi-upload", routerLink: "/Precarga" },
          { label: "Carga", icon: "pi pi-folder", routerLink: "/Carga" },
          { label: "Variación", icon: "pi pi-sort-alt", routerLink: "/Variacion" },
          { label: "Exclusiones", icon: "pi pi-ban", routerLink: "/Exclusiones" }
        ]
      },
      {
        label: "Consultas",
        items: [
          { label: "Consultas", icon: "pi pi-search", routerLink: "/Consultas" },
          { label: "Reportes", icon: "pi pi-chart-bar", routerLink: "/Reportes" },
          { label: "Reimpresión", icon: "pi pi-print", routerLink: "/Reimpresion" }
        ]
      },
      {
        label: "Otros",
        items: [
          { label: "Configuración", icon: "pi pi-cog", routerLink: "/Configuracion" }
        ]
      }
    ];
  }
}