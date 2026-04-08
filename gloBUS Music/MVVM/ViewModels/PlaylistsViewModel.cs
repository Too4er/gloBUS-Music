using gloBUS_Music.Data;
using gloBUS_Music.MVVM.Core;
using gloBUS_Music.MVVM.Model;
using gloBUS_Music.MVVM.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace gloBUS_Music.MVVM.ViewModel
{
    public class PlaylistsViewModel : INotifyPropertyChanged
    {
        private readonly gloBUS_MusicDbContext _context;
        private readonly PlaylistService _playlistService;
        private readonly TrackService _trackService;

        private Playlist _selectedPlaylist;
        private Track _selectedAvailableTrack;
        private Track _selectedPlaylistTrack;
        private string _newPlaylistName;

        public ObservableCollection<Playlist> Playlists { get; set; }
        public ObservableCollection<Track> AvailableTracks { get; set; }
        public ObservableCollection<Track> PlaylistTracks { get; set; }

        public ICommand CreatePlaylistCommand { get; }
        public ICommand DeletePlaylistCommand { get; }
        public ICommand AddTrackToPlaylistCommand { get; }
        public ICommand RemoveTrackFromPlaylistCommand { get; }

        public string NewPlaylistName
        {
            get => _newPlaylistName;
            set
            {
                _newPlaylistName = value;
                OnPropertyChanged();
            }
        }

        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged();
                UpdatePlaylistTracks();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Track SelectedAvailableTrack
        {
            get => _selectedAvailableTrack;
            set
            {
                _selectedAvailableTrack = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Track SelectedPlaylistTrack
        {
            get => _selectedPlaylistTrack;
            set
            {
                _selectedPlaylistTrack = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public PlaylistsViewModel(gloBUS_MusicDbContext context)
        {
            _context = context;
            _playlistService = new PlaylistService(_context);
            _trackService = new TrackService(_context);

            Playlists = new ObservableCollection<Playlist>();
            AvailableTracks = new ObservableCollection<Track>();
            PlaylistTracks = new ObservableCollection<Track>();

            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            DeletePlaylistCommand = new RelayCommand(DeletePlaylist, CanDeletePlaylist);
            AddTrackToPlaylistCommand = new RelayCommand(AddTrackToPlaylist, CanAddTrackToPlaylist);
            RemoveTrackFromPlaylistCommand = new RelayCommand(RemoveTrackFromPlaylist, CanRemoveTrackFromPlaylist);

            Refresh();
        }

        public void Refresh()
        {
            LoadTracks();
            LoadPlaylists(_selectedPlaylist?.Id);
        }

        private void LoadTracks()
        {
            AvailableTracks.Clear();

            var tracks = _trackService.GetAllTracks();

            foreach (var track in tracks.OrderBy(t => t.Title))
            {
                AvailableTracks.Add(track);
            }
        }

        private void LoadPlaylists(int? playlistIdToSelect = null)
        {
            Playlists.Clear();

            var playlists = _playlistService.GetAllPlaylists();

            foreach (var playlist in playlists)
            {
                Playlists.Add(playlist);
            }

            if (playlistIdToSelect.HasValue)
            {
                SelectedPlaylist = Playlists.FirstOrDefault(p => p.Id == playlistIdToSelect.Value);
            }
            else if (Playlists.Count > 0)
            {
                SelectedPlaylist = Playlists[0];
            }
            else
            {
                SelectedPlaylist = null;
            }
        }

        private void UpdatePlaylistTracks()
        {
            PlaylistTracks.Clear();

            if (SelectedPlaylist == null)
                return;

            foreach (var track in SelectedPlaylist.Tracks.OrderBy(t => t.Title))
            {
                PlaylistTracks.Add(track);
            }
        }

        private void CreatePlaylist()
        {
            var playlistName = NewPlaylistName?.Trim();

            if (string.IsNullOrWhiteSpace(playlistName))
            {
                MessageBox.Show("Введите название плейлиста.");
                return;
            }

            if (Playlists.Any(p => string.Equals(p.Name, playlistName, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Плейлист с таким названием уже существует.");
                return;
            }

            var createdPlaylist = _playlistService.CreatePlaylist(playlistName);

            NewPlaylistName = string.Empty;
            LoadPlaylists(createdPlaylist.Id);
        }

        private void DeletePlaylist()
        {
            if (SelectedPlaylist == null)
                return;

            var playlistId = SelectedPlaylist.Id;
            var deleted = _playlistService.DeletePlaylist(playlistId);

            if (!deleted)
            {
                MessageBox.Show("Не удалось удалить плейлист.");
                return;
            }

            LoadPlaylists();
        }

        private bool CanDeletePlaylist()
        {
            return SelectedPlaylist != null;
        }

        private void AddTrackToPlaylist()
        {
            if (SelectedPlaylist == null || SelectedAvailableTrack == null)
                return;

            var added = _playlistService.AddTrackToPlaylist(SelectedPlaylist.Id, SelectedAvailableTrack.Id);

            if (!added)
            {
                MessageBox.Show("Этот трек уже есть в плейлисте или данные не найдены.");
                return;
            }

            LoadPlaylists(SelectedPlaylist.Id);
        }

        private bool CanAddTrackToPlaylist()
        {
            return SelectedPlaylist != null && SelectedAvailableTrack != null;
        }

        private void RemoveTrackFromPlaylist()
        {
            if (SelectedPlaylist == null || SelectedPlaylistTrack == null)
                return;

            var removed = _playlistService.RemoveTrackFromPlaylist(SelectedPlaylist.Id, SelectedPlaylistTrack.Id);

            if (!removed)
            {
                MessageBox.Show("Не удалось удалить трек из плейлиста.");
                return;
            }

            LoadPlaylists(SelectedPlaylist.Id);
        }

        private bool CanRemoveTrackFromPlaylist()
        {
            return SelectedPlaylist != null && SelectedPlaylistTrack != null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}