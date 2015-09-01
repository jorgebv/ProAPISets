using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemSetTools
{
    public class Item : IEquatable<Item>, IComparable<Item>
    {
        /// <summary>
        /// English name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Numeric ID of the item in Riot Games API
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// In-game time the item was purchased
        /// </summary>
        public TimeSpan TimeOfPurchase { get; set; }

        public bool IsConsumable { get; set; }

        /// <summary>
        /// Constructor initializes a new item based on the item ID and purchase time
        /// </summary>
        /// <param name="itemId">
        /// Item ID as given in Riot Games API
        /// </param>
        /// <param name="purchaseTime">
        /// In-game time of purchase
        /// </param>
        public Item(int itemId, TimeSpan purchaseTime)
        {
            TimeOfPurchase = purchaseTime;
            Id = itemId;
            var apiItem = ItemUtils.GetAPIItemForItemId(itemId);
            Name = apiItem.Name;
            IsConsumable = apiItem.Consumed;
        }

        /// <summary>
        /// Json serializer beneits from this constructor, but there are no
        /// guarantees the properties are correct. Other constructor should
        /// be used in most cases
        /// </summary>
        public Item()
        {
        }

        /// <summary>
        /// Equality is determined solely by the item ID (for our purposes)
        /// </summary>
        /// <param name="other">Item being compared against</param>
        /// <returns>If the item IDs are equal</returns>
        public bool Equals(Item other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Id == other.Id;
        }

        public int CompareTo(Item other)
        {
            return this.Id - other.Id;
        }
    }

    internal class ItemJsonModel
    {
        public string id { get; set; }
        public int count { get; set; }
    }
}
