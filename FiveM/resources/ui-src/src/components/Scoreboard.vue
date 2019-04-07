<template>
  <v-container fill-height v-if="visible">
    <v-layout
      text-xs-center
      align-center
      justify-center
    >
      <v-flex xs8>
        <v-layout>
          <v-flex xs6>
            <h1 class="text-xs-left" style="color: rgb(255, 255, 255);">Leaderboard</h1>
          </v-flex>
          <v-flex xs6>
            <h1 class="text-xs-right" style="color: rgb(255, 255, 255);">Gun Game V</h1>
          </v-flex>
        </v-layout>
        <v-data-table
          :headers="headers"
          :items="players"
          :rows-per-page-items="[10]"
          :pagination.sync="pagination"
          class="elevation-1"
          dark
        >
          <template v-slot:items="props">
            <td>{{ props.item.ID }}</td>
            <td>{{ props.item.Name }}</td>
            <td>{{ props.item.gameStats.Score }}</td>
            <td>{{ props.item.gameStats.Kills }}</td>
            <td>{{ props.item.gameStats.Deaths }}</td>
          </template>
        </v-data-table>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  export default {
    data () {
      return {
        visible: false,
        headers: [
          {
            text: "ID",
            value: "ID",
            sortable: false,
            align: "center",
          },
          {
            text: "Name",
            value: "Name",
            sortable: false,
            align: "center",
          },
          {
            text: "Score",
            value: "gameStats.Score",
            sortable: false,
            align: "center",
          },
          {
            text: "Kills",
            value: "gameStats.Kills",
            sortable: false,
            align: "center",
          },
          {
            text: "Deaths",
            value: "gameStats.Deaths",
            sortable: false,
            align: "center",
          },
        ],
        players: [],
        pagination: {
          descending: true,
          sortBy: "gameStats.Score",
          rowsPerPage: 10
        }
      }
    },
    computed: {
      pages() {
        if (this.pagination.rowsPerPage == null || this.pagination.totalItems == null) {
          return 0
        } else {
          return Math.ceil(this.pagination.totalItems / this.pagination.rowsPerPage)
        }
      }
    },
    methods: {
      SetPlayers(payload) {
        this.players = payload;
      },
      Show(payload) {
        this.visible = payload == "true";
      },
      Left(payload) {
        var nextPage = this.pagination.page - 1;
        if(nextPage < 1) {
          this.pagination.page = this.pages;
        } else {
          this.pagination.page = nextPage;
        }        
      },
      Right(payload) {
        var nextPage = this.pagination.page + 1;
        if(nextPage > this.pages) {
          this.pagination.page = 1;
        } else {
          this.pagination.page = nextPage;
        }
      }
    },
    mounted() {
      this.listener = window.addEventListener('message', (event) => {
        const item = event.data || event.detail;
        if (this[item.type] && item.payload) this[item.type](item.payload);
      });
    }
  }
</script>

<style>
  ::-webkit-scrollbar { 
      display: none; 
  }
</style>
