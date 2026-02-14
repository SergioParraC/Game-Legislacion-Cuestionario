namespace Game.Managers
{
    public class ScoreManager
    {
        private const int BASE_POINTS = 100;
        private const int MAX_POINTS = 500;
        private const int TIME_MULTIPLIER = 2;
        private const int STREAK_BONUS = 50;
        private const int ERROR_PENALTY = -25;

        public int CalculateScore(int timeRemaining, int streak, bool isCorrect, bool useDoublePoints)
        {
            if (!isCorrect)
                return ERROR_PENALTY;

            int score = BASE_POINTS;
            score += timeRemaining * TIME_MULTIPLIER;
            score += streak * STREAK_BONUS;

            if (useDoublePoints)
                score *= 2;

            return score > MAX_POINTS ? MAX_POINTS : score;
        }

        public int ApplyErrorPenalty(int currentScore)
        {
            int newScore = currentScore + ERROR_PENALTY;
            return newScore < 0 ? 0 : newScore;
        }
    }
}
