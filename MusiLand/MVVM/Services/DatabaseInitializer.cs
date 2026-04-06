using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusiLand.MVVM.Services
{
    public static class DatabaseInitializer
    {
        private static readonly string DatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "MusiLand.db");
        private static readonly string ConnectionString = $"Data Source={DatabasePath}; Version=3;";


        public static void InitializeDatabase()
        {
            // Создаём папку, если её нет
            if (!Directory.Exists("Database"))
            {
                Directory.CreateDirectory("Database");
            }

            // Проверяем, существует ли файл базы данных
            if (!File.Exists(DatabasePath))
            {
                File.Delete(DatabasePath);
                SQLiteConnection.CreateFile(DatabasePath);
            }


            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Tracks (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Title TEXT NOT NULL,
                            Artist TEXT NOT NULL,
                            Link TEXT,
                            Duration INTEGER DEFAULT 0
                        );
                        
                        CREATE TABLE IF NOT EXISTS Playlist (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL
                        );

                        CREATE TABLE IF NOT EXISTS Playlist_Track (
                            PlaylistId INTEGER,
                            TrackId INTEGER,
                            FOREIGN KEY (PlaylistId) REFERENCES Playlist(Id),
                            FOREIGN KEY (TrackId) REFERENCES Tracks(Id),
                            PRIMARY KEY (PlaylistId, TrackId)
                        );
                    ";

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}

