# Auditoría técnica de RegistroEstudianteMAUI

> Documento histórico de la primera intervención de interfaz. Sus reglas, recuentos de pruebas y archivos originales corresponden a aquel momento. Para el estado actual y la entrega académica, consulte [FINAL.md](FINAL.md) y el [README principal](../../README.md). El identificador temporal del proceso de aquella sesión se retiró; Inspect-Windows.ps1 ahora exige un identificador explícito.

Fecha: 8 de noviembre de 2026. SDK: .NET 10.0.302; MAUI: 10.0.20; CommunityToolkit.Mvvm: 8.4.0; Windows App SDK: 1.7.250909003.

## 1. Diagnóstico y causas

| Área | Evidencia del código o ejecución | Causa y corrección |
|---|---|---|
| Fecha | DatePicker con fecha inicial de hace 18 años, cubierto parcialmente por un Label opaco con margen fijo. El servicio aceptaba esa fecha incluso sin selección. | El supuesto placeholder ocultaba parte de una fecha real. Se retiraron Grid y Label superpuestos, FechaSeleccionada y FechaSinSeleccionar. Fecha es DateTime? inicialmente nula; se enlaza en ambos sentidos. En Windows se usa CalendarDatePicker.PlaceholderText. |
| Carrera | La ejecución y el árbol visual nativo mostraron HeaderContentPresenter con 21 px de contenido vacío y 8 px de margen: una fila de 29 px dentro del control de 56 px. El fondo de selección quedaba reducido a 27 px. Véase picker-native.txt. | El encabezado y su plantilla nativa reservaban espacio encima de la selección. Se eliminan HeaderTemplate y Header en esta instancia y se usa PlaceholderText. Se conserva el ComboBox y su indicador nativo, centrando su contenido y manteniendo los 56 px. No se modifica el recurso ni el glifo interno. |
| Entradas | Cada control estaba dentro de FieldFrame; mappings globales quitaban bordes y fondos nativos. | Existían dos responsables del marco y se intervenía globalmente en el aspecto nativo. Se retiraron los contenedores de los cinco campos y los mappings globales. Se conserva el borde nativo con sus estados de foco y el fondo claro del proyecto. Las tarjetas siguen usando FieldFrame. |
| Adaptación | OnIdiom distingue dispositivos, pero no cambia al estrechar Windows. | Se añadieron estados adaptables: dos columnas desde 800 px y una por debajo. Se mantienen los porcentajes exteriores, máximo de 1280 px y separación aprobados. |
| Guardar deshabilitado | La primera captura mostraba el mismo azul que un botón habilitado pese a CanExecute=false. | Se añadió un estado Disabled con colores diferenciados, sin cambiar la distribución de botones. |
| Lista | CollectionView estaba dentro de una pila y un ScrollView sin límite explícito de altura. | Se acotó a 360 px para proporcionar un área desplazable finita y evitar medir una lista creciente con altura indefinida. Se conserva CollectionView, su plantilla y ObservableCollection. |

Los archivos originales contienen acentos UTF-8 correctos. La lectura de PowerShell con codificación predeterminada podía mostrarlos mal; se verificaron mediante lectura UTF-8. No se sustituyeron acentos por caracteres artificiales. El formato de fecha final es el estándar corto `d`, no una cadena usada para simular un placeholder. El modelo persistido en memoria conserva DateTime; una selección nula se convierte a default al validar, que el servicio ya rechaza.

## 2. Archivos modificados

- RegistroEstudianteMAUI/MauiProgram.cs: eliminación de mappings globales; se conserva DI.
- RegistroEstudianteMAUI/Views/RegistroPage.xaml: eliminación de superposiciones y marcos redundantes, estados adaptables, descripciones accesibles, altura acotada de lista.
- RegistroEstudianteMAUI/Views/RegistroPage.xaml.cs: ajustes exclusivamente de presentación de las instancias nativas de Windows. HandlerChanged contempla la creación/recreación del handler; Loaded de Carrera aplica la configuración después de la inicialización de la plantilla. No contiene validación ni registro de estudiantes.
- RegistroEstudianteMAUI/ViewModels/RegistroViewModel.cs: fecha nullable, retirada del estado de selección artificial y normalización tolerante a null de entradas al construir el modelo.
- RegistroEstudianteMAUI/Resources/Styles/Styles.xaml: fondo y alineación de campos, estado deshabilitado de Guardar.
- tests/RegistroEstudianteMAUI.FunctionalTests/: ejecutable independiente que compila los mismos fuentes de modelo, servicio y ViewModel; no incorpora otra implementación de negocio ni dependencias de pruebas nuevas.
- docs/auditoria/: informe, logs, copias de los cuatro archivos originales afectados con mayor alcance, diagnóstico del árbol nativo, capturas y utilidad de inspección.

No se editaron archivos generados ni se añadieron servicios externos, persistencia o autenticación. No hay repositorio .git en esta carpeta; las copias originales permiten revisar los cambios.

## 3. Compilación final

| Destino | Resultado | Evidencia |
|---|---|---|
| net10.0-windows10.0.19041.0, Debug | 0 errores; 6 advertencias MVVMTK0045 | build-windows.log |
| net10.0-android, Debug | 0 errores; 0 advertencias | build-android-final.log |

