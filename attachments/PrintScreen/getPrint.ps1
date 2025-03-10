param(
    [string]$savePath
)

# Importa a biblioteca de desenho do .NET
Add-Type -AssemblyName System.Drawing

# Certifica-se de que o diretório existe
if (-not (Test-Path -Path $savePath)) {
    New-Item -ItemType Directory -Path $savePath
}

# Obtém as dimensões de cada monitor
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

# Cria um bitmap para armazenar a captura de tela combinada
$bitmap = New-Object System.Drawing.Bitmap($totalWidth, $maxHeight)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)

# Preenche o fundo com uma cor sólida (opcional)
$graphics.Clear([System.Drawing.Color]::Black)

# Captura cada tela e a coloca no bitmap combinado
$offsetX = 0
foreach ($screen in $screens) {
    $bounds = $screen.Bounds

    $screenBitmap = New-Object System.Drawing.Bitmap($bounds.Width, $bounds.Height)
    $screenGraphics = [System.Drawing.Graphics]::FromImage($screenBitmap)

    # Captura a tela individual
    $screenGraphics.CopyFromScreen($bounds.X, $bounds.Y, 0, 0, $bounds.Size)

    # Desenha a captura no bitmap combinado
    $graphics.DrawImage($screenBitmap, $offsetX, 0)

    # Atualiza o deslocamento horizontal
    $offsetX += $bounds.Width

    # Libera os recursos
    $screenGraphics.Dispose()
    $screenBitmap.Dispose()
}

# Define o nome do arquivo de saída
$fileName = Join-Path $savePath "screenshot_all_monitors.png"

if(Test-Path $fileName){
    Rename-Item $fileName + (Get-Date -Format "yyyy-MM-dd-HH-mm-ss") + ".png"
}
# Salva a captura de tela combinada como um arquivo PNG
$bitmap.Save($fileName, [System.Drawing.Imaging.ImageFormat]::Png)

# Libera os recursos
$graphics.Dispose()
$bitmap.Dispose()

Write-Host "Captura de tela combinada salva em: $fileName"
