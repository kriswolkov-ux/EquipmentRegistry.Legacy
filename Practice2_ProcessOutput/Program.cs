using System;
using System.Diagnostics;
using System.ComponentModel;

public class Program
{
    public static async Task Main()
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = "dotnet",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        startInfo.ArgumentList.Add("--info");

        using Process process = new()
        {
            StartInfo = startInfo
        };

        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            process.Start();
            Console.WriteLine($"PID: {process.Id}");

            // Оба чтения нужно начать до ожидания завершения,
            // иначе переполненные буферы могут заблокировать процесс.
            Task<string> stdoutTask = process.StandardOutput.ReadToEndAsync();
            Task<string> stderrTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string stdout = await stdoutTask;
            string stderr = await stderrTask;
            stopwatch.Stop();

            Console.WriteLine($"Длительность: {stopwatch.Elapsed}");
            Console.WriteLine($"Exit code: {process.ExitCode}");
            Console.WriteLine("=== STDOUT ===");
            Console.WriteLine(stdout);
            Console.WriteLine("=== STDERR ===");
            Console.WriteLine(stderr);
        }
        catch (Win32Exception ex)
        {
            stopwatch.Stop();
            Console.WriteLine($"Ошибка запуска: {ex.Message}");
        }
    }
}
