using gloBUS_Music.Data;
using gloBUS_Music.MVVM.Core;
using gloBUS_Music.MVVM.Model;
using gloBUS_Music.MVVM.Services;
using Microsoft.Win32;
using System.IO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace gloBUS_Music.MVVM.ViewModel
{
    public class TracksViewModel : INotifyPropertyChanged
    {
        private readonly gloBUS_MusicDbContext _context;
        private readonly TrackService _trackService;
        private readonly PlayerService _playerService;
        private readonly DispatcherTimer _progressTimer;
        private Track _selectedTrack;
        private bool _isPaused = false;
        private bool _isStopped = false;
        private double _playbackProgress;
        private double _playbackDuration;
        private string _currentTime = "00:00";
        private string _totalTime = "00:00";
        private bool _isUserSeeking = false;
        private bool _isPlaying = false;
        private double _volume = 0.5;
        private double _previousVolume = 0.5;

        public Track NewTrack { get; set; }
        public bool CanRemoveTrack => SelectedTrack != null;
        public bool CanPlayTrack => SelectedTrack != null && !string.IsNullOrEmpty(SelectedTrack.Link);

        public ICommand SelectFileCommand { get; }
        public ICommand AddTrackCommand { get; }
        public ICommand RemoveTrackCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand PauseTrackCommand { get; }
        public ICommand StopTrackCommand { get; }

        public ObservableCollection<Track> Tracks { get; set; }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged();
            }
        }

        public double PlaybackProgress
        {
            get => _playbackProgress;
            set
            {
                _playbackProgress = value;
                OnPropertyChanged();
            }
        }

        public double PlaybackDuration
        {
            get => _playbackDuration;
            set
            {
                _playbackDuration = value;
                OnPropertyChanged();
            }
        }

        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        public string TotalTime
        {
            get => _totalTime;
            set
            {
                _totalTime = value;
                OnPropertyChanged();
            }
        }

        public double Volume
        {
            get => _volume;
            set
            {
                var normalizedValue = Math.Max(0, Math.Min(1, value));

                if (Math.Abs(_volume - normalizedValue) < 0.001)
                    return;

                _volume = normalizedValue;

                if (_volume > 0.001)
                {
                    _previousVolume = _volume;
                }

                _playerService.SetVolume(_volume);

                OnPropertyChanged();
                OnPropertyChanged(nameof(IsMuted));
                OnPropertyChanged(nameof(VolumeIcon));
                OnPropertyChanged(nameof(VolumePercentText));
            }
        }

        public bool IsMuted => Volume <= 0.001;

        public string VolumeIcon => IsMuted ? "🔈" : "🔊";

        public string VolumePercentText => $"{(int)Math.Round(Volume * 100)}%";

        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged();

                OnPropertyChanged(nameof(CanRemoveTrack));
                OnPropertyChanged(nameof(CanPlayTrack));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public TracksViewModel()
        {
            _context = new gloBUS_MusicDbContext();

            _trackService = new TrackService(_context);
            _playerService = new PlayerService();

            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromMilliseconds(500);
            _progressTimer.Tick += UpdatePlaybackProgress;

            Tracks = new ObservableCollection<Track>();
            LoadTracks();

            NewTrack = new Track();

            SelectFileCommand = new RelayCommand(SelectFile);
            AddTrackCommand = new RelayCommand(AddTrack);
            RemoveTrackCommand = new RelayCommand(RemoveTrack, () => CanRemoveTrack);
            PlayTrackCommand = new RelayCommand(PlayTrack, () => CanPlayTrack);
            PauseTrackCommand = new RelayCommand(PauseTrack, CanControlPlayback);
            StopTrackCommand = new RelayCommand(StopTrack, CanControlPlayback);
        }

        public void InitPlayer(MediaElement player)
        {
            _playerService.Init(player);
            _playerService.SetVolume(Volume);

            if (!_progressTimer.IsEnabled)
                _progressTimer.Start();

            CommandManager.InvalidateRequerySuggested();
        }

        private void LoadTracks()
        {
            Tracks.Clear();
            var tracks = _trackService.GetAllTracks();

            foreach (var track in tracks)
            {
                Tracks.Add(track);
            }
        }

        public void AddTrack()
        {
            if (string.IsNullOrWhiteSpace(NewTrack.Title) ||
                string.IsNullOrWhiteSpace(NewTrack.Artist) ||
                string.IsNullOrWhiteSpace(NewTrack.Link) ||
                NewTrack.Duration <= 0)
            {
                MessageBox.Show("Заполните все поля и выберите файл!");
                return;
            }

            _trackService.AddTrack(NewTrack);

            Tracks.Add(NewTrack);

            NewTrack = new Track();
            OnPropertyChanged(nameof(NewTrack));
        }

        private void SelectFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files (*.mp3;*.wav)|*.mp3;*.wav|All Files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                NewTrack.Link = NormalizeText(openFileDialog.FileName);
                FillTrackMetadata(NewTrack.Link);
                OnPropertyChanged(nameof(NewTrack));
            }
        }

        private void FillTrackMetadata(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return;

            var currentTitle = NormalizeText(NewTrack.Title);
            var detectedTitle = GetFallbackTitle(filePath);
            var detectedArtist = "Unknown Artist";
            var detectedDuration = 0;

            try
            {
                var tagFile = TagLib.File.Create(filePath);

                if (!string.IsNullOrWhiteSpace(tagFile.Tag.Title))
                {
                    detectedTitle = tagFile.Tag.Title.Trim();
                }

                if (tagFile.Tag.Performers != null &&
                    tagFile.Tag.Performers.Length > 0 &&
                    !string.IsNullOrWhiteSpace(tagFile.Tag.Performers[0]))
                {
                    detectedArtist = tagFile.Tag.Performers[0].Trim();
                }

                detectedDuration = (int)Math.Ceiling(tagFile.Properties.Duration.TotalSeconds);
            }
            catch
            {
                detectedTitle = GetFallbackTitle(filePath);
                detectedArtist = "Unknown Artist";
                detectedDuration = 0;
            }

            NewTrack.Title = string.IsNullOrWhiteSpace(currentTitle) ? detectedTitle : currentTitle;
            NewTrack.Artist = detectedArtist;
            NewTrack.Duration = detectedDuration;
            NewTrack.Link = NormalizeText(filePath);

            OnPropertyChanged(nameof(NewTrack));
        }

        private string GetFallbackTitle(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath)?.Trim() ?? string.Empty;
        }

        private string NormalizeText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }

        private void RemoveTrack()
        {
            if (SelectedTrack != null)
            {
                _trackService.DeleteTrack(SelectedTrack.Id);

                Tracks.Remove(SelectedTrack);
                SelectedTrack = null;
            }
        }

        private void PlayTrack()
        {
            if (SelectedTrack == null || string.IsNullOrEmpty(SelectedTrack.Link))
            {
                MessageBox.Show("Трек не выбран или ссылка пустая!");
                return;
            }

            _playerService.Play(SelectedTrack.Link);

            _isStopped = false;
            _isPaused = false;
            IsPlaying = true;

            UpdatePlaybackProgress(this, EventArgs.Empty);
            CommandManager.InvalidateRequerySuggested();
        }

        private void PauseTrack()
        {
            _playerService.Pause();
            _isPaused = true;
            _isStopped = false;
            IsPlaying = false;
            UpdatePlaybackProgress(this, EventArgs.Empty);
            CommandManager.InvalidateRequerySuggested();
        }

        private void StopTrack()
        {
            _playerService.Stop();
            _isPaused = false;
            _isStopped = true;
            IsPlaying = false;
            ResetPlaybackProgress();
            CommandManager.InvalidateRequerySuggested();
        }

        private void UpdatePlaybackProgress(object sender, EventArgs e)
        {
            var duration = _playerService.GetDuration();
            var position = _playerService.GetPosition();

            if (duration > 0)
            {
                PlaybackDuration = duration;
                TotalTime = FormatTime(TimeSpan.FromSeconds(duration));
            }
            else
            {
                PlaybackProgress = 1;
                TotalTime = "00:00";
            }

            if (!_isUserSeeking)
            {
                PlaybackProgress = position;
                CurrentTime = FormatTime(TimeSpan.FromSeconds(position));
            }
        }

        private void ResetPlaybackProgress()
        {
            PlaybackDuration = 0;
            PlaybackProgress = 1;
            CurrentTime = "00:00";
            TotalTime = "00:00";
        }

        private string FormatTime(TimeSpan time)
        {
            return time.TotalHours >= 1
                ? time.ToString(@"hh\:mm\:ss")
                : time.ToString(@"mm\:ss");
        }

        public void BeginSeek()
        {
            _isUserSeeking = true;
        }

        public void UpdateSeekPreview(double value)
        {
            if (!_isUserSeeking)
                return;

            PlaybackProgress = value;
            CurrentTime = FormatTime(TimeSpan.FromSeconds(value));
        }

        public void EndSeek(double value)
        {
            _playerService.SetPosition(value);
            PlaybackProgress = value;
            CurrentTime = FormatTime(TimeSpan.FromSeconds(value));
            _isUserSeeking = false;
        }

        public void HandleTrackEnded()
        {
            _isPaused = false;
            _isStopped = true;
            IsPlaying = false;
            ResetPlaybackProgress();
            CommandManager.InvalidateRequerySuggested();
        }

        public void ToggleMute()
        {
            if (IsMuted)
            {
                Volume = _previousVolume > 0.001 ? _previousVolume : 0.5;
            }
            else
            {
                _previousVolume = Volume;
                Volume = 0;
            }
        }

        private bool CanControlPlayback()
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}