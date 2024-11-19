export interface Asistencia {
  idAsistencia: number;
  idAsignatura: number;
  idEstudiante: number;
  nombres: string;
  apellidos: string;
  matricula: string;
  telefono: string;
  correo: string;
  fecha: string;
  presente: boolean;
}