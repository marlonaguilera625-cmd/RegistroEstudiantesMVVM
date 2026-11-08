using RegistroEstudianteMAUI.Services;
using RegistroEstudianteMAUI.ViewModels;

var vm = new RegistroViewModel(new EstudianteService());
var checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
    Console.WriteLine($"PASS: {name}");
    checks++;
}
void Fill()
{
    vm.Nombre = "  María López  ";
    vm.Correo = "maria@example.com";
    vm.Telefono = "9999-1234";
    vm.Carrera = vm.Carreras[2];
}
Check(vm.Fecha is null && !vm.GuardarCommand.CanExecute(null), "Inicio sin fecha y Guardar bloqueado");
bool NoErrors(RegistroViewModel form) => new[] { form.NombreError, form.CorreoError, form.TelefonoError, form.CarreraError, form.FechaError }.All(string.IsNullOrEmpty);
Check(NoErrors(vm), "A: formulario inicial sin errores visibles");
var changedErrors = new List<string>();
vm.PropertyChanged += (_, e) => { if (e.PropertyName?.EndsWith("Error") == true) changedErrors.Add(e.PropertyName); };
vm.Correo = "incorrecto";
Check(vm.CorreoError.Length > 0 && vm.NombreError == "" && vm.TelefonoError == "" && vm.CarreraError == "" && vm.FechaError == ""
    && changedErrors.SequenceEqual(new[] { nameof(vm.CorreoError) }), "B: correo inválido solo muestra y notifica su error");
vm.Correo = "maria@example.com";
Check(NoErrors(vm) && !vm.GuardarCommand.CanExecute(null), "C: corregir correo elimina su error sin habilitar Guardar");
changedErrors.Clear();
vm.Telefono = " ";
Check(vm.TelefonoError.Length > 0 && vm.NombreError == "" && vm.CorreoError == "" && vm.CarreraError == "" && vm.FechaError == ""
    && changedErrors.SequenceEqual(new[] { nameof(vm.TelefonoError) }), "D: teléfono vacío tras normalizar solo muestra su error");
vm.Telefono = "9999-1234";
Check(NoErrors(vm), "Corregir teléfono elimina su error");
changedErrors.Clear();
vm.Nombre = " ";
Check(vm.NombreError.Length > 0 && vm.CorreoError == "" && vm.TelefonoError == "" && vm.CarreraError == "" && vm.FechaError == ""
    && changedErrors.SequenceEqual(new[] { nameof(vm.NombreError) }), "E: modificar Nombre solo actualiza su error");
vm.Nombre = "María López";
changedErrors.Clear();
vm.Carrera = vm.Carreras[0];
Check(NoErrors(vm) && changedErrors.SequenceEqual(new[] { nameof(vm.CarreraError) }), "F: seleccionar Carrera solo notifica su error");
changedErrors.Clear();
vm.Fecha = DateTime.Today.AddDays(1);
Check(vm.FechaError.Length > 0 && vm.NombreError == "" && vm.CorreoError == "" && vm.TelefonoError == "" && vm.CarreraError == ""
    && changedErrors.SequenceEqual(new[] { nameof(vm.FechaError) }), "G: cambiar Fecha solo muestra y notifica su error");
vm.Fecha = DateTime.Today;
Check(NoErrors(vm) && vm.GuardarCommand.CanExecute(null), "I: todos los datos válidos habilitan Guardar");
vm.Nombre = " "; vm.Correo = "incorrecto"; vm.Telefono = " "; vm.Carrera = ""; vm.Fecha = null;
Check(new[] { vm.NombreError, vm.CorreoError, vm.TelefonoError, vm.CarreraError, vm.FechaError }.All(e => e.Length > 0), "Cada campo previamente modificado puede mostrar su propio error");
vm.LimpiarCommand.Execute(null);
Check(NoErrors(vm) && vm.Nombre == "" && vm.Correo == "" && vm.Telefono == "" && vm.Carrera == "" && vm.Fecha is null
    && !vm.GuardarCommand.CanExecute(null), "H: Limpiar borra errores y valores");
vm.Correo = "incorrecto";
Check(vm.CorreoError.Length > 0 && vm.NombreError == "" && vm.TelefonoError == "" && vm.CarreraError == "" && vm.FechaError == "", "H: después de Limpiar se restablecen los estados de interacción");
vm.LimpiarCommand.Execute(null);
vm.GuardarCommand.Execute(null);
Check(NoErrors(vm) && vm.Estudiantes.Count == 0, "Guardar invocado directamente con datos inválidos no revela campos intactos ni registra");
Fill();
Check(!vm.GuardarCommand.CanExecute(null) && vm.FechaError == "", "Fecha intacta oculta su error pero sigue siendo obligatoria");
var commandChanges = 0;
vm.GuardarCommand.CanExecuteChanged += (_, _) => commandChanges++;
vm.Fecha = DateTime.Today.AddDays(1);
Check(!vm.GuardarCommand.CanExecute(null), "Fecha futura bloqueada");
vm.Fecha = DateTime.Today;
Check(vm.GuardarCommand.CanExecute(null) && commandChanges > 0, "Fecha válida habilita Guardar y notifica CanExecute");
vm.Correo = "incorrecto";
Check(!vm.GuardarCommand.CanExecute(null) && vm.CorreoError.Length > 0, "Correo inválido bloqueado");
vm.Correo = "maria@example.com";
vm.Carrera = string.Empty;
Check(!vm.GuardarCommand.CanExecute(null), "Carrera obligatoria");
vm.Carrera = vm.Carreras[2];
var added = 0;
vm.Estudiantes.CollectionChanged += (_, _) => added++;
vm.GuardarCommand.Execute(null);
Check(vm.Estudiantes.Count == 1 && added == 1, "Guardar añade un registro y notifica la colección");
Check(vm.Estudiantes[0].Nombre == "María López" && vm.Estudiantes[0].Fecha == DateTime.Today
    && vm.Estudiantes[0].Carrera == vm.Carreras[2], "Datos del registro conservados y nombre normalizado");
