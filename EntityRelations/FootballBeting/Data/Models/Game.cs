
using System;
using System.Collections;
using System.Collections.Generic;

namespace FootballBeting.Data.Models
{
    public class Game
    {
        public int GameId { get; set; }

        public int HomeTeamGoals { get; set; }

        public int AwayTeamGoals { get; set; }

        public DateTime Datetime { get; set; }

        public double HomeTeamBetRate { get; set; }

        public double AwayTeamBetRate { get; set; }

        public double DrawBetRate { get; set; }

        public double Result { get; set; }

        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }

        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }

        //public ICollection<Game> HomeGames { get; set; } = new HashSet<Game>();
        //public ICollection<Game> AwayGames { get; set; } = new HashSet<Game>();
        public ICollection<PlayerStatistic> PlayerStatistics { get; set; } = new HashSet<PlayerStatistic>();
        public ICollection<Bet> Bets { get; set; } = new HashSet<Bet>();
    }
}
