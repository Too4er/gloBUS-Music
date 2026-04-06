using MusiLand.MVVM.Model;
using MusiLand.MVVM.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MusiLand.MVVM.Core;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Media;

namespace MusiLand.MVVM.ViewModel
{
    public class TracksViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private Track _selectedTrack;
        private MediaPlayer _mediaPlayer = new MediaPlayer();
        private bool _isPaused = false;
        private bool _isStopped = false;
        private TimeSpan _pausedPosition = TimeSpan.Zero;
        public Track NewTrack { get; set; }
        public bool CanRemoveTrack => SelectedTrack != null;

        public ICommand SelectFileCommand { get; }
        public ICommand AddTrackCommand { get; }
        public ICommand RemoveTrackCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand PauseTrackCommand { get; }
        public ICommand StopTrackCommand { get; }

        public ObservableCollection<Track> Tracks { get; set; }

        public Track SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                OnPropertyChanged();
            }
        }

        public TracksViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            _mediaPlayer = new MediaPlayer();
            Tracks = new ObservableCollection<Track>();
            LoadTracks();

            NewTrack = new Track();
            SelectFileCommand = new RelayCommand(SelectFile);
            AddTrackCommand = new RelayCommand(AddTrack);
            RemoveTrackCommand = new RelayCommand(RemoveTrack, () => CanRemoveTrack);
            PlayTrackCommand = new RelayCommand(PlayTrack, CanPlayTrack);
            PauseTrackCommand = new RelayCommand(PauseTrack, CanControlPlayback);
            StopTrackCommand = new RelayCommand(StopTrack, CanControlPlayback);
        }

        private void LoadTracks()
        {
            Tracks.Clear();
            using (var command = _databaseService.Connection.CreateCommand())
            {
                command.CommandText = "SELECT * FROM Tracks";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Tracks.Add(new Track
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Artist = reader.GetString(2),
                            Link = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Duration = reader.GetInt32(4)
                        });
                    }
                }
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

            using (var command = _databaseService.Connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Tracks (Title, Artist, Link, Duration) VALUES (@Title, @Artist, @Link, @Duration)";
                command.Parameters.AddWithValue("@Title", NewTrack.Title);
                command.Parameters.AddWithValue("@Artist", NewTrack.Artist);
                command.Parameters.AddWithValue("@Link", NewTrack.Link);
                command.Parameters.AddWithValue("@Duration", NewTrack.Duration);
                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid()";
                NewTrack.Id = Convert.ToInt32(command.ExecuteScalar());
            }

            Tracks.Add(new Track
            {
                Id = NewTrack.Id,
                Title = NewTrack.Title,
                Artist = NewTrack.Artist,
                Link = NewTrack.Link,
                Duration = NewTrack.Duration
            });

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
                NewTrack.Link = openFileDialog.FileName;
                OnPropertyChanged(nameof(NewTrack));
            }
        }

        private void RemoveTrack()
        {
            if (SelectedTrack != null)
            {
                _databaseService.DeleteTrack(SelectedTrack);
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

            if (_isStopped || _mediaPlayer.Source == null) // Если трек был остановлен
            {
                _mediaPlayer.MediaOpened -= MediaOpenedHandler;
                _mediaPlayer.MediaOpened += MediaOpenedHandler;
                _mediaPlayer.Open(new Uri(SelectedTrack.Link, UriKind.Absolute));
                _isStopped = false;
            }
            else if (_isPaused) // Если был на паузе
            {
                _mediaPlayer.Position = _pausedPosition;
                _mediaPlayer.Play();
                _isPaused = false;
            }
            else // Если просто Play
            {
                _mediaPlayer.Play();
            }
        }

        private void MediaOpenedHandler(object sender, EventArgs e)
        {
            _mediaPlayer.Play();
            _isStopped = false;
            _isPaused = false;
        }

        private void PauseTrack()
        {
            if (_mediaPlayer.Source != null)
            {
                _pausedPosition = _mediaPlayer.Position;
                _mediaPlayer.Pause();
                _isPaused = true;
            }
        }

        private void StopTrack()
        {
            if (_mediaPlayer.Source != null)
            {
                _mediaPlayer.Stop();
                _mediaPlayer.Close(); // Закрываем трек, чтобы он сбросился
                _pausedPosition = TimeSpan.Zero;
                _isPaused = false;
                _isStopped = true;
            }
        }

        private bool CanPlayTrack()
        {
            return SelectedTrack != null && !string.IsNullOrEmpty(SelectedTrack.Link);
        }

        private bool CanControlPlayback()
        {
            return _mediaPlayer != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

