using Classphy.Server.Entities;
using Classphy.Server.Models;
using Classphy.Server.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Classphy.Server.Infraestructure
{
    /// <summary>
    /// Clase que maneja la autenticación de usuarios.
    /// </summary>
    public class Authentication
    {
        private readonly IConfiguration _configuration;
        private readonly Dictionary<string, string> _testUsers;
        private readonly ClassphyContext _ClassphyContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly UsuariosRepo _usuariosRepo;
        /// <summary>
        /// Constructor de la clase Authentication.
        /// </summary>
        /// <param name="ClassphyContext">Contexto de la base de datos de CrowdSolve.</param>
        /// <param name="configuration">Configuración de la aplicación.</param>
        public Authentication(ClassphyContext ClassphyContext, IConfiguration configuration, IPasswordHasher passwordHasher)
        {
            _ClassphyContext = ClassphyContext;
            _configuration = configuration;
            _testUsers = new Dictionary<string, string>
            {
                { "admin", "Pruebas2024" },
                { "profesor", "Pruebas2024" }
            };
            _passwordHasher = passwordHasher;
            _usuariosRepo = new UsuariosRepo(ClassphyContext);
        }

        /// <summary>
        /// Método para iniciar sesión.
        /// </summary>
        /// <param name="credentials">Credenciales de inicio de sesión.</param>
        /// <returns>Resultado de la operación de inicio de sesión.</returns>
        public OperationResult SignIn(Credentials credentials)
        {
            if (credentials == null) return new OperationResult(false, "Credenciales no proporcionadas", false);
            if (string.IsNullOrEmpty(credentials.Username)) return new OperationResult(false, "Nombre de usuario o correo electrónico no proporcionado", false);
            if (string.IsNullOrEmpty(credentials.Password)) return new OperationResult(false, "Contraseña no proporcionada", false);

            if (IsDevelopmentEnvironment())
            {
                if (_testUsers.ContainsKey(credentials.Username) && _testUsers[credentials.Username] == credentials.Password)
                {
                    return new OperationResult(true, "Inicio de sesión exitoso", null, TokenGenerator(credentials.Username, 1, 1));
                }
            }

            var usuario = _ClassphyContext.Set<Usuarios>().Where(x => x.NombreUsuario.Equals(credentials.Username) || x.CorreoElectronico.Equals(credentials.Username)).FirstOrDefault();

            if (usuario == null)
            {
                return new OperationResult(false, "Usuario o contraseña incorrectos");
            }

            if (!_passwordHasher.Check(usuario.Contraseña ?? "", credentials.Password))
            {
                return new OperationResult(false, "Usuario o contraseña incorrectos");
            }

            if (!usuario.Activo)
            {
                return new OperationResult(false, "Este usuario está inactivo");
            }

            string token = TokenGenerator(usuario.NombreUsuario, usuario.idUsuario, usuario.idPerfil);

            var data = _usuariosRepo.GetByUsername(usuario.NombreUsuario);
            data.ContraseñaHashed = null;

            return new OperationResult(true, "Éxito al iniciar sesión", data, token);
        }

        /// <summary>
        /// Genera un token de autenticación.
        /// </summary>
        /// <param name="UserName">Nombre de usuario.</param>
        /// <param name="idUsuario">ID de usuario.</param>
        /// <param name="idPerfil">ID de perfil.</param>
        /// <returns>Token de autenticación.</returns>
        private string TokenGenerator(string UserName, int idUsuario, int idPerfil)
        {
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", Guid.NewGuid().ToString()),
                    new Claim("idUsuario", idUsuario.ToString()),
                    new Claim(ClaimTypes.Role, idPerfil.ToString() ?? ""),
                    new Claim(JwtRegisteredClaimNames.Sub, UserName),
                    new Claim(JwtRegisteredClaimNames.Email, UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(90),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials
                (new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);

            return stringToken;
        }



        /// <summary>
        /// Verifica si el entorno de desarrollo está activo.
        /// </summary>
        /// <returns>true si el entorno de desarrollo está activo, false en caso contrario.</returns>
        private bool IsDevelopmentEnvironment()
        {
            string environment = _configuration["Environment"];
            return environment == "Development";
        }
    }
}