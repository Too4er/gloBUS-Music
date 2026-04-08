using gloBUS_Music.Data;
using gloBUS_Music.MVVM.Core;
using gloBUS_Music.MVVM.Core;
using gloBUS_Music.MVVM.Model;
using gloBUS_Music.MVVM.Services;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace gloBUS_Music.MVVM.ViewModel
{
    public class TracksViewModel : INotifyPropertyChanged
    {
        private readonly gloBUS_MusicDbContext _context;
        private readonly TrackService _trackService;
        private readonly PlayerService _playerService;
        private Track _selectedTrack;
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

        public TracksViewModel()
        {
            _context = new gloBUS_MusicDbContext();

            _trackService = new TrackService(_context);
            _playerService = new PlayerService();

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
                NewTrack.Link = openFileDialog.FileName;
                OnPropertyChanged(nameof(NewTrack));
            }
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
        }

/*        private void MediaOpenedHandler(object sender, EventArgs e)
        {
            _mediaPlayer.Play();
            _isStopped = false;
            _isPaused = false;
        }*/

        private void PauseTrack()
        {
            _playerService.Pause();
            _isPaused = true;
        }

        private void StopTrack()
        {
            _playerService.Stop();

            _pausedPosition = TimeSpan.Zero;
            _isPaused = false;
            _isStopped = true;
        }

        private bool CanPlayTrack()
        {
            return SelectedTrack != null && !string.IsNullOrEmpty(SelectedTrack.Link);
        }

        private bool CanControlPlayback()
        {
            return true;
        }

        public void InitPlayer(MediaElement player)
        {
            _playerService.Init(player);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}