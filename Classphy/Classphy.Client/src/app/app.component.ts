import { Component, OnInit } from '@angular/core';
import { PrimeNGConfig, MessageService } from 'primeng/api';
import esLocale from '../utils/es_locale.json';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  constructor(private primengConfig: PrimeNGConfig, private messageService: MessageService) {}

  ngOnInit() {
    this.primengConfig.zIndex = {
      modal: 1100,
      overlay: 1000,
      menu: 1000,
      tooltip: 1100,
      toast: 1400
    };
    this.primengConfig.setTranslation(esLocale);

    // Example of using MessageService to show a toast
    this.messageService.add({severity:'success', summary:'Service Message', detail:'Via MessageService'});
  }

  title = 'Classphy';
}