const mysql = require('mysql'); //include mysql
const keys = require('./keys'); //include keys.js

var connection = mysql.createConnection(keys.mysql); //create a mysql connection

connection.connect() //connect to mysql

module.exports = { //object that is exported when this file is include in another
	//MySQL Queries
	getUser: "SELECT name, UNIX_TIMESTAMP(timestamp) As 'timestamp', UNIX_TIMESTAMP(lastplayed) As 'lastplayed', wins, games_played, kills, deaths FROM player WHERE steam = ?", 
	findUser: "SELECT name AS 'personaname', steam AS 'steamid', '' AS 'avatarfull' FROM player WHERE name LIKE ? OR steam LIKE ?",
	isBanned: "SELECT ban.reason, ban.expire FROM ban JOIN player ON ban.player_id = id WHERE steam = ?",
	topKills: "SELECT kills, name, steam FROM player ORDER BY kills LIMIT 10",
	topDeaths: "SELECT deaths, name, steam FROM player ORDER BY deaths LIMIT 10",
	topWins: "SELECT wins, name, steam FROM player ORDER BY wins LIMIT 10",
	topLosses: "SELECT games_played - wins As 'losses', name, steam FROM player ORDER BY 'losses' LIMIT 10",
	execute: function(sql, params) { //Asynchronous sql execute function
	  return new Promise((resolve, reject) => { //create a new promise
	    connection.query(sql, params, (error, result, fields) => { //query the server
	      if (error) reject(error); //if error then display error
	      resolve(result); //return result
	    });
	  });
	}
}