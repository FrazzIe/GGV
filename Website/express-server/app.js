const express = require('express'); //include express
const cookieSession = require('cookie-session'); //include cookie-session
const bodyParser = require('body-parser'); //include body-parser
const passportSetup = require('./config/passport-setup'); //include and setup the passport
const passport = require('passport'); //include passport
const axios = require('axios'); //include axios

const keys = require('./config/keys'); //include keys
const sql = require('./config/sql'); //include sql

const publicRoot = '../vue-client/dist'; //The path to where the client website is stored

const app = express(); //Initialise a new instance of express

app.use(express.static(publicRoot)) //Make the express aware that the publicRoot has static assets
app.use(bodyParser.json()) //Create json parser

app.use(cookieSession({  
  keys: keys.session.cookieKeys,
  maxAge: 24 * 60 * 60 * 1000
})) //Create a new user session

app.use(passport.initialize()); //Initialise passport
app.use(passport.session()); //Initialise the passport session

app.get("/", (req, res, next) => { //When the root is requested by a user
  res.sendFile("index.html", { root: publicRoot}) //Display index page in the location of the client website
})

app.get("/login", (req, res, next) => {   //When /login is requested by a user
  passport.authenticate("steam", (err, profile, info) => { //Passport attempts to authenticate the user by redirecting them to the steam login page
    if (err) { //If there is an error then
      return next(err); 
    }

    if (!profile) { //If the profile does not exist then
      return res.status(400).send([profile, "Cannot log in", info]); //refuse login
    }

    req.login(profile, err => { //Login user
      res.send("Logged in");
    });
  })(req, res, next);
});

app.get("/return", (req, res, next) => {  //When /return is requested by a user
  passport.authenticate("steam", (err, profile, info) => { //Passport attempts to authenticate the user by redirecting them to the steam login page
    if (err) { //If there is an error then
      return next(err);
    }

    if (!profile) { //If the profile does not exist then
      return res.status(400).send([profile, "Cannot log in", info]); //refuse login
    }

    req.login(profile, err => { //Login user, redirect to homepage
      res.redirect("/"); //Redirects to home page
    });
  })(req, res, next);
});

app.get("/logout", function(req, res) {  //When /logout is requested by the user
  req.logout(); //Logs the user out using the passport
  res.redirect("/"); //Redirects to home page
});

app.get("/api/user", function(req, res) { //When /api/user is requested by the user
  if(req.isAuthenticated()) { //Check if the user has been authenticated then
    res.send({user: req.session.passport.user}); //return user object
  } else { //If not authenticated then
    res.send({user: false}); //return false
  }
});

app.get("/api/user/:steamid", function(req, res) { //When /api/user/:steamid is requested by the user
  let steamid = req.params.steamid; //Assigns the steamid paramater to a variable

  sql.execute(sql.getUser, [steamid]).then((result) => { //Attempts to find the user in the database with that steam idd
    if (typeof result[0] === "undefined") { //If the user isnt found then
      res.send({user: false}); //return false
    } else { //If the user is found then
      result[0].steamid = steamid; //Add steam id to result object
      res.send({user: result[0]}); //return result object
    }
  });
});

app.get("/api/stats", function(req, res) { //When /api/stats is requested by the user
  sql.execute(sql.topKills, []).then((kills) => { //Selects the users with the top 10 kills from the database
    sql.execute(sql.topDeaths, []).then((deaths) => { //Selects the users with the top 10 deaths from the database
      sql.execute(sql.topWins, []).then((wins) => { //Selects the users with the top 10 wins from the database
        sql.execute(sql.topLosses, []).then((losses) => { //Selects the users with the top 10 losses from the database
          res.send({statistics: { kills: kills, deaths: deaths, wins: wins, losses: losses}}); //Send results to the client
        })
      })
    })
  })
});

app.get("/api/search/:value", function(req, res) { //When /api/search/:value is requested by the user
  let value = req.params.value; //Assigns the search value paramater to a variable

  if (value !== "undefined") {  //Check if the value is not null
    value = encodeURI(value).replace("%20", " "); //Remove html symbols
    value = "%" + value + "%"; //Replace space symbols with spaces

    sql.execute(sql.findUser, [value, value]).then((results) => { //Look for the user in the datatbase
      let size = Object.keys(results).length; //Get the length of the object returned
      
      if(size > 0) { //If the object size is greater than 0 then
        let steamids = ""; //Initialise a string
        for(var i = 0; i < size; i++) { //Loop through each result row
          steamids += "," + results[i].steamid; //Add the steamid from row to the steamids string
        }

        steamids = steamids.substring(1); //Remove the comma at the start of steamids

        axios.get("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + keys.steam.apiKey + "&steamids=" + steamids).then((resp) => { //Make a request to the steam api which will get every users details using steamids
          res.send({ results: resp.data.response.players}); //return the result to the client
        }).catch((err) => { //If error then
          res.send({results: results}); //return results from the sql query
        })
      } else { //If the size is <= 0 then return results from the sql query
        res.send({results: results});
      }
    });
  } else {
    res.send({ results: {}}); //return an empty object
  }
})
app.listen(3000, () => {  //Tell the app to listen for port 3000
  console.log("ggv app listening on port 3000") //Display console msg once the task is completed
})