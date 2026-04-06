using gloBUS_Music.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

// Убедитесь, что System.Windows.Controls.Primitives не подключен, если не нужен



namespace gloBUS_Music.MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для PlaylistContentWindow.xaml
    /// </summary>
    public partial class PlaylistContentWindow : Window
    {
        private readonly Playlist _playlist;

        public PlaylistContentWindow(Playlist playlist)
        {
            InitializeComponent();
            _playlist = playlist;
            DataContext = _playlist;
            LoadPlaylistTracks();
        }

        private void LoadPlaylistTracks()
        {
            // Загрузите треки плейлиста из базы данных и привяжите их к ListBox
            PlaylistTracksListBox.ItemsSource = _playlist.Tracks;
        }

        private void RemoveTrackFromPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (PlaylistTracksListBox.SelectedItem is Track selectedTrack)
            {
                // Удалите трек из плейлиста
                _playlist.Tracks.Remove(selectedTrack);
                LoadPlaylistTracks();
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

