using gloBUS_Music.Data;
using gloBUS_Music.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace gloBUS_Music.MVVM.Views
{
    public partial class PlaylistsPage : Page
    {
        private readonly gloBUS_MusicDbContext _context;
        private readonly PlaylistsViewModel _viewModel;

        public PlaylistsPage(gloBUS_MusicDbContext context)
        {
            InitializeComponent();
            _context = context;
            _viewModel = new PlaylistsViewModel(_context);
            DataContext = _viewModel;
        }

        private void OpenPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedPlaylist == null)
            {
                MessageBox.Show("Выберите плейлист.");
                return;
            }

            var window = new PlaylistContentWindow(_viewModel.SelectedPlaylist, _context);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();

            _viewModel.Refresh();
        }
    }
}