using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using GunGameV.Shared;

namespace GunGameV.Client
{
    public class Client : BaseScript
    {
        private List<User> users = new List<User>();
        private List<User> usersInMatch = new List<User>();
        private User user;
        private Match currentMatch;
        private Map currentMap;
        private long unixTimestamp = 0;

        public Client()
        {
            Debug.WriteLine("GGV CLIENT");
        }

        public List<User> Users { get => users; }

        [Tick]
        private async Task OnPlayerReady()
        {
            await Delay(0);

            if (API.NetworkIsSessionStarted())
            {
                TriggerServerEvent("GGV.Setup");
                Tick -= OnPlayerReady;
            }
        }
        [Tick]
        private async Task MatchWatcher()
        {
            await Delay(0);

            if (user != null && currentMatch != null)
            {
                if (user.InMatch)
                {
                    WeaponHash currentWeapon = (WeaponHash) currentMatch.GetWeapon(user.gameStats.Score);

                    if(!Game.PlayerPed.Weapons.HasWeapon(currentWeapon))
                    {
                        Game.PlayerPed.Weapons.RemoveAll();
                        Game.PlayerPed.Weapons.Give(currentWeapon, 250, true, true);
                    }

                    if(Game.PlayerPed.Weapons.Current.Hash != WeaponHash.Unarmed && Game.PlayerPed.Weapons.Current.Hash != currentWeapon)
                    {
                        Game.PlayerPed.Weapons.Remove(Game.PlayerPed.Weapons.Current);
                    }

                    //TimeSpan.FromSeconds(currentMatch.EndTime - unixTimestamp).ToString(@"mm\:ss"));
                }
            }
        }
        [EventHandler("GGV.Sync.Users")]
        private void SyncUsers(string jsonUsers)
        {
            users = JsonConvert.DeserializeObject<List<User>>(jsonUsers);
            usersInMatch = users.FindAll(user => user.InMatch == true);
            user = users.Find(x => x.ID == API.GetPlayerServerId(Game.Player.Handle).ToString());
        }
        [EventHandler("GGV.Sync.Match")]
        private void SyncMatch(string jsonMatch)
        {
            currentMatch = JsonConvert.DeserializeObject<Match>(jsonMatch);
        }
        [EventHandler("GGV.Sync.Time")]
        private void SyncTime(long _unixTimestamp)
        {
            unixTimestamp = _unixTimestamp;
        }
        [EventHandler("GGV.Match.Join")]
        private void JoinMatch()
        {
            if (currentMatch != null) {
                currentMap = new Map(currentMatch.Map);
                Exports["spawnmanager"].setAutoSpawnCallback(new Action(() =>
                {
                    currentMap.Spawn();
                }));
                Exports["spawnmanager"].forceRespawn();
            }
        }
        [EventHandler("GGV.Match.Leave")]
        private void LeaveMatch()
        {
            Exports["spawnmanager"].setAutoSpawnCallback(null);
            Exports["spawnmanager"].forceRespawn();
            currentMap = null;
        }
    }
}