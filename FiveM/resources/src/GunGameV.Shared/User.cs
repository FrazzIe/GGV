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
        [JsonProperty] //Attribute forces Newtonsoft.Json to serialize the variable
        private string id, name, steam, license, ip; //Players id, name, steam, license and ip address

        public GlobalStats globalStats; //Players global stats
        public GameStats gameStats = new GameStats(); //Players match stats

        public User(string _id, string _name, string _steam, string _license, string _ip, int kills = 0, int deaths = 0, int wins = 0, int gamesPlayed = 0) //Called when a new instance of the User class is created
        {
            id = _id; //Set the id
            name = _name; //Set the name
            steam = _steam; //Set the steam
            license = _license; //Set the license
            ip = _ip; //Set the ip address
            globalStats = new GlobalStats(kills, deaths, wins, gamesPlayed); //Set the overall stats retrieved from the database
        }
        public string ID { get => id; } //Class property that retrieves the ID of the player
        public string Name { get => name; } //Class property that retrieves the Name of the player
        public string Steam { get => steam; } //Class property that retrieves the Steam ID of the player
        public string License { get => license; } //Class property that retrieves License ID of the player
        public string IP { get => ip; } //Class property that retrieves the IP Address of the player
        public bool InMatch { get; set; } = false; //Class property that gets or sets wheather a player is in a match or not
    }
}