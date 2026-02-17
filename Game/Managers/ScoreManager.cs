namespace Game.Managers
{
    public class ScoreManager
    {
        private const int BASE_POINTS = 50;
        private const int TIME_MULTIPLIER = 2;
        private const int ERROR_PENALTY = -25;

        public int CalculateScore(int timeRemaining, int streak, bool isCorrect, bool useDoublePoints)
        {
            if (!isCorrect)
                return ERROR_PENALTY;

            // Calcular puntuación base
            int score = BASE_POINTS;
            score += timeRemaining * TIME_MULTIPLIER;

            // Aplicar bonificación por racha basada en porcentaje
            double streakMultiplier = GetStreakMultiplier(streak);
            score = (int)(score * streakMultiplier);

            if (useDoublePoints)
                score *= 2;

            return score;
        }

        private double GetStreakMultiplier(int streak)
        {
            if (streak == 0)
                return 1.0; // Sin racha, sin bonificación
            else if (streak == 1)
                return 1.05; // 5% adicional
            else if (streak >= 2 && streak <= 4)
                return 1.10; // 10% adicional
            else if (streak >= 5 && streak <= 7)
                return 1.20; // 20% adicional
            else // streak >= 8
                return 1.40; // 40% adicional
        }

        public int ApplyErrorPenalty(int currentScore)
        {
            int newScore = currentScore + ERROR_PENALTY;
            return newScore;
        }
    }
}
