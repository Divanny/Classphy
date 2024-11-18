import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private darkTheme = 'primeng/resources/themes/aura-dark-purple/theme.css';
  private lightTheme = 'primeng/resources/themes/aura-light-purple/theme.css';

  toggleTheme(isDarkMode: boolean): void {
    const themeLink = document.getElementById('theme-link') as HTMLLinkElement;
    const htmlElement = document.documentElement;

    if (isDarkMode) {
      themeLink.href = this.darkTheme;
      htmlElement.classList.add('p-dark');
    } else {
      themeLink.href = this.lightTheme;
      htmlElement.classList.remove('p-dark');
    }
  }
}