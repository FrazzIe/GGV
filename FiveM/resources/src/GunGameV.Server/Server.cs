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

            if (currentMatch != null)
            {
                if ((currentMatch.EndTime - unixTimestamp) <= 0 && currentMatch.Winner == null)
                {
                    List<User> usersInMatch = users.FindAll(user => user.InMatch == true);

                    foreach (User user in usersInMatch)
                    {
                        if (currentMatch.Winner == null)
                        {
                            currentMatch.Winner = user;
                        }
                        else
                        {
                            if(user.gameStats.CompareTo(currentMatch.Winner.gameStats) == 1)
                            {
                                currentMatch.Winner = user;
                            }
                        }

                        Exports["jssql"].execute("UPDATE player SET kills=kills + ?, deaths=deaths + ?, games_played=games_played + 1 WHERE steam=?", new object[] { user.gameStats.Kills, user.gameStats.Deaths, user.Steam });
                        user.InMatch = false;
                        user.globalStats.Update(user.gameStats);
                    }

                    if (currentMatch.Winner != null)
                    {
                        Exports["jssql"].execute("UPDATE player SET wins=wins+1 WHERE steam=?", new[] { currentMatch.Winner.Steam });
                        TriggerClientEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 0, 0 },
                            multiline = true,
                            args = new[] { "GGV", currentMatch.Winner.Name + " won!" },
                        });
                    }

                    currentMatch = null;

                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
                    TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch));
                    TriggerClientEvent("GGV.Match.Leave");
                }
            }
        }
        public List<User> Users { get => users; }

        [EventHandler("playerConnecting")]
        private async void OnPlayerConnecting([FromSource]Player player, string name, dynamic setKickReason, dynamic deferrals)
        {
            deferrals.defer();

            deferrals.update("You have been haulted!");

            if (player.Identifiers["steam"] == null)
            {
                deferrals.done("Steam must be running to connect to this server!");
            }

            string steam64 = Utilities.GetSteam64(player.Identifiers["steam"]);

            dynamic ban = await Exports["jssql"].executeSync("SELECT ban.reason, UNIX_TIMESTAMP(ban.expire) AS 'expire' FROM ban WHERE player_id = (SELECT id FROM player WHERE steam = ?) AND (UNIX_TIMESTAMP(ban.expire) > UNIX_TIMESTAMP())", new[] { steam64 });

            if (ban.Count != 0)
            {
                deferrals.done(string.Format("You have been banned from the server until {0} for {1}", DateTimeOffset.FromUnixTimeSeconds(ban[0].expire).ToString(@"dd/MM/yyyy HH\:mm\:ss UTC"), ban[0].reason));
            }

            deferrals.done();
        }
        [EventHandler("GGV.Setup")]
        private void SetupUser([FromSource]Player player)
        {
            Debug.WriteLine("Attempting to setup {0}", player.Name);

            string steam64 = Utilities.GetSteam64(player.Identifiers["steam"]);

            Exports["jssql"].execute("SELECT wins, games_played, kills, deaths FROM player WHERE steam = ?", new[] { steam64 }, new Action<dynamic>((result) =>
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

            player.TriggerEvent("chat:addMessage", new
            {
                color = new[] { 255, 0, 0 },
                multiline = true,
                args = new[] { "GGV", string.Format("^*^3{0} ^r^0use ^*^1/ggv ^r^0to get started!", player.Name) },
            });
        }
        [EventHandler("baseevents:onPlayerKilled")]
        private void OnPlayerKilled([FromSource] Player victim, int attacker, dynamic deathData)
        {
            if (currentMatch != null)
            {
                if (attacker != -1)
                {
                    Player killer = Players[attacker];

                    if (killer != null)
                    {
                        User killerData = users.Find(user => user.ID == killer.Handle);
                        User victimData = users.Find(user => user.ID == victim.Handle);

                        if(killerData != null && victimData != null)
                        {
                            if(killerData.InMatch && victimData.InMatch)
                            {
                                TriggerClientEvent("chat:addMessage", new
                                {
                                    color = new[] { 255, 0, 0 },
                                    multiline = true,
                                    args = new[] { "GGV", string.Format("^*^8{0} ^r^0killed ^*^5{1}", killer.Name, victim.Name) },
                                });

                                if (deathData.weaponhash == 2725352035)
                                {
                                    victimData.gameStats.Deaths++;
                                    victimData.gameStats.Score--;
                                    killerData.gameStats.Kills++;

                                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
                                } else
                                {
                                    victimData.gameStats.Deaths++;
                                    killerData.gameStats.Kills++;

                                    if (currentMatch.Winner == null)
                                    {
                                        killerData.gameStats.Score++;

                                        if (killerData.gameStats.Score == currentMatch.ScoreLimit)
                                        {
                                            currentMatch.Winner = killerData;

                                            List<User> usersInMatch = users.FindAll(user => user.InMatch == true);

                                            foreach (User user in usersInMatch)
                                            {
                                                Exports["jssql"].execute("UPDATE player SET kills=kills + ?, deaths=deaths + ?, games_played=games_played + 1 WHERE steam=?", new object[] { user.gameStats.Kills, user.gameStats.Deaths, user.Steam });
                                                user.InMatch = false;
                                                user.globalStats.Update(user.gameStats);
                                            }

                                            currentMatch.Winner.globalStats.Wins++;
                                            Exports["jssql"].execute("UPDATE player SET wins=wins+1 WHERE steam=?", new[] { currentMatch.Winner.Steam });
                                            TriggerClientEvent("chat:addMessage", new
                                            {
                                                color = new[] { 255, 0, 0 },
                                                multiline = true,
                                                args = new[] { "GGV", currentMatch.Winner.Name + " won the game!" },
                                            });

                                            currentMatch = null;

                                            TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
                                            TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch));
                                            TriggerClientEvent("GGV.Match.Leave");
                                        } else
                                        {
                                            TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }
        [EventHandler("playerDropped")]
        private void OnPlayerDropped([FromSource] Player player)
        {
            users.RemoveAll(user => user.ID == player.Handle);
            TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
        }
        [EventHandler("rconCommand")]
        private void OnRconCommand(string commandName, dynamic args)
        {
            switch (commandName.ToLower())
            {
                case "ban":
                    if (args.Count >= 4)
                    {
                        int id;
                        int amount;

                        if (int.TryParse(args[0], out id))
                        {
                            if (int.TryParse(args[1], out amount))
                            {
                                long expire;

                                switch(args[2])
                                {
                                    case "h":
                                        expire = Utilities.UnixTimestamp + (amount * 60 * 60);
                                        break;
                                    case "m":
                                        expire = Utilities.UnixTimestamp + (amount * 60);
                                        break;
                                    case "s":
                                        expire = Utilities.UnixTimestamp + amount;
                                        break;
                                    default:
                                        expire = 0;
                                        break;
                                }

                                if (expire != 0)
                                {
                                    Player player = Players[id];

                                    if (player.EndPoint != null)
                                    {
                                        User user = users.Find(x => x.ID == player.Handle);

                                        if (user != null)
                                        {
                                            Exports["jssql"].execute("INSERT INTO ban (`player_id`, `reason`, `expire`) VALUES ((SELECT id FROM player WHERE steam = ?),?, FROM_UNIXTIME(?))", new object[] { user.Steam, args[3], expire });

                                            string banTime = DateTimeOffset.FromUnixTimeSeconds(expire).ToString(@"dd/MM/yyyy HH\:mm\:ss UTC");
                                            
                                            Debug.WriteLine("[GGV] ^3{0} ^0has been banned until ^3{1} ^0for ^3{2}^0", player.Name, banTime, args[3]);

                                            player.Drop(string.Format("You have been banned from the server until {0} for {1}", banTime, args[3]));                                            
                                        }
                                        else
                                        {
                                            player.Drop("Unable to find you in our users list? try rejoining!");
                                            Debug.WriteLine("[GGV] ^3{0} ^0was not found in the users list!", player.Name);
                                            API.CancelEvent();
                                        }
                                    }
                                    else
                                    {
                                        Debug.WriteLine("[GGV] ^1Unable to find player ^3{0}^0", id);
                                        API.CancelEvent();
                                    }
                                } else
                                {
                                    Debug.WriteLine("[GGV] ^1(h)ours, (m)inutes and (s)econds are the only accepted arguments!^0");
                                    API.CancelEvent();
                                }
                            } else
                            {
                                Debug.WriteLine("[GGV] ^1Unable to parse ^3\"{1}\"^1 as an Integer^0", args[1]);
                                API.CancelEvent();
                            }
                        }
                        else
                        {
                            Debug.WriteLine("[GGV] ^1Unable to parse ^3\"{1}\"^1 as an Integer^0", args[0]);
                            API.CancelEvent();
                        }
                    } else
                    {
                        Debug.WriteLine("[GGV] ban [id] [time] [h | m | s] [reason]");
                        API.CancelEvent();
                    }
                    break;
                case "kick":
                    if (args.Count >= 2)
                    {
                        int id;

                        if (int.TryParse(args[0], out id))
                        {
                            Player player = Players[id];

                            if (player.EndPoint != null)
                            {
                                Debug.WriteLine("[GGV] ^3{0} ^0has been kicked for ^3{1}^0", player.Name, args[1]);
                                player.Drop(args[1]);         
                                API.CancelEvent();
                            }
                            else
                            {
                                Debug.WriteLine("[GGV] ^1Unable to find player ^3{0}^0", id);
                                API.CancelEvent();
                            }
                        }
                        else
                        {
                            Debug.WriteLine("[GGV] ^1Unable to parse ^3\"{1}\"^1 as an Integer^0", args[0]);
                            API.CancelEvent();
                        }
                    }
                    else
                    {
                        Debug.WriteLine("[GGV] kick [id] [reason]");
                        API.CancelEvent();
                    }
                    break;
                default:
                    Debug.WriteLine("[GGV] No such command found!");
                    API.CancelEvent();
                    break;
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
                                currentMatch = new Match(10, 20);

                                user.InMatch = true;
                                user.gameStats = new GameStats();

                                TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
                                TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch));

                                messageObject.args[1] = "^*^3" + player.Name + "^r^0 started a match! type ^*^1/ggv join ^r^0 to participate!";
                                TriggerClientEvent("chat:addMessage", messageObject);

                                messageObject.args[1] = "Use ^*^1Z ^r^0to view the scoreboard while playing!";
                                player.TriggerEvent("chat:addMessage", messageObject);

                                player.TriggerEvent("GGV.Match.Join");
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
                                    user.gameStats = new GameStats();

                                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));

                                    player.TriggerEvent("GGV.Match.Join");

                                    messageObject.args[1] = "Use ^*^1Z ^r^0to view the scoreboard while playing!";
                                    player.TriggerEvent("chat:addMessage", messageObject);
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
                        case "leave":
                            if (user.InMatch)
                            {
                                user.InMatch = false;
                                user.gameStats = new GameStats();

                                TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));

                                player.TriggerEvent("GGV.Match.Leave");
                            }
                            else
                            {
                                messageObject.args[1] = "You must be in a match to use this command!";
                                player.TriggerEvent("chat:addMessage", messageObject);
                            }
                            break;
                        case "stats":
                            messageObject.args[1] = "User: ^*^3" + user.Name;
                            player.TriggerEvent("chat:addMessage", messageObject);
                            messageObject.args[1] = "Kills: ^*^3" + user.globalStats.Kills;
                            player.TriggerEvent("chat:addMessage", messageObject);
                            messageObject.args[1] = "Deaths: ^*^3" + user.globalStats.Deaths;
                            player.TriggerEvent("chat:addMessage", messageObject);
                            messageObject.args[1] = "Wins: ^*^3" + user.globalStats.Wins;
                            player.TriggerEvent("chat:addMessage", messageObject);
                            messageObject.args[1] = "Loses: ^*^3" + user.globalStats.Loses;
                            player.TriggerEvent("chat:addMessage", messageObject);
                            break;
                        default:
                            messageObject.args[1] = "Invalid syntax: ^*^1start^r^0, ^*^1join ^*^1leave ^r^0or ^*^1stats ^r^0are the only accepted arguments!";
                            player.TriggerEvent("chat:addMessage", messageObject);
                            break;
                    }
                } else
                {
                    messageObject.args[1] = "Invalid syntax: ^*^1start^r^0, ^*^1join ^*^1leave ^r^0or ^*^1stats ^r^0are the only accepted arguments!";
                    player.TriggerEvent("chat:addMessage", messageObject);
                }
            } else
            {
                player.Drop("Unable to find you in our users list? try rejoining!");
            }
        }
    }
}