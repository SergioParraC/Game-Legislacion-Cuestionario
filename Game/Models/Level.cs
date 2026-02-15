using System.Collections.Generic;

namespace Game.Models
{
    public class Level
    {
        public int Number { get; set; }
        public int TimePerQuestion { get; set; }
        public int QuestionCount { get; set; }
        public bool HasBoss { get; set; }
        public List<Foundation.Models.Question> Questions { get; set; }
        public Foundation.Models.Question ReserveQuestion { get; set; }

        public Level(int number)
        {
            Number = number;
            Questions = [];

            switch (number)
            {
                case 1:
                    TimePerQuestion = 15;
                    QuestionCount = 5;
                    HasBoss = false;
                    break;
                case 2:
                    TimePerQuestion = 12;
                    QuestionCount = 7;
                    HasBoss = false;
                    break;
                case 3:
                    TimePerQuestion = 10;
                    QuestionCount = 10;
                    HasBoss = false;
                    break;
                case 4:
                    TimePerQuestion = 7;
                    QuestionCount = 3;
                    HasBoss = true;
                    break;
            }
        }
    }
}
