# Evidencias para la Presentación 5 — Arquitectura MVVM

Esta guía corresponde a la implementación actual. La descripción funcional y las instrucciones están en el [README principal](README.md); la matriz y los resultados reproducibles están en el [informe final](docs/auditoria/FINAL.md).

## Evidencias del código

| Aspecto | Archivo y evidencia |
|---|---|
| Modelo | `RegistroEstudianteMAUI/Models/Estudiante.cs`: Nombre, Correo, Telefono, Carrera y Fecha. |
| Propiedades observables | `RegistroEstudianteMAUI/ViewModels/RegistroViewModel.cs`: ObservableObject, ObservableProperty y notificaciones por campo. |
| Comando y validación | RegistroViewModel.PuedeGuardar y Guardar; `RegistroEstudianteMAUI/Services/EstudianteService.cs`: reglas centralizadas. |
| Bindings compilados | `RegistroEstudianteMAUI/Views/RegistroPage.xaml`: x:DataType en ContentPage y DataTemplate. |
| Lista reactiva | RegistroViewModel.Estudiantes y CollectionView.ItemsSource en RegistroPage.xaml. |
| DI | `RegistroEstudianteMAUI/MauiProgram.cs`: registros del servicio, ViewModel y página; `App.xaml.cs`: resolución de la página. |
| Separación de responsabilidades | `RegistroEstudianteMAUI/Views/RegistroPage.xaml.cs`: inicialización y ajustes de presentación nativa, sin validaciones ni registro. |

## Capturas existentes

- [Formulario de registro](docs/capturas/01-formulario.png): campos, placeholders, fecha sin selección y Guardar deshabilitado.
- [Lista de estudiantes](docs/capturas/02-lista-estudiantes.png): confirmación y tarjeta con los cinco datos del estudiante.

Ambas imágenes fueron abiertas e inspeccionadas en la auditoría final. Se conservan sus bytes originales. Las imágenes contienen la barra de herramientas de depuración y los datos introducidos en la ejecución; no se editan para la entrega.

## Alcance de las pruebas

Las 67 comprobaciones de consola verifican el servicio y el ViewModel, incluidas las notificaciones de la colección. No equivalen a pruebas visuales de CollectionView ni a una ejecución Android. Los logs finales están identificados en el informe. Las evidencias históricas en docs/auditoria describen intervenciones anteriores y no sustituyen los resultados finales.

No se documentan género, términos, SQLite ni confirmaciones DisplayAlert: esas funcionalidades no forman parte del flujo actual. Los estudiantes permanecen en memoria y se pierden al cerrar.