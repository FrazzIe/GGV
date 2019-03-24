using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core;
using Newtonsoft.Json;
using GunGameV.Shared;

namespace GunGameV.Client
{
    public class Client : BaseScript
    {
        private List<User> users = new List<User>();

        public Client()
        {
            Debug.WriteLine("GGV CLIENT");
        }

        public List<User> Users { get => users; }

        [Tick]
        private async Task OnPlayerReady()
        {
            await Delay(0);

            if (NetworkIsSessionStarted())
            {
                TriggerServerEvent("GGV.Setup");
                Tick -= OnPlayerReady;
            }
        }
        [EventHandler("GGV.Sync")]
        private void SyncUsers(string jsonUsers)
        {

            users = JsonConvert.DeserializeObject<List<User>>(jsonUsers);

            for (int i = 0; i < users.Count; i++)
            {
                Debug.WriteLine("-----------------");
                Debug.WriteLine("User Index {0}", i);
                Debug.WriteLine("User: {0}, {1}, {2}, {3}, {4}", users[i].ID, users[i].Name, users[i].Steam, users[i].License, users[i].IP);
                Debug.WriteLine("Global Stats: {0}, {1}, {2}, {3}", users[i].globalStats.Kills, users[i].globalStats.Deaths, users[i].globalStats.Wins, users[i].globalStats.Loses);
                Debug.WriteLine("Game Stats: {0}, {1}, {2}", users[i].gameStats.Kills, users[i].gameStats.Deaths, users[i].gameStats.Score);
            }
        }
    }
}