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
        public static string GetLicense(Player player) {
            return player.Identifiers["license"];
        }

        public static string GetSteam(Player player)
        {
            return player.Identifiers["steam"];
        }

        public static string GetSteam64(Player player)
        {
            string steam = GetSteam(player);

            if (steam != null)
            {
                return Convert.ToInt32(steam, 16).ToString();
            } else
            {
                return null;
            }
        }
        public static string GetIP(Player player)
        {
            return player.Identifiers["ip"];
        }
        public static long GetUnixTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
    }
}
