using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemSetTools;
using RiotNet;
using RiotNet.Models;
using System.Data.Entity;
using System.Configuration;

namespace GameIndexer
{
    class Program
    {


        static void Main(string[] args)
        {
            try
            {
                var client = new RiotClient(Region.NA, new RiotClientSettings
                {
                    ApiKey = ConfigurationManager.AppSettings["RiotAPIKey"]
                });

                using (var context = new GameContext())
                {
                    foreach (var proUnderObservation in SummonerIds.Pros)
                    {
                        Console.WriteLine("Processing pro with summoner Id {0}", proUnderObservation);

                        var matchList = client.GetMatchList(proUnderObservation, seasons: new Season[] { Season.SEASON2015 }, rankedQueues: new RankedQueue[] { RankedQueue.RANKED_SOLO_5x5 });

                        Console.WriteLine("We have info on {0} games for this pro", matchList.TotalGames);

                        foreach (var game in matchList.Matches)
                        {
                            // some EUW games were causing problems (maybe region transfer?)
                            if (game.PlatformId.Equals("NA1"))
                            {
                                System.Diagnostics.Debug.WriteLine("Processing game id {0}", game.MatchId);
                                var oldItem = context.Games.SingleOrDefault((t) => t.SummonerId == proUnderObservation && t.GameId == game.MatchId);
                                //var oldItem = context.Games.Find(proUnderObservation, game.MatchId);
                                if (oldItem == null) // not in database already
                                {
                                    var singleMatch = client.GetMatch(game.MatchId, true);
                                    var itemListFromSingleMatch = new ItemPurchaseTimeline(singleMatch, proUnderObservation);
                                    var championId = game.Champion;
                                    var proPlayerGame = new ProPlayerGame(proUnderObservation, game.MatchId, championId, itemListFromSingleMatch);
                                    context.Games.Add(proPlayerGame);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                // may not always have a debugger
                Console.WriteLine(e);
                throw e;
            }
        }
    }
}
