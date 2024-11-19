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
    public class AsistenciasController : Controller
    {
        private readonly Logger _logger;
        private readonly int _idUsuarioOnline;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ClassphyContext _classphyContext;
        private readonly AsistenciasRepo _asistenciasRepo;
        private readonly AsignaturasRepo _asignaturasRepo;
        private readonly EstudiantesRepo _estudiantesRepo;

        /// <summary>
        /// Constructor de la clase AsistenciasController.
        /// </summary>
        /// <param name="userAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="classphyContext"></param>
        /// <param name="logger"></param>
        public AsistenciasController(IUserAccessor userAccessor, IPasswordHasher passwordHasher, ClassphyContext classphyContext, Logger logger)
        {
            _logger = logger;
            _idUsuarioOnline = userAccessor.idUsuario;
            _passwordHasher = passwordHasher;
            _classphyContext = classphyContext;
            _asistenciasRepo = new AsistenciasRepo(classphyContext);
            _asignaturasRepo = new AsignaturasRepo(classphyContext);
            _estudiantesRepo = new EstudiantesRepo(classphyContext);
        }

        /// <summary>
        /// Obtiene todas los asistencias de un usuario.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet(Name = "GetAsistencias")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public List<AsistenciasModel> Get()
        {
            var idsPeriodo = _classphyContext.Set<Periodos>().Where(x => x.idUsuario == _idUsuarioOnline).Select(x => x.idPeriodo).ToList();
            var idsAsignaturas = _classphyContext.Set<Asignaturas>().Where(x => idsPeriodo.Contains(x.idPeriodo)).Select(x => x.idAsignatura).ToList();

            List<AsistenciasModel> asistencias = _asistenciasRepo.Get(x => idsAsignaturas.Contains(x.idAsignatura)).ToList();
            return asistencias;
        }

        /// <summary>
        /// Obtiene el listado de asistencia.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpPost("GetListadoAsistencias", Name = "GetListadoAsistencias")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public List<AsistenciasModel> GetListadoAsistencias(ConsultaListadoAsistencia consultaListadoAsistencia)
        {
            var idsPeriodo = _classphyContext.Set<Periodos>().Where(x => x.idUsuario == _idUsuarioOnline).Select(x => x.idPeriodo).ToList();

            var asignatura = _asignaturasRepo.Get(x => x.idAsignatura == consultaListadoAsistencia.idAsignatura && idsPeriodo.Contains(x.idPeriodo)).FirstOrDefault();

            if (asignatura == null) return new List<AsistenciasModel>();

            List<AsistenciasModel> asistencias = _asistenciasRepo.Get(x => x.idAsignatura == consultaListadoAsistencia.idAsignatura && x.Fecha.Date == consultaListadoAsistencia.Fecha.Date).ToList();

            if (asistencias.Count == 0)
            {
                var idsEstudiantes = _classphyContext.Set<EstudiantesAsignatura>().Where(x => x.idAsignatura == consultaListadoAsistencia.idAsignatura).Select(x => x.idEstudiante).ToList();
                var estudiantes = _estudiantesRepo.Get(x => idsEstudiantes.Contains(x.idEstudiante)).ToList();

                foreach (var estudiante in estudiantes)
                {
                    asistencias.Add(new AsistenciasModel()
                    {
                        idAsignatura = consultaListadoAsistencia.idAsignatura,
                        idEstudiante = estudiante.idEstudiante,
                        Nombres = estudiante.Nombres,
                        Apellidos = estudiante.Apellidos,
                        Matricula = estudiante.Matricula,
                        Correo = estudiante.Correo,
                        Telefono = estudiante.Telefono,
                        Fecha = consultaListadoAsistencia.Fecha,
                        Presente = false
                    });
                }
            }

            return asistencias;
        }

        /// <summary>
        /// Guarda un listado de asistencia
        /// </summary>
        /// <param name="listadoAsistencias">Datos del listado de asistencia a guardar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost(Name = "SaveAsistencia")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Post(List<AsistenciasModel> listadoAsistencias)
        {
            using (var trx = _classphyContext.Database.BeginTransaction())
            {
                try
                {
                    if (listadoAsistencias.Count == 0) return new OperationResult(false, "No se ha enviado ninguna asistencia");

                    var idsPeriodo = _classphyContext.Set<Periodos>().Where(x => x.idUsuario == _idUsuarioOnline).Select(x => x.idPeriodo).ToList();

                    var idsAsignaturas = _classphyContext.Set<Asignaturas>().Where(x => idsPeriodo.Contains(x.idPeriodo)).Select(x => x.idAsignatura).ToList();

                    if (idsAsignaturas.Contains(listadoAsistencias[0].idAsignatura) == false) return new OperationResult(false, "La asignatura no se ha encontrado");

                    var asignatura = _asignaturasRepo.Get(x => x.idAsignatura == listadoAsistencias[0].idAsignatura).FirstOrDefault();

                    var asistenciasAnteriores = _asistenciasRepo.Get(x => x.idAsignatura == asignatura.idAsignatura && x.Fecha.Date == listadoAsistencias[0].Fecha.Date).ToList();

                    foreach (var asistencia in asistenciasAnteriores)
                    {
                        _asistenciasRepo.Delete(asistencia.idAsistencia);
                    }

                    foreach (var asistencia in listadoAsistencias)
                    {
                        asistencia.idAsistencia = 0;
                        _asistenciasRepo.Add(asistencia);
                    }

                    _logger.LogHttpRequest(listadoAsistencias);
                    trx.Commit();
                    return new OperationResult(true, $"Listado de asistencias para el día {listadoAsistencias[0].Fecha.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("es-ES"))} en la asignatura {asignatura.Nombre} guardado exitosamente");
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