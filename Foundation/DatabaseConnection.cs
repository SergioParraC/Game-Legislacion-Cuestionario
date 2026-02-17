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
                ('¿Qué problema legal principal surge al inicio de la historia?', 1, 'Error de programación|Uso de idea sin autorización|Falla del servidor|Problema de diseño', 1, 0),
                ('¿Qué protege legalmente el código fuente de un software?', 2, 'Derecho penal|Derecho ambiental|Propiedad intelectual|Derecho deportivo', 1, 0),
                ('¿Por qué es importante firmar contratos al iniciar un proyecto de software?', 0, 'Para definir responsabilidades y propiedad|Para mejorar el diseño|Para acelerar el programa|Para cambiar el lenguaje', 1, 0),
                ('¿Qué tipo de conflicto surge entre los fundadores?', 3, 'Problemas de red|Errores de base de datos|Fallas de seguridad|Disputa por participación y acciones', 1, 0),
                ('¿Qué se protege cuando un desarrollador crea una aplicación original?', 1, 'El hardware|La autoría del software|El internet|La electricidad', 1, 0),
                ('¿Qué documento legal evita malentendidos entre socios de software?', 2, 'Manual técnico|Diagrama UML|Contrato de participación|Código fuente', 1, 0),
                ('¿Qué ocurre cuando se usa una idea de otro proyecto sin permiso?', 0, 'Posible violación de propiedad intelectual|Mejora el software|No pasa nada|Se vuelve código abierto', 2, 0),
                ('¿Qué derecho regula la creación y uso del software?', 3, 'Derecho familiar|Derecho de tránsito|Derecho deportivo|Derecho de autor', 2, 0),
                ('¿Qué debió existir desde el inicio entre los fundadores?', 1, 'Un videojuego|Acuerdos legales claros|Más programadores|Más servidores', 2, 0),
                ('¿Qué protege un acuerdo de confidencialidad (NDA)?', 2, 'Los equipos físicos|Los salarios|La información privada del proyecto|Las redes sociales', 2, 0),
                ('¿Cuál es un riesgo legal de no definir porcentajes de socios?', 0, 'Demandas y conflictos societarios|Mejor rendimiento|Menos usuarios|Mayor velocidad', 2, 0),
                ('¿Qué se entiende por propiedad intelectual en software?', 3, 'Los cables del computador|El sistema operativo|Los servidores físicos|El código y funcionalidades creadas', 2, 0),
                ('¿Qué sucede si un socio modifica las acciones de otro sin aviso?', 1, 'Se mejora el proyecto|Puede considerarse fraude empresarial|No tiene consecuencias|Es un cambio técnico', 3, 0),
                ('¿Por qué es clave documentar el desarrollo de software?', 0, 'Sirve como prueba legal de autoría|Reduce el tamaño del programa|Elimina errores automáticamente|Cambia el diseño visual', 3, 0),
                ('¿Qué conflicto legal refleja el uso de una idea universitaria para crear otra plataforma?', 2, 'Derecho penal|Derecho civil familiar|Apropiación indebida de idea|Derecho deportivo', 3, 0),
                ('¿Qué regula la relación entre desarrolladores que crean una empresa tecnológica?', 3, 'Derecho ambiental|Derecho médico|Derecho penal|Derecho mercantil y societario', 3, 0),
                ('¿Qué consecuencia legal puede tener incumplir un acuerdo de desarrollo?', 1, 'Mayor publicidad|Demandas por incumplimiento contractual|Mejor código|Más usuarios', 3, 0),
                ('¿Qué elemento legal define quién es dueño del software creado?', 0, 'El contrato de desarrollo|El logo del proyecto|El servidor usado|El lenguaje de programación', 3, 0),
                ('¿Qué principio legal se vulnera al usar información confidencial de un cliente en otro proyecto?', 2, 'Derecho de tránsito|Derecho deportivo|Confidencialidad y secreto profesional|Derecho ambiental', 4, 0),
                ('¿Qué tipo de conflicto legal surge cuando varios fundadores reclaman ser creadores del mismo software?', 1, 'Problema técnico|Disputa de autoría y propiedad intelectual|Falla de red|Error de interfaz', 4, 0),
                ('¿Qué aspecto legal se relaciona con definir roles y aportes en un proyecto tecnológico?', 0, 'Responsabilidad contractual de los socios|Velocidad del sistema|Diseño UX|Arquitectura del hardware', 4, 0),
                ('¿Qué enseñanza jurídica principal deja la creación de una red social entre estudiantes?', 3, 'Programar más rápido|Usar cualquier idea libremente|Ignorar los contratos|Formalizar acuerdos y proteger la propiedad intelectual', 4, 1),
                ('¿Qué puede ocurrir si no se establece quién posee el código fuente desde el inicio?', 1, 'Nada relevante|Conflictos legales y demandas por propiedad|Mejor rendimiento|Mayor popularidad', 4, 0),
                ('¿Cuál hubiera sido la mejor forma legal de evitar los conflictos entre fundadores en el desarrollo del software?', 0, 'Firmar contratos claros de propiedad intelectual y participación|Cambiar el lenguaje de programación|Usar más servidores|Diseñar otra interfaz', 4, 0)                
                """;

            using var command = new SqliteCommand(insertQuery, connection);
            command.ExecuteNonQuery();
        }
    }
}
