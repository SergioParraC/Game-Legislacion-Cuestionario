using System.Collections.Generic;

namespace Foundation.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int CorrectAnswerIndex { get; set; }
        public List<string> Options { get; set; }
        public int Difficulty { get; set; }
        public bool IsBossQuestion { get; set; }

        public Question()
        {
            Options = [];
        }

        public string GetCorrectAnswer()
        {
            if (CorrectAnswerIndex >= 0 && CorrectAnswerIndex < Options.Count)
                return Options[CorrectAnswerIndex];
            return string.Empty;
        }

        public bool IsCorrectAnswer(string answer)
        {
            return answer == GetCorrectAnswer();
        }

        public bool IsCorrectAnswer(int index)
        {
            return index == CorrectAnswerIndex;
        }
    }
}
