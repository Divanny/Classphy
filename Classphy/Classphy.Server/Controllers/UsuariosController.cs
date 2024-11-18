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
    public class UsuariosController : Controller
    {
        private readonly Logger _logger;
        private readonly int _idUsuarioOnline;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ClassphyContext _classphyContext;
        private readonly UsuariosRepo _usuariosRepo;

        /// <summary>
        /// Constructor de la clase UsuariosController.
        /// </summary>
        /// <param name="userAccessor"></param>
        /// <param name="passwordHasher"></param>
        /// <param name="classphyContext"></param>
        /// <param name="logger"></param>
        public UsuariosController(IUserAccessor userAccessor, IPasswordHasher passwordHasher, ClassphyContext classphyContext, Logger logger)
        {
            _logger = logger;
            _idUsuarioOnline = userAccessor.idUsuario;
            _passwordHasher = passwordHasher;
            _classphyContext = classphyContext;
            _usuariosRepo = new UsuariosRepo(classphyContext);
        }

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet(Name = "GetUsuarios")]
        [AuthorizeByProfile(PerfilesEnum.Administrador)]
        public List<UsuariosModel> Get()
        {
            List<UsuariosModel> usuarios = _usuariosRepo.Get().ToList();

            foreach (var usuario in usuarios)
            {
                usuario.ContraseñaHashed = null;
            }

            return usuarios;
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <returns>Usuario encontrado.</returns>
        [HttpGet("{idUsuario}", Name = "GetUsuario")]
        [AuthorizeByProfile(PerfilesEnum.Administrador)]
        public UsuariosModel Get(int idUsuario)
        {
            UsuariosModel usuario = _usuariosRepo.Get(idUsuario);

            if (usuario == null) throw new Exception("Este usuario no se ha encontrado");

            usuario.ContraseñaHashed = null;
            return usuario;
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="usuariosModel">Datos del usuario a crear.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPost(Name = "SaveUsuario")]
        [AuthorizeByProfile(PerfilesEnum.Administrador)]
        public OperationResult Post(UsuariosModel usuariosModel)
        {
            try
            {
                if (_usuariosRepo.Any(x => x.NombreUsuario == usuariosModel.NombreUsuario)) return new OperationResult(false, "Este usuario ya existe en el sistema");
                if (_usuariosRepo.Any(x => x.CorreoElectronico == usuariosModel.CorreoElectronico)) return new OperationResult(false, "Este correo electrónico ya está registrado");

                if (_classphyContext.Set<Perfiles>().Find(usuariosModel.idPerfil) == null) return new OperationResult(false, "Este perfil no se ha encontrado");
                if (usuariosModel.Contraseña == null) return new OperationResult(false, "La contraseña no puede estar vacía");
                
                if (usuariosModel.Contraseña.Length < 8) return new OperationResult(false, "La contraseña debe tener al menos 8 caracteres");
                if (usuariosModel.Contraseña.Any(char.IsDigit) == false) return new OperationResult(false, "La contraseña debe tener al menos un número");
                if (usuariosModel.Contraseña.Any(char.IsUpper) == false) return new OperationResult(false, "La contraseña debe tener al menos una letra mayúscula");

                usuariosModel.FechaRegistro = DateTime.Now;
                usuariosModel.ContraseñaHashed = _passwordHasher.Hash(usuariosModel.Contraseña);

                var created = _usuariosRepo.Add(usuariosModel);
                _logger.LogHttpRequest(usuariosModel);
                return new OperationResult(true, "Usuario creado exitosamente", created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="idUsuario">ID del usuario.</param>
        /// <param name="usuariosModel">Datos del usuario a actualizar.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{idUsuario}", Name = "UpdateUsuario")]
        [AuthorizeByProfile(PerfilesEnum.Administrador)]
        public OperationResult Put(int idUsuario, UsuariosModel usuariosModel)
        {
            try
            {
                var usuario = _usuariosRepo.Get(x => x.idUsuario == idUsuario).FirstOrDefault();

                if (usuario == null) return new OperationResult(false, "El usuario no se ha encontrado");

                if (usuario.NombreUsuario != usuariosModel.NombreUsuario && _usuariosRepo.Any(x => x.NombreUsuario == usuariosModel.NombreUsuario)) return new OperationResult(false, "Este usuario ya existe en el sistema");

                if (usuario.CorreoElectronico != usuariosModel.CorreoElectronico && _usuariosRepo.Any(x => x.CorreoElectronico == usuariosModel.CorreoElectronico)) return new OperationResult(false, "Este correo electrónico ya está registrado");

                if (usuario.idPerfil != usuariosModel.idPerfil)
                {
                    var perfil = _classphyContext.Set<Perfiles>().Where(x => x.idPerfil == usuariosModel.idPerfil);
                    if (perfil == null) return new OperationResult(false, "Este perfil no se ha encontrado");
                }

                usuario.NombreUsuario = usuariosModel.NombreUsuario;
                usuario.CorreoElectronico = usuariosModel.CorreoElectronico;
                usuario.idPerfil = usuariosModel.idPerfil;
                usuario.Activo = usuariosModel.Activo;

                _usuariosRepo.Edit(usuario);
                _logger.LogHttpRequest(usuariosModel);

                usuario.ContraseñaHashed = null;
                return new OperationResult(true, "Usuario editado exitosamente", usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Obtiene los perfiles del sistema
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPerfiles", Name = "GetPerfiles")]
        [Authorize]
        public List<Perfiles> GetPerfiles()
        {
            var perfiles = _classphyContext.Set<Perfiles>().ToList();
            return perfiles;
        }
    }
}