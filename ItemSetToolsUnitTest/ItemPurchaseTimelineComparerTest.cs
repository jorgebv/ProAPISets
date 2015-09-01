using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ItemSetTools;

namespace ItemSetToolsUnitTest
{
    [TestClass]
    public class ItemPurchaseTimelineComparerTest
    {
        [TestMethod]
        public void TestItemPurchaseTimelineComparerConstructors()
        {
            var comparer1 = new ItemPurchaseTimelineComparer();
            Assert.IsTrue(comparer1.IgnoreConsumables);
            var comparer2 = new ItemPurchaseTimelineComparer(false);
            Assert.IsFalse(comparer2.IgnoreConsumables);
        }

        [TestMethod]
        public void TestShortTimelinesWithNoConsumablesAreEqual()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // statikk shiv
            timeline1.AddToEndByItemId(3087, TimeSpan.Zero);
            timeline2.AddToEndByItemId(3087, TimeSpan.Zero);

            var comparer = new ItemPurchaseTimelineComparer();
            Assert.AreEqual(0, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestShortTimelinesWithConsumablesAreEqualWhenConsumablesIgnored()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // biscuit
            timeline1.AddToEndByItemId(2009, TimeSpan.Zero);
            // statikk shiv
            timeline1.AddToEndByItemId(3087, TimeSpan.Zero);

            timeline2.AddToEndByItemId(3087, TimeSpan.Zero);

            var comparer = new ItemPurchaseTimelineComparer();
            Assert.AreEqual(0, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestShortTimelinesWithConsumablesAreNotEqualWhenConsumablesConsidered()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // biscuit
            timeline1.AddToEndByItemId(2009, TimeSpan.Zero);
            // statikk shiv
            timeline1.AddToEndByItemId(3087, TimeSpan.Zero);

            timeline2.AddToEndByItemId(3087, TimeSpan.Zero);

            var comparer = new ItemPurchaseTimelineComparer(false);
            Assert.AreEqual(1, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestShortEqualTimelinesWithDifferentLengthsAreEqual()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // statikk shiv
            timeline1.AddToEndByItemId(3087, TimeSpan.Zero);
            // biscuit
            timeline1.AddToEndByItemId(2009, TimeSpan.Zero);

            timeline2.AddToEndByItemId(3087, TimeSpan.Zero);

            var comparer = new ItemPurchaseTimelineComparer(false, false);
            Assert.AreEqual(0, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestOutOfOrderItemsIsDistanceOfTwo()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // statikk shiv then biscuit
            timeline1.AddToEndByItemId(3087, TimeSpan.Zero);
            timeline1.AddToEndByItemId(2009, TimeSpan.Zero);

            // biscuit, then statikk shiv
            timeline2.AddToEndByItemId(2009, TimeSpan.Zero);
            timeline2.AddToEndByItemId(3087, TimeSpan.Zero);

            var comparer = new ItemPurchaseTimelineComparer(false, false);
            Assert.AreEqual(2, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestItemChangeIsDistanceOfOne()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // statikk shiv then biscuit
            timeline1.AddToEndByItemId(3087, TimeSpan.Zero);
            timeline1.AddToEndByItemId(2009, TimeSpan.Zero);

            // statikk shiv then warmogs
            timeline2.AddToEndByItemId(3087, TimeSpan.Zero);
            timeline2.AddToEndByItemId(3083, TimeSpan.Zero);

            var comparer = new ItemPurchaseTimelineComparer(false, false);
            Assert.AreEqual(1, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestAdcStartingItemsAreDistanceOfTwoWithoutTimeGrouping()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // potion, doran's blade, then trinket
            timeline1.AddToEndByItemId(2003, TimeSpan.Zero);
            timeline1.AddToEndByItemId(1055, TimeSpan.Zero);
            timeline1.AddToEndByItemId(3340, TimeSpan.Zero);

            // trinket, dblade, then potion
            timeline2.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline2.AddToEndByItemId(1055, TimeSpan.Zero);
            timeline2.AddToEndByItemId(2003, TimeSpan.Zero);

            var comparer = new ItemPurchaseTimelineComparer(groupItemsByTime: false);
            Assert.AreEqual(2, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestAdcStartingItemsAreDistanceOfZeroWithTimeGrouping()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // potion, doran's blade, then trinket
            timeline1.AddToEndByItemId(2003, TimeSpan.Zero);
            timeline1.AddToEndByItemId(1055, TimeSpan.Zero);
            timeline1.AddToEndByItemId(3340, TimeSpan.Zero);

            // trinket, dblade, then potion
            timeline2.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline2.AddToEndByItemId(1055, TimeSpan.Zero);
            timeline2.AddToEndByItemId(2003, TimeSpan.Zero);

            // don't ignore consumables
            var comparer = new ItemPurchaseTimelineComparer(false);
            Assert.AreEqual(0, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestAdcStartingItemsAreDistanceOfTwoWithTimeGroupingButPurchasedFarApart()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            // potion, doran's blade, then trinket
            timeline1.AddToEndByItemId(2003, TimeSpan.Zero);
            timeline1.AddToEndByItemId(1055, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline1.AddToEndByItemId(3340, TimeSpan.Zero + TimeSpan.FromSeconds(32));

            // trinket, dblade, then potion
            timeline2.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline2.AddToEndByItemId(1055, TimeSpan.Zero);
            timeline2.AddToEndByItemId(2003, TimeSpan.Zero);

            // don't ignore consumables
            var comparer = new ItemPurchaseTimelineComparer(false);
            Assert.AreEqual(2, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestIfReorderingResultsInWorseDistanceWeDontReorderInObviousIdenticalCase()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            timeline1.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline1.AddToEndByItemId(2003, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline1.AddToEndByItemId(1055, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline1.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(48));

            timeline2.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline2.AddToEndByItemId(2003, TimeSpan.Zero);
            timeline2.AddToEndByItemId(1055, TimeSpan.Zero);
            timeline2.AddToEndByItemId(1038, TimeSpan.Zero);

            // reordering would be very bad here, as they were already identical
            var comparer = new ItemPurchaseTimelineComparer(false);
            Assert.AreEqual(0, comparer.DistanceBetween(timeline1, timeline2));
        }

        [TestMethod]
        public void TestIfReorderingResultsInWorseDistanceWeDontReorderInMoreSubtleCase()
        {
            var timeline1 = new ItemPurchaseTimeline();
            var timeline2 = new ItemPurchaseTimeline();

            timeline1.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline1.AddToEndByItemId(2003, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline1.AddToEndByItemId(1055, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline1.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(48));

            timeline2.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline2.AddToEndByItemId(2003, TimeSpan.Zero);
            timeline2.AddToEndByItemId(1055, TimeSpan.Zero);
            timeline2.AddToEndByItemId(3048, TimeSpan.Zero);

            // reordering would be very bad here, as they were already only a distance of 1
            var comparer = new ItemPurchaseTimelineComparer(false);
            Assert.AreEqual(1, comparer.DistanceBetween(timeline1, timeline2));
        }

        // ran into this while testing DBScan, so added it as a regression test here
        // the two timelines under test should be unrelated because the first item and
        // last many items of the long timeline were different
        [TestMethod]
        public void TestVeryLongLongAndDifferentTimelineDoesNotMatchSubTimeline()
        {
            var timeline1 = new ItemPurchaseTimeline();
            timeline1.AddToEndByItemId(3340, TimeSpan.Zero);
            timeline1.AddToEndByItemId(2003, TimeSpan.Zero + TimeSpan.FromSeconds(16));
            timeline1.AddToEndByItemId(1055, TimeSpan.Zero + TimeSpan.FromSeconds(32));
            timeline1.AddToEndByItemId(1038, TimeSpan.Zero + TimeSpan.FromSeconds(48));
            timeline1.AddToEndByItemId(1054, TimeSpan.Zero + TimeSpan.FromSeconds(64));

            var timeline6 = new ItemPurchaseTimeline();
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

            var comparer = new ItemPurchaseTimelineComparer(false);
            Assert.AreEqual(10, comparer.DistanceBetween(timeline1, timeline6));
        }
    }
}
