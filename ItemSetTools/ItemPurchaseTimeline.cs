using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotNet.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace ItemSetTools
{
    /// <summary>
    /// ItemPurchaseTimeline is a line of items in chronological purchase time
    /// It can be added to via itemId or Item, and iterated over through a
    /// standard iterator (foreach loop)
    /// </summary>
    public class ItemPurchaseTimeline : IEnumerable<Item>
    {
        // contains an list of Items sorted by purchase time
        private List<Item> itemList;

        public Item this[int index]
        {
            get
            {
                return itemList[index];
            }
        }

        /// <summary>
        /// Gets the number of elements actually contained in the ItemPurchaseTimeline.
        /// </summary>
        public int Count 
        {
            get
            {
                return itemList.Count;
            }
        }

        [JsonIgnore]
        public string Serialized
        {
            get
            {
                return JsonConvert.SerializeObject(this);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                var jData = JsonConvert.DeserializeObject<IEnumerable<Item>>(value);
                this.itemList.Clear();
                this.itemList.AddRange(jData);
            }
        }

        /// <summary>
        /// Constructs an empty ItemPurchaseTimeline
        /// </summary>
        public ItemPurchaseTimeline()
        {
            itemList = new List<Item>();
        }

        /// <summary>
        /// Constructs an itempurchasetimeline from the given match timeline
        /// and the provided summoner ID
        /// </summary>
        /// <param name="match">
        /// The match the timeline should be extracted from
        /// </param>
        /// <param name="summonerId">
        /// The summoner ID of a player in the match whose
        /// timeline to extract
        /// </param>
        public ItemPurchaseTimeline(MatchDetail match, long summonerId) : this()
        {
            var proParticipantId = (from participantIdentity in match.ParticipantIdentities
                                    where participantIdentity.Player.SummonerId == summonerId
                                    select participantIdentity.ParticipantId).ElementAt(0);

            foreach (var frame in match.Timeline.Frames)
            {
                if (frame.Events != null)
                {
                    foreach (var singleEvent in frame.Events)
                    {
                        if (singleEvent.EventType == EventType.ITEM_PURCHASED
                            && singleEvent.ParticipantId == proParticipantId)
                        {
                            try
                            {
                                AddToEndByItemId(singleEvent.ItemId, singleEvent.Timestamp);
                            }
                            catch(ArgumentException)
                            {
                                System.Diagnostics.Debug.WriteLine("Dropped item with id {0} on game version {1}", singleEvent.ItemId, match.MatchVersion);
                            }
                        }
                    }
                }
            }
        }

        public ItemPurchaseTimeline(IEnumerable<Item> enumerable) : this()
        {
            itemList.AddRange(enumerable);
        }

        /// <summary>
        /// Add an item to the end of the timeline with
        /// zero timestamp. May fail if the zero timestamp
        /// is not a valid end of list entry.
        /// </summary>
        /// <param name="itemId">
        /// ID of the item to be added. If null, method will
        /// succeed but item timeline will not be added.
        /// </param>
        public void AddToEndByItemId(int? itemId)
        {
            AddToEndByItemId(itemId, TimeSpan.Zero);
        }

        /// <summary>
        /// <summary>
        /// Add an item to the end of the timeline by item ID
        /// </summary>
        /// <param name="itemId">
        /// ID of the item to be added. If null, method will
        /// succeed but item timeline will not be added.
        /// </param>
        /// <param name="gameTimePurchased">
        /// In-game time the item was purchased
        /// </param>
        public void AddToEndByItemId(int? itemId, TimeSpan gameTimePurchased)
        {
            if (null == itemId)
            {
                return;
            }

            AddToEnd(new Item(itemId.Value, gameTimePurchased));
        }

        public void AddToEnd(Item itemToAdd)
        {
            if (itemList.Count != 0 && itemList.Last().TimeOfPurchase > itemToAdd.TimeOfPurchase)
            {
                throw new ArgumentException("Gametime is not chronologically later than current last item in list.");
            }

            itemList.Add(itemToAdd);
        }

        /// <summary>
        /// Try to split this item timeline into distinct store visits the player made.
        /// We do this by iterating over the items. Every time we get to a new item,
        /// we open a windows for "purchaseWindowInSeconds" seconds, and the items
        /// purchased in that time are considered one purchase and grouped into an
        /// itemblock. These itemblocks are then returned
        /// </summary>
        /// <param name="purchaseWindowInSeconds">
        /// The length of the window we should use when considering a store session
        /// </param>
        /// <returns>
        /// A list of distinct item blocks generated using the passed in window</returns>
        public List<ItemBlock> TryToSplitIntoPurchases(int purchaseWindowInSeconds)
        {
            var itemBlocks = new List<ItemBlock>();

            for (int i = 0; i < Count; i++)
            {
                // in the case that i is the last item, we will not enter the y loop
                // this implies that the last item is not within the boundar of the items
                // before it, and we need to make sure to add this last item on its own
                if (i == Count - 1)
                {
                    // add a new item instead of adding the existing one. we do this
                    // because our InsertIntoListSortedByItemId drops the timestamp
                    // on everything, and our itemcollection expects sorted timestamps
                    // the timestamps aren't valuable after this so there is no loss
                    itemBlocks.Add(new ItemBlock("Next Purchase", new List<Item>{ itemList[i] }));
                }

                for (int y = i + 1; y < Count; y++)
                {
                    var differenceInTimes = (itemList[y].TimeOfPurchase - itemList[i].TimeOfPurchase).TotalSeconds;
                    if (differenceInTimes < purchaseWindowInSeconds)
                    {
                        // this item is inside our threshold.
                        if (y == Count - 1)
                        {
                            // this item is inside our threshold, but is also the last item. we need to add it
                            // and what we've been building to the list. then set i = y to let the "i" loop know
                            // that this item is covered ("i" loop should then exit because it will increment
                            // after this, and be equal to Count)
                            itemBlocks.Add(BuildItemBlockFromRange(i, Count));
                            i = y;
                        }
                        else
                        {
                            // this item is inside our threshold, and is not the last item. keep going through the
                            // y loop until we hit one of the other cases
                            // continue is not explicity necessary but adds to readablity (for me)
                            continue;
                        }
                    }
                    else
                    {
                        // this item is out of range of our threshold. we need to consider it in the "i"
                        // loop. i will be incremented at the end of this loop around, so set i to one less
                        // then what we want. Then exit the "y" loop.
                        itemBlocks.Add(BuildItemBlockFromRange(i, y));
                        i = y - 1;
                        break;
                    }
                }
            }

            if (itemBlocks.Count != 0)
            {
                itemBlocks[0].Name = "First Purchase";
            }

            return itemBlocks;
        }

        /// <summary>
        /// Takes the items from startRange to endRange (noninclusive) and returns an itemblock
        /// containing them
        /// </summary>
        /// <param name="startRange">Begin index to start adding items</param>
        /// <param name="endRange">Index to stop adding items (this idex is not added)</param>
        /// <returns>An item block defined by the specified range in this timeline</returns>
        private ItemBlock BuildItemBlockFromRange(
            int startRange,
            int endRange)
        {
            // once we've reordered our list, the time actually
            // isn't important for comparison purposes, don't need
            // to make the effort to keep it around. so it's fine
            // to use a list<int> and lose the timespans
            var items = new List<Item>();
            for (int i = startRange; i < endRange; i++)
            {
                items.Add(itemList[i]);
            }

            return new ItemBlock("Next Purchase", items);
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return itemList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The ToString representation is (item name) -> (item name) -> (item name) ...
        /// Purchase times are not included in the default ToString
        /// </summary>
        /// <returns>ToString representation of the ItemPurchaseTimeline</returns>
        override public string ToString()
        {
            string toReturn = string.Empty;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (i == itemList.Count - 1)
                {
                    toReturn += itemList[i].Name;
                }
                else
                {
                    toReturn += string.Format("{0} -> ", itemList[i].Name);
                }
            }

            return toReturn;
        }
    }
}
