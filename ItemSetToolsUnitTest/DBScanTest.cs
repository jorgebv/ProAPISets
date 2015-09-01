using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItemSetTools;

namespace ItemSetToolsUnitTest
{
    [TestClass]
    public class DBScanTest
    {
        private ItemPurchaseTimeline timeline1;
        private ItemPurchaseTimeline timeline2;
        private ItemPurchaseTimeline timeline3;
        private ItemPurchaseTimeline timeline4;
        private ItemPurchaseTimeline timeline5;
        private ItemPurchaseTimeline timeline6;

        [TestInitialize]
        public void TestSetup()
        {
            timeline1 = new ItemPurchaseTimeline();
            timeline1.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline1.AddToEndByItemId(2003, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline1.AddToEndByItemId(1055, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline1.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(48));
            timeline1.AddToEndByItemId(1054, TimeSpan.Zero + TimeSpan.FromSeconds(64));

            // timeline 2 will be similar to item 1, but not identical
            timeline2 = new ItemPurchaseTimeline();
            timeline2.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline2.AddToEndByItemId(2003, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline2.AddToEndByItemId(1055, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline2.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(48));
            timeline2.AddToEndByItemId(2049, TimeSpan.Zero + TimeSpan.FromSeconds(64));

            // timeline 3 is the same as game 1 but is longer. it should be included
            // in their cluster because it is so close (0 distance) to 1
            timeline3 = new ItemPurchaseTimeline();
            timeline3.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline3.AddToEndByItemId(2003, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline3.AddToEndByItemId(1055, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline3.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(48));
            timeline3.AddToEndByItemId(1054, TimeSpan.Zero + TimeSpan.FromSeconds(64));
            timeline3.AddToEndByItemId(2049, TimeSpan.Zero + TimeSpan.FromSeconds(80));
            timeline3.AddToEndByItemId(1037, TimeSpan.Zero + TimeSpan.FromSeconds(96));
            timeline3.AddToEndByItemId(1036, TimeSpan.Zero + TimeSpan.FromSeconds(112));
            timeline3.AddToEndByItemId(1039, TimeSpan.Zero + TimeSpan.FromSeconds(128));
            timeline3.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(144));

            // timeline 4 is a new build
            timeline4 = new ItemPurchaseTimeline();
            timeline4.AddToEndByItemId(3041, TimeSpan.Zero);
            timeline4.AddToEndByItemId(3181, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline4.AddToEndByItemId(3042, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline4.AddToEndByItemId(3301, TimeSpan.Zero + TimeSpan.FromSeconds(48));
            timeline4.AddToEndByItemId(1052, TimeSpan.Zero + TimeSpan.FromSeconds(64));

            // timeline 5 is a duplicate of build 4
            timeline5 = new ItemPurchaseTimeline();
            timeline5.AddToEndByItemId(3041, TimeSpan.Zero);
            timeline5.AddToEndByItemId(3181, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline5.AddToEndByItemId(3042, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline5.AddToEndByItemId(3301, TimeSpan.Zero + TimeSpan.FromSeconds(48));
            timeline5.AddToEndByItemId(1052, TimeSpan.Zero + TimeSpan.FromSeconds(64));

            // timeline 6 is just too different to be in anything and should be noise
            timeline6 = new ItemPurchaseTimeline();
            timeline6.AddToEndByItemId(3041, TimeSpan.Zero);
            timeline6.AddToEndByItemId(3340, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline6.AddToEndByItemId(2003, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline6.AddToEndByItemId(1055, TimeSpan.Zero + TimeSpan.FromSeconds(48));
            timeline6.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(64));
            timeline6.AddToEndByItemId(1054, TimeSpan.Zero + TimeSpan.FromSeconds(80));
            timeline6.AddToEndByItemId(2049, TimeSpan.Zero + TimeSpan.FromSeconds(96));
            timeline6.AddToEndByItemId(1037, TimeSpan.Zero + TimeSpan.FromSeconds(112));
            timeline6.AddToEndByItemId(1036, TimeSpan.Zero + TimeSpan.FromSeconds(128));
            timeline6.AddToEndByItemId(1039, TimeSpan.Zero + TimeSpan.FromSeconds(144));
            timeline6.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(160));
            timeline6.AddToEndByItemId(3181, TimeSpan.Zero + TimeSpan.FromSeconds(176));
            timeline6.AddToEndByItemId(3042, TimeSpan.Zero + TimeSpan.FromSeconds(192));
            timeline6.AddToEndByItemId(3301, TimeSpan.Zero + TimeSpan.FromSeconds(208));
            timeline6.AddToEndByItemId(1052, TimeSpan.Zero + TimeSpan.FromSeconds(224));
        }

        [TestMethod]
        public void TestTwoClustersAreIdentifiedWithTwoRadicallyDifferentBuilds()
        {
            var grouper = new DBScan();
            var dbScanList = new List<DBScanProPlayerGame>();
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 1, 3, timeline1)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 2, 3, timeline2)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 3, 3, timeline3)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 4, 3, timeline4)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 5, 3, timeline5)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 6, 3, timeline6)));

            var results = grouper.GetClusters(dbScanList, 3, 2);
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void TestCenterOfCluster()
        {
            var grouper = new DBScan();
            var dbScanList = new List<DBScanProPlayerGame>();
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 1, 3, timeline1)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 2, 3, timeline2)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 3, 3, timeline3)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 4, 3, timeline4)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 5, 3, timeline5)));
            dbScanList.Add(new DBScanProPlayerGame(new ProPlayerGame(SummonerIds.C9.Sneaky, 6, 3, timeline6)));

            var results = grouper.GetClusters(dbScanList, 3, 2);
            Assert.AreEqual(2, results.Count);
            var centerOfCluster1 = grouper.FindCenterOfCluster(results[0]);

            // 3 should be the center because 1 is closest to 2 and 3 but 3 is equidistant to 1 and is longer
            Assert.AreEqual(3, centerOfCluster1.Game.GameId);

            var centerOfCluster2 = grouper.FindCenterOfCluster(results[1]);
            // this is not interesting because the cluster has 2 identical games, but make sure we don't choke.
            // it isn't important if 4 or 5 is the center, but our algorithm will stumble upon 4 first so it
            // should be returned
            Assert.AreEqual(4, centerOfCluster2.Game.GameId);
        }
    }
}
