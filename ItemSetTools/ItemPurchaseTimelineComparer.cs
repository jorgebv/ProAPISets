using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemSetTools
{
    public class ItemPurchaseTimelineComparer
    {
        /// <summary>
        /// Controls whether consumables such as potions and wards
        /// will be ignored when comparing ItemPurchaseTimelines
        /// 
        /// Defaults to true if not disabled
        /// </summary>
        public bool IgnoreConsumables { get; set; }

        /// <summary>
        /// This will allow the comparer to internally reorder items
        /// that were purchased within a certain time interval.
        /// 
        /// For example, potion -> Doran's blade -> trinket
        /// and trinket -> doran's blade -> potion
        /// has an edit distance of 4, but if they were all purchased
        /// at the beginning of the game in quick succession,
        /// we probably don't want to truly treat this as a distance
        /// of 4.
        /// 
        /// Defaults to true
        /// </summary>
        public bool GroupItemsByTime { get; set; }

        /// <summary>
        /// If GroupItemsByTime is true, this will control the window in
        /// which items will be grouped. Time is given in seconds. Defaults
        /// to 15.
        /// </summary>
        public int ItemGroupingThresholdInSeconds { get; set; }

        /// <summary>
        /// Constructor that allows setting ignoring of consumables
        /// </summary>
        /// <param name="ignoreConsumables">
        /// Whether consumables should be ignored in comparisons made
        /// by this ItemPurchaseTimelineComparer
        /// </param>
        public ItemPurchaseTimelineComparer(
            bool ignoreConsumables = true,
            bool groupItemsByTime = true,
            int itemGroupingThresholdInSeconds = 15)
        {
            IgnoreConsumables = ignoreConsumables;
            GroupItemsByTime = groupItemsByTime;
            ItemGroupingThresholdInSeconds = itemGroupingThresholdInSeconds;
        }

        /// <summary>
        /// The item set comparer can have different policies set on it, such
        /// as ignoring consumables. This method is used internally to check
        /// all of these policies and return whether the item should be considered
        /// </summary>
        /// <param name="item">
        /// The item we are interested in
        /// </param>
        /// <returns>
        /// Whether this item should be considered according to the
        /// current policy of the comparer
        /// </returns>
        private bool ShouldConsiderItemAccordingToPolicy(Item item)
        {
            // currently consumables is the only policy
            if (IgnoreConsumables && item.IsConsumable)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Strip items that can be ignored by policy from the timeline, and return the new timeline
        /// </summary>
        /// <param name="timeline">Timeline that will have items stripped from it</param>
        /// <returns>Timeline containing only items allowed by current comparer policy</returns>
        private ItemPurchaseTimeline StripIgnorableItemsFromTimeline(ItemPurchaseTimeline timeline)
        {
            var newTimeline = new ItemPurchaseTimeline();
            foreach(var item in timeline)
            {
                if (ShouldConsiderItemAccordingToPolicy(item))
                {
                    newTimeline.AddToEnd(item);
                }
            }
            return newTimeline;
        }

        private ItemPurchaseTimeline ReorderListAccountingForItemsPurchasedTogether(ItemPurchaseTimeline timelineToReorder)
        {
            var itemBlocks = timelineToReorder.TryToSplitIntoPurchases(ItemGroupingThresholdInSeconds);
            var newListOfItems = new List<Item>();

            foreach (var itemBlock in itemBlocks)
            {
                itemBlock.Items.Sort();
                foreach (var item in itemBlock.Items)
                {
                    newListOfItems.Add(item);
                }
            }

            var newTimeline = new ItemPurchaseTimeline();
            foreach (var item in newListOfItems)
            {
                // drop the timestamp or we will run into exceptions
                newTimeline.AddToEndByItemId(item.Id);
            }

            return newTimeline;
        }

        // endRange is non-inclusive
        private void InsertIntoListSortedByItemId(
            ItemPurchaseTimeline newTimeline,
            ItemPurchaseTimeline timelineToReorder,
            int startRange,
            int endRange)
        {
            // once we've reordered our list, the time actually
            // isn't important for comparison purposes, don't need
            // to make the effort to keep it around. so it's fine
            // to use a list<int> and lose the timespans
            var itemIds = new List<int>();
            for (int i = startRange; i < endRange; i++)
            {
                itemIds.Add(timelineToReorder[i].Id);
            }

            itemIds.Sort();

            foreach (var itemId in itemIds)
            {
                // re:timespan.zero -- see comment at start of function
                newTimeline.AddToEndByItemId(itemId, TimeSpan.Zero);
            }
        }

        /// <summary>
        /// If two timelines are identical in regards to item purchase order (times are ignored),
        /// then the distance between them is 0. If they are identical, but one is longer (for
        /// example, one game went on much longer but builds were identical up until the shorter
        /// game ended), the distance between them is still 0.
        /// 
        /// If build are different, the edit distance between them is returned
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public int DistanceBetween(ItemPurchaseTimeline one, ItemPurchaseTimeline two)
        {
            var newOne = StripIgnorableItemsFromTimeline(one);
            var newTwo = StripIgnorableItemsFromTimeline(two);

            // these might be used if we regroup items by item
            ItemPurchaseTimeline reorderedListOne = null;
            ItemPurchaseTimeline reorderedListTwo = null;
            if (GroupItemsByTime)
            {
                // it's possible by reordering some items in one list, and not reordering
                // items in another, we artifically change the distance slightly
                // however, if our purchase window is appropriately sized, the net benefit
                // should be positive because we will reorder things that are
                // purchased together often will appear more uniform across games
                // the only time this is a net negative is if some games the item is purchased
                // in a group with others, and some games it is not. to account for this,
                // we'll just calculate the distance against both and return the smaller one
                //
                // it should be possible to reorder the lists more intelligently by giving
                // them knowledge of the other one, but this is much more complicated
                // to implement and so is left as a "nice to have" improvement
                reorderedListOne = ReorderListAccountingForItemsPurchasedTogether(newOne);
                reorderedListTwo = ReorderListAccountingForItemsPurchasedTogether(newTwo);
            }

            var areIdenticalIgnoringLength = AreItemPurchaseTimelinesIdenticalIgnoringLength(newOne, newTwo);
            if (areIdenticalIgnoringLength)
            {
                return 0;
            }
            else
            {
                // check if the reordered lists are identical
                if (GroupItemsByTime)
                {
                    var areReorderedListsIdentical = AreItemPurchaseTimelinesIdenticalIgnoringLength(
                        reorderedListOne, reorderedListTwo);

                    if (areReorderedListsIdentical)
                    {
                        // if they are, we certainly want to return 0
                        return 0;
                    }
                    else
                    {
                        // if not, return the smaller of the two distances, on the chance regrouping
                        // hurt us (this possibility is described above)
                        return Math.Min(EditDistance<Item>(newOne, newTwo),
                            EditDistance<Item>(reorderedListOne, reorderedListTwo));
                    }
                }
                return EditDistance<Item>(newOne, newTwo);
            }
        }

        /// <summary>
        /// Checks if the item timelines are identical (ignoring length). Meaning, if one timeline
        /// is strictly longer than the other but they are identical up to the end of the shorter timeline,
        /// this function would return true
        /// </summary>
        /// <param name="one">Timeline to compare</param>
        /// <param name="two">Timeline to compare</param>
        /// <returns>Whether timelines are identical</returns>
        private bool AreItemPurchaseTimelinesIdenticalIgnoringLength(ItemPurchaseTimeline one, ItemPurchaseTimeline two)
        {
            // timelines are guaranteed to be sorted by order of purchase
            int oneIndex = 0, twoIndex = 0;
            var identicalSoFar = true;
            while (oneIndex < one.Count && twoIndex < two.Count && identicalSoFar)
            {
                var oneItem = one[oneIndex];
                var twoItem = two[twoIndex];

                if (oneItem.Id != twoItem.Id)
                {
                    identicalSoFar = false;
                }

                oneIndex++; twoIndex++;
            }
            return identicalSoFar;
        }

        /// <summary>
        /// Computes the Levenshtein Edit Distance between two enumerables.
        /// This implementation taken from http://blogs.msdn.com/b/toub/archive/2006/05/05/590814.aspx
        /// with minor differences
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerables.</typeparam>
        /// <param name="x">The first enumerable.</param>
        /// <param name="y">The second enumerable.</param>
        /// <returns>The edit distance.</returns>
        private int EditDistance<T>(IEnumerable<T> x, IEnumerable<T> y)
            where T : IEquatable<T>
        {
            // Validate parameters
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");

            // Convert the parameters into IList instances
            // in order to obtain indexing capabilities
            IList<T> first = x as IList<T> ?? new List<T>(x);
            IList<T> second = y as IList<T> ?? new List<T>(y);

            // Get the length of both.  If either is 0, return
            // the length of the other, since that number of insertions
            // would be required.
            int n = first.Count, m = second.Count;
            if (n == 0) return m;
            if (m == 0) return n;

            // Rather than maintain an entire matrix (which would require O(n*m) space),
            // just store the current row and the next row, each of which has a length m+1,
            // so just O(m) space. Initialize the current row.
            int curRow = 0, nextRow = 1;
            int[][] rows = new int[][] { new int[m + 1], new int[m + 1] };
            for (int j = 0; j <= m; ++j) rows[curRow][j] = j;

            // For each virtual row (since we only have physical storage for two)
            for (int i = 1; i <= n; ++i)
            {
                // Fill in the values in the row
                rows[nextRow][0] = i;
                for (int j = 1; j <= m; ++j)
                {
                    int dist1 = rows[curRow][j] + 1;
                    int dist2 = rows[nextRow][j - 1] + 1;
                    int dist3 = rows[curRow][j - 1] +
                        (first[i - 1].Equals(second[j - 1]) ? 0 : 1);

                    rows[nextRow][j] = Math.Min(dist1, Math.Min(dist2, dist3));
                }

                // Swap the current and next rows
                nextRow = curRow;
                curRow = 1 - curRow;
            }

            // Return the computed edit distance
            return rows[curRow][m];
        }
    }
}
