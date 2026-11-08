using RegistroEstudianteMAUI.Models;

namespace RegistroEstudianteMAUI.Services;

public interface IEstudianteService
{
    IReadOnlyList<string> Carreras { get; }
    IReadOnlyDictionary<string, string> Validar(Estudiante estudiante);
}

