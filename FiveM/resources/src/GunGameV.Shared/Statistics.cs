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
        public int Loses { get => (gamesPlayed - Wins); }
        public void Update(GameStats gameStats)
        {
            Kills += gameStats.Kills;
            Deaths += gameStats.Deaths;
            gamesPlayed++;
        }
    }

    public class GameStats : Statistics, IComparable
    {
        public GameStats()
        {
            Kills = 0;
            Deaths = 0;
            Score = 0;
        }
        public int Score { get; set; }
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            GameStats otherStats = obj as GameStats;

            if(otherStats != null)
            {
                int score = this.Score.CompareTo(otherStats.Score);
                if (score == 0)
                {
                    int kills = this.Kills.CompareTo(otherStats.Kills);
                    if(kills == 0)
                    {
                        int deaths = this.Kills.CompareTo(otherStats.Deaths);

                        return deaths;
                    } else
                    {
                        return kills;
                    }
                } else
                {
                    return score;
                }
            } else
            {
                throw new ArgumentException("Object is not a GameStats");
            }
        }
    }
}
