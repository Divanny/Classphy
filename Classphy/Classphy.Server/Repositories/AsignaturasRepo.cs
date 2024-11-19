using Classphy.Server.Entities;
using Classphy.Server.Infraestructure;
using Classphy.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Classphy.Server.Repositories
{
    public class AsignaturasRepo : Repository<Asignaturas, AsignaturasModel>
    {
        public AsignaturasRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<AsignaturasModel, Asignaturas>(a => new Asignaturas()
            {
                idAsignatura = a.idAsignatura,
                Nombre = a.Nombre,
                idPeriodo = a.idPeriodo,
                Descripcion = a.Descripcion
            }),
            (DB, filter) =>
            {
                return (from a in DB.Set<Asignaturas>().Where(filter)
                        join p in DB.Set<Periodos>() on a.idPeriodo equals p.idPeriodo
                        select new AsignaturasModel()
                        {
                            idAsignatura = a.idAsignatura,
                            Nombre = a.Nombre,
                            idPeriodo = a.idPeriodo,
                            Periodo = p.Nombre,
                            Descripcion = a.Descripcion,
                            CantidadEstudiantesAsociados = dbContext.Set<EstudiantesAsignatura>().Count(ea => ea.idAsignatura == a.idAsignatura)
                        });
            }
        )
        {
        }
    }
}