using gloBUS_Music.MVVM.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace gloBUS_Music.MVVM.Controls
{
    public partial class PlayerControl : UserControl
    {
        private TracksViewModel _viewModel;
        private readonly DispatcherTimer _volumePopupOpenTimer;
        private readonly DispatcherTimer _volumePopupCloseTimer;
        private bool _isMouseOverVolumeButton;
        private bool _isMouseOverVolumePopup;

        public PlayerControl()
        {
            InitializeComponent();

            Loaded += PlayerControl_Loaded;
            Unloaded += PlayerControl_Unloaded;

            _volumePopupOpenTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _volumePopupOpenTimer.Tick += VolumePopupOpenTimer_Tick;

            _volumePopupCloseTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(150)
            };
            _volumePopupCloseTimer.Tick += VolumePopupCloseTimer_Tick;
        }

        private void PlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null && DataContext is TracksViewModel vm)
            {
                _viewModel = vm;
                _viewModel.InitPlayer(AudioPlayer);

                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void PlayerControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _volumePopupOpenTimer.Stop();
            _volumePopupCloseTimer.Stop();
            VolumePopup.IsOpen = false;
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

        private void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel?.ToggleMute();
        }

        private void VolumeButton_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOverVolumeButton = true;
            _volumePopupCloseTimer.Stop();

            if (!VolumePopup.IsOpen)
            {
                _volumePopupOpenTimer.Stop();
                _volumePopupOpenTimer.Start();
            }
        }

        private void VolumeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOverVolumeButton = false;
            StartVolumePopupCloseTimer();
        }

        private void VolumePopup_MouseEnter(object sender, MouseEventArgs e)
        {
            _isMouseOverVolumePopup = true;
            _volumePopupCloseTimer.Stop();
        }

        private void VolumePopup_MouseLeave(object sender, MouseEventArgs e)
        {
            _isMouseOverVolumePopup = false;
            StartVolumePopupCloseTimer();
        }

        private void VolumePopupOpenTimer_Tick(object sender, EventArgs e)
        {
            _volumePopupOpenTimer.Stop();

            if (_isMouseOverVolumeButton)
            {
                VolumePopup.IsOpen = true;
            }
        }

        private void VolumePopupCloseTimer_Tick(object sender, EventArgs e)
        {
            _volumePopupCloseTimer.Stop();

            if (!_isMouseOverVolumeButton && !_isMouseOverVolumePopup)
            {
                VolumePopup.IsOpen = false;
            }
        }

        private void StartVolumePopupCloseTimer()
        {
            _volumePopupOpenTimer.Stop();
            _volumePopupCloseTimer.Stop();
            _volumePopupCloseTimer.Start();
        }
    }
}