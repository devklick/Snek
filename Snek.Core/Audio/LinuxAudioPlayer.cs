namespace Snek.Core.Audio;

public class LinuxAudioPlayer : UnixAudioPlayer
{
    private static readonly List<CommandInfo> _commands = [new("aplay", "{0}")];
    public LinuxAudioPlayer() : base(_commands)
    { }
}