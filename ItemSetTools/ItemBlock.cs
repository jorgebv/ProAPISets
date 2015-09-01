using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ItemSetTools
{
    /// <summary>
    /// ItemBlock class only exposes what is planned to be used
    /// in this project, but could easily be expanded to cover
    /// the whole item block definition given by Riot
    /// </summary>
    public class ItemBlock
    {
        /// <summary>
        /// Name of the item block
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of items in the item block
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// ItemBlock constructor
        /// </summary>
        /// <param name="name">Name of the item block that will be displayed in LoL client</param>
        /// <param name="items">List of items in the item block</param>
        public ItemBlock(string name, List<Item> items)
        {
            Name = name;
            Items = items;
        }

        // there are many other fields in an item set, but for this project, we will
        // be defaulting them to a value thus they do not need to be configurable
    }

    internal class ItemBlockJsonModel
    {
        public string type { get; set; }
        public bool recMath { get; set; }
        public int minSummonerLevel { get; set; }
        public int maxSummonerLevel { get; set; }
        public string showIfSummonerSpell { get; set; }
        public string hideIfSummonerSpell { get; set; }
        public ItemJsonModel[] items { get; set; }
    }
}
