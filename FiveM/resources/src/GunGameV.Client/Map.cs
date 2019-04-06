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
        private List<Vector3> spawnpoints;

        public Map(string name)
        {
            spawnpoints = new List<Vector3>();

            string mapJson = API.LoadResourceFile(API.GetCurrentResourceName(), "maps/" + name + ".json");

            if(string.IsNullOrEmpty(mapJson))
            {
                Debug.WriteLine("An error occured when loading the {0} map", name);
                return;
            }

            JArray mapData = JsonConvert.DeserializeObject<JArray>(mapJson);
            
            foreach(JToken spawnpoint in mapData)
            {
                spawnpoints.Add(new Vector3(
                    float.Parse(spawnpoint["X"].ToString()),
                    float.Parse(spawnpoint["Y"].ToString()),
                    float.Parse(spawnpoint["Z"].ToString())
                ));

                Debug.WriteLine("Added spawnpoint X: {0}, Y: {1}, Z: {2}", spawnpoint["X"], spawnpoint["Y"], spawnpoint["Z"]);
            }
        }

        private void FreezePlayer(bool freeze)
        {
            int player = Game.Player.Handle;
            int ped = Game.PlayerPed.Handle;

            API.SetPlayerControl(player, !freeze, 0);

            if(!freeze)
            {
                if(!API.IsEntityVisible(ped))
                {
                    API.SetEntityVisible(ped, true, false);
                }

                if(!API.IsPedInAnyVehicle(ped, false))
                {
                    API.SetEntityCollision(ped, true, false);
                }

                API.FreezeEntityPosition(ped, false);
                API.SetPlayerInvincible(player, false);
            } else
            {
                if (API.IsEntityVisible(ped))
                {
                    API.SetEntityVisible(ped, false, false);
                }

                API.SetEntityCollision(ped, false, false);
                API.FreezeEntityPosition(ped, true);
                API.SetPlayerInvincible(player, true);

                if(!API.IsPedFatallyInjured(ped))
                {
                    API.ClearPedTasksImmediately(ped);
                }
            }
        }
        public async void Spawn()
        {
            API.DoScreenFadeOut(500);

            while (API.IsScreenFadingOut())
            {
                await BaseScript.Delay(0);
            }

            Vector3 spawnpoint = spawnpoints[API.GetRandomIntInRange(0, spawnpoints.Count)];

            FreezePlayer(true);

            API.RequestCollisionAtCoord(spawnpoint.X, spawnpoint.Y, spawnpoint.Z);

            int ped = Game.PlayerPed.Handle;

            API.SetEntityCoordsNoOffset(ped, spawnpoint.X, spawnpoint.Y, spawnpoint.Z, false, false, false);
            API.NetworkResurrectLocalPlayer(spawnpoint.X, spawnpoint.Y, spawnpoint.Z, 0f, true, true);

            API.ClearPedTasksImmediately(ped);
            API.RemoveAllPedWeapons(ped, true);
            API.ClearPedBloodDamage(ped);

            while (!API.HasCollisionLoadedAroundEntity(ped))
            {
                await BaseScript.Delay(0);
            }

            API.ShutdownLoadingScreen();

            API.DoScreenFadeIn(500);

            while (API.IsScreenFadingIn())
            {
                await BaseScript.Delay(0);
            }

            FreezePlayer(false);
        }
    }
}
