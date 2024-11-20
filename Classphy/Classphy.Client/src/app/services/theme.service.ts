import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  private darkTheme = 'assets/themes/aura-dark-purple/theme.css';
  private lightTheme = 'assets/themes/aura-light-purple/theme.css';

  initializeTheme(isDarkMode: boolean): void {
    const themeLink = document.createElement('link');
    themeLink.id = 'theme-link';
    themeLink.rel = 'stylesheet';
    themeLink.href = isDarkMode ? this.darkTheme : this.lightTheme;
    document.head.appendChild(themeLink);
    this.setThemeClass(isDarkMode);
  }

  private setThemeClass(isDarkMode: boolean): void {
    const htmlElement = document.documentElement;
    if (isDarkMode) {
      htmlElement.classList.remove('p-light');
    } else {
      htmlElement.classList.add('p-light');
    }
  }

  toggleTheme(isDarkMode: boolean): void {
    const themeLink = document.getElementById('theme-link') as HTMLLinkElement;
    themeLink.href = isDarkMode ? this.darkTheme : this.lightTheme;
    this.setThemeClass(isDarkMode);
  }
}