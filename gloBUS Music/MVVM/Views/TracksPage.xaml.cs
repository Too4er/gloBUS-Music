using gloBUS_Music.MVVM.Model;
using gloBUS_Music.MVVM.Services;
using gloBUS_Music.MVVM.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gloBUS_Music.MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для TracksPage.xaml
    /// </summary>
    public partial class TracksPage : Page
    {
        private readonly TracksViewModel _viewModel;

        public TracksPage(TracksViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            var vm = (TracksViewModel)DataContext;
            vm.InitPlayer(Player);
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

                var newTrack = new Track
                {
                    Title = fileName,
                    Artist = "Unknown Artist", // Можно добавить получение артиста из метаданных
                    Duration = 0, // Можно добавить логику получения длительности
                    Link = filePath // Сохраняем полный путь
                };

                ((TracksViewModel)DataContext).AddTrack();
            }
        }

        private void RemoveTrack_Click(object sender, RoutedEventArgs e)
        {
            // Теперь просто вызываем команду напрямую
            _viewModel.RemoveTrackCommand.Execute(TrackListBox.SelectedItem);
        }
    }
}
