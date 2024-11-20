import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Asignatura } from '../../models/asignatura.model';
import { Periodo } from '../../models/periodo.model';
import { ImportsModule } from '../../imports';
import { TableModule } from 'primeng/table';
import { Table } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { DatePipe } from '@angular/common';
import { PickListModule } from 'primeng/picklist';

@Component({
  selector: 'app-subjects',
  templateUrl: './subjects.component.html',
  standalone: true,
  imports: [
    ImportsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    DialogModule,
    DropdownModule,
    PickListModule,
  ],
  providers: [DatePipe, ConfirmationService],
})
export class SubjectsComponent implements OnInit {
  asignaturas: Asignatura[] = [];
  periodos: Periodo[] = [];
  selectedPeriodo: Periodo = { idPeriodo: 0, nombre: '', idUsuario: 0, fechaRegistro: new Date() };
  asignaturaDialog: boolean = false;
  periodoDialog: boolean = false;
  asignatura: Asignatura = { idAsignatura: 0, nombre: '', idPeriodo: 0, periodo: '', descripcion: '', cantidadEstudiantesAsociados: 0, estudiantes: [] };
  periodo: Periodo = { idPeriodo: 0, nombre: '', idUsuario: 0, fechaRegistro: new Date() };
  isNewAsignatura: boolean = false;
  isNewPeriodo: boolean = true;
  errors: { [key: string]: string } = {};
  searchValue: string | undefined;
  availableStudents: any[] = [];
  selectedStudents: any[] = [];
  studentsDialog: boolean = false;
  loadingAsignatura: boolean = false;
  loadingPeriodo: boolean = false;
  loadingStudents: boolean = false;

  constructor(private apiService: ApiService, private messageService: MessageService, private confirmationService: ConfirmationService) {}

  ngOnInit() {
    this.loadPeriodos();
  }

  async loadPeriodos() {
    const response = await this.apiService.api.get('/Periodos');
    this.periodos = response.data;
  }

  async loadAsignaturas() {
    if (this.selectedPeriodo) {
      const response = await this.apiService.api.get(`/Asignaturas/${this.selectedPeriodo.idPeriodo}`);
      this.asignaturas = response.data;
    }
  }

  openNewAsignatura() {
    this.asignatura = { idAsignatura: 0, nombre: '', idPeriodo: this.selectedPeriodo?.idPeriodo || 0, periodo: '', descripcion: '', cantidadEstudiantesAsociados: 0, estudiantes: [] };
    this.isNewAsignatura = true;
    this.asignaturaDialog = true;
    this.errors = {};
  }

  openPeriodoDialog() {
    this.periodoDialog = true;
    this.errors = {};
  }

  openNewPeriodo() {
    this.periodo = { idPeriodo: 0, nombre: '', idUsuario: 0, fechaRegistro: new Date() };
    this.isNewPeriodo = true;
    this.periodoDialog = true;
    this.errors = {};
  }

  editAsignatura(asignatura: Asignatura) {
    this.asignatura = { ...asignatura };
    this.isNewAsignatura = false;
    this.asignaturaDialog = true;
    this.errors = {};
  }

  editPeriodo(periodo: Periodo) {
    this.periodo = { ...periodo };
    this.isNewPeriodo = false;
    this.periodoDialog = true;
    this.errors = {};
  }

  hideDialog() {
    this.errors = {};
    this.asignaturaDialog = false;
    this.periodoDialog = false;
  }

  async saveAsignatura() {
    this.loadingAsignatura = true;
    if (this.isNewAsignatura) {
      const response = await this.apiService.api.post('/Asignaturas', this.asignatura);
      if (response.data.success) {
        this.messageService.add({ severity: 'success', summary: 'Asignatura creada', detail: 'La asignatura ha sido creada exitosamente.' });
        this.loadAsignaturas();
        this.asignaturaDialog = false;
      } else {
        this.messageService.add({ severity: 'warn', summary: 'Error al crear asignatura', detail: response.data.message });
        this.errors = response.data.errors || {};
      }
    } else {
      const response = await this.apiService.api.put(`/Asignaturas/${this.asignatura.idAsignatura}`, this.asignatura);
      if (response.data.success) {
        this.messageService.add({ severity: 'success', summary: 'Asignatura actualizada', detail: 'La asignatura ha sido actualizada exitosamente.' });
        this.loadAsignaturas();
        this.asignaturaDialog = false;
      } else {
        this.messageService.add({ severity: 'warn', summary: 'Error al actualizar asignatura', detail: response.data.message });
        this.errors = response.data.errors || {};
      }
    }
    this.loadingAsignatura = false;
  }

