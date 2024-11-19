using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Classphy.Server.Models;

public class AsignaturasModel
{
    public int idAsignatura { get; set; }

    [Required(ErrorMessage = "Debe especificar el nombre de la asignatura")]
    public string? Nombre { get; set; }

    [Required(ErrorMessage = "Debe especificar el periodo al que pertenece la asignatura")]
    public int idPeriodo { get; set; }

    [Required(ErrorMessage = "Debe especificar la descripción de la asignatura")]
    public string? Descripcion { get; set; }
}