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
        public bool IsShufflePenaltyActive { get; set; }
        public bool IsGhostAnswerActive { get; set; }
        public List<string> DisplayedOptions { get; private set; }
        public string LastPenaltyMessage { get; set; }
        public PenaltyType Penality { get; set; }
        private bool _applyExtraPenaltyNextQuestion = false; // Bandera para penalización adicional

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
            // Resetear estados de penalizaciones al iniciar un nuevo nivel
            IsShufflePenaltyActive = false;
            IsGhostAnswerActive = false;
            LastPenaltyMessage = "";
            Penality = PenaltyType.Nothing;
            _applyExtraPenaltyNextQuestion = false; 
            
            CurrentLevel = new Level(levelNumber);
            Player.CurrentLevel = levelNumber;
            CurrentQuestionIndex = 0;

            var allQuestions = _gameFacade.GetQuestionsForLevel(
                levelNumber,
                CurrentLevel.QuestionCount,
                CurrentLevel.HasBoss
            );

            // Separar las preguntas: N preguntas activas + 1 de reserva
            if (!CurrentLevel.HasBoss && allQuestions.Count > CurrentLevel.QuestionCount)
            {
                CurrentLevel.ReserveQuestion = allQuestions[allQuestions.Count - 1];
                CurrentLevel.Questions = allQuestions.GetRange(0, CurrentLevel.QuestionCount);
            }
            else
            {
                CurrentLevel.Questions = allQuestions;
                CurrentLevel.ReserveQuestion = null;
            }

            LoadNextQuestion();
        }
        private void ShuffleOptions()
        {
            DisplayedOptions = [.. CurrentQuestion.Options];
            DisplayedOptions = [.. DisplayedOptions.OrderBy(x => _random.Next())];
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
                ShuffleOptions();
                ApplyLevelPenalties();
            }
        }

        public void ShuffleCurrentOptions()
        {
            // Método público para mezclar las opciones actuales desde el formulario
            DisplayedOptions = [.. DisplayedOptions.OrderBy(x => _random.Next())];
        }

        public bool UsePlayerPowerUp(PowerUpType type)
        {
            if (IsPowerUpBlocked)
                return false;

            // Para el Reintento, verificar que haya pregunta de reserva disponible
            if (type == PowerUpType.Retry && CurrentLevel.ReserveQuestion == null)
                return false;

            if (!Player.UsePowerUp(type))
                return false;

            switch (type)
            {
                case PowerUpType.EliminateOption:
                    EliminateWrongOption();
                    break;
                case PowerUpType.ExtraTime:
                    TimeRemaining += 10;
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
                
                // Si tiene Reintento activo y responde bien, lo desactiva sin efecto
                if (IsRetryAvailable)
                {
                    IsRetryAvailable = false;
                }
                
                // Respuesta correcta: NO se aplica penalización adicional a la siguiente pregunta
                _applyExtraPenaltyNextQuestion = false;
                
                // Avanzar a la siguiente pregunta
                CurrentQuestionIndex++;
            }
            else
            {
                // Si tiene reintento disponible, no aplica penalizaciones
                if (!IsRetryAvailable)
                {
                    Player.CorrectStreak = 0;
                    Player.Score = _scoreManager.ApplyErrorPenalty(Player.Score);
                    
                    // Respuesta incorrecta: GARANTIZAR penalización adicional en la siguiente pregunta
                    _applyExtraPenaltyNextQuestion = true;
                    
                    CurrentQuestionIndex++;
                }
                // Si tiene reintento, se manejará el cambio de pregunta en el Form
            }

            return isCorrect;
        }

        private void ApplyLevelPenalties()
        {
            // Si hubo error en la pregunta anterior, GARANTIZAR una penalización adicional
            if (_applyExtraPenaltyNextQuestion)
            {
                ApplyRandomPenalty();
                _applyExtraPenaltyNextQuestion = false; // Resetear bandera
            }
            
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
            Penality = (PenaltyType)penalties.GetValue(_random.Next(penalties.Length));

            switch (Penality)
            {
                case PenaltyType.ReducedTime:
                    TimeRemaining = Math.Max(5, TimeRemaining - 5);
                    LastPenaltyMessage = "⚠️ ¡Penalización! -5 segundos";
                    break; 
                case PenaltyType.ShuffleOptions:
                    IsShufflePenaltyActive = true;
                    ShuffleOptions();  // Mezcla inicial
                    LastPenaltyMessage = "🔀 ¡Penalización! Opciones se mezclarán, atento!!!";
                    break;
                case PenaltyType.BlockPowerUp:
                    IsPowerUpBlocked = true;
                    LastPenaltyMessage = "⚠️ ¡Penalización! Power-ups bloqueados";
                    break;
                case PenaltyType.GhostAnswer:
                    IsGhostAnswerActive = true;
                    LastPenaltyMessage = "👻 ¡Penalización! Respuesta confusa";
                    break;
                case PenaltyType.Nothing:
                    // Sin penalización
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

        public void ChangeToReserveQuestion()
        {
            if (CurrentLevel.ReserveQuestion != null)
            {
                // Cambiar la pregunta actual por la de reserva
                CurrentQuestion = CurrentLevel.ReserveQuestion;
                TimeRemaining = CurrentLevel.TimePerQuestion;
                
                // Eliminar la pregunta de reserva para que solo se use una vez
                CurrentLevel.ReserveQuestion = null;
                
                // Desactivar el reintento
                IsRetryAvailable = false;
                
                // Mezclar las opciones de la nueva pregunta
                ShuffleOptions();
            }
        }
    }
}
