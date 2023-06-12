namespace Snek.Audio;

public class AudioManager
{
    private readonly AudioPlayer _player;

    public AudioManager()
    {
        _player = AudioPlayer.Create();
    }

    public void PlayPlayerMovedSound()
    {
        _player.Play("PlayerMoved.wav");
    }

    public void PlayEnemyEatenSound()
    {
        _player.Play("EnemyEaten.wav");
    }

    public void PlayPlayerDestroyedSound()
    {
        _player.Play("PlayerDestroyed.wav");
    }
}