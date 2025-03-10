public class ScreenShot
{
    public static string powershellScriptPrintScreen = @"
Add-Type -AssemblyName System.Drawing

$savePath = 'C:\CapturasDeTela'

if (-not (Test-Path -Path $savePath)) {
    New-Item -ItemType Directory -Path $savePath
}

$screens = [System.Windows.Forms.Screen]::AllScreens
$totalWidth = 0
$maxHeight = 0

foreach ($screen in $screens) {
    $bounds = $screen.Bounds
    $totalWidth += $bounds.Width
    if ($bounds.Height -gt $maxHeight) {
        $maxHeight = $bounds.Height
    }
}

$bitmap = New-Object System.Drawing.Bitmap($totalWidth, $maxHeight)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.Clear([System.Drawing.Color]::Black)

$offsetX = 0
foreach ($screen in $screens) {
    $bounds = $screen.Bounds

    $screenBitmap = New-Object System.Drawing.Bitmap($bounds.Width, $bounds.Height)
    $screenGraphics = [System.Drawing.Graphics]::FromImage($screenBitmap)

    $screenGraphics.CopyFromScreen($bounds.X, $bounds.Y, 0, 0, $bounds.Size)
    $graphics.DrawImage($screenBitmap, $offsetX, 0)

    $offsetX += $bounds.Width

    $screenGraphics.Dispose()
    $screenBitmap.Dispose()
}

$fileName = Join-Path $savePath 'Captura_Combinada.png'
$bitmap.Save($fileName, [System.Drawing.Imaging.ImageFormat]::Png)
$graphics.Dispose()
$bitmap.Dispose()

Write-Host 'Captura de tela combinada salva em:' $fileName
";

}