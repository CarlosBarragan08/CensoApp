using SQLite;

namespace CensoApp.Models
{
    public class Visita
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Clave foránea que relaciona esta visita con un Establecimiento
        public int EstablecimientoId { get; set; }
        public int NumeroVisita { get; set; } = 1;

        public string FechaVisita { get; set; } = string.Empty;
        public string PorcentajeCumplimiento { get; set; } = "0";
        public string ConceptoSanitario { get; set; } = "SIN VISITA RECIENTE";
        public string FuncionariosVisita { get; set; } = string.Empty;
        public string ObjetivoVisita { get; set; } = "PROGRAMACION";

        public string Denuncias { get; set; } = "NO";
        public string Eta { get; set; } = "NO";
        public string TieneMedidaSanitaria { get; set; } = "NO";
        public string TipoMedidaSanitaria { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }
}