using Classphy.Server.Entities;
using Classphy.Server.Infraestructure;
using Classphy.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Classphy.Server.Repositories
{
    public class CalificacionesRepo : Repository<Calificaciones, CalificacionesModel>
    {
        public CalificacionesRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<CalificacionesModel, Calificaciones>(c => new Calificaciones()
            {
                idCalificacion = c.idCalificacion,
                idAsignatura = c.idAsignatura,
                idEstudiante = c.idEstudiante,
                MedioTermino = c.MedioTermino,
                Final = c.Final
            }),
            (DB, filter) =>
            {
                return (from c in DB.Set<Calificaciones>().Where(filter)
                        join e in DB.Set<Estudiantes>() on c.idEstudiante equals e.idEstudiante
                        select new CalificacionesModel()
                        {
                            idCalificacion = c.idCalificacion,
                            idAsignatura = c.idAsignatura,
                            idEstudiante = c.idEstudiante,
                            Nombres = e.Nombres,
                            Apellidos = e.Apellidos,
                            Matricula = e.Matricula,
                            Correo = e.Correo,
                            MedioTermino = c.MedioTermino,
                            CantidadFaltas = dbContext.Set<Asistencias>().Count(a => a.idEstudiante == c.idEstudiante && a.idAsignatura == c.idAsignatura && a.Presente == false),
                            Final = c.Final
                        });
            }
        )
        {

        }
    }
}