using CensoApp.Models;
using CensoApp.Services;

namespace CensoApp.Views
{
    public partial class DetallePage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private Establecimiento _establecimiento;

        public DetallePage(DatabaseService dbService, Establecimiento establecimiento)
        {
            InitializeComponent();
            _dbService = dbService;
            _establecimiento = establecimiento;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CargarDatos();
        }

        private async Task CargarDatos()
        {
            var lista = await _dbService.ObtenerEstablecimientosAsync();
            var estActualizado = lista.FirstOrDefault(e => e.Id == _establecimiento.Id);
            if (estActualizado != null)
            {
                _establecimiento = estActualizado;
            }

            var visitas = await _dbService.ObtenerVisitasPorEstablecimientoAsync(_establecimiento.Id);

            LblNombreComercial.Text = _establecimiento.NombreComercial;
            LblNombreComercial.TextColor = ObtenerColorSemaforico(_establecimiento.ConceptoSanitario, _establecimiento.PorcentajeCumplimiento, _establecimiento.TipoRiesgo);
            LblRazonSocial.Text = $"Razón Social: {FormatearTexto(_establecimiento.RazonSocial)}";

            LblNit.Text = $"• NIT / Cédula: {FormatearTexto(_establecimiento.NitOCedula)}";
            LblInscripcion.Text = $"• N° Inscripción: {FormatearTexto(_establecimiento.NumeroInscripcion)}";
            LblRepresentante.Text = $"• Representante Legal: {FormatearTexto(_establecimiento.RepresentanteLegal)}";
            LblTipoSujeto.Text = $"• Tipo Sujeto: {FormatearTexto(_establecimiento.TipoSujeto)}";
            LblEstado.Text = $"• Estado Registro: {FormatearTexto(_establecimiento.Estado)}";

            LblDireccion.Text = $"• Dirección: {FormatearTexto(_establecimiento.Direccion)}";
            LblMunicipio.Text = $"• Municipio: {FormatearTexto(_establecimiento.Municipio)} (Divipola: {FormatearTexto(_establecimiento.CodigoDivipolaMunicipio)})";
            LblDepartamento.Text = $"• Departamento: {FormatearTexto(_establecimiento.Departamento)} (Divipola: {FormatearTexto(_establecimiento.CodigoDivipolaDpto)})";
            LblArea.Text = $"• Área: {FormatearTexto(_establecimiento.Area)}";
            LblTelefono.Text = $"• Teléfono: {FormatearTexto(_establecimiento.Telefono)}";
            LblCorreo.Text = $"• Correo Electrónico: {FormatearTexto(_establecimiento.CorreoElectronico)}";

            LblTipoEstablecimiento.Text = $"• Tipo Establecimiento: {FormatearTexto(_establecimiento.TipoEstablecimiento)}";
            LblRiesgo.Text = $"• Nivel de Riesgo: {FormatearTexto(_establecimiento.TipoRiesgo)}";
            LblAutDec1500.Text = $"• Aut. Sanitaria Dec. 1500: {FormatearTexto(_establecimiento.AutorizacionSanitariaDecreto1500)}";
            LblResAutorizacion.Text = $"• Res. / Fecha Autorización: {FormatearTexto(_establecimiento.NumeroFechaResolucionAutorizacion)}";

            string conceptoActual = string.IsNullOrWhiteSpace(_establecimiento.ConceptoSanitario)
                ? (visitas.FirstOrDefault()?.ConceptoSanitario ?? "PENDIENTE")
                : _establecimiento.ConceptoSanitario;

            LblEstadoSanitarioActual.Text = $"• Último Concepto: {conceptoActual} | Cumplimiento: {_establecimiento.PorcentajeCumplimiento}%";

            var visitasVM = visitas.Select(v => new VisitaItemViewModel
            {
                NumeroVisita = v.NumeroVisita,
                FechaVisita = v.FechaVisita,
                PorcentajeCumplimiento = v.PorcentajeCumplimiento,
                ConceptoSanitario = v.ConceptoSanitario,
                FuncionariosVisita = v.FuncionariosVisita,
                ObjetivoVisita = v.ObjetivoVisita,
                Denuncias = v.Denuncias,
                Eta = v.Eta,
                TieneMedidaSanitaria = v.TieneMedidaSanitaria,
                TipoMedidaSanitaria = v.TipoMedidaSanitaria,
                Observaciones = v.Observaciones,
                ColorConcepto = ObtenerColorSemaforico(v.ConceptoSanitario, v.PorcentajeCumplimiento, null)
            }).ToList();

            ListaVisitas.ItemsSource = visitasVM;
        }

