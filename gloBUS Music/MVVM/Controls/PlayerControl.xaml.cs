using gloBUS_Music.MVVM.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace gloBUS_Music.MVVM.Controls
{
    public partial class PlayerControl : UserControl
    {
        private TracksViewModel _viewModel;

        public PlayerControl()
        {
            InitializeComponent();
            Loaded += PlayerControl_Loaded;
        }

        private void PlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null && DataContext is TracksViewModel vm)
            {
                _viewModel = vm;
                _viewModel.InitPlayer(AudioPlayer);

                // ИЗМЕНЕНИЕ: принудительно обновляем CanExecute у кнопок плеера
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void PlaybackSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            _viewModel?.BeginSeek();
        }

        private void PlaybackSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _viewModel?.EndSeek(PlaybackSlider.Value);
        }

        private void PlaybackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PlaybackSlider.IsMouseCaptureWithin)
            {
                _viewModel?.UpdateSeekPreview(PlaybackSlider.Value);
            }
        }

        private void AudioPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            _viewModel?.HandleTrackEnded();
        }
    }
}