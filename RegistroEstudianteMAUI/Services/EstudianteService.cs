using System.Net.Mail;
using System.Text.RegularExpressions;
using RegistroEstudianteMAUI.Models;

namespace RegistroEstudianteMAUI.Services;

public class EstudianteService : IEstudianteService
{
    public IReadOnlyList<string> Carreras { get; } = Array.AsReadOnly<string>(
        ["Administración de Empresas", "Derecho", "Ingeniería en Sistemas", "Medicina", "Psicología"]);

    private static readonly Regex FormatoNombre = new(
        @"\A\p{L}[\p{L}\p{M}]*(?:(?: +|[-'’])\p{L}[\p{L}\p{M}]*)*\z",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    private static readonly Regex FormatoTelefono = new(
        @"\A(?:\+504[ -]?)?[0-9]{4}[ -]?[0-9]{4}\z",
        RegexOptions.CultureInvariant | RegexOptions.NonBacktracking);

    public IReadOnlyDictionary<string, string> Validar(Estudiante estudiante)
    {
        var errores = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(estudiante.Nombre))
            errores[nameof(estudiante.Nombre)] = "El nombre es obligatorio.";
        else if (!FormatoNombre.IsMatch(estudiante.Nombre.Trim()))
            errores[nameof(estudiante.Nombre)] = "Introduce un nombre válido con letras, sin números ni símbolos inapropiados.";

        if (!CorreoValido(estudiante.Correo))
            errores[nameof(estudiante.Correo)] = "Introduce un correo electrónico válido.";

        if (string.IsNullOrWhiteSpace(estudiante.Telefono))
            errores[nameof(estudiante.Telefono)] = "El teléfono es obligatorio.";
        else if (!FormatoTelefono.IsMatch(estudiante.Telefono.Trim()))
            errores[nameof(estudiante.Telefono)] = "Introduce un teléfono de ocho dígitos, opcionalmente con el prefijo +504.";

        if (!Carreras.Contains(estudiante.Carrera, StringComparer.Ordinal))
            errores[nameof(estudiante.Carrera)] = "Selecciona una carrera.";

        if (estudiante.Fecha == default || estudiante.Fecha.Date > DateTime.Today)
            errores[nameof(estudiante.Fecha)] = "Selecciona una fecha válida que no sea futura.";

        return errores;
    }

    private static bool CorreoValido(string? correo)
    {
        if (string.IsNullOrWhiteSpace(correo))
            return false;

        try
        {
            var limpio = correo.Trim();
            return new MailAddress(limpio).Address == limpio;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}

