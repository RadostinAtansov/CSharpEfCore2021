
using System.Collections;
using System.Collections.Generic;

namespace FootballBeting.Data.Models
{
    public class Player
    {
        public int PlayerId { get; set; }

        public string Name { get; set; }

        public int SquadNumber { get; set; }

        public bool IsInjured { get; set; }

        public int TeamId { get; set; }
        public Team Team { get; set; }

        public int PossitionId { get; set; }
        public Possition Possition { get; set; }

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; } = new HashSet<PlayerStatistic>();
    }
}
