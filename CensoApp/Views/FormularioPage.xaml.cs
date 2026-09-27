using CensoApp.Models;
using CensoApp.Services;
using System.Globalization;

namespace CensoApp.Views
{
    public partial class FormularioPage : ContentPage
    {
        private readonly DatabaseService _dbService;
        private readonly Establecimiento? _establecimientoEdicion;

        public FormularioPage(DatabaseService dbService, Establecimiento? establecimiento = null)
        {
            InitializeComponent();
            _dbService = dbService;
            _establecimientoEdicion = establecimiento;

            if (_establecimientoEdicion != null)
            {
                Title = "Editar Establecimiento";
                CargarDatosParaEdicion();
            }
            else
            {
                Title = "Nuevo Registro";
                EstablecerValoresPorDefecto();
            }
        }

        private void EstablecerValoresPorDefecto()
        {
            PckTipoSujeto.SelectedIndex = 0; // PERSONA NATURAL

            PckDepartamento.SelectedItem = "SUCRE";
            TxtDivipolaDpto.Text = "70";

            PckMunicipio.SelectedItem = "COROZAL";
            TxtDivipolaMuni.Text = "215";

            PckActividadPrincipal.SelectedIndex = 0; // PREPARACIÓN ALIMENTOS

            PckEstado.SelectedIndex = 0; // ACTIVO
            PckArea.SelectedIndex = 0; // URBANA
            PckRiesgo.SelectedIndex = 0; // ALTO
            PckDec1500.SelectedIndex = 0; // NO

            DpFechaVisitaInicial.Date = DateTime.Now;
            PckObjetivoInicial.SelectedIndex = 0; // PROGRAMACIÓN
            PckDenunciasInicial.SelectedIndex = 0; // NO
            PckEtaInicial.SelectedIndex = 0; // NO
            PckMedidaInicial.SelectedIndex = 0; // NO
        }

        private void OnObjetivoInicialSelectedIndexChanged(object sender, EventArgs e)
        {
            string seleccionado = PckObjetivoInicial.SelectedItem?.ToString() ?? "";
            SeccionOtroObjetivoInicial.IsVisible = (seleccionado == "OTRO");
        }

        private List<string> ObtenerSubcategorias(string actividad)
        {
            return actividad switch
            {
                "PREPARACIÓN ALIMENTOS" => new List<string>
                {
                    "Restaurante",
                    "Cafetería",
                    "Panadería y/o Pastelería",
                    "Jugos, Frutería y/o Heladería",
                    "Comidas Rápidas"
                },
                "COMEDORES" => new List<string>
                {
                    "Programas sociales del estado",
                    "Comedores carcelarios o penitenciarios (USPEC)"
                },
                "EXPENDIO ALIMENTOS" => new List<string>
                {
                    "Expendio",
                    "Productos de la pesca"
                },
                "GRANDES SUPERFICIES" => new List<string>
                {
                    "Hipermercado / Supermercado"
                },
                "ENSAMBLE ALIMENTOS" => new List<string>
                {
                    "Ensamble menú",
                    "Ensamble refrigerio"
                },
                "ALMACENAMIENTO" => new List<string>
                {
                    "Almacenamiento a temperatura ambiente",
                    "Depósito de frío"
                },
                "VENTA VÍA PÚBLICA" => new List<string>
                {
                    "Puesto fijo o estacionario",
                    "Puesto móvil o ambulante",
                    "Estacionario con preparación de alimentos",
                    "Ambulante con preparación de alimentos"
                },
                "EXPENDIO BEBIDAS ALCOHÓLICAS" => new List<string>
                {
                    "Expendio Bebidas Alcohólicas"
                },
                "PLAZAS DE MERCADO" => new List<string>
                {
                    "Plaza de Mercado",
                    "Central de Abasto"
                },
                _ => new List<string> { "Restaurante" }
            };
        }

        private void OnActividadPrincipalSelectedIndexChanged(object sender, EventArgs e)
        {
            string actividad = PckActividadPrincipal.SelectedItem?.ToString() ?? "";
            var subcategorias = ObtenerSubcategorias(actividad);

            PckTipoEstablecimiento.ItemsSource = subcategorias;
            if (subcategorias.Count > 0)
            {
                PckTipoEstablecimiento.SelectedIndex = 0;
            }
        }

