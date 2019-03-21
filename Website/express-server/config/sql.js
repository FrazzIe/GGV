const mysql = require('mysql');
const keys = require('./keys');

var connection = mysql.createConnection(keys.mysql);

connection.connect()

module.exports = {
	getUser: "SELECT UNIX_TIMESTAMP(player.timestamp) As 'timestamp', UNIX_TIMESTAMP(player.lastplayed) As 'lastplayed', statistic.wins, statistic.games_played, statistic.kills, statistic.deaths FROM player JOIN statistic ON player.id = statistic.player_id WHERE player.steam = ?",
	isBanned: "SELECT ban.reason, ban.expire FROM ban JOIN player ON ban.player_id = player.id WHERE player.steam = ?",
	topKills: "SELECT statistic.kills, player.name, player.steam FROM statistic JOIN player ON statistic.player_id = player.id ORDER BY statistic.kills LIMIT 10",
	topDeaths: "SELECT statistic.deaths, player.name, player.steam FROM statistic JOIN player ON statistic.player_id = player.id ORDER BY statistic.deaths LIMIT 10",
	topWins: "SELECT statistic.wins, player.name, player.steam FROM statistic JOIN player ON statistic.player_id = player.id ORDER BY statistic.wins LIMIT 10",
	topLosses: "SELECT statistic.games_played - statistic.wins As 'losses', player.name, player.steam FROM statistic JOIN player ON statistic.player_id = player.id ORDER BY 'losses' LIMIT 10",
	execute: function(sql, params) {
	  return new Promise((resolve, reject) => {
	    connection.query(sql, params, (error, result, fields) => {
	      if (error) reject(error);
	      resolve(result);
	    });
	  });
	}
}