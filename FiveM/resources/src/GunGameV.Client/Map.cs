using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GunGameV.Client
{
    public class Map
    {
        private List<Vector3> spawnpoints; //This holds all the spawnpoint for the map

        public Map(string name) //Called when a new instance of the Map class is created
        {
            spawnpoints = new List<Vector3>(); //Initialises the list

            string mapJson = API.LoadResourceFile(API.GetCurrentResourceName(), "maps/" + name + ".json"); //Attempts to load in the map file filled with spawn points

            if(string.IsNullOrEmpty(mapJson)) //Checks if the file is empty
            {
                Debug.WriteLine("An error occured when loading the {0} map", name); //Display error
                return; //Cancel function from going any further
            }

            JArray mapData = JsonConvert.DeserializeObject<JArray>(mapJson); //Convert the json file into a Json Array object
            
            foreach(JToken spawnpoint in mapData) //Loop through each spawnpoint in the json array
            {
                spawnpoints.Add(new Vector3(
                    float.Parse(spawnpoint["X"].ToString()),
                    float.Parse(spawnpoint["Y"].ToString()),
                    float.Parse(spawnpoint["Z"].ToString())
                )); //Create Vector3s for each spawnpoint in the Json Array and add them to the list of spawnpoints
            }
        }

        private void FreezePlayer(bool freeze) //Function to freeze the player in place
        {
            int player = Game.Player.Handle; //The player id
            int ped = Game.PlayerPed.Handle; //The player character id

            API.SetPlayerControl(player, !freeze, 0); //Set if controls are enabled or not depending on the value of freeze

            if(!freeze) //If not freeze then
            {
                if(!API.IsEntityVisible(ped)) //If character is not visible then
                {
                    API.SetEntityVisible(ped, true, false); //Set character visible
                }

                if(!API.IsPedInAnyVehicle(ped, false)) //If character not in vehicle then
                {
                    API.SetEntityCollision(ped, true, false); //Set character can collide with entities
                }

                API.FreezeEntityPosition(ped, false); //Unfreeze the character
                API.SetPlayerInvincible(player, false); //Turn off character godmode
            } else
            {
                if (API.IsEntityVisible(ped)) //If the character is visible then
                {
                    API.SetEntityVisible(ped, false, false); //Set the character invisible
                }

                API.SetEntityCollision(ped, false, false); //Make it so the character can no longer collide with entities
                API.FreezeEntityPosition(ped, true); //Freeze the character
                API.SetPlayerInvincible(player, true); //Set the character invincible

                if(!API.IsPedFatallyInjured(ped)) //Check if the character is injured
                {
                    API.ClearPedTasksImmediately(ped); //If so cancel any tasks the character is currently doing
                }
            }
        }
        public async void Spawn() //Function used for spawning a player in the map during a match
        {
            API.DoScreenFadeOut(500); //Makes the screen fade out for 0.5 seconds

            while (API.IsScreenFadingOut()) //Loops until the screen is finished fading out
            {
                await BaseScript.Delay(0); //Waits 0 miliseconds
            }

            Vector3 spawnpoint = spawnpoints[API.GetRandomIntInRange(0, spawnpoints.Count)]; //Select a random spawnpoint

            FreezePlayer(true); //Freeze the player

            API.RequestCollisionAtCoord(spawnpoint.X, spawnpoint.Y, spawnpoint.Z); //Load the map at the position of the spawnpoint

            int ped = Game.PlayerPed.Handle; //The player character id

            API.SetEntityCoordsNoOffset(ped, spawnpoint.X, spawnpoint.Y, spawnpoint.Z, false, false, false); //Teleport the player to the spawnpoint
            API.NetworkResurrectLocalPlayer(spawnpoint.X, spawnpoint.Y, spawnpoint.Z, 0f, true, true); //Revive the player at the spawnpoint

            API.ClearPedTasksImmediately(ped); //cancel any tasks the character is currently doing
            API.RemoveAllPedWeapons(ped, true); //Remove all the players weapons
            API.ClearPedBloodDamage(ped); //Remove any blood on the player

            while (!API.HasCollisionLoadedAroundEntity(ped)) //Loop until the map has loaded around the player
            {
                await BaseScript.Delay(0); //Waits 0 miliseconds
            }

            API.ShutdownLoadingScreen(); //Shutdown the loading screen if active

            API.DoScreenFadeIn(500); //Makes the screen fade in for 0.5 seconds

            while (API.IsScreenFadingIn()) //Loops until the screen is finished fading in
            {
                await BaseScript.Delay(0); //Waits 0 miliseconds
            }

            FreezePlayer(false); //Unfreezes the player
        }
    }
}