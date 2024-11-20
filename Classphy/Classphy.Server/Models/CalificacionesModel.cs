using System.ComponentModel.DataAnnotations;

namespace Classphy.Server.Entities;

public class CalificacionesModel
{
    [Key]
    public int idCalificacion { get; set; }

    [Required(ErrorMessage = "La asignatura es requerida")]
    public int idAsignatura { get; set; }

    [Required(ErrorMessage = "El estudiante es requerido")]
    public int idEstudiante { get; set; }
    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Matricula { get; set; }

    public string? Correo { get; set; }

    public int CantidadFaltas { get; set; }

    [Range(0, 100, ErrorMessage = "La calificación de medio término debe estar entre 0 y 100"), Required(ErrorMessage = "La calificación de medio término es requerida")]
    public int MedioTermino { get; set; }

    public int? Final { get; set; }
}
