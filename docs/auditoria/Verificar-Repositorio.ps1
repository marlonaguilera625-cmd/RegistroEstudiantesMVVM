$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..'))
$documents = @('README.md', 'EVIDENCIAS-PARA-INFORME.md', 'docs/auditoria/FINAL.md',
    'docs/auditoria/README.md', 'docs/auditoria/INFORME.md')
$linkCount = 0
foreach ($document in $documents) {
    $documentPath = Join-Path $repoRoot $document
    $markdown = [IO.File]::ReadAllText($documentPath)
    foreach ($match in [regex]::Matches($markdown, '!?\[[^\]]*\]\(([^)]+)\)')) {
        $target = $match.Groups[1].Value
        if ($target -match '^https?://') { continue }
        $parts = $target -split '#', 2
        $targetPath = if ($parts[0]) {
            [IO.Path]::GetFullPath((Join-Path (Split-Path $documentPath) $parts[0]))
        } else { $documentPath }
        if (!(Test-Path -LiteralPath $targetPath)) { throw "Enlace inexistente: $document -> $target" }
        if ($parts.Count -eq 2 -and $parts[1]) {
            $targetText = [IO.File]::ReadAllText($targetPath)
            $anchors = @([regex]::Matches($targetText, '(?m)^#{1,6} +(.+?)\r?$') | ForEach-Object {
                $heading = $_.Groups[1].Value.ToLowerInvariant()
                [regex]::Replace($heading, '[^\p{L}\p{N}_ -]', '').Replace(' ', '-')
            })
            if ($anchors -notcontains $parts[1]) { throw "Ancla inexistente: $document -> $target" }
        }
        $linkCount++
    }
}
Write-Output "PASS: $linkCount enlaces relativos y sus anclas existen."

Add-Type -AssemblyName System.Drawing
foreach ($name in @('01-formulario.png', '02-lista-estudiantes.png')) {
    $path = Join-Path $repoRoot "docs/capturas/$name"
    $bytes = [IO.File]::ReadAllBytes($path)
    if ([BitConverter]::ToString($bytes, 0, 8) -ne '89-50-4E-47-0D-0A-1A-0A') { throw "PNG inválido: $name" }
    $picture = [Drawing.Image]::FromFile($path)
    try {
        Write-Output "PASS: $name decodificado, $($picture.Width)x$($picture.Height), SHA256=$((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash)"
    } finally { $picture.Dispose() }
}

$xmlFiles = @(Get-ChildItem (Join-Path $repoRoot 'RegistroEstudianteMAUI') -Recurse -File |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' -and $_.Extension -in @('.xaml', '.csproj', '.xml', '.appxmanifest', '.manifest', '.svg', '.plist') })
$xmlFiles += Get-Item (Join-Path $repoRoot 'RegistroEstudianteMAUI.slnx')
$xmlFiles += Get-Item (Join-Path $repoRoot 'tests/RegistroEstudianteMAUI.FunctionalTests/RegistroEstudianteMAUI.FunctionalTests.csproj')
foreach ($file in $xmlFiles) { $null = [xml][IO.File]::ReadAllText($file.FullName) }
Write-Output "PASS: $($xmlFiles.Count) archivos XML/XAML/proyecto bien formados."

$readme = [IO.File]::ReadAllText((Join-Path $repoRoot 'README.md'))
$block = [regex]::Match($readme, '(?s)```mermaid\r?\n(.*?)```')
if (!$block.Success) { throw 'Falta el bloque Mermaid' }
$lines = @($block.Groups[1].Value.Trim() -split '\r?\n')
if ($lines[0] -ne 'flowchart LR') { throw 'Cabecera Mermaid inesperada' }
$declared = @{}
$referenced = @()
foreach ($line in $lines[1..($lines.Count - 1)]) {
    # Comprueba el subconjunto usado: nodos con etiquetas entre comillas y aristas etiquetadas.
    $edge = [regex]::Match($line, '^ +([A-Z]+)(?:\["([^"\r\n]+)"\])? -->\|([^|\r\n]+)\| ([A-Z]+)(?:\["([^"\r\n]+)"\])?$')
    if (!$edge.Success) { throw "Línea fuera del subconjunto Mermaid comprobado: $line" }
    $referenced += $edge.Groups[1].Value, $edge.Groups[4].Value
    if ($edge.Groups[2].Success) { $declared[$edge.Groups[1].Value] = $edge.Groups[2].Value }
    if ($edge.Groups[5].Success) { $declared[$edge.Groups[4].Value] = $edge.Groups[5].Value }
}
foreach ($node in $referenced) { if (!$declared.ContainsKey($node)) { throw "Nodo Mermaid sin definición: $node" } }
Write-Output "PASS: diagrama con $($declared.Count) nodos y $($lines.Count - 1) aristas coherentes en el subconjunto comprobado; no se ejecuta el renderizador GitHub/Mermaid."

$ignore = [IO.File]::ReadAllText((Join-Path $repoRoot '.gitignore'))
foreach ($pattern in @('.vs/', '**/bin/', '**/obj/', '*.user', 'TestResults/')) {
    if (($ignore -split '\r?\n') -notcontains $pattern) { throw "Exclusión esperada ausente: $pattern" }
}
Write-Output 'PASS: exclusiones de herramientas, compilación y pruebas presentes en .gitignore.'
