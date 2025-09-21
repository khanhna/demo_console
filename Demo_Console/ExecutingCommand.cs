using System.Diagnostics;

namespace Demo_Console;

public class ExecutingCommand
{
    private const string BashFilePath = "/bin/bash"; // Use bash for Linux/WSL
    
    public static async Task ExecuteCommand()
    {
        // Create process start info
        var startInfo = new ProcessStartInfo
        {
            FileName = BashFilePath,
            Arguments =
                "-c \"cd /mnt/c/Workspace/gfpgan && python3 general_process_final_batch_images.py img-sources test-results --slim -0.15 --enlarge 0.05\"",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        
        // For Windows (if not using WSL), uncomment these lines instead:
        // startInfo.FileName = "cmd.exe";
        // startInfo.Arguments = "/c \"cd /mnt/c/Workspace/temp/test && npm i\"";
        
        Console.WriteLine($"Executing: {startInfo.Arguments}");
        
        using var process = new Process { StartInfo = startInfo };
        
        // Handle output data
        process.OutputDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"[OUTPUT] {e.Data}");
            }
        };
        
        // Handle error data
        process.ErrorDataReceived += (_, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                Console.WriteLine($"[ERROR] {e.Data}");
            }
        };
        
        // Start the process
        process.Start();
        
        // Begin async reading of output and error streams
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        // Wait for the process to complete
        await process.WaitForExitAsync();
        
        Console.WriteLine($"Command completed with exit code: {process.ExitCode}");

        Console.WriteLine(process.ExitCode == 0 ? "Command executed successfully!" : "Command failed with errors.");
    }
}