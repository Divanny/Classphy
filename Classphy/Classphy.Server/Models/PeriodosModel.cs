using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Classphy.Server.Models;

public class PeriodosModel
{
    public int idPeriodo { get; set; }

    [Required(ErrorMessage = "Debe especificar el nombre del periodo")]
    public string? Nombre { get; set; }

    public int idUsuario { get; set; }

    public DateTime FechaRegistro { get; set; }
}