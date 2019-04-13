<template>
  <v-app>
    <v-navigation-drawer clipped v-model="drawer" :width="250" app>
      <v-list>
        <v-list-tile v-for="page in navigation" :key="page.title" :to="page.route">
          <v-list-tile-action>
            <v-icon>{{ page.icon }}</v-icon>
          </v-list-tile-action>

          <v-list-tile-content>
            <v-list-tile-title>
              {{ page.title }}
            </v-list-tile-title>
          </v-list-tile-content>
        </v-list-tile>
      </v-list>
      
      <v-footer class="justify-center pl-0" color="primary" inset app>
        <span style="color: white;">&copy; {{ currentYear }} &mdash; Fraser Watt</span>
      </v-footer> 
    </v-navigation-drawer>

    <v-toolbar app clipped-left>
      <v-toolbar-side-icon @click="drawer = !drawer"></v-toolbar-side-icon>
      <v-toolbar-title class="headline">
        GGV
      </v-toolbar-title>
      <v-spacer></v-spacer>

      <v-btn icon @click="searchDlg = !searchDlg">
        <v-icon>search</v-icon>
      </v-btn>

      <a href="/login" v-if="!user">
        <img src="https://steamcommunity-a.akamaihd.net/public/images/signinthroughsteam/sits_01.png">
      </a>

      <v-menu v-model="menu" :close-on-content-click="false" :nudge-bottom="40" left v-if="user">
        <template v-slot:activator="{ on }">
          <v-btn icon v-if="user" v-on="on">
            <v-avatar size="36px">
              <img :src="user.avatar">
            </v-avatar>
          </v-btn>
        </template>

        <v-card>
          <v-list>
            <v-list-tile>
              <v-list-tile-avatar>
                <img :src="user.avatar">
              </v-list-tile-avatar>

              <v-list-tile-content>
                <v-list-tile-title>{{ user.name }}</v-list-tile-title>
                <v-list-tile-sub-title><a :href="user.link">{{ user.steam }}</a></v-list-tile-sub-title>
              </v-list-tile-content>
            </v-list-tile>
          </v-list>

          <v-divider></v-divider>

          <v-list>
            <v-list-tile v-for="page in userNavigation" :key="page.title" :to="page.route">
              <v-list-tile-action>
                <v-icon>{{ page.icon }}</v-icon>
              </v-list-tile-action>

              <v-list-tile-content>
                <v-list-tile-title>
                  {{ page.title }}
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>

            <v-list-tile href="/logout">
              <v-list-tile-action>
                <v-icon>exit_to_app</v-icon>
              </v-list-tile-action>

              <v-list-tile-content>
                <v-list-tile-title>
                  Logout
                </v-list-tile-title>
              </v-list-tile-content>
            </v-list-tile>
          </v-list>
        </v-card>
      </v-menu>
    </v-toolbar>

    <v-content>
      <router-view/>

      <v-dialog v-model="searchDlg" persistent max-width="500px">
        <v-card>
          <v-toolbar color="primary" dark>
            <v-toolbar-title>Search for player</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-container grid-list-md>
              <v-layout wrap>
                <v-flex xs12>
                  <v-text-field v-model="searchField" prepend-icon="search" label="Player name or Steam64" :rules="searchRules" counter @change="findPlayer"></v-text-field>
                </v-flex>
              </v-layout>
            </v-container>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary darken-1" flat @click="findPlayer" :loading="searchLoad">Search</v-btn>
            <v-btn color="primary darken-1" flat @click="searchDlg = false">Exit</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>

      <v-dialog v-model="searchResultsDlg" persistent max-width="500px">
        <v-card>
          <v-toolbar color="primary" dark>
            <v-toolbar-title>Search results</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-list>
              <v-list-tile v-for="(usr, index) in searchResults" :key="usr.id">
                <v-list-tile-avatar>
                  <img :src="usr.avatarfull">
                </v-list-tile-avatar>

                <v-list-tile-content>
                  <v-list-tile-title>
                    {{ usr.personaname }}
                  </v-list-tile-title>
                  <v-list-tile-sub-title>
                    <a :href="'http://steamcommunity.com/profiles/' + usr.steamid">Steam profile</a>
                  </v-list-tile-sub-title>
                </v-list-tile-content>

                <v-list-tile-action>
                  <v-btn flat icon color="primary" @click="getPlayer(usr.steamid)">
                    <v-icon>more_horiz</v-icon>
                  </v-btn>
                </v-list-tile-action>
              </v-list-tile>
            </v-list>
            <span class="body-1 text-xs-center" v-if="Object.keys(searchResults).length == 0">No results found</span>
          </v-card-text>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="primary darken-1" flat @click="searchResultsDlg = false">Exit</v-btn>
          </v-card-actions>
        </v-card>
      </v-dialog>
    </v-content>
  </v-app>
