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
