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
        private User user;
        private Match currentMatch;

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
        {

            {
            }
        }
        [EventHandler("GGV.Sync.Users")]
        private void SyncUsers(string jsonUsers)
        {
            users = JsonConvert.DeserializeObject<List<User>>(jsonUsers);
            user = users.Find(x => x.ID == API.GetPlayerServerId(Game.Player.Handle).ToString());
        }
        [EventHandler("GGV.Sync.Match")]
        private void SyncMatch(string jsonMatch)
        {
            currentMatch = JsonConvert.DeserializeObject<Match>(jsonMatch);
        }
    }
}