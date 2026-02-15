using Game.Enums;
using System.Collections.Generic;

namespace Game.Models
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public int CurrentLevel { get; set; }
        public int CorrectStreak { get; set; }
        public Dictionary<PowerUpType, int> PowerUps { get; set; }

        public Player(string name)
        {
            Name = name;
            Score = 0;
            CurrentLevel = 1;
            CorrectStreak = 0;
            PowerUps = new Dictionary<PowerUpType, int>
            {
                { PowerUpType.EliminateOption, 2 },
                { PowerUpType.ExtraTime, 2 },
                { PowerUpType.Retry, 2 },
                { PowerUpType.DoublePoints, 2 }
            };
        }

        public bool UsePowerUp(PowerUpType type)
        {
            if (PowerUps[type] > 0)
            {
                PowerUps[type]--;
                return true;
            }
            return false;
        }

        public void AddPowerUp(PowerUpType type, int quantity = 1)
        {
            PowerUps[type] += quantity;
        }
    }
}
