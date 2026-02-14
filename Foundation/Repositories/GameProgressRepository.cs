using Foundation.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace Foundation.Repositories
{
    public class GameProgressRepository
    {
        public int SaveProgress(GameProgressModel progress)
        {
            using var connection = DatabaseConnection.Instance.GetConnection();
            connection.Open();
            string query = """
                INSERT INTO GameProgress (PlayerName, CurrentLevel, MaxScore, DatePlayed) 
                VALUES (@PlayerName, @CurrentLevel, @MaxScore, @DatePlayed);
                SELECT last_insert_rowid();
                """;

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@PlayerName", progress.PlayerName);
            command.Parameters.AddWithValue("@CurrentLevel", progress.CurrentLevel);
            command.Parameters.AddWithValue("@MaxScore", progress.MaxScore);
            command.Parameters.AddWithValue("@DatePlayed", progress.DatePlayed.ToString("yyyy-MM-dd HH:mm:ss"));

            return Convert.ToInt32(command.ExecuteScalar());
        }

        public void UpdateProgress(GameProgressModel progress)
        {
            using var connection = DatabaseConnection.Instance.GetConnection();
            connection.Open();
            string query = """
                UPDATE GameProgress 
                SET CurrentLevel = @CurrentLevel, MaxScore = @MaxScore, DatePlayed = @DatePlayed 
                WHERE Id = @Id
                """;

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Id", progress.Id);
            command.Parameters.AddWithValue("@CurrentLevel", progress.CurrentLevel);
            command.Parameters.AddWithValue("@MaxScore", progress.MaxScore);
            command.Parameters.AddWithValue("@DatePlayed", progress.DatePlayed.ToString("yyyy-MM-dd HH:mm:ss"));

            command.ExecuteNonQuery();
        }

        public List<GameProgressModel> GetTopScores(int limit = 10)
        {
            var scores = new List<GameProgressModel>();
            using var connection = DatabaseConnection.Instance.GetConnection();
            connection.Open();
            string query = "SELECT * FROM GameProgress ORDER BY MaxScore DESC LIMIT @Limit";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Limit", limit);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                scores.Add(new GameProgressModel
                {
                    Id = reader.GetInt32(0),
                    PlayerName = reader.GetString(1),
                    CurrentLevel = reader.GetInt32(2),
                    MaxScore = reader.GetInt32(3),
                    DatePlayed = DateTime.Parse(reader.GetString(4))
                });
            }

            return scores;
        }
    }
}
