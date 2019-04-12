using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GunGameV.Shared
{
    abstract public class Statistics //Cannot be instantiated because its abstract
    {
        public int Kills { get; set; } //Class property to get or set Kills
        public int Deaths { get; set; } //Class property to get or set Deaths
    }

    public class GlobalStats : Statistics //Holds a users overall Statistics, Inherits from Statistics for Kills and Deaths
    {
        [JsonProperty] //Attribute forces Newtonsoft.Json to serialize the variable
        private int gamesPlayed; //Number of games played

        public GlobalStats(int kills = 0, int deaths = 0, int wins = 0, int _gamesPlayed = 0) //Called when a new instance of the GlobalStats class is created
        {
            Kills = kills; //Sets the kills
            Deaths = deaths; //Sets the deaths
            Wins = wins; //Sets the wins
            gamesPlayed = _gamesPlayed; //Sets the games played
        }
        public int Wins { get; set; } //Class property to get or set the Wins
        public int Losses { get => (gamesPlayed - Wins); } //Class property to get the number of losses
        public void Update(GameStats gameStats) //Function thats called when a match ends to add the game stats to the overall stats
        {
            Kills += gameStats.Kills; //Add match kills
            Deaths += gameStats.Deaths; //Add match deaths
            gamesPlayed++; //Increment games played by 1
        }
    }

    public class GameStats : Statistics, IComparable //Holds a users Statisitics per match, Inherits from Statistics for Kills and Deaths, Inherits from IComparable for the CompareTo method
    {
        public GameStats() //Called when a new instance of the GameStats class is created
        {
            Kills = 0; //Set kills to 0
            Deaths = 0; //Set deaths to 0
            Score = 0; //Set score to 0
        }
        public int Score { get; set; } //Class property to get or set the Score
        public int CompareTo(object obj) //Function thats usedd to compare another object of the same type
        {
            if (obj == null) return 1; //If obj is null then return greater than

            GameStats otherStats = obj as GameStats; //Cast obj to a GameStats type

            if(otherStats != null) //Check if otherStats is not null
            {
                int score = this.Score.CompareTo(otherStats.Score); //Compare scores
                if (score == 0) //If score is equal then
                {
                    int kills = this.Kills.CompareTo(otherStats.Kills); //Compare kills
                    if(kills == 0) //If kills is equal then
                    {
                        int deaths = this.Kills.CompareTo(otherStats.Deaths); //Compare deaths

                        return deaths; //Return result of deaths comparison
                    } else
                    {
                        return kills; //Return result of kills comparison
                    }
                } else
                {
                    return score; //Return result of score comparison
                }
            } else
            {
                throw new ArgumentException("Object is not a GameStats"); //Throw an error as obj was not a GameStats type
            }
        }
    }
}
