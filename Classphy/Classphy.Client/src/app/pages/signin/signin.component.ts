import { Component } from '@angular/core';
import { ImportsModule } from '../../imports';

@Component({
  selector: 'app-signin',
  templateUrl: './signin.component.html',
  standalone: true,
  imports: [ImportsModule]
})
export class SignInComponent {
  mail!: string;
  password!: string;

}
