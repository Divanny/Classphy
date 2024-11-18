import { Component } from '@angular/core';
import { ImportsModule } from '../../imports';
import { NavbarComponent } from '../../layouts/navbar/navbar.component';
import { SidebarDesktopComponent } from '../../layouts/sidebar-desktop/sidebar-desktop.component';
import { CommonModule } from '@angular/common';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  standalone: true,
  providers: [MessageService],
  imports: [
    CommonModule,
    ImportsModule,
    NavbarComponent,
    SidebarDesktopComponent,
  ],
})
export class LayoutComponent {
  constructor(private messageService: MessageService) {}
}
