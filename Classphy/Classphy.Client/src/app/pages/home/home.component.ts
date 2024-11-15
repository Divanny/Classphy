import { Component } from '@angular/core';
import { ImportsModule } from '../../imports';
import { NavbarComponent } from '../../layouts/navbar/navbar.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  standalone: true,
  imports: [ImportsModule, NavbarComponent]
})
export class HomeComponent {

}
