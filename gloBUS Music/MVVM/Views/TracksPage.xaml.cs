using gloBUS_Music.MVVM.Model;
using gloBUS_Music.MVVM.Services;
using gloBUS_Music.MVVM.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gloBUS_Music.MVVM.Views
{
    public partial class TracksPage : Page
    {
        private readonly TracksViewModel _viewModel;

        public TracksPage(TracksViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            _viewModel.InitPlayer(AudioPlayer);
        }

        private void AddTrack_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Audio Files (*.mp3;*.wav)|*.mp3;*.wav|All Files (*.*)|*.*",
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                var newTrack = new gloBUS_Music.MVVM.Model.Track
                {
                    Title = fileName,
                    Artist = "Unknown Artist",
                    Duration = 0,
                    Link = filePath
                };

                ((TracksViewModel)DataContext).AddTrack();
            }
        }

        private void RemoveTrack_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.RemoveTrackCommand.Execute(TrackListBox.SelectedItem);
        }
        private void PlaybackSlider_DragStarted(object sender, DragStartedEventArgs e)
        {
            _viewModel.BeginSeek();
        }
        private void PlaybackSlider_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            _viewModel.EndSeek(PlaybackSlider.Value);
        }
        private void PlaybackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (PlaybackSlider.IsMouseCaptureWithin)
            {
                _viewModel.UpdateSeekPreview(PlaybackSlider.Value);
            }
        }
    }
}