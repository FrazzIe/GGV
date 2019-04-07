<template>
  <v-app>
    <v-content>
      <Scoreboard/>
    </v-content>
  </v-app>
</template>

<script>
import Scoreboard from './components/Scoreboard'

export default {
  name: 'App',
  components: {
    Scoreboard
  },
  data () {
    return {
      //
    }
  }
}
</script>

<style>
  #app {
    background-color: rgba(0, 0, 0, 0.0);
  }
  ::-webkit-scrollbar { 
      display: none; 
  }
</style>