using FootballBeting.Data.Models;
using FootballBeting.Data.Setting;
using Microsoft.EntityFrameworkCore;

namespace FootballBeting.Data
{
    public class FootballBettingContext : DbContext
    {
        public FootballBettingContext()
        {
        }

        public FootballBettingContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Bet> Bets { get; set; }

        public DbSet<Color> Colors { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Game> Games { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<PlayerStatistic> PlayerStatistics { get; set; }

        public DbSet<Possition> Possitions { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Town> Towns { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseSqlServer(DataConnection.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bet>(bet =>
            {
                bet
                    .HasOne(u => u.User)
                    .WithMany(b => b.Bets)
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                bet
                    .HasOne(g => g.Game)
                    .WithMany(b => b.Bets)
                    .HasForeignKey(g => g.GameId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Team>(team => 
            {
                team
                .HasOne(pc => pc.PrimaryKitColor)
                .WithMany(c => c.PrimaryKitTeams)
                .HasForeignKey(pc => pc.PrimaryKitColorId);

                team
                .HasOne(sc => sc.SecondaryKitColor)
                .WithMany(c => c.SecondaryKitTeams)
                .HasForeignKey(sc => sc.SecondaryKitColorId)
                .OnDelete(DeleteBehavior.Restrict);

                team
                .HasOne(t => t.Town)
                .WithMany(tt => tt.Teams)
                .HasForeignKey(t => t.TownId)
                .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Game>(game =>
            {
                game
                    .HasOne(ht => ht.HomeTeam)
                    .WithMany(hg => hg.HomeGames)
                    .HasForeignKey(ht => ht.HomeTeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                game
                    .HasOne(at => at.AwayTeam)
                    .WithMany(ag => ag.AwayGames)
                    .HasForeignKey(at => at.AwayTeamId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Player>(player => 
            {
                player
                    .HasOne(ot => ot.Team)
                    .WithMany(mp => mp.Players)
                    .HasForeignKey(ot => ot.TeamId)
                    .OnDelete(DeleteBehavior.Restrict);

                player
                    .HasOne(p => p.Possition)
                    .WithMany(pp => pp.PlayersPossition)
                    .HasForeignKey(p => p.PossitionId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<PlayerStatistic>(PStatistic => 
            {
                PStatistic.HasKey(ps => new { ps.GameId, ps.PlayerId });

                PStatistic
                    .HasOne(g => g.Game)
                    .WithMany(ps => ps.PlayerStatistics)
                    .HasForeignKey(g => g.GameId)
                    .OnDelete(DeleteBehavior.Restrict);

                PStatistic
                    .HasOne(p => p.Player)
                    .WithMany(ps => ps.PlayerStatistics)
                    .HasForeignKey(p => p.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Town>(town =>
            {
                town
                    .HasOne(c => c.Country)
                    .WithMany(t => t.Towns)
                    .HasForeignKey(c => c.CountryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
