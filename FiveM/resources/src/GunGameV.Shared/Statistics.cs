using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GunGameV.Shared
{
    abstract public class Statistics
    {
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }

    public class GlobalStats : Statistics
    {
        [JsonProperty]
        private int gamesPlayed;

        public GlobalStats(int kills = 0, int deaths = 0, int wins = 0, int _gamesPlayed = 0)
        {
            Kills = kills;
            Deaths = deaths;
            Wins = wins;
            gamesPlayed = _gamesPlayed;
        }
        public int Wins { get; set; }
        public int Loses { get => gamesPlayed - Wins; }
    }

    public class GameStats : Statistics
    {
        public GameStats(int kills = 0, int deaths = 0, int score = 0)
        {
            Kills = kills;
            Deaths = deaths;
            Score = score;
        }
        public int Score { get; set; }
    }
}
