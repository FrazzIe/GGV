const passport = require('passport'); //include passport
const SteamStrategy = require('passport-steam').Strategy; //include steam strategy
const keys = require('./keys'); //include kets
const sql = require('./sql'); //include sql

passport.use(  //Make the passport use the SteamStrategy for user authentication
  new SteamStrategy(
    {
      returnURL: 'http://localhost:3000/return', //Return url
      realm: 'http://localhost:3000/', //Main url
      apiKey: keys.steam.apiKey //Steam api key
    },
    (identifier, profile, done) => {
      let user = {
        link: identifier,
        steam: profile.id,
        name: profile.displayName,
        avatar: profile.photos[2].value
      } //Holds a user

      sql.execute(sql.getUser, [profile.id]).then((result) => { //Retrieves the user from the ddatabase
        if (typeof result[0] === "undefined") { //If the user does not exist then 
          done(null, false); //Refuse to authenticate the user
        } else {
          user.timestamp = result[0].timestamp;
          user.lastplayed = result[0].lastplayed;
          user.wins = result[0].wins;
          user.games_played = result[0].games_played;
          user.kills = result[0].kills;
          user.deaths = result[0].deaths; //Add database values to the user object

          console.log(user);
          
          done(null, user); //Set the user cookie, login successful
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