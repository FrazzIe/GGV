resource_manifest_version "05cfa83c-a124-4cfa-a768-c24a5811d8f9"

shared_scripts {
	"Newtonsoft.Json.dll",
	"GunGameV.Shared.net.dll",
}
client_scripts {
	"GunGameV.Client.net.dll",
}

server_scripts {
	"GunGameV.Server.net.dll",
}

ui_page "ui/index.html"

files {
	"maps/garage.json",
	"maps/bunker.json",
	"maps/iaa.json",
	"ui/index.html",
	"ui/js/app.js",
	"ui/css/app.css",
}