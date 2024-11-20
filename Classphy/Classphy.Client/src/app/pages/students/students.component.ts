import { Component, OnInit } from '@angular/core';
import { ApiService } from '../../services/api.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { ImportsModule } from '../../imports';
import { TableModule } from 'primeng/table';
import { Table } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-students',
  templateUrl: './students.component.html',
  standalone: true,
  imports: [
    ImportsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    DialogModule,
    DropdownModule,
  ],
  providers: [ConfirmationService],
})
export class StudentsComponent implements OnInit {
  students: any[] = [];
  studentDialog: boolean = false;
  student: any = {};
  isNewStudent: boolean = false;
  errors: { [key: string]: string } = {};
  searchValue: string | undefined;
  genderFilter: string | undefined;
  availableSubjects: any[] = [];
  selectedSubjects: any[] = [];
  subjectsDialog: boolean = false;
  loadingStudent: boolean = false;
  loadingSubjects: boolean = false;

  constructor(
    private apiService: ApiService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit() {
    this.loadStudents();
  }

  async loadStudents() {
    const response = await this.apiService.api.get('/Estudiantes');
    this.students = response.data;
    if (this.genderFilter) {
      console.log(this.genderFilter);
      this.students = this.students.filter(student => student.genero === this.genderFilter);
    }
  }

  openNewStudent() {
    this.student = {};
    this.isNewStudent = true;
    this.studentDialog = true;
    this.errors = {};
  }

  editStudent(student: any) {
    this.student = { ...student };
    this.isNewStudent = false;
    this.studentDialog = true;
    this.errors = {};
  }

  hideDialog() {
    this.errors = {};
    this.studentDialog = false;
  }

  async saveStudent() {
    this.loadingStudent = true;
    if (this.isNewStudent) {
      const response = await this.apiService.api.post('/Estudiantes', this.student);
      if (response.data.success) {
        this.messageService.add({
          severity: 'success',
          summary: 'Estudiante creado',
          detail: 'El estudiante ha sido creado exitosamente.',
        });
        this.loadStudents();
        this.studentDialog = false;
      } else {
        this.messageService.add({
          severity: 'warn',
          summary: 'Error al crear estudiante',
          detail: response.data.message,
        });
        this.errors = response.data.errors || {};
      }
    } else {
      const response = await this.apiService.api.put(`/Estudiantes/${this.student.idEstudiante}`, this.student);
      if (response.data.success) {
        this.messageService.add({
          severity: 'success',
          summary: 'Estudiante actualizado',
          detail: 'El estudiante ha sido actualizado exitosamente.',
        });
        this.loadStudents();
        this.studentDialog = false;
      } else {
        this.messageService.add({
          severity: 'warn',
          summary: 'Error al actualizar estudiante',
          detail: response.data.message,
        });
        this.errors = response.data.errors || {};
      }
    }
    this.loadingStudent = false;
  }

  confirmDeleteStudent(event: Event, student: any) {
    this.confirmationService.confirm({
      target: event.target as EventTarget,
      message: '¿Está seguro de que desea eliminar este estudiante?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.deleteStudent(student);
      }
    });
  }

  async deleteStudent(student: any) {
    const response = await this.apiService.api.delete(`/Estudiantes/${student.idEstudiante}`);
    if (response.data.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Estudiante eliminado',
        detail: 'El estudiante ha sido eliminado exitosamente.',
      });
      this.loadStudents();
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: 'Error al eliminar estudiante',
        detail: response.data.message,
      });
    }
  }

  async openSubjectsDialog(student: any) {
    this.student = { ...student };
    this.errors = {}; // Clear errors
    const response = await this.apiService.api.get('/Estudiantes/GetAsignaturasDisponibles');
    this.availableSubjects = response.data.filter((subject: any) => 
      !student.asignaturas.some((assigned: any) => assigned.idAsignatura === subject.idAsignatura)
    );
    this.selectedSubjects = student.asignaturas || [];
    this.subjectsDialog = true;
  }

  async saveSubjects() {
    this.loadingSubjects = true;
    const response = await this.apiService.api.put(`/Estudiantes/AsociarAsignaturas/${this.student.idEstudiante}`, this.selectedSubjects);
    if (response.data.success) {
      this.messageService.add({
        severity: 'success',
        summary: 'Asignaturas asociadas',
        detail: 'Las asignaturas han sido asociadas exitosamente.',
      });
      this.loadStudents();
      this.subjectsDialog = false;
    } else {
      this.messageService.add({
        severity: 'warn',
        summary: 'Error al asociar asignaturas',
        detail: response.data.message,
      });
    }
    this.loadingSubjects = false;
  }
}
