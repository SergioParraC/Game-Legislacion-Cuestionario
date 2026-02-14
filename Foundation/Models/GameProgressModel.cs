using System;

namespace Foundation.Models
{
    public class GameProgressModel
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int CurrentLevel { get; set; }
        public int MaxScore { get; set; }
        public DateTime DatePlayed { get; set; }
    }
}
