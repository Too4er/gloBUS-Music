using System;
using System.Windows.Controls;

namespace gloBUS_Music.MVVM.Services;

public class PlayerService
{
    private MediaElement _player;

    public void Init(MediaElement player)
    {
        _player = player;
    }

    public void Play(string url)
    {
        if (_player == null) return;

        _player.Source = new Uri(url);
        _player.Play();
    }

    public void Pause()
    {
        _player?.Pause();
    }

    public void Stop()
    {
        _player?.Stop();
    }

    public void SetPosition(double seconds)
    {
        if (_player == null) return;

        _player.Position = TimeSpan.FromSeconds(seconds);
    }

    public double GetPosition()
    {
        return _player?.Position.TotalSeconds ?? 0;
    }

    public double GetDuration()
    {
        if (_player?.NaturalDuration.HasTimeSpan == true)
            return _player.NaturalDuration.TimeSpan.TotalSeconds;

        return 0;
    }
}