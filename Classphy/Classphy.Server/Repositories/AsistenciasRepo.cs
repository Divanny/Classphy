using Classphy.Server.Entities;
using Classphy.Server.Infraestructure;
using Classphy.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Classphy.Server.Repositories
{
    public class AsistenciasRepo : Repository<Asistencias, AsistenciasModel>
    {
        public AsistenciasRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<AsistenciasModel, Asistencias>(a => new Asistencias()
            {
                idAsistencia = a.idAsistencia,
                idEstudiante = a.idEstudiante,
                idAsignatura = a.idAsignatura,
                Fecha = a.Fecha,
                Presente = a.Presente
            }),
            (DB, filter) =>
            {
                return (from a in DB.Set<Asistencias>().Where(filter)
                        join e in DB.Set<Estudiantes>() on a.idEstudiante equals e.idEstudiante
                        select new AsistenciasModel()
                        {
                            idAsistencia = a.idAsistencia,
                            idAsignatura = a.idAsignatura,
                            Fecha = a.Fecha,
                            idEstudiante = a.idEstudiante,
                            Nombres = e.Nombres,
                            Apellidos = e.Apellidos,
                            Correo = e.Correo,
                            Telefono = e.Telefono,
                            Matricula = e.Matricula,
                            Presente = a.Presente
                        });
            }
        )
        {

        }
    }
}