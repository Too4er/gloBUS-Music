using gloBUS_Music.Data;
using gloBUS_Music.MVVM.Model;
using gloBUS_Music.MVVM.Services;
using System.Linq;
using System.Windows;

namespace gloBUS_Music.MVVM.Views
{
    public partial class PlaylistContentWindow : Window
    {
        private readonly PlaylistService _playlistService;
        private Playlist _playlist;

        public PlaylistContentWindow(Playlist playlist, gloBUS_MusicDbContext context)
        {
            InitializeComponent();
            _playlistService = new PlaylistService(context);
            _playlist = _playlistService.GetPlaylistById(playlist.Id) ?? playlist;
            DataContext = _playlist;
            LoadPlaylistTracks();
        }

        private void LoadPlaylistTracks()
        {
            PlaylistTracksListBox.ItemsSource = null;
            PlaylistTracksListBox.ItemsSource = _playlist.Tracks.OrderBy(t => t.Title).ToList();
        }

        private void RemoveTrackFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistTracksListBox.SelectedItem is not Track selectedTrack)
            {
                MessageBox.Show("Выберите трек.");
                return;
            }

            var removed = _playlistService.RemoveTrackFromPlaylist(_playlist.Id, selectedTrack.Id);

            if (!removed)
            {
                MessageBox.Show("Не удалось удалить трек из плейлиста.");
                return;
            }

            var updatedPlaylist = _playlistService.GetPlaylistById(_playlist.Id);

            if (updatedPlaylist != null)
            {
                _playlist = updatedPlaylist;
                DataContext = _playlist;
                LoadPlaylistTracks();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}