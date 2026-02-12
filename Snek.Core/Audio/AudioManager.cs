namespace Snek.Core.Audio;

public class AudioManager(bool enabled = true)
{
    private readonly AudioPlayer _player = AudioPlayer.Create();
    public bool Enabled { get; } = enabled;

    public void PlayPlayerMovedSound()
    {
        if (Enabled) _player.Play("PlayerMoved.wav");
    }

    public void PlayEnemyEatenSound()
    {
        if (Enabled) _player.Play("EnemyEaten.wav");
    }

    public void PlayPlayerDestroyedSound()
    {
        if (Enabled) _player.Play("PlayerDestroyed.wav");
    }
}