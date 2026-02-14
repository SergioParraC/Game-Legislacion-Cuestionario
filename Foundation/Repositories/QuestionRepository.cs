using Foundation.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Repositories
{
    public class QuestionRepository
    {
        public List<Question> GetQuestionsByDifficulty(int difficulty, bool isBoss = false)
        {
            var questions = new List<Question>();
            using var connection = DatabaseConnection.Instance.GetConnection();
            connection.Open();
            string query = "SELECT * FROM Questions WHERE Difficulty = @Difficulty AND IsBossQuestion = @IsBoss ORDER BY RANDOM()";

            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@Difficulty", difficulty);
            command.Parameters.AddWithValue("@IsBoss", isBoss ? 1 : 0);

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var question = new Question
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    CorrectAnswerIndex = reader.GetInt32(2),
                    Difficulty = reader.GetInt32(4),
                    IsBossQuestion = reader.GetInt32(5) == 1
                };

                string optionsString = reader.GetString(3);
                question.Options = [.. optionsString.Split('|')];
                questions.Add(question);
            }

            return questions;
        }

        public Question GetRandomQuestion(int difficulty)
        {
            var questions = GetQuestionsByDifficulty(difficulty);
            if (questions.Count == 0) return null;

            Random random = new();
            return questions[random.Next(questions.Count)];
        }
    }
}
