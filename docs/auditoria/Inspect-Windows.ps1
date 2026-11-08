param([Parameter(Mandatory = $true)][int]$ApplicationProcessId,
      [string]$Capture = 'windows-desktop.png', [int]$Width = 1100, [int]$Height = 1050)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName System.Drawing
Add-Type @'
using System;
using System.Runtime.InteropServices;
public class AuditCapture {
 [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdc, uint flags);
}
'@
$app = Get-Process -Id $ApplicationProcessId
$root = [System.Windows.Automation.AutomationElement]::FromHandle($app.MainWindowHandle)
$transform = [System.Windows.Automation.TransformPattern]$root.GetCurrentPattern([System.Windows.Automation.TransformPattern]::Pattern)
$transform.Resize($Width, $Height)
$rect = $root.Current.BoundingRectangle
$bitmap = New-Object Drawing.Bitmap ([int]$rect.Width), ([int]$rect.Height)
$graphics = [Drawing.Graphics]::FromImage($bitmap)
$dc = $graphics.GetHdc()
$ok = [AuditCapture]::PrintWindow($app.MainWindowHandle, $dc, 2)
$graphics.ReleaseHdc($dc)
$bitmap.Save((Join-Path $PSScriptRoot $Capture))
$graphics.Dispose()
$bitmap.Dispose()
if (!$ok) { throw 'PrintWindow failed' }
$root.FindAll([System.Windows.Automation.TreeScope]::Descendants, [System.Windows.Automation.Condition]::TrueCondition) |
    ForEach-Object { '{0}|{1}|enabled={2}|{3}' -f $_.Current.ControlType.ProgrammaticName, $_.Current.Name, $_.Current.IsEnabled, $_.Current.BoundingRectangle } |
    Set-Content (Join-Path $PSScriptRoot 'ui-tree.txt')
