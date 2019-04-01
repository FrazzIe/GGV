using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace GunGameV.Server
{
    public class Match : Shared.Match
    {
        public Match(int _length = 10, int _scoreLimit = 20)
        {
            startTime = Utilities.UnixTimestamp;
            length = _length * 60;
            scoreLimit = _scoreLimit;
            weapons = new List<uint>();

            Random random = new Random();

            while (weapons.Count != (scoreLimit - 1))
            {
                uint weapon = Utilities.Weapons[random.Next(0, Utilities.Weapons.Count - 1)];

                if (!weapons.Contains(weapon))
                {
                    weapons.Add(weapon);
                }
            }

            weapons.Add(Utilities.MeleeWeapons[random.Next(0, Utilities.MeleeWeapons.Count - 1)]);
        }
    }
}