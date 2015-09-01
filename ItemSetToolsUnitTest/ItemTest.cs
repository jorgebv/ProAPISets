using System;
using ItemSetTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ItemSetToolsUnitTest
{
    [TestClass]
    public class ItemTest
    {
        [TestMethod]
        public void TestItemConstructor()
        {
            var item = new Item(3089, new TimeSpan(10000));
            Assert.AreEqual(3089, item.Id);
            Assert.AreEqual(1, item.TimeOfPurchase.TotalMilliseconds);
            Assert.AreEqual("Rabadon's Deathcap", item.Name);
            Assert.IsFalse(item.IsConsumable);
        }

        [TestMethod]
        public void TestIsConsumable()
        {
            var item1 = new Item(2009, new TimeSpan(10000));
            Assert.AreEqual("Total Biscuit of Rejuvenation", item1.Name);
            Assert.IsTrue(item1.IsConsumable);

            var item2 = new Item(2044, TimeSpan.Zero);
            Assert.AreEqual("Stealth Ward", item2.Name);
            Assert.IsTrue(item2.IsConsumable);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException),
            "An item with an invalid item ID was inappropraitely allowed")]
        public void TestInvalidItemIdGeneratesException()
        {
            new Item(-1, new TimeSpan(10));
        }

        [TestMethod]
        public void TestTwoItemsWithSameItemIdAreEqual()
        {
            var item1 = new Item(2009, new TimeSpan(10000));
            var item2 = new Item(2009, TimeSpan.Zero);
            Assert.IsTrue(item1.Equals(item2));
            Assert.IsTrue(item2.Equals(item1));
            // one of the gurantees of equals
            Assert.IsFalse(item1.Equals(null));
            Assert.AreNotSame(item1, item2);
        }

        [TestMethod]
        public void TestCompareTo()
        {
            var item1 = new Item(2009, TimeSpan.Zero);
            var item2 = new Item(2044, TimeSpan.Zero);
            Assert.IsTrue(item1.CompareTo(item2) < 0);
            Assert.IsTrue(item2.CompareTo(item1) > 0);
        }
    }
}
