namespace Classphy.Server.Models;

public class AsistenciasModel
{
    public int idAsistencia { get; set; }

    public int idAsignatura { get; set; }

    public int idEstudiante { get; set; }

    public string? Nombres { get; set; }

    public string? Apellidos { get; set; }

    public string? Matricula { get; set; }
    
    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public DateTime Fecha { get; set; }

    public bool Presente { get; set; }
}