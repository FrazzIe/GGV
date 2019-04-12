using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;
using CitizenFX.Core;
using Newtonsoft.Json;
using GunGameV.Shared;

namespace GunGameV.Server //JsonConvert.SerializeObject is used to convert Objects to json strings to prevent data loss when transferring objects from server to client
{
    public class Server : BaseScript //Inherits from the BaseScript in CitizenFX.Core.Server
    {
        private List<User> users = new List<User>(); //This holds each users data
        private Match currentMatch; //This holds the current match instance

        public Server()
        {
            Debug.WriteLine("SERVER STARTED"); //Displays a message in the console that shows that the script has started successfully
        }
        [Tick] //This attribute will register the Task below as a Tick Handler
        private async Task MatchWatcher() //This asynchronous Task will be ran every Tick
        {
            await Delay(1000); //Wait 1 second before continuing

            long unixTimestamp = Utilities.UnixTimestamp; //Get the current unix utc timestamp

            TriggerClientEvent("GGV.Sync.Time", unixTimestamp); //Sync the server time with the client

            if (currentMatch != null) //Check if their is a match active
            {
                if ((currentMatch.EndTime - unixTimestamp) <= 0 && currentMatch.Winner == null) //Check if there is no time remaing in the match and no winner has been determined
                {
                    List<User> usersInMatch = users.FindAll(user => user.InMatch == true); //Retrieve every user currently in a match

                    foreach (User user in usersInMatch) //Loop through each user
                    {
                        if (currentMatch.Winner == null) //Check if this is the first loop
                        {
                            currentMatch.Winner = user; //Assign the user as the winner
                        }
                        else //If this is not the first time looping then
                        {
                            if(user.gameStats.CompareTo(currentMatch.Winner.gameStats) == 1) //Check if the users stats are higher than the current winners stats
                            {
                                currentMatch.Winner = user; //Set the new winner
                            }
                        }

                        Exports["jssql"].execute("UPDATE player SET kills=kills + ?, deaths=deaths + ?, games_played=games_played + 1 WHERE steam=?", new object[] { user.gameStats.Kills, user.gameStats.Deaths, user.Steam }); //Update the users kills, deaths and gamesplayed in the database
                        user.InMatch = false; //Remove the user from the match as it is now finished
                        user.globalStats.Update(user.gameStats); //Update the users stats locally
                    }

                    if (currentMatch.Winner != null) //If a winner has been determine then
                    {
                        currentMatch.Winner.globalStats.Wins++; //Increment the winners wins by one locally
                        Exports["jssql"].execute("UPDATE player SET wins=wins+1 WHERE steam=?", new[] { currentMatch.Winner.Steam }); //Increment the wins column in the database
                        TriggerClientEvent("chat:addMessage", new
                        {
                            color = new[] { 255, 0, 0 },
                            multiline = true,
                            args = new[] { "GGV", currentMatch.Winner.Name + " won!" },
                        }); //Send an event to every client which will display a message in the chat box that announces the winner
                    }

                    currentMatch = null; //The match is now over so set the current instance to null

                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users)); //Sync the users list with each client
                    TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch)); //Sync the current match with each client
                    TriggerClientEvent("GGV.Match.Leave"); //Remove each user from match they were currently in
                }
            }
        }
        public List<User> Users { get => users; } //Property used to retrieve the users list from an other class

        [EventHandler("playerConnecting")] //This attribute will register the function below as an Event Handler
        private async void OnPlayerConnecting([FromSource]Player player, string name, dynamic setKickReason, dynamic deferrals) //This event is called when a player attempts a connection to the server
        {
            deferrals.defer(); //Stops the user from joining and puts them on hold

            deferrals.update("You have been haulted!"); //Sends the user a message that they've been put on hold

            if (player.Identifiers["steam"] == null) //Check if the user has steam running in the background
            {
                deferrals.done("Steam must be running to connect to this server!"); //Prevent the user from joining if steam isn't running
            }

            string steam64 = Utilities.GetSteam64(player.Identifiers["steam"]); //Convert the users steam id from hexidecimal to decimal

            dynamic ban = await Exports["jssql"].executeSync("SELECT ban.reason, UNIX_TIMESTAMP(ban.expire) AS 'expire' FROM ban WHERE player_id = (SELECT id FROM player WHERE steam = ?) AND (UNIX_TIMESTAMP(ban.expire) > UNIX_TIMESTAMP())", new[] { steam64 }); //Check if the user is banned in the database

            if (ban.Count != 0) //If a result has been returned then
            {
                deferrals.done(string.Format("You have been banned from the server until {0} for {1}", DateTimeOffset.FromUnixTimeSeconds(ban[0].expire).ToString(@"dd/MM/yyyy HH\:mm\:ss UTC"), ban[0].reason)); //Remove the user from the server with a message stating that they are banned and for how long and why
            }

            deferrals.done(); //Let the user enter the server as validation checks are complete
        }
        [EventHandler("GGV.Setup")] //This attribute will register the function below as an Event Handler
        private void SetupUser([FromSource]Player player) //This event is called when the player has successfully loaded into the server
        {
            Debug.WriteLine("Attempting to setup {0}", player.Name); //Displays a message in console

            string steam64 = Utilities.GetSteam64(player.Identifiers["steam"]); //Convert the users steam id from hexidecimal to decimal

            Exports["jssql"].execute("SELECT wins, games_played, kills, deaths FROM player WHERE steam = ?", new[] { steam64 }, new Action<dynamic>((result) => //Find a users statistics in the database
            {
                if (result.Count == 0) //If no result was found then
                {
                    Exports["jssql"].execute("INSERT INTO player (`steam`, `license`, `ip`, `name`) VALUES (?, ?, ?, ?)", new[] { steam64, player.Identifiers["license"], player.EndPoint, player.Name }); //Insert a new user into the database

                    Users.Add(new User(player.Handle, player.Name, steam64, player.Identifiers["license"], player.EndPoint)); //Create a new user instance and add the user to the users list

                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));
                }
                else //If a result was returned from the database
                {
                    Users.Add(new User(player.Handle, player.Name, steam64, player.Identifiers["license"], player.EndPoint, result[0].kills, result[0].deaths, result[0].wins, result[0].games_played)); //Create a new user instance and add the user to the users list

                    Exports["jssql"].execute("UPDATE player SET ip=?, name=? WHERE steam=?", new[] { player.EndPoint, player.Name, steam64 }); //Update the users name and ip address in the database

                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users)); //Sync the users list with each client
                }
            }));

            if (currentMatch != null) //Check if there is a match
            {
                TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch)); //Sync the current match with each client
            }

            player.TriggerEvent("chat:addMessage", new
            {
                color = new[] { 255, 0, 0 },
                multiline = true,
                args = new[] { "GGV", string.Format("^*^3{0} ^r^0use ^*^1/ggv ^r^0to get started!", player.Name) },
            }); //Send an event to the client which will display a message in the chat box that informs the user of how to get started
        }
        [EventHandler("baseevents:onPlayerKilled")] //This attribute will register the function below as an Event Handler
        private void OnPlayerKilled([FromSource] Player victim, int attacker, dynamic deathData) //This event is called when a player is killed by something other than them self
        {
            if (currentMatch != null) //Check if there is a match
            {
                if (attacker != -1) //Check if the attacker is a player
                {
                    Player killer = Players[attacker]; //Find the attacker in the Players list

                    if (killer != null) //Check if killer was found in the player list
                    {
                        User killerData = users.Find(user => user.ID == killer.Handle); //Find the killers user instance in the users list
                        User victimData = users.Find(user => user.ID == victim.Handle); //Find the victims user instance in the users list

                        if(killerData != null && victimData != null) //Check if both the victim and killer were found in the users list
                        {
                            if(killerData.InMatch && victimData.InMatch) //Check if the victim and killer are both in a match
                            {
                                TriggerClientEvent("chat:addMessage", new
                                {
                                    color = new[] { 255, 0, 0 },
                                    multiline = true,
                                    args = new[] { "GGV", string.Format("^*^8{0} ^r^0killed ^*^5{1}", killer.Name, victim.Name) },
                                }); //Send an event to every client which will display a message in the chat box that announces who killed who

                                if (deathData.weaponhash == 2725352035) //Check if the weapon used to kill the victim was fists
                                {
                                    victimData.gameStats.Deaths++; //Add a death to the victims user data
                                    victimData.gameStats.Score--; //Remove a score point from the victims user data
                                    killerData.gameStats.Kills++; //Add a kill to the killers user data

                                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users)); //Sync the users list with each client
                                } else //If the weapon used wasn't fists then
                                {
                                    victimData.gameStats.Deaths++; //Add a death to the victims user data
                                    killerData.gameStats.Kills++; //Add a kill to the killers user data

                                    if (currentMatch.Winner == null) //Check if the game hasn't been won
                                    {
                                        killerData.gameStats.Score++; //Add a score point to the killers user data

                                        if (killerData.gameStats.Score == currentMatch.ScoreLimit) //Check if the killer has reached the match score limit
                                        {
                                            currentMatch.Winner = killerData; //Set the winner as the killer

                                            List<User> usersInMatch = users.FindAll(user => user.InMatch == true); //Retrieve every user currently in a match

                                            foreach (User user in usersInMatch) //Loop through each user
                                            {
                                                Exports["jssql"].execute("UPDATE player SET kills=kills + ?, deaths=deaths + ?, games_played=games_played + 1 WHERE steam=?", new object[] { user.gameStats.Kills, user.gameStats.Deaths, user.Steam }); //Update the users kills, deaths and gamesplayed in the database
                                                user.InMatch = false; //Remove the user from the match as it is now finishe
                                                user.globalStats.Update(user.gameStats); //Update the users stats locally
                                            }

                                            currentMatch.Winner.globalStats.Wins++; //Increment the winners wins by one locally
                                            Exports["jssql"].execute("UPDATE player SET wins=wins+1 WHERE steam=?", new[] { currentMatch.Winner.Steam }); //Increment the wins column in the database
                                            TriggerClientEvent("chat:addMessage", new
                                            {
                                                color = new[] { 255, 0, 0 },
                                                multiline = true,
                                                args = new[] { "GGV", currentMatch.Winner.Name + " won the game!" },
                                            }); //Send an event to every client which will display a message in the chat box that announces the winner

                                            currentMatch = null; //The match is now over so set the current instance to null

                                            TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users)); //Sync the users list with each client
                                            TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch)); //Sync the current match with each client
                                            TriggerClientEvent("GGV.Match.Leave"); //Remove each user from match they were currently in
                                        } else //If the killer hasn't reached the score limit then
                                        {
                                            TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users)); //Sync the users list with each client
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        [EventHandler("playerDropped")] //This attribute will register the function below as an Event Handler
        private void OnPlayerDropped([FromSource] Player player) //This event is called when a player leaves the server
        {
            users.RemoveAll(user => user.ID == player.Handle); //Removes any instance of the user from the users list
            TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users)); //Sync the users list with each client
        }
        [EventHandler("rconCommand")] //This attribute will register the function below as an Event Handler
        private void OnRconCommand(string commandName, dynamic args) //This event is called when a server command is entered into the server console
        {
            switch (commandName.ToLower()) //Input the command name in lower case into the switch statement
            {
                case "ban": //If the command is `ban` then
                    if (args.Count >= 4) //If the correct amount of arguments have been entered then
                    {
                        int id; //initialise id
                        int amount; //initialise amount

                        if (int.TryParse(args[0], out id)) //If arguement 0 was parsed successfully from a string to an int then assign the parsed value to id and continue
                        {
                            if (int.TryParse(args[1], out amount)) //If arguement 1 was parsed successfully from a string to an int then assign the parsed value to amount and continue
                            {
                                long expire; //initialse expire

                                switch(args[2]) //Search if the correct time format was entered
                                {
                                    case "h": //Hours
                                        expire = Utilities.UnixTimestamp + (amount * 60 * 60); //Add x amount of hours to the current unix timestamp
                                        break;
                                    case "m": //Minutes
                                        expire = Utilities.UnixTimestamp + (amount * 60); //Add x amount of minutes to the current unix timestamp
                                        break;
                                    case "s": //Seconds
                                        expire = Utilities.UnixTimestamp + amount; //Add x amount of seconds to the current unix timestamp
                                        break;
                                    default:
                                        expire = 0; //Set expire to 0 as a int cannot be null
                                        break;
                                }

                                if (expire != 0) //Check if the expire time is not 0
                                {
                                    Player player = Players[id]; //Find the player in the players list using the id

                                    if (player.EndPoint != null) //Check if the player exists
                                    {
                                        User user = users.Find(x => x.ID == player.Handle); //Find the user instance in the users list

                                        if (user != null) //Check if the user instance exists
                                        {
                                            Exports["jssql"].execute("INSERT INTO ban (`player_id`, `reason`, `expire`) VALUES ((SELECT id FROM player WHERE steam = ?),?, FROM_UNIXTIME(?))", new object[] { user.Steam, args[3], expire }); //Insert player ban into the database

                                            string banTime = DateTimeOffset.FromUnixTimeSeconds(expire).ToString(@"dd/MM/yyyy HH\:mm\:ss UTC"); //Convert unix timestamp to human readable date and time
                                            
                                            Debug.WriteLine("[GGV] ^3{0} ^0has been banned until ^3{1} ^0for ^3{2}^0", player.Name, banTime, args[3]); //Display a message to the server about the ban

                                            player.Drop(string.Format("You have been banned from the server until {0} for {1}", banTime, args[3])); //Kick the player from the server
                                        }
                                        else //If the user instance does not exist
                                        {
                                            Debug.WriteLine("[GGV] ^3{0} ^0was not found in the users list!", player.Name); //Display error message
                                            player.Drop("Unable to find you in our users list? try rejoining!"); //Remove the player from the server as they must have a user instance
                                            API.CancelEvent(); //Cancel the current running event
                                        }
                                    }
                                    else //If the player does not exist
                                    {
                                        Debug.WriteLine("[GGV] ^1Unable to find player ^3{0}^0", id); //Display error message
                                        API.CancelEvent(); //Cancel the current running event
                                    }
                                }
                                else //If expire time is 0 then
                                {
                                    Debug.WriteLine("[GGV] ^1(h)ours, (m)inutes and (s)econds are the only accepted arguments!^0"); //Display syntax message in console
                                    API.CancelEvent(); //Cancel the current running event
                                }
                            }
                            else //If the conversion from string to int failed then
                            {
                                Debug.WriteLine("[GGV] ^1Unable to parse ^3\"{1}\"^1 as an Integer^0", args[1]); //Display error message
                                API.CancelEvent(); //Cancel the current running event
                            }
                        }
                        else //If the conversion from string to int failed then
                        {
                            Debug.WriteLine("[GGV] ^1Unable to parse ^3\"{1}\"^1 as an Integer^0", args[0]); //Display error message
                            API.CancelEvent(); //Cancel the current running event
                        }
                    } else
                    {
                        Debug.WriteLine("[GGV] ban [id] [time] [h | m | s] [reason]");
                        API.CancelEvent(); //Cancel the current running event
                    }
                    break;
                case "kick": //If the command is `kick` then
                    if (args.Count >= 2) //If the correct amount of arguments have been entered then
                    {
                        int id; //initialise id

                        if (int.TryParse(args[0], out id)) //If arguement 0 was parsed successfully from a string to an int then assign the parsed value to id and continue
                        {
                            Player player = Players[id]; //Find the player in the players list using the id

                            if (player.EndPoint != null) //Check if the player exists
                            {
                                Debug.WriteLine("[GGV] ^3{0} ^0has been kicked for ^3{1}^0", player.Name, args[1]); //Display a message that the player has been kicked successfully
                                player.Drop(args[1]); //Remove the player from the server
                                API.CancelEvent(); //Cancel the current running event
                            }
                            else //If the player does not exist
                            {
                                Debug.WriteLine("[GGV] ^1Unable to find player ^3{0}^0", id); //Display error message
                                API.CancelEvent(); //Cancel the current running event
                            }
                        }
                        else //If the conversion from string to int failed then
                        {
                            Debug.WriteLine("[GGV] ^1Unable to parse ^3\"{1}\"^1 as an Integer^0", args[0]); //Display error message
                            API.CancelEvent(); //Cancel the current running event
                        }
                    }
                    else //If the correct amount of arguments have not been entered then
                    {
                        Debug.WriteLine("[GGV] kick [id] [reason]"); //Display message of the correct syntax
                        API.CancelEvent(); //Cancel the current running event
                    }
                    break;
                default: //if the command isn't a case in the switch then
                    Debug.WriteLine("[GGV] No such command found!"); //Display message that the command isn't found
                    API.CancelEvent(); //Cancel the current running event
                    break;
            }
        }
        [Command("ggv")] //This attribute will register the function below as a Command Handler
        private void OnGunGameVCommand(Player player, string[] args) { //This function is called when the command `ggv` is entered in the client console or chat box by a player
            User user = users.Find(x => x.ID == player.Handle); //Find the user instance in the users list
            var messageObject = new
            {
                color = new[] { 255, 0, 0 },
                multiline = true,
                args = new[] { "GGV", "" },
            }; //Create a template message object

            if (user != null) //Check if the user exists
            {
                if (args.Length > 0) //Check if the number of arguments entered is greater than 0
                {
                    switch (args[0]) //Find option in switch statement
                    {
                        case "start": //Used to start a gun game match
                            if (currentMatch == null) //Check if a match doesn't exist
                            {
                                currentMatch = new Match(10, 20); //Create a new match with a match length of 10 minutes and a score limit of 20 points

                                user.InMatch = true; //Set the user in the match
                                user.gameStats = new GameStats(); //Reset the users game stats by creating a new instance

                                TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users)); //Sync the users list with each client
                                TriggerClientEvent("GGV.Sync.Match", JsonConvert.SerializeObject(currentMatch)); //Sync the current match with each client

                                messageObject.args[1] = "^*^3" + player.Name + "^r^0 started a match! type ^*^1/ggv join ^r^0 to participate!"; //Set the message objects message
                                TriggerClientEvent("chat:addMessage", messageObject); //Send the message to the clients chat box

                                messageObject.args[1] = "Use ^*^1Z ^r^0to view the scoreboard while playing!"; //Set the message objects message
                                player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box

                                player.TriggerEvent("GGV.Match.Join"); //Transport the client into the match
                            }
                            else //If a match is already in progress then
                            {
                                messageObject.args[1] = "A match is already in progress!"; //Set the message objects message
                                player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                            }
                            break;
                        case "join": //Used to join the current gun game match in progress
                            if (currentMatch != null) //Check if a match exists
                            {
                                if (!user.InMatch) //Check if the user is not already in a match
                                {
                                    user.InMatch = true; //Set the user in the match
                                    user.gameStats = new GameStats(); //Reset the users game stats by creating a new instance

                                    TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users)); //Sync the users list with each client

                                    player.TriggerEvent("GGV.Match.Join"); //Transport the client into the match

                                    messageObject.args[1] = "Use ^*^1Z ^r^0to view the scoreboard while playing!"; //Set the message objects message
                                    player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                                }
                                else //If the user is already in a match then
                                {
                                    messageObject.args[1] = "You are already in a match!"; //Set the message objects message
                                    player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                                }
                            }
                            else //If a match doesn't exist then
                            {
                                messageObject.args[1] = "A match must be started to join one!"; //Set the message objects message
                                player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                            }
                            break;
                        case "leave": //Used to leave the current gun game match in progress
                            if (user.InMatch) //Check if the user is in a match
                            {
                                user.InMatch = false; //Remove the user from the match as it is now finished
                                user.gameStats = new GameStats(); //Reset the users game stats by creating a new instance

                                TriggerClientEvent("GGV.Sync.Users", JsonConvert.SerializeObject(users));

                                player.TriggerEvent("GGV.Match.Leave"); //Sync the users list with each client
                            }
                            else //If the user isn't in a match then
                            {
                                messageObject.args[1] = "You must be in a match to use this command!"; //Set the message objects message
                                player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                            }
                            break;
                        case "stats": //Used to view the players overall statistics in game
                            messageObject.args[1] = "User: ^*^3" + user.Name; //Set the message objects message
                            player.TriggerEvent("chat:addMessage", messageObject);
                            messageObject.args[1] = "Kills: ^*^3" + user.globalStats.Kills; //Set the message objects message
                            player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                            messageObject.args[1] = "Deaths: ^*^3" + user.globalStats.Deaths; //Set the message objects message
                            player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                            messageObject.args[1] = "Wins: ^*^3" + user.globalStats.Wins; //Set the message objects message
                            player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                            messageObject.args[1] = "Losses: ^*^3" + user.globalStats.Losses; //Set the message objects message
                            player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                            break;
                        default: //If option isn't found then
                            messageObject.args[1] = "Invalid syntax: ^*^1start^r^0, ^*^1join ^*^1leave ^r^0or ^*^1stats ^r^0are the only accepted arguments!"; //Set the message objects message
                            player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                            break;
                    }
                } else
                {
                    messageObject.args[1] = "Invalid syntax: ^*^1start^r^0, ^*^1join ^*^1leave ^r^0or ^*^1stats ^r^0are the only accepted arguments!"; //Set the message objects message
                    player.TriggerEvent("chat:addMessage", messageObject); //Send the message to the clients chat box
                }
            } else //If the user does not exist then
            {
                player.Drop("Unable to find you in our users list? try rejoining!"); //Remove the user from the server
            }
        }
    }
}