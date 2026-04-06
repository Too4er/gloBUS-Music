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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gloBUS_Music.MVVM.Controls
{
    /// <summary>
    /// Логика взаимодействия для NavigationMenu.xaml
    /// </summary>
    public partial class NavigationMenu : UserControl
    {
        // Определяем события для навигации
        public event EventHandler NavigateToTracks;
        public event EventHandler NavigateToPlaylists;

        private bool isMenuOpen = true; // Статус меню (открыто или закрыто)

        public NavigationMenu()
        {
            InitializeComponent();
        }

        // Обработчик для кнопки, которая скрывает/показывает меню
        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (isMenuOpen)
            {
                // Анимация для закрытия меню
                Storyboard storyboard = (Storyboard)this.Resources["SlideMenuClose"];
                storyboard.Begin();
                isMenuOpen = false;
            }
            else
            {
                // Анимация для открытия меню
                Storyboard storyboard = (Storyboard)this.Resources["SlideMenuOpen"];
                storyboard.Begin();
                isMenuOpen = true;
            }
        }

        // Обработчик для кнопки "Tracks"
        private void NavigateToTracks_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем событие навигации на страницу треков
            NavigateToTracks?.Invoke(this, EventArgs.Empty);
        }

        // Обработчик для кнопки "Playlists"
        private void NavigateToPlaylists_Click(object sender, RoutedEventArgs e)
        {
            // Вызываем событие навигации на страницу плейлистов
            NavigateToPlaylists?.Invoke(this, EventArgs.Empty);
        }

        // Метод для открытия меню (можно вызывать из других мест)
        public void OpenMenu()
        {
            // Запуск анимации открытия меню
            Storyboard storyboard = (Storyboard)this.Resources["SlideMenuOpen"];
            storyboard.Begin();
            isMenuOpen = true; // Обновляем статус
        }

        // Метод для закрытия меню (можно вызывать из других мест)
        public void CloseMenu()
        {
            // Запуск анимации закрытия меню
            Storyboard storyboard = (Storyboard)this.Resources["SlideMenuClose"];
            storyboard.Begin();
            isMenuOpen = false; // Обновляем статус
        }
    }
}
