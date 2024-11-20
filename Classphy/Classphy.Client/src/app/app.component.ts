import { Component, OnInit } from '@angular/core';
import { PrimeNGConfig, MessageService } from 'primeng/api';
import esLocale from '../utils/es_locale.json';
import { ThemeService } from './services/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  constructor(
    private primengConfig: PrimeNGConfig, 
    private messageService: MessageService,
    private themeService: ThemeService
  ) {}

  ngOnInit() {
    this.primengConfig.zIndex = {
      modal: 1100,
      overlay: 1000,
      menu: 1000,
      tooltip: 1100,
      toast: 1400
    };
    this.primengConfig.setTranslation(esLocale);

    // Initialize theme based on localStorage
    const isDarkMode = localStorage.getItem('isDarkMode') === 'true';
    this.themeService.initializeTheme(isDarkMode);

    // Example of using MessageService to show a toast
    this.messageService.add({severity:'success', summary:'Service Message', detail:'Via MessageService'});
  }

  title = 'Classphy';
}