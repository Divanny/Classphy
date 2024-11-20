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
    public class PeriodosController : Controller
    {
        private readonly Logger _logger;
        private readonly int _idUsuarioOnline;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ClassphyContext _classphyContext;
        private readonly PeriodosRepo _periodosRepo;

        /// <summary>
        /// Constructor de la clase PeriodosController.
        /// </summary>
        /// <param name="userAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="classphyContext"></param>
        /// <param name="logger"></param>
        public PeriodosController(IUserAccessor userAccessor, IPasswordHasher passwordHasher, ClassphyContext classphyContext, Logger logger)
        {
            _logger = logger;
            _idUsuarioOnline = userAccessor.idUsuario;
            _passwordHasher = passwordHasher;
            _classphyContext = classphyContext;
            _periodosRepo = new PeriodosRepo(classphyContext);
        }

        /// <summary>
        /// Obtiene todos los periodos de un usuario.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet(Name = "GetPeriodos")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public List<PeriodosModel> Get()
        {
            List<PeriodosModel> periodos = _periodosRepo.Get(x => x.idUsuario == _idUsuarioOnline).ToList();
            return periodos;
        }

        /// <summary>
        /// Obtiene un periodo por su ID.
        /// </summary>
        /// <param name="idPeriodo">ID del periodo.</param>
        /// <returns>Periodo encontrado.</returns>
        [HttpGet("{idPeriodo}", Name = "GetPeriodo")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public PeriodosModel Get(int idPeriodo)
        {
            PeriodosModel? periodos = _periodosRepo.Get(x => x.idPeriodo == idPeriodo && x.idUsuario == _idUsuarioOnline).FirstOrDefault();

            if (periodos == null) throw new Exception("Este período no se ha encontrado");

            return periodos;
        }

        /// <summary>
        /// Crea un nuevo período.
        /// </summary>
        /// <param name="periodosModel">Datos del período a crear.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost(Name = "SavePeriodo")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Post(PeriodosModel periodosModel)
        {
            try
            {
                var periodo = _periodosRepo.Get(x => x.Nombre == periodosModel.Nombre).FirstOrDefault();
                if (periodo != null)
                {
                    return new OperationResult(false, "Ya existe un período con este nombre");
                }

                periodosModel.FechaRegistro = DateTime.Now;
                periodosModel.idUsuario = _idUsuarioOnline;

                var created = _periodosRepo.Add(periodosModel);
                _logger.LogHttpRequest(periodosModel);
                return new OperationResult(true, "Período creado exitosamente", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un período existente.
        /// </summary>
        /// <param name="idPeriodo">ID del período.</param>
        /// <param name="periodosModel">Datos del período a actualizar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{idPeriodo}", Name = "UpdatePeriodo")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Put(int idPeriodo, PeriodosModel periodosModel)
        {
            try
            {
                var periodo = _periodosRepo.Get(x => x.idUsuario == _idUsuarioOnline && x.idPeriodo == idPeriodo).FirstOrDefault();

                if (periodo == null) return new OperationResult(false, "El período no se ha encontrado");

                if (_periodosRepo.Get(x => x.Nombre == periodosModel.Nombre && x.idPeriodo != idPeriodo).Count() > 0)
                {
                    return new OperationResult(false, "Ya existe un período con este nombre");
                }

                _periodosRepo.Edit(periodosModel);
                _logger.LogHttpRequest(periodosModel);

                return new OperationResult(true, "Período editado exitosamente", periodo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Elimina un período existente.
        /// </summary>
        /// <param name="idPeriodo">ID del período.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpDelete("{idPeriodo}", Name = "DeletePeriodo")]
        [AuthorizeByProfile(PerfilesEnum.Profesor)]
        public OperationResult Delete(int idPeriodo)
        {
            try
            {
                var periodo = _periodosRepo.Get(x => x.idUsuario == _idUsuarioOnline && x.idPeriodo == idPeriodo).FirstOrDefault();

                if (periodo == null) return new OperationResult(false, "El período no se ha encontrado");

                if (_classphyContext.Set<Asignaturas>().Any(x => x.idPeriodo == idPeriodo)) return new OperationResult(false, "No se puede eliminar un período con asignaturas asociadas");
                _periodosRepo.Delete(idPeriodo);
                _logger.LogHttpRequest(periodo);

                return new OperationResult(true, "Período eliminado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }
    }
}