        private void OnMunicipioSelectedIndexChanged(object sender, EventArgs e)
        {
            string mpio = PckMunicipio.SelectedItem?.ToString() ?? "";
            TxtDivipolaMuni.Text = mpio switch
            {
                "SINCELEJO" => "001",
                "BUENAVISTA" => "110",
                "CAIMITO" => "124",
                "COLOSO" => "204",
                "COROZAL" => "215",
                "COVEÑAS" => "221",
                "EL ROBLE" => "230",
                "GALERAS" => "235",
                "GUARANDA" => "265",
                "LA UNIÓN" => "400",
                "LOS PALMITOS" => "418",
                "MAJAGUAL" => "429",
                "MORROA" => "473",
                "OVEJAS" => "508",
                "PALMITO" => "523",
                "SAMPUÉS" => "670",
                "SAN BENITO ABAD" => "678",
                "SAN JUAN DE BETULIA" => "702",
                "SAN MARCOS" => "708",
                "SAN ONOFRE" => "713",
                "SAN PEDRO" => "717",
                "SINCÉ" => "742",
                "SUCRE" => "771",
                "TOLÚ" => "820",
                "TOLÚ VIEJO" => "823",
                _ => "215"
            };
        }

        private void OnPorcentajeInicialTextChanged(object sender, TextChangedEventArgs e)
        {
            string textoLimpio = e.NewTextValue?.Replace(',', '.') ?? "";

            if (double.TryParse(textoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out double pct))
            {
                if (pct < 0 || pct > 100)
                {
                    LblConceptoInicialAuto.Text = "PORCENTAJE INVÁLIDO (0 - 100)";
                    LblConceptoInicialAuto.TextColor = Color.FromArgb("#64748B");
                }
                else if (pct < 60)
                {
                    LblConceptoInicialAuto.Text = "DESFAVORABLE";
                    LblConceptoInicialAuto.TextColor = Color.FromArgb("#EF4444");
                }
                else if (pct >= 60 && pct < 90)
                {
                    LblConceptoInicialAuto.Text = "FAVORABLE CON REQUERIMIENTOS";
                    LblConceptoInicialAuto.TextColor = Color.FromArgb("#F59E0B");
                }
                else
                {
                    LblConceptoInicialAuto.Text = "FAVORABLE";
                    LblConceptoInicialAuto.TextColor = Color.FromArgb("#22C55E");
                }
            }
            else
            {
                LblConceptoInicialAuto.Text = "INGRESE UN PORCENTAJE VÁLIDO";
                LblConceptoInicialAuto.TextColor = Color.FromArgb("#64748B");
            }
        }

        private void CargarDatosParaEdicion()
        {
            if (_establecimientoEdicion == null) return;

            TxtNombreComercial.Text = _establecimientoEdicion.NombreComercial;
            TxtRazonSocial.Text = _establecimientoEdicion.RazonSocial;
            TxtNit.Text = _establecimientoEdicion.NitOCedula;
            TxtInscripcion.Text = _establecimientoEdicion.NumeroInscripcion;
            TxtRepresentante.Text = _establecimientoEdicion.RepresentanteLegal;

            PckTipoSujeto.SelectedItem = _establecimientoEdicion.TipoSujeto;
            PckEstado.SelectedItem = _establecimientoEdicion.Estado;

            TxtDireccion.Text = _establecimientoEdicion.Direccion;
            PckMunicipio.SelectedItem = _establecimientoEdicion.Municipio;
            TxtDivipolaMuni.Text = _establecimientoEdicion.CodigoDivipolaMunicipio;
            PckDepartamento.SelectedItem = _establecimientoEdicion.Departamento;
            TxtDivipolaDpto.Text = _establecimientoEdicion.CodigoDivipolaDpto;
            PckArea.SelectedItem = _establecimientoEdicion.Area;
            TxtTelefono.Text = _establecimientoEdicion.Telefono;
            TxtCorreo.Text = _establecimientoEdicion.CorreoElectronico;

            string tipoEstGuardado = _establecimientoEdicion.TipoEstablecimiento ?? "";
            string actividadEncontrada = "PREPARACIÓN ALIMENTOS";

            if (PckActividadPrincipal.ItemsSource is IList<string> actividades)
            {
                foreach (var act in actividades)
                {
                    if (ObtenerSubcategorias(act).Contains(tipoEstGuardado, StringComparer.OrdinalIgnoreCase))
                    {
                        actividadEncontrada = act;
                        break;
                    }
                }
            }

            PckActividadPrincipal.SelectedItem = actividadEncontrada;
            PckTipoEstablecimiento.ItemsSource = ObtenerSubcategorias(actividadEncontrada);
            PckTipoEstablecimiento.SelectedItem = tipoEstGuardado;

            PckRiesgo.SelectedItem = _establecimientoEdicion.TipoRiesgo;
            PckDec1500.SelectedItem = _establecimientoEdicion.AutorizacionSanitariaDecreto1500;
            TxtResolucionAutorizacion.Text = _establecimientoEdicion.NumeroFechaResolucionAutorizacion;

            SeccionVisitaInicial.IsVisible = false;
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombreComercial.Text))
            {
                await DisplayAlert("Campo Requerido", "El Nombre Comercial es obligatorio.", "OK");
                return;
            }

