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
