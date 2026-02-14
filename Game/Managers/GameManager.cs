using Foundation.Models;
using Game.Enums;
using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Managers
{
    public class GameManager
    {
        public Player Player { get; private set; }
        public Level CurrentLevel { get; private set; }
        public Question CurrentQuestion { get; private set; }
        public int CurrentQuestionIndex { get; private set; }
        public int TimeRemaining { get; set; }
        public bool IsPowerUpBlocked { get; set; }
        public bool IsDoublePointsActive { get; set; }
        public bool IsRetryAvailable { get; set; }
        public List<string> DisplayedOptions { get; private set; }
        public string LastPenaltyMessage { get; set; }

        private readonly ScoreManager _scoreManager;
        private readonly Facade.GameFacade _gameFacade;
        private readonly Random _random;

        public GameManager(string playerName)
        {
            Player = new Player(playerName);
            _scoreManager = new ScoreManager();
            _gameFacade = new Facade.GameFacade();
            _random = new Random();
            CurrentQuestionIndex = 0;
        }

        public void StartLevel(int levelNumber)
        {
            CurrentLevel = new Level(levelNumber);
            Player.CurrentLevel = levelNumber;
            CurrentQuestionIndex = 0;

            CurrentLevel.Questions = _gameFacade.GetQuestionsForLevel(
                levelNumber,
                CurrentLevel.QuestionCount,
                CurrentLevel.HasBoss
            );

            LoadNextQuestion();
        }

        public void LoadNextQuestion()
        {
            if (CurrentQuestionIndex < CurrentLevel.Questions.Count)
            {
                CurrentQuestion = CurrentLevel.Questions[CurrentQuestionIndex];
                TimeRemaining = CurrentLevel.TimePerQuestion;
                IsPowerUpBlocked = false;
                IsDoublePointsActive = false;
                IsRetryAvailable = false;
                LastPenaltyMessage = string.Empty;

                ShuffleOptions();
                ApplyLevelPenalties();

            }
        }

        private void ShuffleOptions()
        {
            DisplayedOptions = [.. CurrentQuestion.Options];
            DisplayedOptions = [.. DisplayedOptions.OrderBy(x => _random.Next())];
        }

        public bool UsePlayerPowerUp(PowerUpType type)
        {
            if (IsPowerUpBlocked)
                return false;

            if (!Player.UsePowerUp(type))
                return false;

            switch (type)
            {
                case PowerUpType.EliminateOption:
                    EliminateWrongOption();
                    break;
                case PowerUpType.ExtraTime:
                    TimeRemaining += 5;
                    break;
                case PowerUpType.Retry:
                    IsRetryAvailable = true;
                    break;
                case PowerUpType.DoublePoints:
                    IsDoublePointsActive = true;
                    break;
            }

            return true;
        }

        private void EliminateWrongOption()
        {
            var correctAnswer = CurrentQuestion.GetCorrectAnswer();
            var wrongOptions = DisplayedOptions.Where(o => o != correctAnswer).ToList();

            if (wrongOptions.Count > 0)
            {
                string optionToRemove = wrongOptions[_random.Next(wrongOptions.Count)];
                DisplayedOptions.Remove(optionToRemove);
            }
        }

        public bool CheckAnswer(string answer)
        {
            bool isCorrect = CurrentQuestion.IsCorrectAnswer(answer);

            if (isCorrect)
            {
                Player.CorrectStreak++;
                int earnedPoints = _scoreManager.CalculateScore(
                    TimeRemaining,
                    Player.CorrectStreak,
                    true,
                    IsDoublePointsActive
                );
                Player.Score += earnedPoints;
            }
            else
            {
                Player.CorrectStreak = 0;
                Player.Score = _scoreManager.ApplyErrorPenalty(Player.Score);
                ApplyRandomPenalty();
            }

            CurrentQuestionIndex++;
            return isCorrect;
        }

        private void ApplyLevelPenalties()
        {
            if (CurrentLevel.Number >= 2 && _random.Next(100) < 20)
            {
                ApplyRandomPenalty();
            }

            if (CurrentLevel.Number >= 3 && _random.Next(100) < 40)
            {
                ApplyRandomPenalty();
            }

            if (CurrentLevel.HasBoss)
            {
                ApplyBossPenalty();
            }
        }

        private void ApplyRandomPenalty()
        {
            var penalties = Enum.GetValues(typeof(PenaltyType));
            var penalty = (PenaltyType)penalties.GetValue(_random.Next(penalties.Length));

            switch (penalty)
            {
                case PenaltyType.ReducedTime:
                    TimeRemaining = Math.Max(3, TimeRemaining - 3);
                    LastPenaltyMessage = "?? ¡Penalización! -3 segundos";
                    break;
                case PenaltyType.ShuffleOptions:
                    ShuffleOptions();
                    LastPenaltyMessage = "?? ¡Penalización! Opciones mezcladas";
                    break;
                case PenaltyType.BlockPowerUp:
                    IsPowerUpBlocked = true;
                    LastPenaltyMessage = "?? ¡Penalización! Power-ups bloqueados";
                    break;
            }
        }

        private void ApplyBossPenalty()
        {
            TimeRemaining -= 2;
            IsPowerUpBlocked = true;
            LastPenaltyMessage = "?? ¡JEFE FINAL! Power-ups bloqueados y -2 segundos";
        }

        public bool IsLevelComplete()
        {
            return CurrentQuestionIndex >= CurrentLevel.Questions.Count;
        }

        public void SaveProgress()
        {
            _gameFacade.SaveGameProgress(Player.Name, Player.CurrentLevel, Player.Score);
        }
    }
}
