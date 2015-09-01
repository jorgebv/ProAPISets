using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemSetTools
{
    /// <summary>
    /// This class contains constants that can be used to map
    /// well known summoners (in this case, pros), to summoner IDs
    /// </summary>
    public class SummonerIds
    {
        public static List<int> Pros = new List<int>()
        {
            C9.Balls,
            C9.Hai,
            C9.Incarnation,
            C9.LemonNation,
            C9.Sneaky,
            TiP.Adrian,
            TiP.Apollo,
            TiP.Gate,
            TiP.Impact,
            TiP.Rush,
            TL.IWillDominate,
            TL.Piglet,
            TL.Quas,
            TL.Xpecial,
            CLG.aphromoo,
            CLG.Doublelift,
            CLG.Pobelter,
            CLG.Xmithie,
            CLG.ZionSpartan,
            RNG.AlexIch,
            RNG.Crumbzz,
            RNG.Maple,
            RNG.Remilia,
            RNG.RF,
            TSM.Bjergsen,
            TSM.Dyrus,
            TSM.Lustboy,
            TSM.Santorin,
            TSM.WildTurtle,
            GV.Altec,
            GV.BunnyFuFuu,
            GV.Hauntzer,
            GV.Keane,
            GV.Move,
            DIG.CoreJJ,
            DIG.Gamsu,
            DIG.Helios,
            DIG.Kiwikid,
            DIG.Shiphtur
        };

        public static Dictionary<string, int> NameToSummonerId = new Dictionary<string, int>()
        {
             // TiP
            {"Impact", SummonerIds.TiP.Impact},
            {"Rush", SummonerIds.TiP.Rush},
            {"Adrian", SummonerIds.TiP.Adrian},
            {"Apollo", SummonerIds.TiP.Apollo},
            {"Gate", SummonerIds.TiP.Gate},
            // C9
            {"Sneaky", SummonerIds.C9.Sneaky},
            {"Hai", SummonerIds.C9.Hai},
            {"Lemon", SummonerIds.C9.LemonNation},
            {"Balls", SummonerIds.C9.Balls},
            {"Incarnation", SummonerIds.C9.Incarnation},
            // TL
            {"Piglet", SummonerIds.TL.Piglet},
            {"Quas", SummonerIds.TL.Quas},
            {"Fenix", SummonerIds.TL.Fenix},
            {"IWillDominate", SummonerIds.TL.IWillDominate},
            {"Xpecial", SummonerIds.TL.Xpecial},
            // CLG
            {"ZionSpartan", SummonerIds.CLG.ZionSpartan},
            {"Pobelter", SummonerIds.CLG.Pobelter},
            {"Doublelift", SummonerIds.CLG.Doublelift},
            {"aphromoo", SummonerIds.CLG.aphromoo},
            {"Xmithie", SummonerIds.CLG.Xmithie},
            // RNG
            {"Crumbzz", SummonerIds.RNG.Crumbzz},
            {"Maple", SummonerIds.RNG.Maple},
            {"AlexIch", SummonerIds.RNG.AlexIch},
            {"Remilia", SummonerIds.RNG.Remilia},
            {"RF Legendary", SummonerIds.RNG.RF},
            // TSM
            {"Dyrus", SummonerIds.TSM.Dyrus},
            {"Santorin", SummonerIds.TSM.Santorin},
            {"Lustboy", SummonerIds.TSM.Lustboy},
            {"WildTurtle", SummonerIds.TSM.WildTurtle},
            {"Bjergsen", SummonerIds.TSM.Bjergsen},
            // Gravity
            {"Bunny FuFuu", SummonerIds.GV.BunnyFuFuu},
            {"Move", SummonerIds.GV.Move},
            {"Altec", SummonerIds.GV.Altec},
            {"Keane", SummonerIds.GV.Keane},
            {"Hauntzer", SummonerIds.GV.Hauntzer},
            // Dignitas
            {"CoreJJ", SummonerIds.DIG.CoreJJ},
            {"Kiwikid", SummonerIds.DIG.Kiwikid},
            {"Shiphtur", SummonerIds.DIG.Shiphtur},
            {"Helios", SummonerIds.DIG.Helios},
            {"Gamsu", SummonerIds.DIG.Gamsu}
        };

        public class C9
        {
            public const int Sneaky = 51405;
            public const int Hai = 492066;
            public const int LemonNation = 44979325;
            public const int Balls = 44989299;
            public const int Incarnation = 68479082;
        }

        public class TiP
        {
            public const int Impact = 65389100;
            public const int Rush = 65399098;
            public const int Adrian = 58060767;
            public const int Apollo = 7250;
            public const int Gate = 21428926;
        }

        public class TL
        {
            public const int Piglet = 65389094;
            public const int Quas = 50539313;
            public const int Fenix = 65409091;
            public const int IWillDominate = 38249375;
            public const int Xpecial = 19199530;
        }

        public class CLG
        {
            public const int ZionSpartan = 19738326;
            public const int Pobelter = 2648;
            public const int Doublelift = 20132258;
            public const int aphromoo = 442232;
            public const int Xmithie = 19761072;
        }

        public class RNG
        {
            public const int Crumbzz = 20239565;
            public const int Maple = 31804754;
            public const int AlexIch = 65009177;
            public const int Remilia = 43839117;
            public const int RF = 28650045;
        }

        public class TSM
        {
            public const int Dyrus = 5908;
            public const int Santorin = 57029179;
            public const int Lustboy = 58849083;
            public const int WildTurtle = 42060215;
            public const int Bjergsen = 63581619;
        }

        public class GV
        {
            public const int BunnyFuFuu = 20441329;
            public const int Move = 68159251;
            public const int Altec = 22140119;
            public const int Keane = 57039082;
            public const int Hauntzer = 401169;
        }

        public class DIG
        {
            public const int CoreJJ = 62599179;
            public const int Kiwikid = 24332462;
            public const int Shiphtur = 20833616;
            public const int Helios = 55779191;
            public const int Gamsu = 37962884;
        }
    }
}