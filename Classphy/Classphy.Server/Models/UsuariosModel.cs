using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Classphy.Server.Models
{
    public class UsuariosModel
    {
        public int idUsuario { get; set; }

        [Required(ErrorMessage = "Se debe especificar el perfil")]
        [Range(1, int.MaxValue, ErrorMessage = "Se debe especificar un perfil válido")]
        public int idPerfil { get; set; }
        public string? NombrePerfil { get; set; }

        [Required(ErrorMessage = "Se debe especificar el nombre de usuario")]
        [StringLength(50, ErrorMessage = "No puede exceder a los 50 carácteres")]
        [Unicode(false)]
        public string? NombreUsuario { get; set; }

        [Required(ErrorMessage = "Se debe especificar el correo electrónico")]
        [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
        [StringLength(50, ErrorMessage = "No puede exceder a los 100 carácteres")]
        public string? CorreoElectronico { get; set; }
        public string? ContraseñaHashed { get; set; }
        public string? Contraseña { get; set; }

        [Required(ErrorMessage = "Se debe especificar los nombres")]
        [StringLength(50, ErrorMessage = "No puede exceder a los 50 carácteres")]
        [Unicode(false)]
        public string? Nombres { get; set; }

        [Required(ErrorMessage = "Se debe especificar los apellidos")]
        [StringLength(50, ErrorMessage = "No puede exceder a los 50 carácteres")]
        [Unicode(false)]
        public string? Apellidos { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = "Se debe especificar el estatus del usuario")]
        public bool Activo { get; set; }
    }
}
