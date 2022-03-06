using System;
using System.Collections.Generic;

namespace FootballBeting.Data.Models
{
    public class Bet
    {
        public int BetId { get; set; }

        public decimal Amounth { get; set; }

        public double Prediction { get; set; }

        public DateTime Datetime { get; set; }

        public int GameId { get; set; }
        public Game Game { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
