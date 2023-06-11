using System.Diagnostics;

namespace Snek.Audio;

public class LinuxAudioPlayer : AudioPlayer
{
    private class CommandInfo
    {
        public string Command { get; set; }
        public string ArgumentTemplate { get; set; }
        public bool Installed { get; set; }
        public CommandInfo(string command, string argumentTemplate, bool installed)
        {
            Command = command;
            ArgumentTemplate = argumentTemplate;
            Installed = installed;
        }
    }

    private static readonly List<CommandInfo> _commandsInfo = new()
    {
        new("aplay", "{0}", false)
    };

    public LinuxAudioPlayer()
    {
        CheckInstalledCommands();
    }

    public override void Play(string file)
    {
        var commandInfo = _commandsInfo.First(c => c.Installed);
        if (commandInfo == null) return;
        ExecuteCommand($"{commandInfo.Command} {string.Format(commandInfo.ArgumentTemplate, file)}");
    }

    private static void CheckInstalledCommands()
    {
        foreach (var entry in _commandsInfo)
        {
            var command = entry.Command;
            var process = ExecuteCommand($"command -v {command}");

            process.WaitForExit();
            entry.Installed = process.ExitCode == 0;
        }
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
                CreateNoWindow = true,
                RedirectStandardError = true,
            }
        };

        process.Start();
        return process;
    }
}