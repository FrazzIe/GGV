<template>
  <v-container fill-height grid-list-md v-if="search">
    <v-layout text-xs-center align-center justify-center wrap>
      <v-flex xs12><a :href="'http://steamcommunity.com/profiles/' + search.steamid" class="display-2">{{ search.name }}'s Statistics</a></v-flex>
      <v-flex xs3>
        <v-layout column>
          <v-flex xs>
            Kills
          </v-flex xs>
          <v-flex xs>
            <img src="../assets/rifle.svg" width="200" height="200" alt="weapon image">
          </v-flex>
          <v-flex xs>
            <span class="subheading">{{ search.kills }}</span>
          </v-flex>
        </v-layout>
      </v-flex>
      <v-flex xs3>
        <v-layout column>
          <v-flex xs>
            Deaths
          </v-flex xs>
          <v-flex xs>
            <img src="../assets/skull.svg" width="200" height="200" alt="skull image">
          </v-flex>
          <v-flex xs>
            <span class="subheading">{{ search.deaths }}</span>
          </v-flex>
        </v-layout>
      </v-flex>
      <v-flex xs3>
        <v-layout column>
          <v-flex xs>
            Wins
          </v-flex xs>
          <v-flex xs>
            <img src="../assets/winner.svg" width="200" height="200" alt="star image">
          </v-flex>
          <v-flex xs>
            <span class="subheading">{{ search.wins }}</span>
          </v-flex>
        </v-layout>
      </v-flex>
      <v-flex xs3>
        <v-layout column>
          <v-flex xs>
            Losses
          </v-flex xs>
          <v-flex xs>
            <img src="../assets/stairs.svg" width="200" height="200" alt="lose image">
          </v-flex>
          <v-flex xs>
            <span class="subheading">{{ search.games_played - search.wins }}</span>
          </v-flex>
        </v-layout>
      </v-flex>
    </v-layout>
  </v-container>
  <v-container v-else fill-height grid-list-md>
    <v-layout text-xs-center align-center justify-center>
      <v-flex xs>
        Make use of the search function to view the full extent of this page!
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import { mapState, mapMutations } from 'vuex';

  export default {
    data () {
      return {
      }
    },
    computed: {
      ...mapState(['search'])
    },
  }
</script>

<style>

</style>
