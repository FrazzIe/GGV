using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace GunGameV.Server
{
    public static class Utilities
    {
        private static List<string> weapons = new List<string>
        {
            //Pistols
            "WEAPON_PISTOL",
            "WEAPON_PISTOL_MK2",
            "WEAPON_COMBATPISTOL",
            "WEAPON_PISTOL50",
            "WEAPON_SNSPISTOL",
            "WEAPON_SNSPISTOL_MK2",
            "WEAPON_HEAVYPISTOL",
            "WEAPON_VINTAGEPISTOL",
            "WEAPON_MARKSMANPISTOL",
            "WEAPON_REVOLVER",
            "WEAPON_REVOLVER_MK2",
            "WEAPON_DOUBLEACTION",
            "WEAPON_APPISTOL",
            "WEAPON_STUNGUN",
            "WEAPON_FLAREGUN",
            //SMGs
            "WEAPON_MICROSMG",
            "WEAPON_MACHINEPISTOL",
            "WEAPON_MINISMG",
            "WEAPON_SMG",
            "WEAPON_SMG_MK2",
            "WEAPON_ASSAULTSMG",
            "WEAPON_COMBATPDW",
            "WEAPON_MG",
            "WEAPON_COMBATMG",
            "WEAPON_COMBATMG_MK2",
            "WEAPON_GUSENBERG",
            //Rifles
            "WEAPON_ASSAULTRIFLE",
            "WEAPON_ASSAULTRIFLE_MK2",
            "WEAPON_CARBINERIFLE",
            "WEAPON_CARBINERIFLE_MK2",
            "WEAPON_ADVANCEDRIFLE",
            "WEAPON_SPECIALCARBINE",
            "WEAPON_SPECIALCARBINE_MK2",
            "WEAPON_BULLPUPRIFLE",
            "WEAPON_BULLPUPRIFLE_MK2",
            "WEAPON_COMPACTRIFLE",
            //Snipers
            "WEAPON_SNIPERRIFLE",
            "WEAPON_HEAVYSNIPER_MK2",
            "WEAPON_HEAVYSNIPER",
            "WEAPON_MARKSMANRIFLE",
            "WEAPON_MARKSMANRIFLE_MK2",
            //Shotguns
            "WEAPON_PUMPSHOTGUN",
            "WEAPON_PUMPSHOTGUN_MK2",
            "WEAPON_SAWNOFFSHOTGUN",
            "WEAPON_BULLPUPSHOTGUN",
            "WEAPON_ASSAULTSHOTGUN",
            "WEAPON_HEAVYSHOTGUN",
            "WEAPON_MUSKET",
            "WEAPON_DBSHOTGUN",
            "WEAPON_AUTOSHOTGUN",
            //Explosives
            "WEAPON_GRENADELAUNCHER",
            "WEAPON_RPG",
            "WEAPON_MINIGUN",
            "WEAPON_FIREWORK",
            "WEAPON_RAILGUN",
            "WEAPON_HOMINGLAUNCHER",
            "WEAPON_COMPACTLAUNCHER",
        };
        private static List<string> meleeWeapons = new List<string>
        {
            "WEAPON_KNIFE",
            "WEAPON_NIGHTSTICK",
            "WEAPON_HAMMER",
            "WEAPON_BAT",
            "WEAPON_CROWBAR",
            "WEAPON_GOLFCLUB",
            "WEAPON_BOTTLE",
            "WEAPON_DAGGER",
            "WEAPON_HATCHET",
            "WEAPON_KNUCKLE",
            "WEAPON_MACHETE",
            "WEAPON_FLASHLIGHT",
            "WEAPON_SWITCHBLADE",
            "WEAPON_BATTLEAXE",
            "WEAPON_POOLCUE",
            "WEAPON_WRENCH",
        };

        public static string GetSteam64(string hex)
        {
            return Convert.ToInt32(hex, 16).ToString();
        }
        public static long UnixTimestamp { get => DateTimeOffset.UtcNow.ToUnixTimeSeconds(); }
        public static List<string> Weapons { get => weapons; }
        public static List<string> MeleeWeapons { get => meleeWeapons; }
    }
}
