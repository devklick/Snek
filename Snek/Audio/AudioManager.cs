namespace Snek.Audio;

public class AudioManager
{
    private readonly AudioPlayer _player;
    public bool Enabled { get; }

    public AudioManager(bool enabled = true)
    {
        Enabled = enabled;
        _player = AudioPlayer.Create();
    }

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