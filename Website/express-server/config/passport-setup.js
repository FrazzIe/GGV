const passport = require('passport');
const SteamStrategy = require('passport-steam').Strategy;
const keys = require('./keys');

passport.use(  
  new SteamStrategy(
    {
      returnURL: 'http://localhost:3000/return',
      realm: 'http://localhost:3000/',
      apiKey: keys.steam.apiKey
    },
    (identifier, profile, done) => {
      profile.identifier = identifier;

      done(null, profile)
    }
  )
)

passport.serializeUser((user, done) => {  
  done(null, user)
})

passport.deserializeUser((obj, done) => {  
  done(null, obj)
})