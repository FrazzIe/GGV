const express = require('express');
const cookieSession = require('cookie-session');
const bodyParser = require('body-parser');
const passportSetup = require('./config/passport-setup');
const passport = require('passport');
const axios = require('axios');

const keys = require('./config/keys');
const sql = require('./config/sql');

const publicRoot = '../vue-client/dist';

const app = express();

app.use(express.static(publicRoot))
app.use(bodyParser.json())

app.use(cookieSession({  
  keys: keys.session.cookieKeys,
  maxAge: 24 * 60 * 60 * 1000
}))

app.use(passport.initialize());
app.use(passport.session());

app.get("/", (req, res, next) => {
  res.sendFile("index.html", { root: publicRoot})
})

app.get("/login", (req, res, next) => {  
  passport.authenticate("steam", (err, profile, info) => {
    if (err) {
      return next(err);
    }

    if (!profile) {
      return res.status(400).send([profile, "Cannot log in", info]);
    }

    req.login(profile, err => {
      res.send("Logged in");
    });
  })(req, res, next);
});

app.get("/return", (req, res, next) => {  
  passport.authenticate("steam", (err, profile, info) => {
    if (err) {
      return next(err);
    }

    if (!profile) {
      return res.status(400).send([profile, "Cannot log in", info]);
    }

    req.login(profile, err => {
      res.redirect("/");
    });
  })(req, res, next);
});

app.get("/logout", function(req, res) {  
  req.logout();
  res.redirect("/");
});

app.get("/api/user", function(req, res) {
  if(req.isAuthenticated()) {
    res.send({user: req.session.passport.user});
  } else {
    res.send({user: false});
  }
});

app.get("/api/user/:steamid", function(req, res) {
  let steamid = req.params.steamid;

  sql.execute(sql.getUser, [steamid]).then((result) => {
    if (typeof result[0] === "undefined") {
      res.send({user: false});
    } else {
      result[0].steamid = steamid;
      res.send({user: result[0]});
    }
  });
});

app.get("/api/stats", function(req, res) {
  sql.execute(sql.topKills, []).then((kills) => {
    sql.execute(sql.topDeaths, []).then((deaths) => {
      sql.execute(sql.topWins, []).then((wins) => {
        sql.execute(sql.topLosses, []).then((losses) => {
          res.send({statistics: { kills: kills, deaths: deaths, wins: wins, losses: losses}});
        })
      })
    })
  })
});

app.get("/api/search/:value", function(req, res) {
  let value = req.params.value;

  if (value !== "undefined") {
    value = encodeURI(value).replace("%20", " ");
    value = "%" + value + "%";

    sql.execute(sql.findUser, [value, value]).then((results) => {
      let size = Object.keys(results).length;
      
      if(size > 0) {
        let steamids = "";
        for(var i = 0; i < size; i++) {
          steamids += "," + results[i].steamid;
        }

        steamids = steamids.substring(1);

        axios.get("http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=" + keys.steam.apiKey + "&steamids=" + steamids).then((resp) => {
          res.send({ results: resp.data.response.players});
        }).catch((err) => {
          res.send({results: results});
        })
      } else {
        res.send({results: results});
      }
    });
  } else {
    res.send({ results: {}});
  }
})
app.listen(3000, () => {  
  console.log("ggv app listening on port 3000")
})