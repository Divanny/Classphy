import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { PrimeNGConfig } from 'primeng/api';
import esLocale from '../utils/es_locale.json';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
  constructor(private http: HttpClient, private primengConfig: PrimeNGConfig) {}

  ngOnInit() {
    this.primengConfig.zIndex = {
      modal: 1100,
      overlay: 1000,
      menu: 1000,
      tooltip: 1100,
    };
    this.primengConfig.setTranslation(esLocale);
  }

  title = 'Classphy';
}
