using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemSetTools;

namespace ItemSetGenerator
{
    class ItemSetGenerator
    {
        static void Main(string[] args)
        {
            using (var context = new GameContext())
            {
                var summonerIdsInTable = (from y in context.Games
                                       select y.SummonerId).Distinct().ToList();
                Console.WriteLine("There are {0} summoners in the database", summonerIdsInTable.Count());

                foreach (var proPlayerSummonerId in summonerIdsInTable)
                {
                    Console.WriteLine("Running through summoner id {0}", proPlayerSummonerId);
                    // we will only even both to run any algorithm to try an generate
                    // an item set if the pro has more than 5 games on the champion
                    var listOfGamesAccordingToChampion = (from x in context.Games
                                                          where x.SummonerId == proPlayerSummonerId
                                                          group x by x.ChampionId into champLists
                                                          where champLists.Count() >= 5
                                                          select champLists).ToList();

                    // the champList contains all games this pro played with 1 particular champ
                    foreach (var champList in listOfGamesAccordingToChampion)
                    {
                        Console.WriteLine("Running through champ id {0}", champList.Key);
                        Console.WriteLine("Games count on this champ is is {0}", champList.Count());

                        var dbScanList = new List<DBScanProPlayerGame>();
                        foreach (var game in champList)
                        {
                            System.Diagnostics.Debug.WriteLine("Game Id is {0}", game.GameId);
                            var dbScanProPlayerGame = new DBScanProPlayerGame(game);
                            dbScanList.Add(dbScanProPlayerGame);
                        }

                        Console.WriteLine("Running DBScan on games");
                        var dbScan = new DBScan();
                        var clusters = dbScan.GetClusters(dbScanList, 5, 3);

                        foreach (var oneCluster in clusters)
                        {
                            Console.WriteLine("Cluster of count {0} identified", oneCluster.Count);
                        }

                        if (clusters.Count != 0)
                        {
                            Console.WriteLine("Trying to find biggest cluster");
                            var biggestCluster = (from cluster in clusters
                                                  orderby cluster.Count
                                                  select cluster).First();

                            var centerOfBiggestCluster = dbScan.FindCenterOfCluster(biggestCluster);
                            var timelineFromCenter = centerOfBiggestCluster.Game.ItemPurchaseTimeline;
                            var itemBlocks = timelineFromCenter.TryToSplitIntoPurchases(15);

                            var itemSet = new ItemSet("Item Set", itemBlocks);
                            var itemSetJson = itemSet.ToJson();
                            Console.WriteLine("Generated item set with json\n{0}", itemSetJson);

                            var oldItem = context.ItemSets.SingleOrDefault((t) => t.SummonerId == proPlayerSummonerId && t.ChampionId == champList.Key);
                            //var oldItem = context.ItemSets.Find(proPlayerSummonerId, champList.Key);
                            if (oldItem != null)
                            {
                                oldItem.ItemSetJson = itemSetJson;
                            }
                            else
                            {
                                context.ItemSets.Add(new ProPlayerItemSet(proPlayerSummonerId, champList.Key, itemSetJson));
                            }
                        }
                        else
                        {
                            Console.WriteLine("No clusters found");
                        }
                    }
                }
                context.SaveChanges();
            }
            // TODO: remove
            Console.ReadLine();
        }
    }
}
