using CensoApp.Models;
using CensoApp.Services;
using System.Globalization;

namespace CensoApp.Views
{
    public partial class NuevaVisitaPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private readonly Establecimiento _establecimiento;
        private readonly int _numeroVisita;

        public NuevaVisitaPage(DatabaseService dbService, Establecimiento establecimiento, int numeroVisita)
        {
            InitializeComponent();
            _dbService = dbService;
            _establecimiento = establecimiento;
            _numeroVisita = numeroVisita;

            LblTituloVisita.Text = $"REGISTRAR VISITA #{_numeroVisita}";
            DpFechaVisita.Date = DateTime.Now;

            PckObjetivo.SelectedIndex = 0; // PROGRAMACIÓN
            PckDenuncias.SelectedIndex = 0; // NO
            PckEta.SelectedIndex = 0; // NO
            PckMedida.SelectedIndex = 0; // NO
        }

        private void OnObjetivoSelectedIndexChanged(object sender, EventArgs e)
        {
            string seleccionado = PckObjetivo.SelectedItem?.ToString() ?? "";
            SeccionOtroObjetivo.IsVisible = (seleccionado == "OTRO");
        }

        private void OnPorcentajeTextChanged(object sender, TextChangedEventArgs e)
        {
            string textoLimpio = e.NewTextValue?.Replace(',', '.') ?? "";

            if (double.TryParse(textoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out double pct))
            {
                if (pct < 0 || pct > 100)
                {
                    LblConceptoAuto.Text = "PORCENTAJE INVÁLIDO (0 - 100)";
                    LblConceptoAuto.TextColor = Color.FromArgb("#64748B");
                }
                else if (pct < 60)
                {
                    LblConceptoAuto.Text = "DESFAVORABLE";
                    LblConceptoAuto.TextColor = Color.FromArgb("#EF4444");
                }
                else if (pct >= 60 && pct < 90)
                {
                    LblConceptoAuto.Text = "FAVORABLE CON REQUERIMIENTOS";
                    LblConceptoAuto.TextColor = Color.FromArgb("#F59E0B");
                }
                else
                {
                    LblConceptoAuto.Text = "FAVORABLE";
                    LblConceptoAuto.TextColor = Color.FromArgb("#22C55E");
                }
            }
            else
            {
                LblConceptoAuto.Text = "INGRESE UN PORCENTAJE VÁLIDO";
                LblConceptoAuto.TextColor = Color.FromArgb("#64748B");
            }
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            string textoLimpio = TxtPorcentaje.Text?.Replace(',', '.') ?? "";

            if (!double.TryParse(textoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out double pct) || pct < 0 || pct > 100)
            {
                await DisplayAlert("Atención", "Por favor ingrese un porcentaje válido entre 0 y 100.", "OK");
                return;
            }

            DateTime fechaSeleccionada = DpFechaVisita.Date ?? DateTime.Now;
            string pctFormateado = pct.ToString("0.##", CultureInfo.InvariantCulture);

            string motivoSeleccionado = PckObjetivo.SelectedItem?.ToString() ?? "PROGRAMACIÓN";
            if (motivoSeleccionado == "OTRO" && !string.IsNullOrWhiteSpace(TxtOtroObjetivo.Text))
            {
                motivoSeleccionado = $"OTRO: {TxtOtroObjetivo.Text.Trim()}";
            }

            var nuevaVisita = new Visita
            {
                EstablecimientoId = _establecimiento.Id,
                NumeroVisita = _numeroVisita,
                FechaVisita = fechaSeleccionada.ToString("dd/MM/yyyy"),
                PorcentajeCumplimiento = pctFormateado,
                ConceptoSanitario = LblConceptoAuto.Text,
                FuncionariosVisita = TxtFuncionarios.Text?.Trim() ?? "N/A",
                ObjetivoVisita = motivoSeleccionado,
                Denuncias = PckDenuncias.SelectedItem?.ToString() ?? "NO",
                Eta = PckEta.SelectedItem?.ToString() ?? "NO",
                TieneMedidaSanitaria = PckMedida.SelectedItem?.ToString() ?? "NO",
                TipoMedidaSanitaria = TxtTipoMedida.Text?.Trim() ?? "",
                Observaciones = TxtObservaciones.Text?.Trim() ?? ""
            };

            await _dbService.GuardarVisitaAsync(nuevaVisita);

            _establecimiento.PorcentajeCumplimiento = pctFormateado;
            _establecimiento.ConceptoSanitario = LblConceptoAuto.Text;
            await _dbService.GuardarEstablecimientoAsync(_establecimiento);

            await DisplayAlert("Éxito", $"Visita #{_numeroVisita} registrada correctamente.", "OK");
            await Navigation.PopAsync();
        }
    }
}