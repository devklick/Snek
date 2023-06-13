namespace Snek.Audio;

public class OsxAudioPlayer : UnixAudioPlayer
{
    private static readonly List<CommandInfo> _commands = new()
    {
        new("afplay", "{0}", false)
    };
    public OsxAudioPlayer() : base(_commands)
    { }
}