using Classphy.Server.Entities;
using Classphy.Server.Infraestructure;
using Classphy.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Classphy.Server.Repositories
{
    public class PeriodosRepo : Repository<Periodos, PeriodosModel>
    {
        public PeriodosRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<PeriodosModel, Periodos>(p => new Periodos()
            {
                idPeriodo = p.idPeriodo,
                idUsuario = p.idUsuario,
                Nombre = p.Nombre,
                FechaRegistro = p.FechaRegistro ?? DateTime.Now
            }),
            (DB, filter) =>
            {
                return (from p in DB.Set<Periodos>().Where(filter)
                        select new PeriodosModel()
                        {
                            idPeriodo = p.idPeriodo,
                            idUsuario = p.idUsuario,
                            Nombre = p.Nombre,
                            FechaRegistro = p.FechaRegistro
                        });
            }
        )
        {

        }
    }
}