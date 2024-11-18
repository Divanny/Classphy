import { Component } from '@angular/core';
import { ImportsModule } from '../../imports';
import { NavbarComponent } from '../../layouts/navbar/navbar.component';
import { SidebarDesktopComponent } from '../../layouts/sidebar-desktop/sidebar-desktop.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  standalone: true,
  imports: [ImportsModule, NavbarComponent, SidebarDesktopComponent]
})
export class HomeComponent {

}
