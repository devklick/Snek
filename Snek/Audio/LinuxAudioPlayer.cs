namespace Snek.Audio;

public class LinuxAudioPlayer : UnixAudioPlayer
{
    private static readonly List<CommandInfo> _commands = new()
    {
        new("aplay", "{0}", false)
    };
    public LinuxAudioPlayer() : base(_commands)
    { }
}