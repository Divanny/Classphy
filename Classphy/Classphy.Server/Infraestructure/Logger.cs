using Classphy.Server.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Classphy.Server.Infraestructure
{
    /// <summary>
    /// Clase que se encarga de realizar el registro de las actividades y errores en la aplicación.
    /// </summary>
    public class Logger
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int OnlineUserID;
        private readonly ClassphyContext _ClassphyContext;

        /// <summary>
        /// Constructor de la clase Logger.
        /// </summary>
        /// <param name="serviceProvider">Proveedor de servicios.</param>
        /// <param name="userAccessor">Accesor de usuario.</param>
        /// <param name="configuration">Configuración de la aplicación.</param>
        public Logger(IServiceProvider serviceProvider, IUserAccessor userAccessor, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Classphy");
          
            var contextOptions = new DbContextOptionsBuilder<ClassphyContext>()
                                    .UseSqlServer(connectionString)
                                    .Options;

            _httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            OnlineUserID = userAccessor.idUsuario;
            _ClassphyContext = new ClassphyContext(contextOptions);
        }

        /// <summary>
        /// Registra una solicitud HTTP en alguna actividad.
        /// </summary>
        /// <param name="data">Datos de la solicitud HTTP.</param>
        public void LogHttpRequest(object data)
        {
            LogActividades log = new()
            {
                URL = _httpContextAccessor.HttpContext.Request.Path,
                idUsuario = OnlineUserID,
                Metodo = _httpContextAccessor.HttpContext.Request.Method,
                Fecha = DateTime.Now,
                Data = data == null ? String.Empty : JsonConvert.SerializeObject(data)
            };

            _ClassphyContext.Set<LogActividades>().Add(log);
            _ClassphyContext.SaveChanges();
        }

        /// <summary>
        /// Registra un error en el registro de errores.
        /// </summary>
        /// <param name="ex">Excepción que se produjo.</param>
        public void LogError(Exception ex)
        {
            LogErrores log = new()
            {
                idUsuario = OnlineUserID,
                Fecha = DateTime.Now,
                Mensaje = ex.Message,
                StackTrace = ex.StackTrace,
                Fuente = ex.Source
            };

            _ClassphyContext.Set<LogErrores>().Add(log);
            _ClassphyContext.SaveChanges();
        }

        /// <summary>
        /// Registra una solicitud HTTP en alguna actividad.
        /// </summary>
        /// <param name="log">Registro de actividad.</param>
        public void LogHttpRequest(LogActividades log)
        {
            _ClassphyContext.Set<LogActividades>().Add(log);
            _ClassphyContext.SaveChanges();
        }
    }
}
