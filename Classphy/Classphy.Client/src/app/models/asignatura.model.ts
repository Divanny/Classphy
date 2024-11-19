export interface Asignatura {
  idAsignatura: number;
  nombre: string;
  idPeriodo: number;
  descripcion: string;
  cantidadEstudiantesAsociados: number;
  estudiantes: any[]; 
}