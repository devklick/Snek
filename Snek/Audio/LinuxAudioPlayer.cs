using System.Diagnostics;

namespace Snek.Audio;

public class LinuxAudioPlayer : AudioPlayer
{
    public override IReadOnlyList<string> SupportedFileTypes => _supportedFileTypes;

    private static readonly Dictionary<string, string> _cliPlayers = new()
    {
        { ".wav", "aplay" }
    };
    private readonly List<string> _supportedFileTypes = GetSupportedFileTypes();

    public LinuxAudioPlayer()
    { }

    public override void Play(string file)
    {
        var fileType = Path.GetExtension(file);
        if (!_cliPlayers.TryGetValue(fileType.ToLower(), out var cliPlayer))
        {
            // TODO: Handle the scenario where we do not have a CLI player installed that can support the file type
            return;
        }

        ExecuteCommand($"{cliPlayer} {file}");
    }

    private static List<string> GetSupportedFileTypes()
    {
        var supported = new List<string>();
        foreach (var entry in _cliPlayers)
        {
            var fileType = entry.Key;
            var cliPlayer = entry.Value;
            var process = ExecuteCommand($"command -v {cliPlayer}");

            process.WaitForExit();
            var installed = process.ExitCode == 0;

            if (installed)
            {
                supported.Add(fileType);
            }
        }

        return supported;
    }

    private static Process ExecuteCommand(string command)
    {
        var escapedArgs = command.Replace("\"", "\\\"");

        var process = new Process()
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"{escapedArgs}\"",

                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
            }
        };

        process.Start();
        return process;
    }
}