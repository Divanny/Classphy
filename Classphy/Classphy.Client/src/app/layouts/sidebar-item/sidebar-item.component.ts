import { Component, Input } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-sidebar-item',
  templateUrl: './sidebar-item.component.html',
  standalone: true,
  imports: [CommonModule, ButtonModule],
})
export class SidebarItemComponent {
  @Input() item: MenuItem | undefined;

  constructor(private router: Router) {}

  isActive(): boolean {
    return this.router.url === this.item?.routerLink;
  }

  navigate(): void {
    this.router.navigate([this.item?.routerLink]);
  }
}