Se usó `dotnet build RegistroEstudianteMAUI/RegistroEstudianteMAUI.csproj -f <destino> --no-restore`, con dependencias disponibles en caché. Las advertencias de Windows corresponden a campos ObservableProperty y compatibilidad AOT/WinRT. Se mantiene el patrón soportado por el proyecto y el paquete instalado: la comprobación con propiedades parciales no generó implementación con esta configuración y se descartó. No se habilitó lenguaje preview, no se actualizaron paquetes y no se ocultaron advertencias. La compilación final usa campos observables.

El proyecto de pruebas se restauró desde la caché. NuGet emitió NU1900 al no poder consultar el índice remoto de vulnerabilidades; las pruebas compilaron y se ejecutaron. No se afirma haber realizado una auditoría de dependencias en línea.

## 4. Pruebas realmente ejecutadas

### ViewModel y servicio

`dotnet run --project tests/RegistroEstudianteMAUI.FunctionalTests --no-restore`

12 comprobaciones correctas, registradas en pruebas-funcionales.log:

1. Inicio sin fecha y Guardar bloqueado.
2. Fecha obligatoria sin selección implícita.
3. Fecha futura bloqueada.
4. Fecha válida habilita Guardar y notifica CanExecuteChanged.
5. Correo inválido bloqueado.
6. Carrera obligatoria.
7. Guardar añade un registro y emite CollectionChanged.
8. Datos conservados y nombre normalizado.
9. Guardar limpia campos y bloquea el siguiente registro.
10. Confirmación conservada y errores vacíos después de guardar.
11. Limpiar restablece formulario sin borrar estudiantes.
12. Segundo registro actualiza la colección.

### Aplicación Windows real

Se inició el ejecutable compilado y se inspeccionó mediante UI Automation y capturas PrintWindow. No se usaron imágenes simuladas.

- Inicialización real de página, DI, bindings compilados y controles, sin excepción observada durante la sesión.
- Captura del formulario: fecha limpia y sin etiqueta superpuesta; un solo marco en entradas, cursor y foco visibles; Carrera con campo completo y flecha centrada.
- Selección real de Ingeniería en Sistemas mediante SelectionItemPattern del ComboBox.
- Introducción de nombre, teléfono y correo mediante ValuePattern de los Entry. Se verificaron sus valores y desaparición del placeholder de nombre en la captura windows-filled.png.
- Apertura real del calendario y selección mediante SelectionItemPattern. La selección futura 08/12/2026 dejó Guardar deshabilitado y mostró error; la selección 01/11/2026 lo habilitó.
- Activación de Guardar mediante InvokePattern: confirmación «Registro guardado para María López.», campos y fecha vacíos, Guardar nuevamente deshabilitado y tarjeta con nombre, correo, teléfono, carrera y fecha en CollectionView.
- Desplazamiento del ScrollView hasta la lista para revisar la tarjeta.
- Introducción de otro nombre y activación de Limpiar: los tres Entry quedaron vacíos y el estudiante guardado siguió presente.
- Cambio de ventana entre 1100 y 520 px: dos columnas y una columna respectivamente, sin superposición observada.

Evidencias: windows-desktop.png, windows-filled.png, windows-saved.png, windows-list.png, windows-narrow.png, ui-functional.log, ui-tree.txt. Las capturas muestran el área que PrintWindow expone en este escritorio; algunas tarjetas y zonas inferiores requieren desplazamiento y no aparecen completas en una única imagen.

## 5. Pendientes y límites

- Android compilado, pero no ejecutado: no se verificaron calendario, teclado, foco, lista ni accesibilidad en emulador o dispositivo. La consulta inicial de adb falló intentando crear `\\.android` por permisos. No se alteró la configuración externa del SDK ni se instalaron emuladores.
- No se probaron lector de pantalla, escalas de texto extremas, todos los temas, orientación horizontal en teléfono ni grandes cantidades de registros.
- Las seis advertencias AOT/WinRT requieren una revisión separada si se desea publicar con AOT; no son errores de compilación.
- No se dispone de una traza de la excepción no controlada histórica. No reapareció durante esta ejecución; esto no demuestra ausencia de cualquier excepción futura.
- La configuración nativa es local a dos controles y está protegida por WINDOWS y comprobación del tipo real. La representación nativa debe revisarse al actualizar MAUI/WinUI.

## 6. Arquitectura y composición conservadas

Se conservan Models, ViewModels, Views y Services; modelo con los cinco datos; CommunityToolkit.Mvvm, ObservableProperty, RelayCommand y CanExecute; x:DataType en página y plantilla; ObservableCollection enlazada a CollectionView; registro de servicio, ViewModel y página mediante DI; y code-behind sin negocio.

Se conservan paleta azul y fondo claro, encabezado centrado y sus tamaños, formulario amplio, porcentajes exteriores, máximo de 1280 px, columnas equilibradas, altura general de 56 px, botones en la misma fila y sección de estudiantes. Los cambios internos restauran el marco nativo, eliminan el encabezado interno redundante de Carrera y permiten una columna en ventanas estrechas.

Referencia de compatibilidad: [DatePicker de .NET MAUI 10: fecha nullable](https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/datepicker?view=net-maui-10.0). Para la causa de Carrera se inspeccionó además la plantilla instalada `Microsoft.WindowsAppSDK/1.7.250909003/lib/net6.0-windows10.0.17763.0/Microsoft.WinUI/Themes/generic.xaml` y se midió el árbol visual de la aplicación real.
