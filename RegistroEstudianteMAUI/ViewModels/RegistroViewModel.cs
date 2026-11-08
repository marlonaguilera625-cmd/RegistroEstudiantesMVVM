using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RegistroEstudianteMAUI.Models;
using RegistroEstudianteMAUI.Services;
using System.Collections.ObjectModel;

namespace RegistroEstudianteMAUI.ViewModels;

public partial class RegistroViewModel : ObservableObject
{
    private readonly IEstudianteService _estudianteService;
    private readonly HashSet<string> _camposModificados = [];
    private bool _limpiandoFormulario;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    [NotifyPropertyChangedFor(nameof(NombreError))]
    private string nombre = string.Empty;

    partial void OnNombreChanged(string value) => MarcarCampoModificado(nameof(Estudiante.Nombre));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    [NotifyPropertyChangedFor(nameof(CorreoError))]
    private string correo = string.Empty;

    partial void OnCorreoChanged(string value) => MarcarCampoModificado(nameof(Estudiante.Correo));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    [NotifyPropertyChangedFor(nameof(TelefonoError))]
    private string telefono = string.Empty;

    partial void OnTelefonoChanged(string value) => MarcarCampoModificado(nameof(Estudiante.Telefono));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    [NotifyPropertyChangedFor(nameof(CarreraError))]
    private string carrera = string.Empty;

    partial void OnCarreraChanged(string value) => MarcarCampoModificado(nameof(Estudiante.Carrera));

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
    [NotifyPropertyChangedFor(nameof(FechaError))]
    private DateTime? fecha;

    partial void OnFechaChanged(DateTime? value) => MarcarCampoModificado(nameof(Estudiante.Fecha));

    [ObservableProperty]
    private string mensaje = string.Empty;

    public ObservableCollection<Estudiante> Estudiantes { get; } = [];
    public IReadOnlyList<string> Carreras => _estudianteService.Carreras;

    public string NombreError => ObtenerError(nameof(Estudiante.Nombre));
    public string CorreoError => ObtenerError(nameof(Estudiante.Correo));
    public string TelefonoError => ObtenerError(nameof(Estudiante.Telefono));
    public string CarreraError => ObtenerError(nameof(Estudiante.Carrera));
    public string FechaError => ObtenerError(nameof(Estudiante.Fecha));

    public RegistroViewModel(IEstudianteService estudianteService)
    {
        _estudianteService = estudianteService;
    }

    private bool PuedeGuardar() => _estudianteService.Validar(CrearEstudiante()).Count == 0;

    [RelayCommand(CanExecute = nameof(PuedeGuardar))]
    private void Guardar()
    {
        var estudiante = CrearEstudiante();
        var errores = _estudianteService.Validar(estudiante);
        if (errores.Count > 0)
        {
            Mensaje = "Revisa los campos indicados antes de continuar.";
            return;
        }

        Estudiantes.Add(estudiante);
        Mensaje = $"Registro guardado para {estudiante.Nombre}.";
        LimpiarCampos();
    }

    [RelayCommand]
    private void Limpiar()
    {
        LimpiarCampos();
        Mensaje = string.Empty;
    }

    private void LimpiarCampos()
    {
        _limpiandoFormulario = true;
        _camposModificados.Clear();
        Nombre = string.Empty;
        Correo = string.Empty;
        Telefono = string.Empty;
        Carrera = string.Empty;
        Fecha = null;
        _limpiandoFormulario = false;
        NotificarErrores();
    }

    private void MarcarCampoModificado(string propiedad)
    {
        if (_limpiandoFormulario)
            return;

        _camposModificados.Add(propiedad);
    }

    private void NotificarErrores()
    {
        OnPropertyChanged(nameof(NombreError));
        OnPropertyChanged(nameof(CorreoError));
        OnPropertyChanged(nameof(TelefonoError));
        OnPropertyChanged(nameof(CarreraError));
        OnPropertyChanged(nameof(FechaError));
    }

    private Estudiante CrearEstudiante() => new()
    {
        Nombre = (Nombre ?? string.Empty).Trim(),
        Correo = (Correo ?? string.Empty).Trim(),
        Telefono = (Telefono ?? string.Empty).Trim(),
        Carrera = Carrera ?? string.Empty,
        Fecha = Fecha?.Date ?? default
    };

    private string ObtenerError(string propiedad) =>
        _camposModificados.Contains(propiedad) &&
        _estudianteService.Validar(CrearEstudiante()).TryGetValue(propiedad, out var error) ? error : string.Empty;
}

