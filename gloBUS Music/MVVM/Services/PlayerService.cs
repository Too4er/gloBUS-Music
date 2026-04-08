using System;
using System.Windows.Controls;

namespace gloBUS_Music.MVVM.Services;

public class PlayerService
{
    private MediaElement _player;
    private string _currentUrl;
    private double _volume = 0.5;

    public void Init(MediaElement player)
    {
        _player = player;

        if (_player != null)
        {
            _player.Volume = _volume;
        }
    }

    public void Play(string url)
    {
        if (_player == null || string.IsNullOrWhiteSpace(url))
            return;

        if (_player.Source == null || !string.Equals(_currentUrl, url, StringComparison.OrdinalIgnoreCase))
        {
            _player.Source = new Uri(url, UriKind.Absolute);
            _currentUrl = url;
        }

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
    public void SetVolume(double volume)
    {
        _volume = Math.Max(0, Math.Min(1, volume));

        if (_player != null)
        {
            _player.Volume = _volume;
        }
    }
}