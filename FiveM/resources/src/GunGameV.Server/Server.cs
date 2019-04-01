using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core;
using Newtonsoft.Json;
using GunGameV.Shared;

namespace GunGameV.Server
{
    public class Server : BaseScript
    {
        private List<User> users = new List<User>();
        private Match currentMatch;

        public Server()
        {
            Debug.WriteLine("SERVER STARTED");
        }
        [Tick]
        private async Task MatchWatcher()
        {
            await Delay(1000);

            long unixTimestamp = Utilities.UnixTimestamp;

            TriggerClientEvent("GGV.Sync.Time", unixTimestamp);

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

        [Command("ggv")]
        private void OnGunGameVCommand(Player player, string[] args) {
            User user = users.Find(x => x.ID == player.Handle);
            var messageObject = new
            {
                color = new[] { 255, 0, 0 },
                multiline = true,
                args = new[] { "GGV", "" },
            };

            if (user != null)
            {
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "start":
                            if (currentMatch == null)
                            {
                                currentMatch = new Match(1, 20);

                                TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch));
                            }
                            else
                            {
                                messageObject.args[1] = "A match is already in progress!";
                                player.TriggerEvent("chat:addMessage", messageObject);
                            }
                            break;
                        case "join":
                            if (currentMatch != null)
                            {
                                if (!user.InMatch)
                                {
                                    user.InMatch = true;

                                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));

                                    player.TriggerEvent("GGV.Match.Join");
                                }
                                else
                                {
                                    messageObject.args[1] = "You are already in a match!";
                                    player.TriggerEvent("chat:addMessage", messageObject);
                                }
                            }
                            else
                            {
                                messageObject.args[1] = "A match must be started to join one!";
                                player.TriggerEvent("chat:addMessage", messageObject);
                            }
                            break;
                            {

                            }
                            {
                                messageObject.args[1] = "You must be in a match to use this command!";
                                player.TriggerEvent("chat:addMessage", messageObject);
                            }
                            break;
                        default:
                            messageObject.args[1] = "Invalid syntax: start, join or leave are the only accepted arguments!";
                            player.TriggerEvent("chat:addMessage", messageObject);
                            break;
                    }
                } else
                {
                    messageObject.args[1] = "Invalid syntax: start, join or leave are the only accepted arguments!";
                    player.TriggerEvent("chat:addMessage", messageObject);
                }
            } else
            {
                player.Drop("Unable to find you in our users list? try rejoining!");
            }
        }

                        }
                        else
                        {
                            player.TriggerEvent("chat:addMessage", new
                            {
                                color = new[] { 255, 0, 0 },
                                multiline = true,
                                args = new[] { "GGV", "You must be in a match to use this command!" },
                            });
                        }
                        break;
                    default:
                        player.TriggerEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 0, 0 },
                            multiline = true,
                            args = new[] { "GGV", "Invalid syntax: start, join or leave are the only accepted arguments!" },
                        });
                        break;
                }
            } else
            {
                player.Drop("Unable to find you in our users list? try rejoining!");
            }
        }
    }
}