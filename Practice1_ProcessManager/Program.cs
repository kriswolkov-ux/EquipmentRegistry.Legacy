using System;
using System.Diagnostics;
using System.ComponentModel;

public class Program
{
    public static async Task Main()
    {
        // Используем dotnet --info как безопасную утилиту.
        // Если dotnet недоступен через PATH, замените
        // FileName на путь к другой разрешённой утилите.

        ProcessStartInfo startInfo = new()
        {
            FileName = "dotnet",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("--info");
        startInfo.WorkingDirectory = Environment.CurrentDirectory;

        using Process process = new()
        {
            StartInfo = startInfo
        };

        Stopwatch stopwatch = new();

        try
        {
            stopwatch.Start();
            process.Start();
            Console.WriteLine($"PID: {process.Id}");

            // ExitCode доступен только после фактического завершения процесса.
            await process.WaitForExitAsync();
            stopwatch.Stop();

            Console.WriteLine($"Длительность: {stopwatch.Elapsed}");
            Console.WriteLine($"Exit code: {process.ExitCode}");
        }
        catch (Win32Exception ex)
        {
            Console.WriteLine($"Ошибка запуска: {ex.Message}");
            Environment.ExitCode = 1;
        }
    }
}
