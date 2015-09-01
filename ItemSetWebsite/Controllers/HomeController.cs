using ItemSetTools;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ItemSetWebsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            long summonerId = SummonerIds.C9.Balls;
            long championId = 8;

            using (GameContext context = new GameContext())
            {
                ProPlayerItemSet itemSet = context.ItemSets.SingleOrDefault(t => t.SummonerId == summonerId && t.ChampionId == championId);
                if (itemSet == null)
                {
                    // return Content(string.Format("Cannot find item set with SummonerId {0} and ChampionId {1}", summonerId, championId));
                    return View("Index");
                }
                return View("ItemSetJson", itemSet);
            }
        }

        public ActionResult GetItemSetJson(string summonerName, string championName)
        {
            long summonerId = SummonerIds.NameToSummonerId[summonerName];
            long championId = ChampionIds.NameToId[championName];

            using (GameContext context = new GameContext())
            {
                ProPlayerItemSet itemSet = context.ItemSets.SingleOrDefault(t => t.SummonerId == summonerId && t.ChampionId == championId);
                if (itemSet == null)
                {
                    return HttpNotFound();
                }
                return Content(itemSet.ItemSetJson);
            }
        }
    }
}