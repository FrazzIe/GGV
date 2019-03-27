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
        {
        {

        public static string GetSteam64(string hex)
        {
            return Convert.ToInt32(hex, 16).ToString();
        }
        public static long UnixTimestamp { get => DateTimeOffset.UtcNow.ToUnixTimeSeconds(); }
    }
}
