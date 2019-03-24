using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core;
using Newtonsoft.Json;
using GunGameV.Shared;

namespace GunGameV.Server
{
    public class Server : BaseScript
    {
        private List<User> users = new List<User>();
        
        public Server()
        {
            Debug.WriteLine("SERVER STARTED");
        }

        public List<User> Users { get => users; }

        [EventHandler("playerConnecting")]
        private void OnPlayerConnecting([FromSource]Player player, string name, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();

            deferrals.update("You have been haulted!");
            Exports["jssql"].execute("SELECT * FROM player WHERE steam = ?", new[] { "523" }, new Action<dynamic>((result) => 
            {
                Debug.WriteLine(result.GetType().FullName);
                Debug.WriteLine(result[0].GetType().FullName);
                Debug.WriteLine(result[0].steam);
            }));
            deferrals.done("");
        }

        [EventHandler("GGV.Setup")]
        private void SetupUser([FromSource]Player player)
        {
            Debug.WriteLine("Attempting to setup {0}", player.Name);

            string steam = Utilities.GetSteam64(player);
            string license = player.Identifiers["license"];
            string ip = player.EndPoint;

            Users.Add(new User(player.Handle, player.Name, steam, license, ip));

            TriggerClientEvent("GGV.Sync", JsonConvert.SerializeObject(users));
        }
    }
}