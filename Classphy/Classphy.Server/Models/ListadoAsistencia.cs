namespace Classphy.Server.Models
{
    public class ConsultaListadoAsistencia
    {
        public int idAsignatura { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
