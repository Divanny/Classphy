using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Classphy.Server.Models;

public class EstudiantesModel
{
    public int idEstudiante { get; set; }

    public int idUsuario { get; set; }

    [Required(ErrorMessage = "El campo Nombres es requerido")]
    public string? Nombres { get; set; }

    [Required(ErrorMessage = "El campo Apellidos es requerido")]
    public string? Apellidos { get; set; }

    [Required(ErrorMessage = "El campo Matricula es requerido")]
    public string? Matricula { get; set; }

    [Required(ErrorMessage = "El campo Correo es requerido")]
    [EmailAddress(ErrorMessage = "El campo Correo debe ser una dirección de correo electrónico válida")]
    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    [Required(ErrorMessage = "El campo Genero es requerido")]
    public string? Genero { get; set; }
    public int CantidadAsignaturasAsociadas { get; set; }
    public List<AsignaturasModel>? Asignaturas { get; set; }
}
