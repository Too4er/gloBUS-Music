using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MusiLand
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Если в App.xaml указан StartupUri, этот код не нужен
            // var mainWindow = new MVVM.Views.MainWindow();
            // mainWindow.Show();
        }
    }
}
