using System.Runtime.InteropServices;

namespace Snek.Audio;

public abstract class AudioPlayer
{
    public virtual bool Enabled { get; protected set; }
    public abstract void Play(string file);

    public static AudioPlayer Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return new LinuxAudioPlayer();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new WindowsAudioPlayer();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return new OsxAudioPlayer();
        else
            throw new Exception("No implementation exist for the current OS");
    }
}