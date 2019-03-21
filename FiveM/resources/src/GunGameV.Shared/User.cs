namespace GunGameV.Shared
{
    public class User
    {
        private int id;
        private string name, steam, license, ip;

        public GlobalStats globalStats;
        public GameStats gameStats = new GameStats();

        public User(int _id, string _name, string _steam, string _license, string _ip, int kills = 0, int deaths = 0, int wins = 0, int gamesPlayed = 0)
        {
            id = _id;
            name = _name;
            steam = _steam;
            license = _license;
            ip = _ip;
            globalStats = new GlobalStats(kills, deaths, wins, gamesPlayed);
        }
        public int ID { get => id; }
        public string Name { get => name; }
        public string Steam { get => steam; }
        public string License { get => license; }
        public string IP { get => ip; }
    }
}