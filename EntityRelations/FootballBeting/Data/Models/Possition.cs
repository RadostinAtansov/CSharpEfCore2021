
using System.Collections;
using System.Collections.Generic;

namespace FootballBeting.Data.Models
{
    public class Possition
    {
        public int PossitionId { get; set; }

        public string Name { get; set; }

        public ICollection<Player> PlayersPossition { get; set; } = new HashSet<Player>();
    }
}