</template>

<script>
import { mapState, mapMutations } from 'vuex'; //import vuex data
import axios from 'axios'; //import axios

export default {
  name: 'App',
  components: {

  },
  data () { //variables
    return {
      drawer: true, //bool that is tied to wheather the navigation drawer is open or closed
      menu: false, //bool that is tied to wheather the user menu in the top right is open or closed
      searchDlg: false, //bool that is tied to wheather the search dialog is visible or not
      searchResultsDlg: false, //bool that is tied to wheather the search results ddialog is visible or not
      searchField: "", //string that is tied to the v-text-field in the search dialog
      searchRules: { //validation rules that is tied to the v-text-fiel in the search dialog
        counter: value => value.length > 3 || 'Too short', //Makes sure the string is greater than 3 characters long
      },
      searchResults: [] //Array of search results
    }
  },
  computed: { //computed variables
    ...mapState(['navigation', 'userNavigation', 'user']), //variables stored in vuex
    currentYear(){ //Used for displaying the year in the copyright notice
      return new Date().getFullYear();
    }
  },
  methods: { //methods
    ...mapMutations(['setUser', 'setSearch']), //used for setting variables in the vuex store
    getUser() { //Used to get the user from the server
      let self = this //lets the vue instance be referenced by self so it can be used in a promise
      axios.get("/api/user").then((resp) => { //Sends a request to the server
        if(resp.data.user !== "undefined") { //Check if the user has been returned
          self.setUser(resp.data.user); //Sets the user in the vuex store to the data returned
        }
      }).catch((err) => { //If there is an error getting the user
        console.log(err); //Display error
        self.$router.push("/"); //Go to homepage
      });
    },
    findPlayer() { //Used to find players who have similar names or steam ids to what was entered in the search field in the search dialog
      if(this.searchField.length > 3) { //Check if the fields length is greater than 3
        let self = this //lets the vue instance be referenced by self so it can be used in a promise
        axios.get("/api/search/" + this.searchField).then((resp) => { //Send a request to the server with the value of the search field
          self.searchResults = resp.data.results; //Set the array of search results to the array returned from the server
          self.searchDlg = false; //Set the search dialog to hide
          self.searchResultsDlg = true; //Set the search results dialog to appear
        }).catch((err) => { //If there is an error while searching
          console.log(err); //Display error
          self.$router.push("/"); //Go to homepage
        });
      }
    },
    getPlayer(steamid) { //Used to find a specific player using their steam id
      let self = this //lets the vue instance be referenced by self so it can be used in a promise
      axios.get("api/user/" + steamid).then((resp) => { //Send a request to the server with the value of the steam id
        if(resp.data.user !== "undefined") { //Check if a result was retuned
          self.setSearch(resp.data.user); //Sets the search in the vuex store to the data returned
        } else {
          self.setSearch(false); //Sets the search to false as no data was returned
        }
        this.searchResultsDlg = false; //Set the search results dialog to hide
        self.$router.push("/search"); //Navigate to the search page to view the users profile that was searched for
      }).catch((err) => { //If there is an error while getting a player
        console.log(err); //Display error
        self.$router.push("/"); //Go to homepage
      })
    }
  },
  mounted(){ //When page is loaded
    this.getUser(); //Get the clients details
  }
}
</script>
