# Documentación de auditoría

- [FINAL.md](FINAL.md): informe vigente, matriz académica, verificaciones y pendientes de publicación.
- `final-windows.log`, `final-android.log` y `final-pruebas.log`: resultados ejecutados en la revisión final.
- [Verificar-Repositorio.ps1](Verificar-Repositorio.ps1): comprobación local de enlaces Markdown a archivos, PNG y XML; no compila ni ejecuta la aplicación.
- [INFORME.md](INFORME.md): informe histórico de la corrección inicial de interfaz.

Los archivos `*.original.*.txt`, los árboles `ui-*.txt`, `picker-native.txt`, los logs sin prefijo `final-` y las capturas `windows-*.png` se conservan como historial técnico. No representan necesariamente las validaciones ni el estado final del formulario. Las dos capturas oficiales se encuentran en [docs/capturas](../capturas).

`Inspect-Windows.ps1` es una utilidad de inspección de una aplicación Windows que ya está ejecutándose. Requiere `-ApplicationProcessId` para evitar reutilizar un PID histórico. No inicia procesos; cambia el tamaño de esa ventana y escribe la captura y el árbol de automatización en esta carpeta. No debe usarse para sustituir las capturas oficiales.