Check(vm.Fecha is null && vm.Nombre == "" && vm.Carrera == "" && !vm.GuardarCommand.CanExecute(null), "Guardar limpia campos y bloquea el siguiente registro");
Check(vm.Mensaje.Contains("María López") && vm.FechaError == "", "Confirmación conservada sin errores tras guardar");
Fill(); vm.Fecha = DateTime.Today.AddYears(-18);
vm.LimpiarCommand.Execute(null);
Check(vm.Nombre == "" && vm.Correo == "" && vm.Telefono == "" && vm.Carrera == "" && vm.Fecha is null
    && vm.Mensaje == "" && vm.Estudiantes.Count == 1, "Limpiar restablece formulario sin borrar estudiantes");
Fill(); vm.Fecha = DateTime.Today;
vm.GuardarCommand.Execute(null);
Check(vm.Estudiantes.Count == 2 && added == 2, "Segundo registro actualiza colección");
// Aísla cada regla manteniendo los otros cuatro datos válidos.
Fill(); vm.Fecha = DateTime.Today;
foreach (var invalid in new[] { "23", "María23", "María@José", "", " ", "José--Luis", "\u0301José" })
{
    vm.Nombre = invalid;
    Check(vm.NombreError.Length > 0 && !vm.GuardarCommand.CanExecute(null), $"Nombre inválido: '{invalid}'");
}
foreach (var valid in new[] { "María José Aguilera", "José Núñez", "María-José", "O'Connor", "Jose\u0301", "  María  José  " })
{
    vm.Nombre = valid;
    Check(vm.NombreError == "" && vm.GuardarCommand.CanExecute(null), $"Nombre válido: '{valid}'");
}
vm.Correo = "33";
Check(vm.CorreoError.Length > 0 && !vm.GuardarCommand.CanExecute(null), "Correo 33 inválido");
vm.Correo = "usuario@gmail.com";
Check(vm.CorreoError == "" && vm.GuardarCommand.CanExecute(null), "Correo usuario@gmail.com válido");
foreach (var invalid in new[] { "rd", "9999888", "999988888", "+503 9999-8888", "9999rd88", "９９９９８８８８", "9999/8888", "", " " })
{
    vm.Telefono = invalid;
    Check(vm.TelefonoError.Length > 0 && !vm.GuardarCommand.CanExecute(null), $"Teléfono inválido: '{invalid}'");
}
foreach (var valid in new[] { "99998888", "+504 9999-8888", "+50499998888", "9999-8888", "9999 8888" })
{
    vm.Telefono = valid;
    Check(vm.TelefonoError == "" && vm.GuardarCommand.CanExecute(null), $"Teléfono válido: '{valid}'");
}
foreach (var invalid in new[] { "", "Carrera inexistente" })
{
    vm.Carrera = invalid;
    Check(vm.CarreraError.Length > 0 && !vm.GuardarCommand.CanExecute(null), "Carrera vacía o ajena al catálogo bloqueada");
}
foreach (var carrera in vm.Carreras)
{
    vm.Carrera = carrera;
    Check(vm.CarreraError == "" && vm.GuardarCommand.CanExecute(null), $"Carrera válida: {carrera}");
}
vm.Fecha = null;
Check(vm.FechaError.Length > 0 && !vm.GuardarCommand.CanExecute(null), "Fecha no seleccionada bloqueada");
vm.Fecha = DateTime.Today.AddDays(1);
Check(vm.FechaError.Length > 0 && !vm.GuardarCommand.CanExecute(null), "Fecha futura bloqueada con las nuevas reglas");
vm.Fecha = DateTime.Today;
Check(NoErrors(vm) && vm.GuardarCommand.CanExecute(null), "Fecha seleccionada válida y formulario completo habilitado");
vm.LimpiarCommand.Execute(null);
vm.Nombre = "23";
Check(vm.NombreError.Length > 0 && vm.CorreoError == "" && vm.TelefonoError == "" && vm.CarreraError == "" && vm.FechaError == "", "Nombre numérico solo muestra su propia advertencia");
vm.LimpiarCommand.Execute(null);
vm.Telefono = "rd";
Check(vm.TelefonoError.Length > 0 && vm.NombreError == "" && vm.CorreoError == "" && vm.CarreraError == "" && vm.FechaError == "", "Teléfono con letras solo muestra su propia advertencia");
vm.LimpiarCommand.Execute(null);
Check(NoErrors(vm) && !vm.GuardarCommand.CanExecute(null), "Limpiar restablece advertencias y Guardar tras las nuevas validaciones");
Console.WriteLine($"{checks} comprobaciones funcionales correctas. No prueban la interfaz nativa.");
