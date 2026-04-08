using gloBUS_Music.MVVM.ViewModel;
using gloBUS_Music.MVVM.Views;
using System;
using System.Windows;
using gloBUS_Music.Data;

namespace gloBUS_Music.MVVM.Views
{
    public partial class MainWindow : Window
    {
        private readonly gloBUS_MusicDbContext _context;
        private readonly TracksViewModel _tracksViewModel;

        public MainWindow()
        {
            InitializeComponent();

            _context = new gloBUS_MusicDbContext();
            _tracksViewModel = new TracksViewModel();

            GlobalPlayerControl.DataContext = _tracksViewModel;

            NavigationMenu.NavigateToTracks += NavigateToTracks_Click;
            NavigationMenu.NavigateToPlaylists += NavigateToPlaylists_Click;

            NavigateToTracks();
        }

        private void NavigateToTracks()
        {
            MainFrame.Navigate(new TracksPage(_tracksViewModel));
        }

        private void NavigateToTracks_Click(object sender, EventArgs e)
        {
            NavigateToTracks();
        }

        private void NavigateToPlaylists_Click(object sender, EventArgs e)
        {
            MainFrame.Navigate(new PlaylistsPage(_context));
        }
    }
}