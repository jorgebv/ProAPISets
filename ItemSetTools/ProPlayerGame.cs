using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemSetTools;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ItemSetTools
{
    /// <summary>
    /// The Entity Framework DbContext we will be using.
    /// The tables are a table of ProPlayerGames, and a
    /// table of ProPlayerItemSets
    /// </summary>
    public class GameContext : DbContext
    {
        public DbSet<ProPlayerGame> Games { get; set; }

        public DbSet<ProPlayerItemSet> ItemSets { get; set; }

        public GameContext() : base("DBConnectionInfo")
        {

        }

        /// <summary>
        /// This makes sure we can serialize the ProPlayerGame into the database
        /// It gets serialized as json using the Serialized property
        /// </summary>
        /// <param name="modelBuilder">modelBuilder</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<GameContext>(new DropCreateDatabaseIfModelChanges<GameContext>());

            modelBuilder.ComplexType<ItemPurchaseTimeline>()
                .Ignore(p => p.Count)
                .Property(p => p.Serialized)
                .HasColumnName("ItemPurchaseTimelineJson");
        }
    }

    /// <summary>
    /// This class is stored in the database and defines the unique key for
    /// an item set
    /// </summary>
    public class ProPlayerItemSet
    {
        [Key]
        [Column(Order = 0)]
        public long SummonerId { get; set; }
        [Key]
        [Column(Order = 1)]
        public long ChampionId { get; set; }
        public virtual string ItemSetJson { get; set; }

        public ProPlayerItemSet()
        {

        }

        public ProPlayerItemSet(long summonerId, long championId, string itemSetJson)
        {
            SummonerId = summonerId;
            ChampionId = championId;
            ItemSetJson = itemSetJson;
        }
    }

    /// <summary>
    /// This class is stored in the database and defines the unique key for a pro player game
    /// </summary>
    public class ProPlayerGame
    {
        [Key][Column(Order = 0)]
        public long SummonerId { get; set; }
        [Key][Column(Order = 1)]
        public long GameId { get; set; }
        public long ChampionId { get; set; }
        public virtual ItemPurchaseTimeline ItemPurchaseTimeline { get; set; }

        /// <summary>
        /// Default constructor. Properties can be set after construction
        /// </summary>
        public ProPlayerGame()
        {
            ItemPurchaseTimeline = new ItemPurchaseTimeline();
        }

        /// <summary>
        /// Constructor that initializes properties with passed in values
        /// </summary>
        /// <param name="summonerId">
        /// Summoner ID the item timeline corresponds to
        /// </param>
        /// <param name="gameId">
        /// Game ID the item timeline corresponds to
        /// </param>
        /// <param name="itemPurchaseTimeline">
        /// The item purchase timeline for the given summoner and game
        /// </param>
        public ProPlayerGame(long summonerId, long gameId, long championId, ItemPurchaseTimeline itemPurchaseTimeline)
        {
            SummonerId = summonerId;
            GameId = gameId;
            ChampionId = championId;
            ItemPurchaseTimeline = itemPurchaseTimeline;
        }
    }
}
