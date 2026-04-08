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

        public MainWindow()
        {
            InitializeComponent();

            _context = new gloBUS_MusicDbContext();

            NavigationMenu.NavigateToTracks += NavigateToTracks_Click;
            NavigationMenu.NavigateToPlaylists += NavigateToPlaylists_Click;

            NavigateToTracks();
        }

        private void NavigateToTracks()
        {
            var vm = new TracksViewModel();
            MainFrame.Navigate(new TracksPage(vm));
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