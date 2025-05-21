using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoFinalEMP.Controller;
using System.Data.SQLite;
using System.IO;

namespace ProyectoFinalEMP.Controller
{
    class SQLiteManager
    {
        private readonly string dbPath;

        public SQLiteManager()
        {
            // Carpeta segura para escritura: C:\Users\TUUSUARIO\AppData\Roaming\Gesem
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Gesem");
            Directory.CreateDirectory(folder); // La crea si no existe

            dbPath = Path.Combine(folder, "rememberme.db");

            string connectionString = $"Data Source={dbPath};Version=3;";

            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            string sql = @"
            CREATE TABLE IF NOT EXISTS SesionGuardada (
            email TEXT PRIMARY KEY,
            password TEXT NOT NULL
            );
        ";

            using var command = new SQLiteCommand(sql, connection);
            command.ExecuteNonQuery();
        }

        public void GuardarSesion(string email, string password)
        {
            string connectionString = $"Data Source={dbPath};Version=3;";
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            string borrar = "DELETE FROM SesionGuardada;";
            using (var cmd = new SQLiteCommand(borrar, connection)) { cmd.ExecuteNonQuery(); }

            string insertar = "INSERT OR REPLACE INTO SesionGuardada (email, password) VALUES (@e, @p)";
            using var command = new SQLiteCommand(insertar, connection);
            command.Parameters.AddWithValue("@e", email);
            command.Parameters.AddWithValue("@p", password);
            command.ExecuteNonQuery();
        }

        public string[] CargarSesion()
        {
            string connectionString = $"Data Source={dbPath};Version=3;";
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();

            string consulta = "SELECT email, password FROM SesionGuardada LIMIT 1";
            using var command = new SQLiteCommand(consulta, connection);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                string email = reader.GetString(0);
                string password = reader.GetString(1);
                return new string[] { email, password };
            }

            return null;
        }
    }
}
