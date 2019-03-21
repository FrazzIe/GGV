const express = require('express');
const cookieSession = require('cookie-session');
const bodyParser = require('body-parser');
const passportSetup = require('./config/passport-setup');
const passport = require('passport');

const keys = require('./config/keys');

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
