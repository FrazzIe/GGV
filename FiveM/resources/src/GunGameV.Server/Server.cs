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

            if (player.Identifiers["steam"] == null)
            {
                deferrals.done("Steam must be running to connect to this server!");
            }

            deferrals.done();
        }

        [EventHandler("GGV.Setup")]
        private void SetupUser([FromSource]Player player)
        {
            Debug.WriteLine("Attempting to setup {0}", player.Name);

            string steam64 = Utilities.GetSteam64(player.Identifiers["steam"]);

            Exports["jssql"].execute("SELECT wins, games_played, kills, deaths FROM player WHERE steam = ?", new[] { "steam64" }, new Action<dynamic>((result) =>
            {
                if (result.Count == 0)
                {
                    Exports["jssql"].execute("INSERT INTO player (`steam`, `license`, `ip`, `name`) VALUES (?, ?, ?, ?)", new[] { steam64, player.Identifiers["license"], player.EndPoint, player.Name });

                    Users.Add(new User(player.Handle, player.Name, steam64, player.Identifiers["license"], player.EndPoint));

                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
                }
                else
                {
                    Users.Add(new User(player.Handle, player.Name, steam64, player.Identifiers["license"], player.EndPoint, result[0].kills, result[0].deaths, result[0].wins, result[0].games_played));

                    Exports["jssql"].execute("UPDATE player SET ip=?, name=? WHERE steam=?", new[] { player.EndPoint, player.Name, steam64 });

                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
                }
            }));

            if (currentMatch != null)
            {
                TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch));
            }
        }


        }
    }
}