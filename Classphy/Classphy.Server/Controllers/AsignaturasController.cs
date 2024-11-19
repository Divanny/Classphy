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
    public class AsignaturasController : Controller
    {
        private readonly Logger _logger;
        private readonly int _idUsuarioOnline;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ClassphyContext _classphyContext;
        private readonly AsignaturasRepo _asignaturasRepo;

        /// <summary>
        /// Constructor de la clase AsignaturasController.
        /// </summary>
        /// <param name="userAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="classphyContext"></param>
        /// <param name="logger"></param>
        public AsignaturasController(IUserAccessor userAccessor, IPasswordHasher passwordHasher, ClassphyContext classphyContext, Logger logger)
        {
            _logger = logger;
            _idUsuarioOnline = userAccessor.idUsuario;
            _passwordHasher = passwordHasher;
            _classphyContext = classphyContext;
            _asignaturasRepo = new AsignaturasRepo(classphyContext);
        }

        /// <summary>
        /// Obtiene todos los asignaturas de un usuario de un período en específico.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet("{idPeriodo}", Name = "GetAsignaturas")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public List<AsignaturasModel> Get(int idPeriodo)
        {
            List<AsignaturasModel> asignaturas = _asignaturasRepo.Get(x => x.idPeriodo == idPeriodo).ToList();
            return asignaturas;
        }

        /// <summary>
        /// Crea una nueva asignatura.
        /// </summary>
        /// <param name="asignaturasModel">Datos de la asignatura a crear.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost(Name = "SaveAsignatura")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Post(AsignaturasModel asignaturasModel)
        {
            try
            {
                if (_asignaturasRepo.Any(x => x.Nombre == asignaturasModel.Nombre && x.idPeriodo == asignaturasModel.idPeriodo)) return new OperationResult(false, "Ya existe una asignatura para este período con el mismo nombre");
                if (_classphyContext.Set<Periodos>().Any(x => x.idPeriodo == asignaturasModel.idPeriodo && x.idUsuario == _idUsuarioOnline) == false) return new OperationResult(false, "El período no se ha encontrado");

                var created = _asignaturasRepo.Add(asignaturasModel);
                _logger.LogHttpRequest(asignaturasModel);
                return new OperationResult(true, "Asignatura creada exitosamente", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Actualiza una asignatura existente.
        /// </summary>
        /// <param name="idAsignatura">ID de la asignatura.</param>
        /// <param name="asignaturasModel">Datos de la asignatura a actualizar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{idAsignatura}", Name = "UpdateAsignatura")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Put(int idAsignatura, AsignaturasModel asignaturasModel)
        {
            try
            {
                var asignatura = _asignaturasRepo.Get(x => x.idAsignatura == idAsignatura && x.idPeriodo == asignaturasModel.idPeriodo).FirstOrDefault();

                if (asignatura == null) return new OperationResult(false, "La asignatura no se ha encontrado");

                if (_asignaturasRepo.Any(x => x.Nombre == asignaturasModel.Nombre && x.idPeriodo == asignaturasModel.idPeriodo && x.idAsignatura != idAsignatura)) return new OperationResult(false, "Ya existe una asignatura para este período con el mismo nombre");

                if (_classphyContext.Set<Periodos>().Any(x => x.idPeriodo == asignaturasModel.idPeriodo && x.idUsuario == _idUsuarioOnline) == false) return new OperationResult(false, "El período no se ha encontrado");

                _asignaturasRepo.Edit(asignaturasModel);
                _logger.LogHttpRequest(asignaturasModel);

                return new OperationResult(true, "Asignatura editada exitosamente", asignaturasModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Elimina una asignatura existente.
        /// </summary>
        /// <param name="idAsignatura">ID de la asignatura.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{idAsignatura}", Name = "DeleteAsignatura")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Delete(int idAsignatura)
        {
            try
            {
                var asignatura = _asignaturasRepo.Get(x => x.idAsignatura == idAsignatura).FirstOrDefault();

                if (asignatura == null) return new OperationResult(false, "La asignatura no se ha encontrado");

                if (_classphyContext.Set<Periodos>().Any(x => x.idPeriodo == asignatura.idPeriodo && x.idUsuario == _idUsuarioOnline) == false) return new OperationResult(false, "El período no se ha encontrado");

                _asignaturasRepo.Delete(idAsignatura);
                _logger.LogHttpRequest(asignatura);
                return new OperationResult(true, "Asignatura eliminada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }
    }
}