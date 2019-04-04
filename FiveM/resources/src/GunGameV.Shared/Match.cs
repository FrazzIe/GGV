using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GunGameV.Shared
{
    abstract public class Match
    {
        [JsonProperty]
        protected long startTime;
        [JsonProperty]
        protected int length, scoreLimit;
        [JsonProperty]
        protected List<uint> weapons;
        [JsonProperty]
        protected string mapName;

        public uint GetWeapon(int index)
        {
            return weapons[index];
        }
        public int ScoreLimit { get => scoreLimit; }
        public long StartTime { get => startTime; }
        public long EndTime { get => (startTime + length); }
        public string Map { get => mapName; }
        public User Winner { get; set; }
    }
}