            bool esNuevo = (_establecimientoEdicion == null);
            var est = _establecimientoEdicion ?? new Establecimiento();

            est.NombreComercial = TxtNombreComercial.Text?.Trim() ?? string.Empty;
            est.RazonSocial = TxtRazonSocial.Text?.Trim() ?? string.Empty;
            est.NitOCedula = TxtNit.Text?.Trim() ?? string.Empty;
            est.NumeroInscripcion = TxtInscripcion.Text?.Trim() ?? string.Empty;
            est.RepresentanteLegal = TxtRepresentante.Text?.Trim() ?? string.Empty;
            est.TipoSujeto = PckTipoSujeto.SelectedItem?.ToString() ?? "PERSONA NATURAL";
            est.TipoEstablecimiento = PckTipoEstablecimiento.SelectedItem?.ToString() ?? "Restaurante";
            est.Estado = PckEstado.SelectedItem?.ToString() ?? "ACTIVO";

            est.Direccion = TxtDireccion.Text?.Trim() ?? string.Empty;
            est.Municipio = PckMunicipio.SelectedItem?.ToString() ?? "COROZAL";
            est.CodigoDivipolaMunicipio = TxtDivipolaMuni.Text?.Trim() ?? "215";
            est.Departamento = PckDepartamento.SelectedItem?.ToString() ?? "SUCRE";
            est.CodigoDivipolaDpto = TxtDivipolaDpto.Text?.Trim() ?? "70";
            est.Area = PckArea.SelectedItem?.ToString() ?? "URBANA";
            est.Telefono = TxtTelefono.Text?.Trim() ?? string.Empty;
            est.CorreoElectronico = TxtCorreo.Text?.Trim() ?? string.Empty;

            est.TipoRiesgo = PckRiesgo.SelectedItem?.ToString() ?? "ALTO";
            est.AutorizacionSanitariaDecreto1500 = PckDec1500.SelectedItem?.ToString() ?? "NO";
            est.NumeroFechaResolucionAutorizacion = TxtResolucionAutorizacion.Text?.Trim() ?? string.Empty;

            string pctTextoLimpio = TxtPorcentajeInicial.Text?.Replace(',', '.') ?? "0";
            double.TryParse(pctTextoLimpio, NumberStyles.Any, CultureInfo.InvariantCulture, out double pctVal);

            if (esNuevo)
            {
                est.PorcentajeCumplimiento = pctVal.ToString("0.##", CultureInfo.InvariantCulture);
                est.ConceptoSanitario = LblConceptoInicialAuto.Text;
            }

            await _dbService.GuardarEstablecimientoAsync(est);

            if (esNuevo)
            {
                DateTime fechaInicial = DpFechaVisitaInicial.Date ?? DateTime.Now;

                string motivoSeleccionado = PckObjetivoInicial.SelectedItem?.ToString() ?? "PROGRAMACIÓN";
                if (motivoSeleccionado == "OTRO" && !string.IsNullOrWhiteSpace(TxtOtroObjetivoInicial.Text))
                {
                    motivoSeleccionado = $"OTRO: {TxtOtroObjetivoInicial.Text.Trim()}";
                }

                var visitaInicial = new Visita
                {
                    EstablecimientoId = est.Id,
                    NumeroVisita = 1,
                    FechaVisita = fechaInicial.ToString("dd/MM/yyyy"),
                    PorcentajeCumplimiento = pctVal.ToString("0.##", CultureInfo.InvariantCulture),
                    ConceptoSanitario = LblConceptoInicialAuto.Text,
                    FuncionariosVisita = TxtFuncionariosInicial.Text?.Trim() ?? "N/A",
                    ObjetivoVisita = motivoSeleccionado,
                    Denuncias = PckDenunciasInicial.SelectedItem?.ToString() ?? "NO",
                    Eta = PckEtaInicial.SelectedItem?.ToString() ?? "NO",
                    TieneMedidaSanitaria = PckMedidaInicial.SelectedItem?.ToString() ?? "NO",
                    TipoMedidaSanitaria = TxtTipoMedidaInicial.Text?.Trim() ?? string.Empty,
                    Observaciones = TxtObservacionesInicial.Text?.Trim() ?? string.Empty
                };

                await _dbService.GuardarVisitaAsync(visitaInicial, est);
            }

            await DisplayAlert("Éxito", "El registro se guardó correctamente.", "OK");
            await Navigation.PopAsync();
        }

        private async void OnCancelarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}