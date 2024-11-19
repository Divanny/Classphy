using Classphy.Server.Entities;
using Classphy.Server.Enums;
using Classphy.Server.Infraestructure;
using Classphy.Server.Models;
using Classphy.Server.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classphy.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiantesController : Controller
    {
        private readonly Logger _logger;
        private readonly int _idUsuarioOnline;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ClassphyContext _classphyContext;
        private readonly EstudiantesRepo _estudiantesRepo;

        /// <summary>
        /// Constructor de la clase EstudiantesController.
        /// </summary>
        /// <param name="userAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="classphyContext"></param>
        /// <param name="logger"></param>
        public EstudiantesController(IUserAccessor userAccessor, IPasswordHasher passwordHasher, ClassphyContext classphyContext, Logger logger)
        {
            _logger = logger;
            _idUsuarioOnline = userAccessor.idUsuario;
            _passwordHasher = passwordHasher;
            _classphyContext = classphyContext;
            _estudiantesRepo = new EstudiantesRepo(classphyContext);
        }

        /// <summary>
        /// Obtiene todos los estudiantes de un usuario.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet(Name = "GetEstudiantes")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public List<EstudiantesModel> Get()
        {
            List<EstudiantesModel> estudiantes = _estudiantesRepo.Get(x => x.idUsuario == _idUsuarioOnline).ToList();
            estudiantes.ForEach(estudiante => estudiante.Asignaturas = _estudiantesRepo.GetAsignaturasEstudiante(estudiante.idEstudiante).ToList());
            return estudiantes;
        }

        /// <summary>
        /// Obtiene un estudiante por su ID.
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante.</param>
        /// <returns>Estudiante encontrado.</returns>
        [HttpGet("{idEstudiante}", Name = "GetEstudiante")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public EstudiantesModel Get(int idEstudiante)
        {
            EstudiantesModel? estudiante = _estudiantesRepo.Get(x => x.idEstudiante == idEstudiante && x.idUsuario == _idUsuarioOnline).FirstOrDefault();

            if (estudiante == null) throw new Exception("Este usuario no se ha encontrado");

            estudiante.Asignaturas = _estudiantesRepo.GetAsignaturasEstudiante(estudiante.idEstudiante).ToList();

            return estudiante;
        }

        /// <summary>
        /// Crea un nuevo estudiante.
        /// </summary>
        /// <param name="estudiantesModel">Datos del estudiante a crear.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost(Name = "SaveEstudiante")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Post(EstudiantesModel estudiantesModel)
        {
            try
            {
                var estudiante = _estudiantesRepo.Get(x => x.idUsuario == _idUsuarioOnline && x.Matricula == estudiantesModel.Matricula).FirstOrDefault();

                if (estudiante != null) return new OperationResult(false, "Ya existe un estudiante con esa matrícula");

                estudiantesModel.idUsuario = _idUsuarioOnline;

                var created = _estudiantesRepo.Add(estudiantesModel);
                _logger.LogHttpRequest(estudiantesModel);
                return new OperationResult(true, "Estudiante creado exitosamente", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un estudiante existente.
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante.</param>
        /// <param name="estudiantesModel">Datos del estudiante a actualizar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{idEstudiante}", Name = "UpdateEstudiante")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Put(int idEstudiante, EstudiantesModel estudiantesModel)
        {
            try
            {
                var estudiante = _estudiantesRepo.Get(x => x.idUsuario == _idUsuarioOnline && x.idEstudiante == idEstudiante).FirstOrDefault();

                if (estudiante == null) return new OperationResult(false, "El estudiante no se ha encontrado");

                if (estudiante.Matricula != estudiantesModel.Matricula && _estudiantesRepo.Any(x => x.Matricula == estudiantesModel.Matricula)) return new OperationResult(false, "Ya existe un estudiante con esa matrícula");

                estudiantesModel.idUsuario = _idUsuarioOnline;

                _estudiantesRepo.Edit(estudiantesModel);
                _logger.LogHttpRequest(estudiantesModel);

                return new OperationResult(true, "Estudiante editado exitosamente", estudiante);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Elimina un estudiante existente.
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{idEstudiante}", Name = "DeleteEstudiante")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Delete(int idEstudiante)
        {
            using (var transaction = _classphyContext.Database.BeginTransaction())
            {
                try
                {
                    var estudiante = _estudiantesRepo.Get(x => x.idUsuario == _idUsuarioOnline && x.idEstudiante == idEstudiante).FirstOrDefault();

                    if (estudiante == null) return new OperationResult(false, "El estudiante no se ha encontrado");

                    _classphyContext.Set<EstudiantesAsignatura>().RemoveRange(_classphyContext.Set<EstudiantesAsignatura>().Where(x => x.idEstudiante == idEstudiante));

                    _estudiantesRepo.Delete(idEstudiante);
                    _logger.LogHttpRequest(estudiante);

                    transaction.Commit();
                    return new OperationResult(true, "Estudiante eliminado exitosamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Obtiene las asignaturas disponibles para asociar a un estudiante.
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAsignaturasDisponibles", Name = "GetAsignaturasDisponibles")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public List<AsignaturasModel> GetAsignaturasDisponibles()
        {
            var periodosUsuario = _classphyContext.Set<Periodos>().Where(x => x.idUsuario == _idUsuarioOnline).Select(x => x.idPeriodo).ToList();
            return _estudiantesRepo.asignaturasRepo.Get(x => periodosUsuario.Contains(x.idPeriodo)).ToList();
        }

        /// <summary>
        /// Asocia asignaturas a un estudiante.
        /// </summary>
        /// <param name="idEstudiante">ID del estudiante.</param>
        /// <param name="asignaturas">Asignaturas para asociar</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPut("AsociarAsignaturas/{idEstudiante}", Name = "AsociarAsignaturas")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult AsociarAsignaturas(int idEstudiante, List<AsignaturasModel> asignaturas)
        {
            using (var transaction = _classphyContext.Database.BeginTransaction())
            {
                try
                {
                    var estudiante = _estudiantesRepo.Get(x => x.idUsuario == _idUsuarioOnline && x.idEstudiante == idEstudiante).FirstOrDefault();

                    if (estudiante == null) return new OperationResult(false, "El estudiante no se ha encontrado");

                    _classphyContext.Set<EstudiantesAsignatura>().RemoveRange(_classphyContext.Set<EstudiantesAsignatura>().Where(x => x.idEstudiante == idEstudiante));

                    foreach (var asignatura in asignaturas)
                    {
                        _classphyContext.Set<EstudiantesAsignatura>().Add(new EstudiantesAsignatura
                        {
                            idAsignatura = asignatura.idAsignatura,
                            idEstudiante = idEstudiante
                        });
                    }

                    _classphyContext.SaveChanges();
                    transaction.Commit();
                    return new OperationResult(true, "Asignaturas asociadas exitosamente");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex);
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}