using ItemSetTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ItemSetToolsUnitTest
{
    [TestClass]
    public class ItemSetTest
    {
        [TestMethod]
        public void TestJsonGenerationSucceeds()
        {
            var itemsInItemBlock = new List<Item>();
            itemsInItemBlock.Add(new Item(3060, TimeSpan.Zero));
            var anItemBlock = new ItemBlock("my first block", itemsInItemBlock);
            var itemBlocks = new List<ItemBlock>();
            itemBlocks.Add(anItemBlock);
            var itemSet = new ItemSet("first item set", itemBlocks);
            var json = itemSet.ToJson();

            string expectedJsonOutput = @"
            {
              ""title"": ""first item set"",
              ""type"": ""custom"",
              ""map"": ""SR"",
              ""mode"": ""CLASSIC"",
              ""priority"": false,
              ""sortrank"": 0,
              ""blocks"": [
                {
                  ""type"": ""my first block"",
                  ""recMath"": false,
                  ""minSummonerLevel"": -1,
                  ""maxSummonerLevel"": -1,
                  ""showIfSummonerSpell"": """",
                  ""hideIfSummonerSpell"": """",
                  ""items"": [
                    {
                      ""id"": ""3060"",
                      ""count"": 1
                    }
                  ]
                }
              ]
            }";
            Assert.IsNotNull(json);
            string a = System.Text.RegularExpressions.Regex.Replace(expectedJsonOutput, @"\s", "");
            string b = System.Text.RegularExpressions.Regex.Replace(json, @"\s", "");
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void TestItemBlockConstructor()
        {
            var itemList = new List<Item>();
            var name = "testname";
            var itemBlock = new ItemBlock(name, itemList);
            Assert.AreSame(itemList, itemBlock.Items);
            Assert.AreSame(name, itemBlock.Name);
        }

        [TestMethod]
        public void TestStuff()
        {
            using (GameContext context = new GameContext())
            {
                ProPlayerItemSet itemSet = context.ItemSets.SingleOrDefault(t => t.SummonerId == SummonerIds.C9.Balls && t.ChampionId == 8);
                Assert.IsNotNull(itemSet);
            }
        }
    }
}