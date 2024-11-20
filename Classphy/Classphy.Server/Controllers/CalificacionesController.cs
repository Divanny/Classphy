using Classphy.Server.Entities;
using Classphy.Server.Enums;
using Classphy.Server.Infraestructure;
using Classphy.Server.Models;
using Classphy.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Classphy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalificacionesController : Controller
    {
        private readonly Logger _logger;
        private readonly int _idUsuarioOnline;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ClassphyContext _classphyContext;
        private readonly CalificacionesRepo _calificacionesRepo;
        private readonly AsignaturasRepo _asignaturasRepo;
        private readonly EstudiantesRepo _estudiantesRepo;
        private readonly AsistenciasRepo _asistenciasRepo;

        /// <summary>
        /// Constructor de la clase CalificacionesController.
        /// </summary>
        /// <param name="userAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="classphyContext"></param>
        /// <param name="logger"></param>
        public CalificacionesController(IUserAccessor userAccessor, IPasswordHasher passwordHasher, ClassphyContext classphyContext, Logger logger)
        {
            _logger = logger;
            _idUsuarioOnline = userAccessor.idUsuario;
            _passwordHasher = passwordHasher;
            _classphyContext = classphyContext;
            _calificacionesRepo = new CalificacionesRepo(classphyContext);
            _asignaturasRepo = new AsignaturasRepo(classphyContext);
            _estudiantesRepo = new EstudiantesRepo(classphyContext);
            _asistenciasRepo = new AsistenciasRepo(classphyContext);
        }

        /// <summary>
        /// Obtiene todas los Calificaciones de un usuario.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet(Name = "GetCalificaciones")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public List<CalificacionesModel> Get()
        {
            var idsPeriodo = _classphyContext.Set<Periodos>().Where(x => x.idUsuario == _idUsuarioOnline).Select(x => x.idPeriodo).ToList();
            var idsAsignaturas = _classphyContext.Set<Asignaturas>().Where(x => idsPeriodo.Contains(x.idPeriodo)).Select(x => x.idAsignatura).ToList();

            List<CalificacionesModel> Calificaciones = _calificacionesRepo.Get(x => idsAsignaturas.Contains(x.idAsignatura)).ToList();
            return Calificaciones;
        }

        /// <summary>
        /// Obtiene el listado de Calificacion.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet("GetListadoCalificaciones/{idAsignatura}", Name = "GetListadoCalificaciones")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public List<CalificacionesModel> GetListadoCalificaciones(int idAsignatura)
        {
            var idsPeriodo = _classphyContext.Set<Periodos>().Where(x => x.idUsuario == _idUsuarioOnline).Select(x => x.idPeriodo).ToList();

            var asignatura = _asignaturasRepo.Get(x => x.idAsignatura == idAsignatura && idsPeriodo.Contains(x.idPeriodo)).FirstOrDefault();

            if (asignatura == null) return new List<CalificacionesModel>();

            List<CalificacionesModel> Calificaciones = _calificacionesRepo.Get(x => x.idAsignatura == idAsignatura).ToList();

            if (Calificaciones.Count == 0)
            {
                var idsEstudiantes = _classphyContext.Set<EstudiantesAsignatura>().Where(x => x.idAsignatura == idAsignatura).Select(x => x.idEstudiante).ToList();
                var estudiantes = _estudiantesRepo.Get(x => idsEstudiantes.Contains(x.idEstudiante)).ToList();

                foreach (var estudiante in estudiantes)
                {
                    Calificaciones.Add(new CalificacionesModel()
                    {
                        idAsignatura = idAsignatura,
                        idEstudiante = estudiante.idEstudiante,
                        Nombres = estudiante.Nombres,
                        Apellidos = estudiante.Apellidos,
                        Matricula = estudiante.Matricula,
                        Correo = estudiante.Correo,
                        CantidadFaltas = _asistenciasRepo.Get(x => x.idEstudiante == estudiante.idEstudiante && x.idAsignatura == idAsignatura && x.Presente == false).Count(),
                        MedioTermino = 0,
                        Final = null

                    });
                }
            }

            return Calificaciones;
        }

        /// <summary>
        /// Guarda un listado de Calificacion
        /// </summary>
        /// <param name="listadoCalificaciones">Datos del listado de Calificacion a guardar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost(Name = "SaveCalificacion")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Post(List<CalificacionesModel> listadoCalificaciones)
        {
            using (var trx = _classphyContext.Database.BeginTransaction())
            {
                try
                {
                    if (listadoCalificaciones.Count == 0) return new OperationResult(false, "No se ha enviado ninguna Calificacion");

                    var idsPeriodo = _classphyContext.Set<Periodos>().Where(x => x.idUsuario == _idUsuarioOnline).Select(x => x.idPeriodo).ToList();

                    var idsAsignaturas = _classphyContext.Set<Asignaturas>().Where(x => idsPeriodo.Contains(x.idPeriodo)).Select(x => x.idAsignatura).ToList();

                    if (idsAsignaturas.Contains(listadoCalificaciones[0].idAsignatura) == false) return new OperationResult(false, "La asignatura no se ha encontrado");

                    var asignatura = _asignaturasRepo.Get(x => x.idAsignatura == listadoCalificaciones[0].idAsignatura).FirstOrDefault();

                    var calificacionesAnteriores = _calificacionesRepo.Get(x => x.idAsignatura == asignatura.idAsignatura).ToList();

                    foreach (var Calificacion in calificacionesAnteriores)
                    {
                        _calificacionesRepo.Delete(Calificacion.idCalificacion);
                    }

                    foreach (var Calificacion in listadoCalificaciones)
                    {
                        if (Calificacion.MedioTermino < 0 || Calificacion.MedioTermino > 99)
                            return new OperationResult(false, $"La calificación de medio término debe estar entre 0 y 99 para el estudiante con matrícula {Calificacion.Matricula}");
                        if (Calificacion.Final.HasValue && (Calificacion.Final < 0 || Calificacion.Final > 100))
                            return new OperationResult(false, $"La calificación final debe estar entre 0 y 100 para el estudiante con matrícula {Calificacion.Matricula}");
                        if (Calificacion.Final.HasValue && (Calificacion.MedioTermino + Calificacion.Final > 100))
                            return new OperationResult(false, $"La suma de la calificación de medio término y final no puede ser mayor a 100 para el estudiante con matrícula {Calificacion.Matricula}");

                        Calificacion.idCalificacion = 0;
                        _calificacionesRepo.Add(Calificacion);
                    }

                    _logger.LogHttpRequest(listadoCalificaciones);
                    trx.Commit();
                    return new OperationResult(true, $"Listado de calificaciones para la asignatura {asignatura.Nombre} guardado exitosamente");
                }
                catch (Exception ex)
                {
                    trx.Rollback();
                    _logger.LogError(ex);
                    throw;
                }
            }
        }
    }
}