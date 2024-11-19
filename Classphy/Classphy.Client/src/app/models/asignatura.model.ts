export interface Asignatura {
  idAsignatura: number;
  nombre: string;
  idPeriodo: number;
  periodo: string;
  descripcion: string;
  cantidadEstudiantesAsociados: number;
  estudiantes: any[]; 
}