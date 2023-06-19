using System.Diagnostics;

using Snek.Core.Audio;

public abstract class UnixAudioPlayer : AudioPlayer
{
    public class CommandInfo
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

    protected readonly IReadOnlyList<CommandInfo> _commandInfos;

    public UnixAudioPlayer(List<CommandInfo> commandInfos)
    {
        _commandInfos = CheckInstalledCommands(commandInfos);
        Enabled = _commandInfos.Any(c => c.Installed);
    }

    public override void Play(string file)
    {
        if (!Enabled) return;
        var commandInfo = _commandInfos.First(c => c.Installed);
        if (commandInfo == null) return;
        ExecuteCommand($"{commandInfo.Command} {string.Format(commandInfo.ArgumentTemplate, file)}");
    }

    protected static List<CommandInfo> CheckInstalledCommands(List<CommandInfo> commandInfos)
    {
        foreach (var entry in commandInfos)
        {
            var command = entry.Command;
            var process = ExecuteCommand($"command -v {command}");

            process.WaitForExit();
            entry.Installed = process.ExitCode == 0;
        }
        return commandInfos;
    }

    protected static Process ExecuteCommand(string command)
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