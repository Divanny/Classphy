import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { MessageService } from 'primeng/api';
import { Asistencia } from '../../models/asistencia.model';
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
  selector: 'app-attendance',
  templateUrl: './attendance.component.html',
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
export class AttendanceComponent implements OnInit {
  asignaturas: Asignatura[] = [];
  selectedAsignatura: Asignatura | undefined;
  selectedDate: Date | undefined;
  attendanceRecords: Asistencia[] = [];
  attendanceDialog: boolean = false;
  rollCallDialog: boolean = false;
  currentRollCallIndex: number = 0;
  errors: { [key: string]: string } = {};
  isDateValid: boolean = true;
  today: Date = new Date();

  constructor(private apiService: ApiService, private messageService: MessageService) {}

  ngOnInit() {
    this.loadAsignaturas();
    this.checkDateValidity();
    this.setupAutoLoadAttendance();
  }

  setupAutoLoadAttendance() {
    this.apiService.api.get('/Asignaturas').then(() => {
      this.loadAttendance();
    });
  }

  checkDateValidity() {
    if (this.selectedDate) {
      const today = new Date();
      this.isDateValid = this.selectedDate <= today;
    }
  }

  async loadAsignaturas() {
    const response = await this.apiService.api.get('/Asignaturas');
    console.log(response);
    this.asignaturas = response.data;
    this.loadAttendance();
  }

  async loadAttendance() {
    if (this.selectedAsignatura && this.selectedDate) {
      const formattedDate = this.selectedDate.toISOString().split('T')[0];
      const response = await this.apiService.api.post('/Asistencias/GetListadoAsistencias', {
        idAsignatura: this.selectedAsignatura.idAsignatura,
        fecha: formattedDate,
      });
      this.attendanceRecords = response.data;
      if (this.attendanceRecords.length === 0) {
        this.messageService.add({
          severity: 'info',
          summary: 'Sin estudiantes',
          detail: 'La asignatura no tiene ningún estudiante para la fecha seleccionada.',
        });
      }
    }
  }

  async saveAttendance() {
    if (this.attendanceRecords.length > 0) {
      const response = await this.apiService.api.post('/Asistencias', this.attendanceRecords);
      if (response.data.success) {
        this.messageService.add({ severity: 'success', summary: 'Asistencia guardada', detail: response.data.message });
      } else {
        this.messageService.add({ severity: 'warn', summary: 'Error al guardar asistencia', detail: response.data.message });
        this.errors = response.data.errors || {};
      }
    }
  }

  openAttendanceDialog() {
    this.attendanceDialog = true;
    this.errors = {};
  }

  hideDialog() {
    this.errors = {};
    this.attendanceDialog = false;
  }

  toggleAttendance(record: Asistencia) {
    record.presente = !record.presente;
  }

  startRollCall() {
    this.currentRollCallIndex = 0;
    this.rollCallDialog = true;
  }

  nextRollCall() {
    if (this.currentRollCallIndex < this.attendanceRecords.length - 1) {
      this.currentRollCallIndex++;
    } else {
      this.rollCallDialog = false;
      this.messageService.add({ severity: 'info', summary: 'Pase de Lista completo', detail: 'Se ha completado el pase de lista. Recuerde guardar los cambios.' });
    }
  }

  previousRollCall() {
    if (this.currentRollCallIndex > 0) {
      this.currentRollCallIndex--;
    }
  }

  markPresent() {
    this.attendanceRecords[this.currentRollCallIndex].presente = true;
    this.nextRollCall();
  }

  markAbsent() {
    this.attendanceRecords[this.currentRollCallIndex].presente = false;
    this.nextRollCall();
  }

  cancelRollCall() {
    this.rollCallDialog = false;
  }
}