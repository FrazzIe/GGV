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
import { mapState, mapMutations } from 'vuex';
import axios from 'axios';

export default {
  name: 'App',
  components: {

  },
  data () {
    return {
      drawer: true,
      menu: false,
      searchDlg: false,
      searchField: "",
      searchRules: {
        counter: value => value.length > 3 || 'Too short',
      },
      searchResults: []
    }
  },
  computed: {
    ...mapState(['navigation', 'userNavigation', 'user']),
    currentYear(){
      return new Date().getFullYear();
    }
  },
  methods: {
    ...mapMutations(['setUser']),
    getUser() {
      let self = this
      axios.get("/api/user").then((resp) => {
        if(resp.data.user !== "undefined") {
          self.setUser(resp.data.user);
        }
      }).catch((err) => {
        console.log(err);
        self.$router.push("/");
      });
    },
    findPlayer() {
      if(this.searchField.length > 3) {
        let self = this
        axios.get("/api/search/" + this.searchField).then((resp) => {
          self.searchResults = resp.data.results;
          self.searchLoad = false;
          self.searchDlg = false;
          self.searchResultsDlg = true;
        }).catch((err) => {
          console.log(err);
          self.$router.push("/");      
        });
      }
    },
  },
  mounted(){
    this.getUser();
  }
}
</script>
