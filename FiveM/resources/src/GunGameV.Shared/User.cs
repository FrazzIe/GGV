using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GunGameV.Shared
{
    public class User
    {
        [JsonProperty]
        private string id, name, steam, license, ip;

        public GlobalStats globalStats;
        public GameStats gameStats = new GameStats();

        public User(string _id, string _name, string _steam, string _license, string _ip, int kills = 0, int deaths = 0, int wins = 0, int gamesPlayed = 0)
        {
            id = _id;
            name = _name;
            steam = _steam;
            license = _license;
            ip = _ip;
            globalStats = new GlobalStats(kills, deaths, wins, gamesPlayed);
        }
        public string ID { get => id; }
        public string Name { get => name; }
        public string Steam { get => steam; }
        public string License { get => license; }
        public string IP { get => ip; }
        public bool InMatch { get; set; } = false;
    }
}