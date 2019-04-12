using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using GunGameV.Shared;

namespace GunGameV.Client
{
    public class Client : BaseScript
    {
        private List<User> users = new List<User>(); //This holds each users data
        private List<User> usersInMatch = new List<User>(); //This holds each users data that is in a match
        private User user; //This holds the clients user data
        private Match currentMatch; //This holds the current match instance
        private Map currentMap; //This holds the current map instance
        private HUD hud; //This holds the current hud instance
        private long unixTimestamp = 0; //This holds the synced unix timestamp from the server

        public Client()
        {
            Debug.WriteLine("GGV CLIENT"); //Displays a message in the console that shows that the script has started successfully
            hud = new HUD(); //Creates a new instance of the HUD
        }

        public List<User> Users { get => users; } //Property used to retrieve the users list from an other class

        private void SendNuiMessage(string type, object payload) //Used to send messages to NUI which is a chromium overlay
        {
            dynamic message = new ExpandoObject(); //Create a new instance of ExpandoObject
            message.type = type; //Set the type of message
            message.payload = payload; //Set the payload

            API.SendNuiMessage(JsonConvert.SerializeObject(message)); //Convert ExpandoObject to json and send to NUI
        }
        [Tick] //This attribute will register the Task below as a Tick Handler
        private async Task OnPlayerReady() //This asynchronous Task will be ran every Tick until the client has fully loaded into the server
        {
            await Delay(0);

            if (API.NetworkIsSessionStarted()) //Check if the client is fully loaded in
            {
                TriggerServerEvent("GGV.Setup"); //Setup user
                Tick -= OnPlayerReady; //Remove Tick
            }
        }
        [Tick] //This attribute will register the Task below as a Tick Handler
        private async Task MatchWatcher() //This asynchronous Task will be ran every Tick
        {
            if (user != null) //Check if the user exists
            {
                if (currentMatch != null) //Check if a match exists
                {
                    if (user.InMatch) //Check if the user is in a match
                    {
                        WeaponHash currentWeapon = (WeaponHash)currentMatch.GetWeapon(user.gameStats.Score); //Get the weapon that the player should have

                        if (!Game.PlayerPed.Weapons.HasWeapon(currentWeapon)) //Check if the player has the weapon and if not then
                        {
                            Game.PlayerPed.Weapons.RemoveAll(); //Remove all weapons
                            Game.PlayerPed.Weapons.Give(currentWeapon, 250, true, true); //Give player weapon
                        }

                        if (Game.PlayerPed.Weapons.Current.Hash != WeaponHash.Unarmed && Game.PlayerPed.Weapons.Current.Hash != currentWeapon) //Check if player has a weapon that they shouldn't have then
                        {
                            Game.PlayerPed.Weapons.Remove(Game.PlayerPed.Weapons.Current); //Remove weapon
                        }

                        hud.Time = TimeSpan.FromSeconds(currentMatch.EndTime - unixTimestamp).ToString(@"mm\:ss"); //Set the time remaining on the HUD
                        hud.Draw(); //Draw the HUD
                    }
                }

                for (int i = 0; i < 255; i++) //Loop through all possible player indexes
                {
                    if (API.NetworkIsPlayerActive(i)) //Check if this is a player
                    {
                        API.SetCanAttackFriendly(API.GetPlayerPed(i), user.InMatch, user.InMatch); //Set wheather or not the player can kill others depending on if they are in a game
                        API.NetworkSetFriendlyFireOption(user.InMatch); //Set wheather or not the player can kill others depending on if they are in a game
                    }
                }
            }
        }
        [Tick] //This attribute will register the Task below as a Tick Handler
        private async Task ScoreboardWatcher() //This asynchronous Task will be ran every Tick
        {
            if (user != null && currentMatch != null) //Check if the user exists and a match exists
            {
                if (user.InMatch) //Check if the user is in a match
                {
                    if (API.IsControlPressed(1, 20)) //Check if Z is pressed
                    {
                        SendNuiMessage("Show", "true"); //Send a message to NUI which will show the scoreboard

                        while (API.IsControlPressed(1, 20) && currentMatch != null && user.InMatch) //Loop while Z is pressed and a match is active and the player is in a match
                        {
                            await Delay(0); //Wait 0 milisecondds

                            if(API.IsControlJustPressed(0, 174)) //Check if left arrow is pressed
                            {
                                SendNuiMessage("Left", "left"); //Send a message to NUI which will go back a page in the scoreboard
                            }

                            if (API.IsControlJustPressed(0, 175)) //Check if right arrow is pressed
                            {
                                SendNuiMessage("Right", "right"); //Send a message to NUI which will go forward a page in the scoreboard
                            }
                        }

                        SendNuiMessage("Show", "false"); //Send a message to NUI which will hide the scoreboard
                    }
                }
            }
        }
        [EventHandler("GGV.Sync.Users")] //This attribute will register the function below as an Event Handler
        private void SyncUsers(string jsonUsers) //This event is called when the server syncs users
        {
            users = JsonConvert.DeserializeObject<List<User>>(jsonUsers); //Converts json string to a list of users
            usersInMatch = users.FindAll(user => user.InMatch == true); //Finds every user in a match
            usersInMatch.Sort((x, y) => y.gameStats.CompareTo(x.gameStats)); //Sorts every user in a match by stats
            user = users.Find(x => x.ID == API.GetPlayerServerId(Game.Player.Handle).ToString()); //Gets the users instance

            SendNuiMessage("SetPlayers", usersInMatch); //Sends the users in a match to NUI which will update the scoreboard

            if (user != null) //Check if user is not null
            {
                if (user.InMatch) //Check if user is in a match
                {
                    if(usersInMatch.Count >= 1) //Check if there is one or more players in a match
                    {
                        if (usersInMatch[0].ID != user.ID) //Check if the user with the top score does not equal the user
                        {
                            hud.Highscore = usersInMatch[0].gameStats.Score; //Set the highscore to the player in first place
                        }
                        else if (usersInMatch.Count > 1) //Check if there is more than one player in a match
                        {
                            hud.Highscore = usersInMatch[1].gameStats.Score; //Set the highscore to the player in second place
                        }
                        else 
                        {
                            hud.Highscore = 0; //Set the highscore to 0
                        }
                    } else
                    {
                        hud.Highscore = 0; //Set the highscore to 0
                    }

                    hud.Score = user.gameStats.Score; //Set the score to the users score
                }
            }
        }
        [EventHandler("GGV.Sync.Match")] //This attribute will register the function below as an Event Handler
        private void SyncMatch(string jsonMatch) //This event is called when the server syncs the current match
        {
            currentMatch = JsonConvert.DeserializeObject<Match>(jsonMatch); //Converts json string to a instance of the match class
        }
        [EventHandler("GGV.Sync.Time")] //This attribute will register the function below as an Event Handler
        private void SyncTime(long _unixTimestamp) //This event is called when the server sync the unix timestamp
        {
            unixTimestamp = _unixTimestamp; //Sets the unix timestamp
        }
        [EventHandler("GGV.Match.Join")] //This attribute will register the function below as an Event Handler
        private void JoinMatch() //This event is called when the user joins a match
        {
            if (currentMatch != null) { //Checks that there is a match
                currentMap = new Map(currentMatch.Map); //Creates a new instance of the map class
                Exports["spawnmanager"].setAutoSpawnCallback(new Action(() =>
                {
                    currentMap.Spawn();
                })); //Changes the spawnmanager to make players respawn in the match map
                Exports["spawnmanager"].forceRespawn(); //Teleports player to the match
            }
        }
        [EventHandler("GGV.Match.Leave")] //This attribute will register the function below as an Event Handler
        private void LeaveMatch() //This event is called when the user leaves a match or a match finishes
        {
            Exports["spawnmanager"].setAutoSpawnCallback(null); //Puts the spawnmanagers respawning back to normal
            Exports["spawnmanager"].forceRespawn(); //Teleports player out of the match
            currentMap = null; //Sets the instance of the map class to null
        }
    }
}