<template>
  <v-app>
    <v-content>
      <Scoreboard/>
    </v-content>
  </v-app>
</template>

<script>
export default {
  name: 'App',
  components: {
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