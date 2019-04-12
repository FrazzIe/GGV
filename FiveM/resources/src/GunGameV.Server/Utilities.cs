using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunGameV.Server
{
    public static class Utilities //Class of static data that is referred to
    {
        private static List<uint> weapons = new List<uint> //List of weapons
        {
            //Pistols
            453432689,
            3219281620,
            1593441988,
            2578377531,
            3218215474,
            2285322324,
            3523564046,
            137902532,
            3696079510,
            3249783761,
            3415619887,
            2548703416,
            584646201,
            //SMGs
            324215364,
            3675956304,
            3173288789,
            736523883,
            2024373456,
            4024951519,
            171789620,
            2634544996,
            2144741730,
            3686625920,
            1627465347,
            //Rifles
            3220176749,
            961495388,
            2210333304,
            4208062921,
            2937143193,
            3231910285,
            2526821735,
            2132975508,
            2228681469,
            1649403952,
            //Snipers
            100416529,
            205991906,
            177293209,
            3342088282,
            1785463520,
            //Shotguns
            487013001,
            1432025498,
            2017895192,
            2640438543,
            3800352039,
            2828843422,
            984333226,
            4019527611,
            317205821,
            //Explosives
            2726580491,
            2982836145,
            1119849093,
            2138347493,
            1834241177,
            1672152130,
            125959754,
        };
        private static List<uint> meleeWeapons = new List<uint> //List of melee weapons
        {
            2578778090,
            1737195953,
            1317494643,
            2508868239,
            2227010557,
            1141786504,
            4192643659,
            2460120199,
            4191993645,
            3638508604,
            3713923289,
            2343591895,
            3756226112,
            3441901897,
            2484171525,
            419712736,
        };
        private static List<string> maps = new List<string> //List of map names
        {
            "bunker",
            "garage",
            "iaa",
        };
        public static string GetSteam64(string hex) //This function converts a hexidecimal string to a decimal and returns it
        {
            return Convert.ToInt64(hex, 16).ToString();
        }
        public static long UnixTimestamp { get => DateTimeOffset.UtcNow.ToUnixTimeSeconds(); } //This class property returns the current unix utc timestamp in seconds
        public static List<uint> Weapons { get => weapons; } //This class property returns the list of weapons
        public static List<uint> MeleeWeapons { get => meleeWeapons; } //This class property returns the list of melee weapons
        public static List<string> Maps { get => maps; } //This class property returns the list of map names
    }
}
