const passport = require('passport');
const SteamStrategy = require('passport-steam').Strategy;
const keys = require('./keys');
const sql = require('./sql');

passport.use(  
  new SteamStrategy(
    {
      returnURL: 'http://localhost:3000/return',
      realm: 'http://localhost:3000/',
      apiKey: keys.steam.apiKey
    },
    (identifier, profile, done) => {
      let user = {
        link: identifier,
        steam: profile.id,
        name: profile.displayName,
        avatar: profile.photos[2].value
      }

      sql.execute(sql.getUser, [profile.id]).then((result) => {
        if (typeof result[0] === "undefined") {
          done(null, false);
        } else {
          user.timestamp = result[0].timestamp;
          user.lastplayed = result[0].lastplayed;
          user.wins = result[0].wins;
          user.games_played = result[0].games_played;
          user.kills = result[0].kills;
          user.deaths = result[0].deaths;

          console.log(user);
          
          done(null, user);
        }
      })
    }
  )
)

passport.serializeUser((user, done) => {  
  done(null, user)
})

passport.deserializeUser((obj, done) => {  
  done(null, obj)
})