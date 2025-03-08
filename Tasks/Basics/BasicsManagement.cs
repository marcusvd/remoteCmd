// using System;
// using System.Diagnostics;
// using System.Drawing;
// using System.Drawing.Imaging;
// using System.Windows.Forms;

namespace remoteCmd.Tasks.Basic;
public class BasicsManagement
{

    public static void Shutdown(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/s /t 15";
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Shutdown in 15 seconds.");
    }
    public static void Logoff(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/l";
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Logoff action executed.");
    }
    public static void Reboot(AppSettings _appSettings)
    {
        string command = "shutdown";
        string param = "/r /t 15";
        Tools.ProcessExecutorNoWaitCmdLine(command, param, _appSettings, "Reboot in 15 seconds.");
    }
    // public static void GetScreen(AppSettings _appSettings)
    // {
    //     try
    //     {
    //         // Capture all screens
    //         int screenIndex = 0;
    //         foreach (Screen screen in Screen.AllScreens)
    //         {
    //             Bitmap screenshot = CaptureScreen(screen);
    //             string filePath = $"screenshot_{screenIndex}.png";
    //             screenshot.Save("c:\\maut\\"+filePath, ImageFormat.Png);
    //             Console.WriteLine($"Screenshot saved to: {filePath}");
    //             screenIndex++;
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Error: {ex.Message}");
    //     }
    // }

    // static Bitmap CaptureScreen(Screen screen)
    // {
    //     Rectangle bounds = screen.Bounds;
    //     Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);

    //     using (Graphics graphics = Graphics.FromImage(bitmap))
    //     {
    //         graphics.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
    //     }

    //     return bitmap;
    // }
}