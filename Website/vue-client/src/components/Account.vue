<template>
  <v-container fill-height grid-list-md v-if="user">
    <v-layout text-xs-center align-center justify-center wrap>
      <v-flex xs12><span class="display-2">My Stats</span></v-flex>
      <v-flex xs3>
        <v-layout column>
          <v-flex xs>
            Kills
          </v-flex xs>
          <v-flex xs>
            <img src="../assets/rifle.svg" width="200" height="200" alt="weapon image">
          </v-flex>
          <v-flex xs>
            <span class="subheading">{{ user.kills }}</span> <!-- display kills -->
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
            <span class="subheading">{{ user.deaths }}</span> <!-- display deaths -->
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
            <span class="subheading">{{ user.wins }}</span> <!-- display wins -->
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
            <span class="subheading">{{ user.games_played - user.wins }}</span> <!-- display losses -->
          </v-flex>
        </v-layout>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import { mapState, mapMutations } from 'vuex'; //import vuex data

  export default {
    data () {
      return {
      }
    },
    computed: {
      ...mapState(['user']) //reference a property thats stored in vuex
    },
  }
</script>

<style>

</style>
