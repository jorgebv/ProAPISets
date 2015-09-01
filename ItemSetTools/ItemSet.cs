using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ItemSetTools
{
    /// <summary>
    /// The ItemSet class only exposes what is actually utilized in this project,
    /// but could easily be expanded to meet the whole definition of an item set
    /// given by Riot
    /// </summary>
    public class ItemSet
    {
        /// <summary>
        /// Title of the item set page
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// An item block is a list of items. An item set has multiple item blocks.
        /// </summary>
        public List<ItemBlock> ItemBlocks { get; set; }

        // there are many other fields in an item set, but for this project, we will
        // be defaulting them to a value thus they do not need to be configurable

        /// <summary>
        /// Default constructor. Properties can be set post construction
        /// </summary>
        public ItemSet()
        {

        }

        /// <summary>
        /// Constructor that takes in item blocks to initalize property
        /// </summary>
        /// <param name="title">Title of the item set page</param>
        /// <param name="itemBlocks">Items blocks in this item set</param>
        public ItemSet(string title, List<ItemBlock> itemBlocks)
        {
            Title = title;
            ItemBlocks = itemBlocks;
        }

        public string ToJson()
        {
            var model = new ItemSetJsonModel();
            model.title = Title;
            model.type = "custom";
            model.map = "SR";
            model.mode = "CLASSIC";
            model.priority = false;
            model.sortrank = 0;
            model.blocks = new ItemBlockJsonModel[ItemBlocks.Count];
            for(int itemBlockIndex = 0; itemBlockIndex < ItemBlocks.Count; itemBlockIndex++)
            {
                var itemBlockJsonModel = new ItemBlockJsonModel();
                itemBlockJsonModel.type = ItemBlocks[itemBlockIndex].Name;
                itemBlockJsonModel.hideIfSummonerSpell = "";
                itemBlockJsonModel.showIfSummonerSpell = "";
                itemBlockJsonModel.maxSummonerLevel = -1;
                itemBlockJsonModel.minSummonerLevel = -1;
                itemBlockJsonModel.recMath = false;

                var items = new ItemJsonModel[ItemBlocks[itemBlockIndex].Items.Count];
                for (int itemIndex = 0; itemIndex < ItemBlocks[itemBlockIndex].Items.Count; itemIndex++)
                {
                    var itemJsonModel = new ItemJsonModel();
                    itemJsonModel.id = ItemBlocks[itemBlockIndex].Items[itemIndex].Id.ToString();
                    itemJsonModel.count = 1;
                    items[itemIndex] = itemJsonModel;
                }

                itemBlockJsonModel.items = items;

                model.blocks[itemBlockIndex] = itemBlockJsonModel;
            }

            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }

    }

    internal class ItemSetJsonModel
    {
        public string title { get; set; }
        public string type { get; set; }
        public string map { get; set; }
        public string mode { get; set; }
        public bool priority { get; set; }
        public int sortrank { get; set; }
        public ItemBlockJsonModel[] blocks { get; set; }
    }
}
