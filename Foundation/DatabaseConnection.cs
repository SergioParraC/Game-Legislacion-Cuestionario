using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace Foundation
{
    public class DatabaseConnection
    {
        private static DatabaseConnection _instance;
        private readonly string _connectionString;

        private DatabaseConnection()
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameDatabase.db");
            _connectionString = $"Data Source={dbPath}";
            InitializeDatabase();
        }

        public static DatabaseConnection Instance
        {
            get
            {
                _instance ??= new DatabaseConnection();
                return _instance;
            }
        }

        public SqliteConnection GetConnection()
        {
            return new SqliteConnection(_connectionString);
        }

        private void InitializeDatabase()
        {
            using var connection = GetConnection();
            connection.Open();
            CreateTables(connection);
            SeedInitialData(connection);
        }

        private void CreateTables(SqliteConnection connection)
        {
            string createTables = """
                CREATE TABLE IF NOT EXISTS Questions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Text TEXT NOT NULL,
                    CorrectAnswerIndex INTEGER NOT NULL,
                    Options TEXT NOT NULL,
                    Difficulty INTEGER NOT NULL,
                    IsBossQuestion INTEGER DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS GameProgress (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerName TEXT NOT NULL,
                    CurrentLevel INTEGER DEFAULT 1,
                    MaxScore INTEGER DEFAULT 0,
                    DatePlayed TEXT NOT NULL
                );
                """;

            using var command = new SqliteCommand(createTables, connection);
            command.ExecuteNonQuery();
        }

        private void SeedInitialData(SqliteConnection connection)
        {
            string checkQuery = "SELECT COUNT(*) FROM Questions";
            using var command = new SqliteCommand(checkQuery, connection);
            long count = (long)command.ExecuteScalar();
            if (count == 0)
            {
                InsertSampleQuestions(connection);
            }
        }

        private void InsertSampleQuestions(SqliteConnection connection)
        {
            string insertQuery = """
                INSERT INTO Questions (Text, CorrectAnswerIndex, Options, Difficulty, IsBossQuestion) VALUES
                ('¿Cuál es la capital de España?', 0, 'Madrid|Barcelona|Valencia|Sevilla', 1, 0),
                ('¿Cuántos continentes hay?', 2, '5|6|7|8', 1, 0),
                ('¿Quién pintó la Mona Lisa?', 1, 'Picasso|Leonardo da Vinci|Van Gogh|Dalí', 1, 0),
                ('¿En qué año comenzó la Segunda Guerra Mundial?', 1, '1935|1939|1941|1945', 1, 0),
                ('¿Cuál es el río más largo del mundo?', 2, 'Nilo|Yangtsé|Amazonas|Misisipi', 1, 0),
                ('¿Cuál es el animal terrestre más rápido?', 1, 'León|Guepardo|Tigre|Leopardo', 1, 0),
                ('¿Cuál es el planeta más grande del sistema solar?', 2, 'Marte|Saturno|Júpiter|Neptuno', 2, 0),
                ('¿En qué año llegó el hombre a la Luna?', 1, '1965|1969|1972|1980', 2, 0),
                ('¿Cuál es el océano más grande?', 2, 'Atlántico|Índico|Pacífico|Ártico', 2, 0),
                ('¿Quién escribió "Cien años de soledad"?', 0, 'Gabriel García Márquez|Mario Vargas Llosa|Julio Cortázar|Carlos Fuentes', 2, 0),
                ('¿Cuál es la montaña más alta del mundo?', 1, 'K2|Everest|Kilimanjaro|Aconcagua', 2, 0),
                ('¿Cuántos huesos tiene el cuerpo humano adulto?', 2, '186|196|206|216', 2, 0),
                ('¿En qué continente está Egipto?', 1, 'Asia|África|Europa|Oceanía', 2, 0),
                ('¿Cuántos lados tiene un hexágono?', 3, '4|5|7|6', 2, 0),
                ('¿Qué elemento químico tiene el símbolo Au?', 2, 'Plata|Aluminio|Oro|Hierro', 3, 0),
                ('¿Cuál es la velocidad de la luz aproximadamente?', 0, '299,792 km/s|150,000 km/s|400,000 km/s|200,000 km/s', 3, 0),
                ('¿Quién escribió Don Quijote de la Mancha?', 1, 'Lope de Vega|Miguel de Cervantes|Calderón|Quevedo', 3, 0),
                ('¿En qué año cayó el Muro de Berlín?', 2, '1985|1987|1989|1991', 3, 0),
                ('¿Cuál es la capital de Australia?', 1, 'Sídney|Canberra|Melbourne|Brisbane', 3, 0),
                ('¿Cuál es el metal más abundante en la corteza terrestre?', 0, 'Aluminio|Hierro|Cobre|Oro', 3, 0),
                ('¿Cuántos días tiene un año bisiesto?', 2, '364|365|366|367', 3, 0),
                ('¿Cuál es la partícula subatómica con carga negativa?', 2, 'Protón|Neutrón|Electrón|Quark', 4, 1),
                ('¿Cuál es la constante de Planck aproximadamente?', 1, '3.14 × 10?³?|6.626 × 10?³?|9.81 × 10?³?|1.23 × 10?³?', 4, 1),
                ('¿Quién formuló la teoría de la relatividad?', 0, 'Albert Einstein|Isaac Newton|Stephen Hawking|Niels Bohr', 4, 1)
                """;

            using var command = new SqliteCommand(insertQuery, connection);
            command.ExecuteNonQuery();
        }
    }
}
