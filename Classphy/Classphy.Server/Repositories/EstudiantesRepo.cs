using Classphy.Server.Entities;
using Classphy.Server.Infraestructure;
using Classphy.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Classphy.Server.Repositories
{
    public class EstudiantesRepo : Repository<Estudiantes, EstudiantesModel>
    {
        public AsignaturasRepo asignaturasRepo { get; set; }
        public EstudiantesRepo(DbContext dbContext) : base
        (
            dbContext,
            new ObjectsMapper<EstudiantesModel, Estudiantes>(e => new Estudiantes()
            {
                idEstudiante = e.idEstudiante,
                idUsuario = e.idUsuario,
                Matricula = e.Matricula,
                Nombres = e.Nombres,
                Apellidos = e.Apellidos,
                Correo = e.Correo,
                Genero = e.Genero,
                Telefono = e.Telefono
            }),
            (DB, filter) =>
            {
                return (from e in DB.Set<Estudiantes>().Where(filter)
                        select new EstudiantesModel()
                        {
                            idEstudiante = e.idEstudiante,
                            idUsuario = e.idUsuario,
                            Matricula = e.Matricula,
                            Nombres = e.Nombres,
                            Apellidos = e.Apellidos,
                            Correo = e.Correo,
                            Genero = e.Genero,
                            Telefono = e.Telefono,
                            CantidadAsignaturasAsociadas = dbContext.Set<EstudiantesAsignatura>().Count(ea => ea.idEstudiante == e.idEstudiante)
                        });
            }
        )
        {
            asignaturasRepo = new AsignaturasRepo(dbContext);
        }

        public List<AsignaturasModel> GetAsignaturasEstudiante(int idEstudiante)
        {
            var idsAsignaturas = dbContext.Set<EstudiantesAsignatura>().Where(ea => ea.idEstudiante == idEstudiante).Select(x => x.idAsignatura);
            List<AsignaturasModel> asignaturas = asignaturasRepo.Get(x => idsAsignaturas.Contains(x.idAsignatura)).ToList();
            return asignaturas;
        }
    }
}