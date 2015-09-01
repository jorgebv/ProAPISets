using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotNet;
using RiotNet.Models;
using System.Configuration;

namespace ItemSetTools
{
    public class ItemUtils
    {
        private static StaticItemList itemList;

        // it would be nice to use the version number to do these items correctly,
        // but I rely on item id for comparisons so it needs to be the same
        // After learning they can change like this, it would be ideal to use a version
        // independent comparison scheme, but that is let as a future
        private static Dictionary<int, int> legacyItems = new Dictionary<int, int>()
        {
            {3280, 1309},
            {3282, 1305},
            {3281, 1307},
            {3284, 1306},
            {3283, 1308},
            {3278, 1333},
            {3279, 1331},
            {3250, 1304},
            {3251, 1302},
            {3254, 1301},
            {3255, 1314},
            //{}
            {3252, 1300},
            {3253, 1303},
            {3263, 1318},
            {3264, 1316},
            {3265, 1324},
            {3266, 1322},
            {3260, 1319},
            {3261, 1317},
            {3262, 1315},
            {3257, 1310},
            {3256, 1312},
            {3259, 1311},
            {3258, 1313},
            {3276, 1332},
            {3277, 1330},
            {3274, 1326},
            {3275, 1334},
            {3272, 1325},
            {3273, 1328},
            {3270, 1329},
            {3271, 1327},
            {3269, 1321},
            {3268, 1323},
            {3267, 1320},
        };

        private static void InitializeItemListIfNeeded()
        {
            if (itemList == null)
            {
                var client = new RiotClient(Region.NA, new RiotClientSettings
                {
                    ApiKey = ConfigurationManager.AppSettings["RiotAPIKey"]
                });
                itemList = client.GetStaticItems(itemListData: new string[] { "consumed" });
            }
        }

        /// <summary>
        /// Returns an API item for a given item id.
        /// </summary>
        /// <param name="itemId">Item ID according to Riot API</param>
        /// <returns>
        /// An API item for a given item id. The returned item
        /// has a zero purchased time.
        /// </returns>
        internal static StaticItem GetAPIItemForItemId(int itemId)
        {
            InitializeItemListIfNeeded();

            StaticItem item;
            if (itemList.Data.TryGetValue(itemId.ToString(), out item))
            {
                return item;
            }

            // otherwise try the legacy one
            // trygetvalue is more efficient if this throws often,
            // but this throwing is likely programmer error, so the try is better
            try
            {
                return itemList.Data[legacyItems[itemId].ToString()];
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException("Item ID is not a valid item.");
            }
        }

        public static string GetNameForItemId(int itemId)
        {
            return GetAPIItemForItemId(itemId).Name;
        }

        public static bool GetIsConsumableForItemId(int itemId)
        {
            return GetAPIItemForItemId(itemId).Consumed;
        }
    }
}
