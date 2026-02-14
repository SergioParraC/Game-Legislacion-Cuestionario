using Foundation.Models;
using Foundation.Repositories;
using System.Collections.Generic;

namespace Facade
{
    public class GameFacade
    {
        private readonly QuestionRepository _questionRepository;
        private readonly GameProgressRepository _progressRepository;

        public GameFacade()
        {
            _questionRepository = new QuestionRepository();
            _progressRepository = new GameProgressRepository();
        }

        public List<Question> GetQuestionsForLevel(int level, int count, bool includeBoss = false)
        {
            var questions = new List<Question>();
            var normalQuestions = _questionRepository.GetQuestionsByDifficulty(level);

            for (int i = 0; i < count && i < normalQuestions.Count; i++)
            {
                questions.Add(normalQuestions[i]);
            }

            if (includeBoss)
            {
                var bossQuestions = _questionRepository.GetQuestionsByDifficulty(4, true);
                if (bossQuestions.Count > 0)
                    questions.Add(bossQuestions[0]);
            }

            return questions;
        }

        public int SaveGameProgress(string playerName, int level, int score)
        {
            var progress = new GameProgressModel
            {
                PlayerName = playerName,
                CurrentLevel = level,
                MaxScore = score,
                DatePlayed = System.DateTime.Now
            };
            return _progressRepository.SaveProgress(progress);
        }

        public void UpdateGameProgress(int progressId, int level, int score)
        {
            var progress = new GameProgressModel
            {
                Id = progressId,
                CurrentLevel = level,
                MaxScore = score,
                DatePlayed = System.DateTime.Now
            };
            _progressRepository.UpdateProgress(progress);
        }

        public List<GameProgressModel> GetLeaderboard(int limit = 10)
        {
            return _progressRepository.GetTopScores(limit);
        }
    }
}