  async savePeriodo() {
    this.loadingPeriodo = true;
    if (this.isNewPeriodo) {
      const response = await this.apiService.api.post('/Periodos', this.periodo);
      if (response.data.success) {
        this.messageService.add({ severity: 'success', summary: 'Periodo creado', detail: 'El periodo ha sido creado exitosamente.' });
        this.loadPeriodos();
        this.periodoDialog = false;
      } else {
        this.messageService.add({ severity: 'warn', summary: 'Error al crear periodo', detail: response.data.message });
        this.errors = response.data.errors || {};
      }
    } else {
      const response = await this.apiService.api.put(`/Periodos/${this.periodo.idPeriodo}`, this.periodo);
      if (response.data.success) {
        this.messageService.add({ severity: 'success', summary: 'Periodo actualizado', detail: 'El periodo ha sido actualizado exitosamente.' });
        this.loadPeriodos();
        this.periodoDialog = false;
      } else {
        this.messageService.add({ severity: 'warn', summary: 'Error al actualizar periodo', detail: response.data.message });
        this.errors = response.data.errors || {};
      }
    }
    this.loadingPeriodo = false;
  }

  cancelEditPeriodo() {
    this.periodo = { idPeriodo: 0, nombre: '', idUsuario: 0, fechaRegistro: new Date() };
    this.isNewPeriodo = true;
    this.errors = {};
  }

  confirmDeleteAsignatura(event: Event, asignatura: Asignatura) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: '¿Está seguro de que desea eliminar esta asignatura?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.deleteAsignatura(asignatura);
      }
    });
  }

  async deleteAsignatura(asignatura: Asignatura) {
    const response = await this.apiService.api.delete(`/Asignaturas/${asignatura.idAsignatura}`);
    if (response.data.success) {
      this.messageService.add({ severity: 'success', summary: 'Asignatura eliminada', detail: 'La asignatura ha sido eliminada exitosamente.' });
      this.loadAsignaturas();
    } else {
      this.messageService.add({ severity: 'warn', summary: 'Error al eliminar asignatura', detail: response.data.message });
    }
  }

  confirmDeletePeriodo(event: Event, periodo: Periodo) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: '¿Está seguro de que desea eliminar este periodo?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.deletePeriodo(periodo);
      }
    });
  }

  async deletePeriodo(periodo: Periodo) {
    const response = await this.apiService.api.delete(`/Periodos/${periodo.idPeriodo}`);
    if (response.data.success) {
      this.messageService.add({ severity: 'success', summary: 'Periodo eliminado', detail: 'El periodo ha sido eliminado exitosamente.' });
      this.loadPeriodos();
    } else {
      this.messageService.add({ severity: 'warn', summary: 'Error al eliminar periodo', detail: response.data.message });
    }
  }

  async openStudentsDialog(asignatura: Asignatura) {
    this.asignatura = { ...asignatura };
    const response = await this.apiService.api.get('/Asignaturas/GetEstudiantesDisponibles');
    this.availableStudents = response.data.filter((student: any) => 
      !asignatura.estudiantes.some((assigned: any) => assigned.idEstudiante === student.idEstudiante)
    );
    this.selectedStudents = asignatura.estudiantes || [];
    this.studentsDialog = true;
  }

  async saveStudents() {
    this.loadingStudents = true;
    const response = await this.apiService.api.put(`/Asignaturas/AsociarEstudiantes/${this.asignatura.idAsignatura}`, this.selectedStudents);
    if (response.data.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Estudiantes asociados',
        detail: 'Los estudiantes han sido asociados exitosamente.',
      });
      this.loadAsignaturas();
      this.studentsDialog = false;
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: 'Error al asociar estudiantes',
        detail: response.data.message,
      });
    }
    this.loadingStudents = false;
  }
}