# Auditoría final — Presentación 5: Arquitectura MVVM

Fecha: 8 de noviembre de 2026. Ámbito: carpeta local RegistroEstudiantesMVVM. Se priorizaron documentación y organización; no se modificaron fuentes de la aplicación ni de las pruebas, recursos, configuraciones de proyecto o solución, validaciones, comandos, navegación o DI.

## Archivos revisados

- Models/Estudiante.cs; ViewModels/RegistroViewModel.cs; Services/IEstudianteService.cs y EstudianteService.cs.
- Views/RegistroPage.xaml y su code-behind; App.xaml y App.xaml.cs; MauiProgram.cs; MainPage y AppShell.
- RegistroEstudianteMAUI.csproj, solución .slnx, Properties/launchSettings.json, Resources y configuración de Platforms.
- README raíz, EVIDENCIAS-PARA-INFORME.md, documentación y artefactos de docs/auditoria, README e imágenes de docs/capturas.
- Proyecto independiente de pruebas, Program.cs y .gitignore.

## Cambios de documentación y organización

| Archivo | Cambio |
|---|---|
| README.md | Descripción académica actual, estructura real, responsabilidades MVVM, observabilidad, bindings, comandos, reglas, DI, comandos de compilación/ejecución, Mermaid y dos imágenes relativas. |
| EVIDENCIAS-PARA-INFORME.md | Sustituye referencias obsoletas a género, términos, DisplayAlert, validaciones en code-behind y arranque con AppShell por evidencias de la implementación actual. |
| docs/auditoria/README.md | Índice que distingue resultados finales y archivos históricos. |
| docs/auditoria/INFORME.md | Aviso de documento histórico y enlace al estado vigente; se conserva el informe original. |
| docs/auditoria/Inspect-Windows.ps1 | Exige ApplicationProcessId explícito; elimina dependencia de un identificador de proceso de una sesión anterior, que podía ser reutilizado por otro proceso. No es código de la aplicación. |
| docs/auditoria/Verificar-Repositorio.ps1 | Verificación reproducible de enlaces, anclas, PNG, XML, exclusiones y del subconjunto de sintaxis Mermaid usado. |
| docs/auditoria/FINAL.md | Informe vigente y matriz académica. |
| docs/auditoria/final-*.log | Logs de compilaciones Windows/Android, pruebas y verificación documental finales. |

Eliminaciones concretas, sin borrado recursivo:

- `docs/capturas/README.md`: correspondía a Presentación 3, afirmaba que aún no había imágenes y enumeraba nombres inexistentes. La información útil de conservar evidencias reales se incorporó al README principal y a la guía de evidencias.
- `docs/auditoria/runtime-pid.txt`: PID temporal de una ejecución terminada; no era configuración ni evidencia académica. La utilidad de inspección ya no lo necesita.

No se eliminaron capturas, respaldos originales ni fuentes. .gitignore se conserva: ya excluye .vs, bin, obj, archivos de usuario y TestResults. Las carpetas de compilación y .vs pueden existir localmente, pero no deben incluirse en una publicación.

## Matriz de cumplimiento académico

| N.º | Requisito | Estado | Evidencia concreta |
|---|---|---|---|
| 1 | Carpetas Models, ViewModels, Views y Services | CUMPLE | Las cuatro existen bajo RegistroEstudianteMAUI y contienen sus fuentes correspondientes. |
| 2 | Modelo con nombre, correo, teléfono, carrera y fecha | CUMPLE | [Estudiante.cs](../../RegistroEstudianteMAUI/Models/Estudiante.cs), líneas 5–9: cinco propiedades públicas. |
| 3 | RegistroViewModel con observabilidad de CommunityToolkit.Mvvm | CUMPLE | [RegistroViewModel.cs](../../RegistroEstudianteMAUI/ViewModels/RegistroViewModel.cs): ObservableObject, seis ObservableProperty y atributos de notificación de errores y CanExecute. |
| 4 | Bindings compilados mediante x:DataType | CUMPLE | [RegistroPage.xaml](../../RegistroEstudianteMAUI/Views/RegistroPage.xaml), líneas 7 y 110: tipos de ViewModel y Estudiante. Compilación con SourceGen habilitado en el proyecto. |
| 5 | GuardarCommand condicionado a datos válidos | CUMPLE | RegistroViewModel.cs, líneas 67 y 69: PuedeGuardar consulta el servicio y RelayCommand declara CanExecute. Guardar vuelve a validar antes de agregar. |
| 6 | ObservableCollection presentada por CollectionView | CUMPLE | RegistroViewModel.cs, línea 53: Estudiantes observable. RegistroPage.xaml, línea 107: CollectionView.ItemsSource enlazado a Estudiantes. Guardar agrega en la línea 80. |
| 7 | DI de página y ViewModel; code-behind sin negocio | CUMPLE | [MauiProgram.cs](../../RegistroEstudianteMAUI/MauiProgram.cs), líneas 22–24: servicio singleton, ViewModel y página transient. [App.xaml.cs](../../RegistroEstudianteMAUI/App.xaml.cs), línea 14: resuelve la página. [RegistroPage.xaml.cs](../../RegistroEstudianteMAUI/Views/RegistroPage.xaml.cs): inicialización y ajustes nativos de presentación, sin validación ni registro. |
| 8 | Diagrama MVVM en README | CUMPLE | [README principal](../../README.md): bloque flowchart LR con App, DI, View, ViewModel, Service, Model, colección y CollectionView. |
| 9 | Dos capturas reales incluidas en README | CUMPLE | README incorpora exactamente las rutas docs/capturas/01-formulario.png y docs/capturas/02-lista-estudiantes.png. Ambas se abrieron y decodificaron. |

