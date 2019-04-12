using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace GunGameV.Server
{
    public class Match : Shared.Match //Inherits from the Match class in GunGameV.Shared
    {
        public Match(int _length = 10, int _scoreLimit = 20) //Called when a new instance of the match class is created
        {
            startTime = Utilities.UnixTimestamp; //Set the starting time of the match to the current unix time
            length = _length * 60; //Set the length of the match, convert parameter to minutes
            scoreLimit = _scoreLimit; //Set the score limit
            weapons = new List<uint>(); //Initialise a new List of unsigned integers

            Random random = new Random(); //Initialise a new instance of Random

            mapName = Utilities.Maps[random.Next(0, Utilities.Maps.Count - 1)]; //Select a random map name

            while (weapons.Count != (scoreLimit - 1)) //Loop until the number of weapons in the list is equal to the score limit minus one
            {
                uint weapon = Utilities.Weapons[random.Next(0, Utilities.Weapons.Count - 1)]; //Select a random weapon

                if (!weapons.Contains(weapon)) //Check for duplicates
                {
                    weapons.Add(weapon); //Add weapon to list
                }
            }

            weapons.Add(Utilities.MeleeWeapons[random.Next(0, Utilities.MeleeWeapons.Count - 1)]); //Add random melee weapon for final weapon
        }
    }
}