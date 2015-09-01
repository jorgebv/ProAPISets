using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiotNet;
using RiotNet.Models;
using System.Configuration;

namespace SummonerIDFinder
{
    // The idea behind this program is just to grab the summoner IDs for all pro players
    // summoner names are bound to change and so it is useful to have summoner IDs filed away
    //
    // The names in the below list are only accurate as of 8/25/2015 and may not necessarily be
    // the current names at the date of viewing
    //
    // The output from this program, ran on 8/25/2015, is pasted at the bottom of the file
    class LookupByName
    {
        // in the case of multiple accounts, The one I believe to be most active as of 8/25/2015 is listed first
        // as of 8/25/2015
        //
        // there may also be more accounts they have that I am not aware of, this is not necessarily
        // a comprehensive list
        public static readonly IEnumerable<string> SummonerNames = new ReadOnlyCollection<string>(new List<string>
        {
            // TiP
            "Yasuo gosu", // Impact
            "TiP Impact",
            "TiP Rush",
            "Adrian Ma",
            "TiP Adrian", // Adrian
            "SIick Mahony", // Apollo
            "Gate",
            "TiP Gate",
            // C9
            "C9 Sneaky",
            "C9 Hai",
            "C9 Lemon",
            "BlG HUEVOS", // Balls
            "C9 Jensen", // Incarnation
            // TL
            "Icarrynp", // Piglet
            "Liquid Piglet",
            "GodPiglet", // Piglet
            "Entranced", // Quas
            "Quasmire", // Quas
            "Liquid Quas", // Quas
            "Fenix TL",
            "RNGmlxg", // IWillDominate
            "Liquid Xpecial",
            // CLG
            "ZionSpartan",
            "Pobelter",
            "Eugene J Park", // Pobelter
            "Doublelift",
            "Peng Yiliang", // Doublelift
            "aphromoo",
            "sejuanimain", // Xmithie
            // RNG
            "RNG CRBZ", // Crumbzz
            "Renegade Maple",
            "RNG Alex Ichy",
            "Remi x Revy", // Remilia
            "LaGMaN", // RF Legendary
            "RF Legendary",
            // TSM
            "Dyrus",
            "suryD", // Dyrus
            "HotGuy6Pack", // Santorin
            "Casker", // Lustboy
            "WildTurtl",
            "Turtle the Cat", // WildTurtle
            "I am Bjerg", // Bjergsen
            // Gravity
            "GV Bunny FuFuu",
            "Unstoppable Move", // Move
            "wxmx", // Altec
            "GV Keane",
            "Ice", // Hauntzer
            // Dignitas
            "DuBuKiD", // Core JJ
            "Kiwikid",
            "Chapanya", // Shiphtur
            "HelioScarlet", // Helios
            "Loopercorn" // Gamsu
        });

        static void Main(string[] args)
        {
            var client = new RiotClient(Region.NA, new RiotClientSettings
            {
                ApiKey = ConfigurationManager.AppSettings["RiotAPIKey"]
            });

            foreach(var summonerName in SummonerNames)
            {
                var summonerId = client.GetSummonerBySummonerName(summonerName);
                Console.WriteLine(String.Format("{0},{1}", summonerName, summonerId.Id));
            }
        }
    }
}

//Yasuo gosu,65389100
//TiP Impact,65389099
//TiP Rush,65399098
//Adrian Ma,58060767
//TiP Adrian,65409096
//SIick Mahony,7250
//Gate,21428926
//TiP Gate,44879120
//C9 Sneaky,51405
//C9 Hai,492066
//C9 Lemon,44979325
//BlG HUEVOS,44989299
//C9 Jensen,68479082
//Icarrynp,65389094
//Liquid Piglet,62374009
//GodPiglet,65409090
//Entranced,50539313
//Quasmire,50759302
//Liquid Quas,20017478
//Fenix TL,65409091
//RNGmlxg,68619391
//Liquid Xpecial,19199530
//ZionSpartan,19738326
//Pobelter,2648
//Eugene J Park,51409228
//Doublelift,20132258
//Peng Yiliang,44979328
//aphromoo,442232
//sejuanimain,19761072
//RNG CRBZ,20239565
//Renegade Maple,31804754
//RNG Alex Ichy,65009177
//Remi x Revy,43839117
//LaGMaN,28650045
//RF Legendary,19209176
//Dyrus,5908
//suryD,89197
//HotGuy6Pack,57029179
//Casker,58849083
//WildTurtl,42060215
//Turtle the Cat,18991200
//I am Bjerg,63581619
//GV Bunny FuFuu,20441329
//Unstoppable Move,68159251
//wxmx,22140119
//GV Keane,57039082
//Ice,401169
//DuBuKiD,62599179
//Kiwikid,24332462
//Chapanya,20833616
//HelioScarlet,55779191
//Loopercorn,37962884
