namespace Snek.Core.Audio;

public class OsxAudioPlayer : UnixAudioPlayer
{
    private static readonly List<CommandInfo> _commands = [new("afplay", "{0}")];
    public OsxAudioPlayer() : base(_commands)
    { }
}