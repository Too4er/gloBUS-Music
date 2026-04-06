using MusiLand.MVVM.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using MaterialDesignThemes.Wpf; // Подключаем Material Design
using MusiLand.MVVM.Services; // Для работы с базой данных
using MusiLand.MVVM.Model; // Для работы с моделью треков
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Data.SQLite;
using MusiLand.MVVM.ViewModel;

namespace MusiLand.MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _databaseService;
        public ObservableCollection<Track> Tracks { get; set; }
        private const string DatabasePath = "Data Source=Database/MusiLand.db;";

        public MainWindow()
        {
            InitializeComponent();

            _databaseService = new DatabaseService();
            Tracks = new ObservableCollection<Track>();


            // Подключение событий от NavigationMenu
            NavigationMenu.NavigateToTracks += NavigateToTracks_Click;
            NavigationMenu.NavigateToPlaylists += NavigateToPlaylists_Click;

            if (_databaseService == null)
            {
                return;
            }
            if (_databaseService.Connection == null)
            {
                return;
            }

            LoadTracks(); // Загрузка треков
            var tracksViewModel = new TracksViewModel(_databaseService);
            MainFrame.Navigate(new TracksPage(tracksViewModel));

        }

        private void NavigateToTracks_Click(object sender, EventArgs e)
        {
            var tracksViewModel = new TracksViewModel(_databaseService);
            MainFrame.Navigate(new TracksPage(tracksViewModel)); // Передаем сервис базы данных и список треков на страницу треков
        }

        private void NavigateToPlaylists_Click(object sender, EventArgs e)
        {
            MainFrame.Navigate(new PlaylistsPage(_databaseService));
        }

        private void LoadTracks()
        {
            var command = _databaseService.Connection.CreateCommand();
            command.CommandText = "SELECT Id FROM Tracks";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Tracks.Add(new Track
                    {
                        Id = reader.GetInt32(0),

                    });
                }
            }
        }

        public void AddTrack(Track track)
        {
            var command = _databaseService.Connection.CreateCommand();
            command.CommandText = "INSERT INTO Tracks (Title, Artist, Duration) VALUES (@Title, @Artist, @Duration)";
            command.Parameters.AddWithValue("@Title", track.Title);
            command.Parameters.AddWithValue("@Artist", track.Artist);
            command.Parameters.AddWithValue("@Duration", track.Duration);
            command.ExecuteNonQuery();

            // Получаем ID добавленного трека
            command.CommandText = "SELECT last_insert_rowid()";
            track.Id = (int)(long)command.ExecuteScalar();

            Tracks.Add(track);
        }
    }

}