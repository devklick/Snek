using System.Runtime.InteropServices;

namespace Snek.Core.Audio;

/// <summary>
/// See <see href=2https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/how-to-use-platform-invoke-to-play-a-wave-file"/>
/// </summary>
public class WindowsAudioPlayer : AudioPlayer
{
    public override bool Enabled => true;

    [DllImport("winmm.DLL", EntryPoint = "PlaySound", SetLastError = true, CharSet = CharSet.Unicode, ThrowOnUnmappableChar = true)]
    private static extern bool PlaySound(string szSound, IntPtr hMod, PlaySoundFlags flags);

    [System.Flags]
    public enum PlaySoundFlags : int
    {
        SND_SYNC = 0x0000,
        SND_ASYNC = 0x0001,
        SND_NODEFAULT = 0x0002,
        SND_LOOP = 0x0008,
        SND_NOSTOP = 0x0010,
        SND_NOWAIT = 0x00002000,
        SND_FILENAME = 0x00020000,
        SND_RESOURCE = 0x00040004
    }

    public override void Play(string fileName)
    {
        if (!Enabled) return;
        PlaySound(fileName, IntPtr.Zero, PlaySoundFlags.SND_ASYNC);
    }
}