using MusiLand.MVVM.Model;
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
    public class DatabaseService
    {
        private const string DatabasePath = "Database/MusiLand.db";
        public SQLiteConnection Connection { get; private set; }

        public DatabaseService()
        {
            try
            {
                // Гарантируем, что база данных создана перед подключением
                DatabaseInitializer.InitializeDatabase();

                if (!File.Exists(DatabasePath))
                {
                    throw new FileNotFoundException("Ошибка: файл базы данных не найден.");
                }

                string connectionString = $"Data Source={DatabasePath};Version=3;";
                Connection = new SQLiteConnection(connectionString);
                Connection.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка DatabaseService: {ex.Message}");
            }
        }

        public void ExecuteQuery(string query)
        {
            using (var command = new SQLiteCommand(query, Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public SQLiteDataReader ExecuteReader(string query)
        {
            using (var command = new SQLiteCommand(query, Connection))
            {
                return command.ExecuteReader();
            }
        }

        public void DeleteTrack(Track track)
        {
            using (var command = new SQLiteCommand("DELETE FROM Tracks WHERE Id = @id", Connection))
            {
                command.Parameters.AddWithValue("@id", track.Id);
                command.ExecuteNonQuery();
            }
        }
        public void CloseConnection()
        {
            Connection.Close();
        }
    }
}
