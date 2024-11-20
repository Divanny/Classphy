import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { MessageService } from 'primeng/api';
import { Asignatura } from '../../models/asignatura.model';
import { ImportsModule } from '../../imports';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { CheckboxModule } from 'primeng/checkbox';
import { CalendarModule } from 'primeng/calendar';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es';

registerLocaleData(localeEs, 'es');

@Component({
  selector: 'app-grades',
  templateUrl: './grades.component.html',
  standalone: true,
  imports: [
    ImportsModule,
    TableModule,
    ButtonModule,
    DialogModule,
    DropdownModule,
    CheckboxModule,
    CalendarModule,
  ],
  providers: [],
})
export class GradesComponent implements OnInit {
  asignaturas: Asignatura[] = [];
  selectedAsignatura: Asignatura | undefined;
  grades: any[] = [];
  errors: { [key: string]: string } = {};
  loadingGrades: boolean = false;

  constructor(private apiService: ApiService, private messageService: MessageService) {}

  ngOnInit() {
    this.loadAsignaturas();
  }

  async loadAsignaturas() {
    const response = await this.apiService.api.get('/Asignaturas');
    this.asignaturas = response.data;
    this.loadGrades();
  }

  async loadGrades() {
    if (this.selectedAsignatura) {
      const response = await this.apiService.api.get(`/Calificaciones/GetListadoCalificaciones/${this.selectedAsignatura.idAsignatura}`);
      this.grades = response.data;
      this.grades.forEach(grade => {
        grade.literal = this.calculateLiteral(grade);
      });
    }
  }

  async saveGrades() {
    this.loadingGrades = true;
    if (this.grades.length > 0) {
      const response = await this.apiService.api.post('/Calificaciones', this.grades);
      if (response.data.success) {
        this.messageService.add({ severity: 'success', summary: 'Calificaciones guardadas', detail: response.data.message });
      } else {
        this.messageService.add({ severity: 'warn', summary: 'Error al guardar calificaciones', detail: response.data.message });
        this.errors = response.data.errors || {};
      }
    }
    this.loadingGrades = false;
  }

  calculateLiteral(grade: any) {
    if (grade.final === null) return '';
    const total = grade.medioTermino + grade.final;
    if (total >= 90) return 'A';
    if (total >= 80) return 'B';
    if (total >= 70) return 'C';
    return 'F';
  }
}