        private static string FormatearTexto(string? valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? "N/A" : valor;
        }

        private async void OnNuevaVisitaClicked(object sender, EventArgs e)
        {
            var visitas = await _dbService.ObtenerVisitasPorEstablecimientoAsync(_establecimiento.Id);
            int siguienteNumeroVisita = (visitas != null && visitas.Count > 0)
                ? visitas.Max(v => v.NumeroVisita) + 1
                : 1;

            await Navigation.PushAsync(new NuevaVisitaPage(_dbService, _establecimiento, siguienteNumeroVisita));
        }

        private async void OnEditarClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FormularioPage(_dbService, _establecimiento));
        }

        private async void OnEliminarClicked(object sender, EventArgs e)
        {
            bool confirmar = await DisplayAlert(
                "Eliminar Establecimiento",
                $"¿Está seguro de eliminar '{_establecimiento.NombreComercial}' y todo su historial de visitas?",
                "Eliminar",
                "Cancelar"
            );

            if (confirmar)
            {
                await _dbService.EliminarEstablecimientoAsync(_establecimiento);
                await DisplayAlert("Eliminado", "El registro fue eliminado correctamente.", "OK");
                await Navigation.PopAsync();
            }
        }

        private static Color ObtenerColorSemaforico(string? concepto, string? porcentaje, string? riesgo)
        {
            string c = concepto?.ToUpper() ?? "";
            string r = riesgo?.ToUpper() ?? "";
            double pct = -1;

            if (!string.IsNullOrWhiteSpace(porcentaje))
            {
                string textoLimpio = porcentaje.Replace("%", "").Replace(',', '.').Trim();
                double.TryParse(textoLimpio, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out pct);
            }

            if (c.Contains("DESFAVORABLE") || r == "ALTO" || (pct >= 0 && pct < 60))
            {
                return Color.FromArgb("#EF4444"); // Rojo
            }
            if (c.Contains("REQUERIMIENTOS") || r == "MEDIO" || (pct >= 60 && pct < 90))
            {
                return Color.FromArgb("#F59E0B"); // Naranja
            }
            if (c.Contains("FAVORABLE") || r == "BAJO" || pct >= 90)
            {
                return Color.FromArgb("#22C55E"); // Verde
            }

            return Color.FromArgb("#22C55E");
        }
    }

    public class VisitaItemViewModel
    {
        public int NumeroVisita { get; set; }
        public string NumeroVisitaTexto => $"VISITA #{NumeroVisita}";
        public string FechaVisita { get; set; } = string.Empty;
        public string PorcentajeCumplimiento { get; set; } = string.Empty;
        public string ConceptoSanitario { get; set; } = string.Empty;
        public string CumplimientoYConceptoTexto => $"Cumplimiento: {PorcentajeCumplimiento}% - Concepto: {ConceptoSanitario}";
        public Color ColorConcepto { get; set; } = Color.FromArgb("#22C55E");

        public string FuncionariosVisita { get; set; } = string.Empty;
        public string FuncionariosTexto => $"• Funcionarios: {(string.IsNullOrWhiteSpace(FuncionariosVisita) ? "N/A" : FuncionariosVisita)}";

        public string ObjetivoVisita { get; set; } = string.Empty;
        public string ObjetivoTexto => $"• Objetivo Visita: {(string.IsNullOrWhiteSpace(ObjetivoVisita) ? "N/A" : ObjetivoVisita)}";

        public string Denuncias { get; set; } = string.Empty;
        public string DenunciasTexto => $"• Atención Denuncias: {(string.IsNullOrWhiteSpace(Denuncias) ? "NO" : Denuncias)}";

        public string Eta { get; set; } = string.Empty;
        public string EtaTexto => $"• Atención Brote ETA: {(string.IsNullOrWhiteSpace(Eta) ? "NO" : Eta)}";

        public string TieneMedidaSanitaria { get; set; } = string.Empty;
        public string TipoMedidaSanitaria { get; set; } = string.Empty;
        public string MedidaTexto => $"• Medida Sanitaria: {(string.IsNullOrWhiteSpace(TieneMedidaSanitaria) ? "NO" : TieneMedidaSanitaria)} {(string.IsNullOrWhiteSpace(TipoMedidaSanitaria) ? "" : $"({TipoMedidaSanitaria})")}";

        public string Observaciones { get; set; } = string.Empty;
        public string ObservacionesTexto => string.IsNullOrWhiteSpace(Observaciones) ? "• Observaciones: Sin observaciones" : $"• Observaciones: {Observaciones}";
    }
}