La comprobación se realiza contra los siete requisitos proporcionados en la solicitud; no se recibió una rúbrica externa adicional con otros criterios.

## Capturas e integridad

Las dos imágenes oficiales se abrieron con la herramienta de inspección visual y se verificaron como PNG decodificables:

- [01-formulario.png](../capturas/01-formulario.png): 1365 × 698; formulario inicial, placeholders y Guardar deshabilitado.
- [02-lista-estudiantes.png](../capturas/02-lista-estudiantes.png): 1365 × 716; confirmación y tarjeta del estudiante con nombre, correo, teléfono, carrera y fecha.

No se generaron, editaron ni reemplazaron imágenes. Los hashes SHA256 antes y después coinciden. Las capturas contienen la barra de depuración y los datos introducidos en aquella ejecución; se conservaron tal como fueron proporcionadas. La verificación visual comprueba su contenido, no establece por sí sola una cadena externa de autenticidad.

Se compararon también hashes de todos los archivos fuente, recursos y pruebas excluyendo bin/obj: sin cambios respecto al inicio de esta intervención. Se comprobó XML bien formado y existencia de enlaces relativos, incluidas las anclas utilizadas.

## Comprobaciones ejecutadas

Entorno: SDK .NET 10.0.302, MAUI resuelto 10.0.20, CommunityToolkit.Mvvm 8.4.0, workloads Windows y Android instalados. Desde la raíz:

```powershell
dotnet build RegistroEstudianteMAUI/RegistroEstudianteMAUI.csproj -f net10.0-windows10.0.19041.0 --no-restore
dotnet build RegistroEstudianteMAUI/RegistroEstudianteMAUI.csproj -f net10.0-android --no-restore
dotnet run --project tests/RegistroEstudianteMAUI.FunctionalTests --no-restore
powershell -NoProfile -ExecutionPolicy Bypass -File docs/auditoria/Verificar-Repositorio.ps1
git status --short --branch
git rev-parse --show-toplevel
```

El parámetro ExecutionPolicy del proceso de PowerShell permite ejecutar el script local de comprobación en este entorno; no cambia la configuración persistente del equipo.

| Comprobación | Resultado | Evidencia |
|---|---|---|
| Windows Debug | 0 errores; 6 advertencias MVVMTK0045 ya existentes | [final-windows.log](final-windows.log) |
| Android Debug | 0 errores; 0 advertencias | [final-android.log](final-android.log) |
| Pruebas de servicio y ViewModel | 67 comprobaciones correctas | [final-pruebas.log](final-pruebas.log) |
| Enlaces relativos y anclas | Destinos existentes; comprobación automatizada | [final-documentacion.log](final-documentacion.log) |
| Imágenes y XML | PNG decodificables, hashes registrados y XML bien formado | final-documentacion.log |
| Mermaid | Revisión de coherencia con el código y verificación del subconjunto usado: nodos con etiquetas entre comillas y aristas etiquetadas; todas las referencias están definidas | Verificar-Repositorio.ps1 y final-documentacion.log |
| Ejecución Windows | Ejecutable iniciado; ventana y encabezado real localizados mediante UI Automation; cierre normal solicitado | Comprobación de arranque en esta sesión |
| Estado Git | Ambos comandos fallan: «not a git repository»; no existe .git en esta carpeta | Inspección local y ejecución de comandos |

La compilación usó las dependencias existentes; no demuestra una restauración completa desde un equipo nuevo. Las pruebas emiten NU1900 porque no se puede consultar el índice de vulnerabilidades remoto; esto no impidió ejecutar las 67 comprobaciones. Las advertencias de Windows se refieren a compatibilidad AOT/WinRT de los campos observables; no se ocultaron ni se cambiaron paquetes o lenguaje.

El diagrama utiliza sintaxis básica de flowchart compatible con GitHub. No se ejecutó el motor Mermaid ni un renderizador remoto de GitHub; la comprobación automatizada cubre exclusivamente el subconjunto escrito, además de la revisión manual de su significado.

En esta auditoría se verificó el arranque Windows, no se repitió una prueba interactiva exhaustiva del formulario. Las pruebas de consola no prueban controles nativos. No se ejecutó Android ni se probaron iOS o Mac Catalyst.

## Hallazgos conservados y pendientes

1. **Git local pendiente:** no existe .git, por lo que no se pueden verificar rama, archivos rastreados, historial o remotos. No se ejecutaron git init, commit, operaciones remotas ni git push. Es necesario inicializar o recuperar el repositorio local y revisar el primer conjunto de archivos antes de publicar.
2. MainPage y AppShell no participan en App.CreateWindow. Se conservaron para no modificar código funcional o navegación. MainPage referencia Headline y SubHeadline, que no aparecen definidos en los diccionarios actuales; es una referencia latente de la plantilla inactiva, no un fallo reproducido de RegistroPage. No debe activarse esa página sin revisarla. Se informa sin corregir, conforme a la restricción del usuario.
3. Los estilos de RadioButton y CheckBox y el recurso dotnet_bot provienen de la plantilla; no justifican alterar Resources durante una entrega de documentación.
4. El ejecutable de pruebas está fuera de la solución .slnx: se documenta el comando específico y no se altera la solución para incorporarlo.
5. Los artefactos previos de docs/auditoria se conservan como historia identificada, sin presentarlos como resultados actuales. El nombre del proyecto es RegistroEstudianteMAUI (singular); el de la carpeta raíz es RegistroEstudiantesMVVM.
6. Quedan pendientes una restauración desde un entorno limpio, ejecución Android y revisión del diagrama en el renderizador de GitHub. No se necesita un dispositivo para la compilación Android ya realizada.

La preparación académica y documental está verificada. La publicación queda pendiente de la configuración local de Git y de la autorización posterior para cualquier publicación remota.