using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GunGameV.Shared
{
    abstract public class Match //Cannot be instantiated because its abstract
    {
        [JsonProperty] //Attribute forces Newtonsoft.Json to serialize the variable
        protected long startTime; //Match start time
        [JsonProperty] //Attribute forces Newtonsoft.Json to serialize the variable
        protected int length, scoreLimit; //Match length and score limit
        [JsonProperty] //Attribute forces Newtonsoft.Json to serialize the variable
        protected List<uint> weapons; //List of Match weapons
        [JsonProperty] //Attribute forces Newtonsoft.Json to serialize the variable
        protected string mapName; //Match map name

        public uint GetWeapon(int index) //Get weapon from the list
        {
            return weapons[index]; //Return weapon uint
        }
        public int ScoreLimit { get => scoreLimit; } //Class property for retrieving the score limit
        public long StartTime { get => startTime; } //Class property for retrieving the start time
        public long EndTime { get => (startTime + length); } //Class property for retrieving the end time
        public string Map { get => mapName; } //Class property for retrieving the map name
        public User Winner { get; set; } //Class property for retrieving the winner
    }
}
