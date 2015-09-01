using System;
using ItemSetTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiotNet.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ItemSetToolsUnitTest
{
    [TestClass]
    public class ItemPurchaseTimelineTest
    {
        [TestMethod]
        public void TestItemPurchaseTimelineSimpleConstructor()
        {
            var timeline = new ItemPurchaseTimeline();
            Assert.AreEqual(0, timeline.Count);
        }

        [TestMethod]
        public void TestAddToEndSucceeds()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEnd(new Item(2009, TimeSpan.Zero));
            timeline.AddToEnd(new Item(2009, TimeSpan.Zero));
            timeline.AddToEnd(new Item(2009, new TimeSpan(10)));
            Assert.AreEqual(3, timeline.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "An item with an invalid time was inappropraitely allowed to be added")]
        public void TestAddToEndExceptionsWithInvalidTime()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEnd(new Item(2009, new TimeSpan(10)));
            timeline.AddToEnd(new Item(2009, TimeSpan.Zero));
        }

        [TestMethod]
        public void TestAddToEndByItemIdSucceeds()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(2009, TimeSpan.Zero);
            timeline.AddToEndByItemId(2009, TimeSpan.Zero);
            timeline.AddToEndByItemId(2009, new TimeSpan(10));
            Assert.AreEqual(3, timeline.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "An item with an invalid time was inappropraitely allowed to be added")]
        public void TestAddToEndByItemIdExceptionsWithInvalidTime()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(2009, new TimeSpan(10));
            timeline.AddToEndByItemId(2009, TimeSpan.Zero);
        }

        [TestMethod]
        public void TestIsEnumerableInOrder()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(2009, new TimeSpan(10));
            timeline.AddToEndByItemId(3087, new TimeSpan(20));
            timeline.AddToEndByItemId(3290, new TimeSpan(30));
            Assert.AreEqual(3, timeline.Count);

            var previousItemId = 0;
            var previousTimestamp = TimeSpan.Zero;
            foreach(var item in timeline)
            {
                Assert.IsTrue(item.Id > previousItemId);
                Assert.IsTrue(item.TimeOfPurchase > previousTimestamp);

                previousItemId = item.Id;
                previousTimestamp = item.TimeOfPurchase;
            }
        }

        [TestMethod]
        public void TestIsArrayIndexable()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(2009, new TimeSpan(10));
            timeline.AddToEndByItemId(3087, new TimeSpan(20));
            timeline.AddToEndByItemId(3290, new TimeSpan(30));
            Assert.AreEqual(3, timeline.Count);
            Assert.AreEqual(2009, timeline[0].Id);
            Assert.AreEqual(3087, timeline[1].Id);
            Assert.AreEqual(3290, timeline[2].Id);
        }

        [TestMethod]
        public void TestToString()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(3086, new TimeSpan(10));
            timeline.AddToEndByItemId(3087, new TimeSpan(20));
            Assert.AreEqual("Zeal -> Statikk Shiv", timeline.ToString());
        }

        [TestMethod]
        public void TestOneBigItemBlock()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(3086, new TimeSpan(10));
            timeline.AddToEndByItemId(3087, new TimeSpan(20));
            timeline.AddToEndByItemId(3087, new TimeSpan(20));
            var itemBlocks = timeline.TryToSplitIntoPurchases(15);
            Assert.AreEqual(1, itemBlocks.Count);
            Assert.AreEqual("First Purchase", itemBlocks[0].Name);
        }

        [TestMethod]
        public void TestTwoItemBlock()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(3086, TimeSpan.Zero);
            timeline.AddToEndByItemId(3087, TimeSpan.FromSeconds(14));
            timeline.AddToEndByItemId(3087, TimeSpan.FromSeconds(16));
            var itemBlocks = timeline.TryToSplitIntoPurchases(15);
            Assert.AreEqual(2, itemBlocks.Count);
            Assert.AreEqual(2, itemBlocks[0].Items.Count);
            Assert.AreEqual("First Purchase", itemBlocks[0].Name);
            Assert.AreEqual(1, itemBlocks[1].Items.Count);
            Assert.AreEqual("Next Purchase", itemBlocks[1].Name);
        }

        [TestMethod]
        public void TestMatchDetailConstructor()
        {
            var matchDetails = new MatchDetail();
            matchDetails.ParticipantIdentities = new List<MatchParticipantIdentity>();
            var identityWeIgnore = new MatchParticipantIdentity();
            identityWeIgnore.ParticipantId = 3;
            identityWeIgnore.Player = new MatchPlayer();
            identityWeIgnore.Player.SummonerId = SummonerIds.C9.Sneaky;
            matchDetails.ParticipantIdentities.Add(identityWeIgnore);

            var identityWeWant = new MatchParticipantIdentity();
            identityWeWant.ParticipantId = 4;
            identityWeWant.Player = new MatchPlayer();
            identityWeWant.Player.SummonerId = SummonerIds.C9.Balls;
            matchDetails.ParticipantIdentities.Add(identityWeWant);

            matchDetails.Timeline = new Timeline();
            matchDetails.Timeline.Frames = new List<Frame>();
            var framesList = matchDetails.Timeline.Frames;

            var frame = new Frame();
            frame.Events = new List<Event>();
            
            var eventWeWant = new Event();
            eventWeWant.EventType = EventType.ITEM_PURCHASED;
            eventWeWant.ParticipantId = 4;
            eventWeWant.ItemId = 3068;

            var eventWeIgnore = new Event();
            eventWeIgnore.EventType = EventType.ITEM_PURCHASED;
            eventWeIgnore.ParticipantId = 3;
            eventWeIgnore.ItemId = 3087;

            frame.Events.Add(eventWeIgnore);
            frame.Events.Add(eventWeWant);

            framesList.Add(frame);

            var timeline = new ItemPurchaseTimeline(matchDetails, SummonerIds.C9.Balls);
            Assert.AreEqual(1, timeline.Count);
            Assert.AreEqual(3068, timeline[0].Id);

            timeline = new ItemPurchaseTimeline(matchDetails, SummonerIds.C9.Sneaky);
            Assert.AreEqual(1, timeline.Count);
            Assert.AreEqual(3087, timeline[0].Id);
        }

        [TestMethod]
        public void TestJsonSerialization()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(3086, TimeSpan.Zero);
            timeline.AddToEndByItemId(3087, TimeSpan.FromSeconds(14));
            timeline.AddToEndByItemId(3087, TimeSpan.FromSeconds(16));
            var json = JsonConvert.SerializeObject(timeline);
            var enumerable = JsonConvert.DeserializeObject<IEnumerable<Item>>(json);
            var newTimeilne = new ItemPurchaseTimeline(enumerable);
            Assert.AreEqual(3, newTimeilne.Count);
            Assert.AreEqual(3086, newTimeilne[0].Id);
            Assert.AreEqual(3087, newTimeilne[1].Id);
            Assert.AreEqual(3087, newTimeilne[2].Id);
        }

        [TestMethod]
        public void TestCanBeWrittenToDatabase()
        {
            var timeline = new ItemPurchaseTimeline();
            timeline.AddToEndByItemId(3086, TimeSpan.Zero);
            timeline.AddToEndByItemId(3087, TimeSpan.FromSeconds(14));
            timeline.AddToEndByItemId(3087, TimeSpan.FromSeconds(16));

            var context = new GameContext();
            var proGame = new ProPlayerGame();
            proGame.GameId = 1;
            proGame.SummonerId = 5;
            proGame.ItemPurchaseTimeline = timeline;
            context.Games.Add(proGame);
            context.SaveChanges();
        }
    }
}
