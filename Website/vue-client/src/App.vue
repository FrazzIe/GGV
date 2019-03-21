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
      menu: false
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
        if(resp.data.user) {
          self.setUser(resp.data.user);
        }
      }).catch((err) => {
        console.log(err);
        self.$router.push("/");
      });
    },
  },
  mounted(){
    this.getUser();
  }
}
</script>
