using SQLite;

namespace CensoApp.Models
{
    public class Establecimiento
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Identificación Territorial y Registro
        public string Departamento { get; set; } = "SUCRE";
        public string Municipio { get; set; } = "COROZAL";
        public string CodigoDivipolaDpto { get; set; } = "70";
        public string CodigoDivipolaMunicipio { get; set; } = "215";
        public string NumeroInscripcion { get; set; } = string.Empty;

        // Identificación del Establecimiento
        public string NombreComercial { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string RepresentanteLegal { get; set; } = string.Empty;
        public string NitOCedula { get; set; } = string.Empty;

        // Ubicación y Contacto
        public string Direccion { get; set; } = string.Empty;
        public string Area { get; set; } = "URBANA";
        public string Telefono { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;

        // Clasificación Sanitaria
        public string TipoSujeto { get; set; } = "PREPARACION DE ALIMENTOS";
        public string TipoEstablecimiento { get; set; } = "RESTAURANTE";
        public string TipoRiesgo { get; set; } = "ALTO";

        // Estado y Normativa General
        public string Estado { get; set; } = "ACTIVO";
        public string AutorizacionSanitariaDecreto1500 { get; set; } = "NO";
        public string NumeroFechaResolucionAutorizacion { get; set; } = string.Empty;

        // Resumen de la Última Visita (Para mostrar en listas)
        public string PorcentajeCumplimiento { get; set; } = "0";
        public string ConceptoSanitario { get; set; } = "SIN VISITA RECIENTE";
    }
}