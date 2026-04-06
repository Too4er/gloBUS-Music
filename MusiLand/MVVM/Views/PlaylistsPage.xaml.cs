using MusiLand.MVVM.Model;
using MusiLand.MVVM.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusiLand.MVVM.Views
{
    public partial class PlaylistsPage : Page
    {
        private readonly DatabaseService _databaseService;

        public PlaylistsPage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;
            // Логика инициализации страницы плейлистов
        }

        private void PlaylistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Логика обработки изменения выбора
            var selectedPlaylist = (sender as ListBox)?.SelectedItem;
            if (selectedPlaylist != null)
            {
                // Действия с выбранным элементом
                Console.WriteLine($"Выбрано: {selectedPlaylist}");
            }
        }
    }
   }

