using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core;

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
                return;
            }
        }
        [EventHandler("GGV.Sync")]
        private void SyncUsers(string jsonUsers)
        {
            users = (List<User>) Json.From(jsonUsers);

            Debug.WriteLine(users[0].ToString());
        }
    }
}