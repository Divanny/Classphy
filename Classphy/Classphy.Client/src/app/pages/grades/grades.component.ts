
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
import jsPDF from 'jspdf';
import 'jspdf-autotable';

declare module 'jspdf' {
  interface jsPDF {
    autoTable: (options: any) => jsPDF;
  }
}

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

  calculateAcumulada(grade: any) {
    if (grade.final === null) return grade.medioTermino;
    return grade.medioTermino + grade.final;
  }

  generatePDF() {
    const doc = new jsPDF();
    const logo = new Image();
    logo.src = '../../assets/classphy.png';

    logo.onload = () => {
      const logoWidth = 50;
      const logoHeight = (logoWidth / 866) * 186; // Maintain aspect ratio
      doc.addImage(logo, 'PNG', 14, 10, logoWidth, logoHeight);

      doc.setFontSize(18);
      doc.text('Reporte de Calificaciones', 14, 50);

      const isFinalReport = this.grades.some(grade => grade.final !== null);
      const reportType = isFinalReport ? 'Calificación Final' : 'Calificación de Medio Término';
      doc.setFontSize(14);
      doc.text(`Asignatura: ${this.selectedAsignatura?.nombre}`, 14, 60);
      doc.text(`Periodo: ${this.selectedAsignatura?.periodo}`, 14, 66);
      doc.text(`Tipo de Reporte: ${reportType}`, 14, 72);

      const columns = ['Estudiante', 'Matrícula', 'Faltas', 'Medio Término', 'Final', 'Acumulada', 'Literal'];
      const rows = this.grades.map(grade => [
        `${grade.nombres} ${grade.apellidos}`,
        grade.matricula,
        grade.cantidadFaltas,
        grade.medioTermino,
        grade.final,
        this.calculateAcumulada(grade),
        grade.literal
      ]);

      doc.autoTable({
        head: [columns],
        body: rows,
        startY: 80,
        theme: 'grid',
        headStyles: { fillColor: '#8B5CF6' },
        styles: { fontSize: 10, cellPadding: 3 },
        alternateRowStyles: { fillColor: '#f3f3f3' },
        didDrawPage: (data : any) => {
          // Footer
          const pageCount = doc.getNumberOfPages();
          const pageSize = doc.internal.pageSize;
          const pageHeight = pageSize.height ? pageSize.height : pageSize.getHeight();
          doc.setFontSize(10);
          doc.text(`Página ${data.pageNumber} de ${pageCount}`, data.settings.margin.left, pageHeight - 10);
          doc.text(`Fecha: ${new Date().toLocaleDateString()}`, data.settings.margin.left, pageHeight - 20);
        }
      });

      doc.save('calificaciones.pdf');
    };
  }
}