<template>
  <v-container fill-height grid-list-md>
    <v-layout text-xs-center align-center justify-center>
      <v-flex xs2>
        <span class="subheading">Top Kills</span>
        <v-data-table
          :headers="headers.kills"
          :items="statistics.kills"
          :rows-per-page-items="[10]"
          :pagination.sync="pagination.kills"
          class="elevation-1"
          hide-actions
        >
          <template v-slot:items="props">
            <td>{{ props.item.name }}</td>
            <td>{{ props.item.kills }}</td>
          </template>
        </v-data-table>
      </v-flex>
      <v-flex xs2>
        <span class="subheading">Top Deaths</span>
        <v-data-table
          :headers="headers.deaths"
          :items="statistics.deaths"
          :rows-per-page-items="[10]"
          :pagination.sync="pagination.deaths"
          class="elevation-1"
          hide-actions
        >
          <template v-slot:items="props">
            <td>{{ props.item.name }}</td>
            <td>{{ props.item.deaths }}</td>
          </template>
        </v-data-table>
      </v-flex>
      <v-flex xs2>
        <span class="subheading">Top Wins</span>
        <v-data-table
          :headers="headers.wins"
          :items="statistics.wins"
          :rows-per-page-items="[10]"
          :pagination.sync="pagination.wins"
          class="elevation-1"
          hide-actions
        >
          <template v-slot:items="props">
            <td>{{ props.item.name }}</td>
            <td>{{ props.item.wins }}</td>
          </template>
        </v-data-table>
      </v-flex>
      <v-flex xs2>
        <span class="subheading">Top Losses</span>
        <v-data-table
          :headers="headers.losses"
          :items="statistics.losses"
          :rows-per-page-items="[10]"
          :pagination.sync="pagination.losses"
          class="elevation-1"
          hide-actions
        >
          <template v-slot:items="props">
            <td>{{ props.item.name }}</td>
            <td>{{ props.item.losses }}</td>
          </template>
        </v-data-table>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
  import { mapState, mapMutations } from 'vuex';
  import axios from 'axios';

  export default {
    data () {
      return {
        headers: {
          kills: [
            {
              text: "Name",
              value: "name",
              sortable: false,
              align: "center",
            },
            {
              text: "Kills",
              value: "kills",
              sortable: false,
              align: "center",
            },
          ],
          deaths: [
            {
              text: "Name",
              value: "name",
              sortable: false,
              align: "center",
            },
            {
              text: "Deaths",
              value: "deaths",
              sortable: false,
              align: "center",
            },
          ],
          wins: [
            {
              text: "Name",
              value: "name",
              sortable: false,
              align: "center",
            },
            {
              text: "Wins",
              value: "wins",
              sortable: false,
              align: "center",
            },
          ],
          losses: [
            {
              text: "Name",
              value: "name",
              sortable: false,
              align: "center",
            },
            {
              text: "Loses",
              value: "losses",
              sortable: false,
              align: "center",
            }
          ],
        },
        pagination: {
          kills: {
            descending: true,
            sortBy: "kills",
            rowsPerPage: 10
          },
          deaths: {
            descending: true,
            sortBy: "deaths",
            rowsPerPage: 10
          },
          wins: {
            descending: true,
            sortBy: "wins",
            rowsPerPage: 10
          },
          losses: {
            descending: true,
            sortBy: "losses",
            rowsPerPage: 10
          }
        },
        statistics: {
          kills: [],
          deaths: [],
          wins: [],
          losses: []
        }
      }
    },
    computed: {
      ...mapState(['user'])
    },
    methods: {
      getStatistics() {
        let self = this
        axios.get("/api/stats").then((resp) => {
          self.statistics = resp.data.statistics;
        }).catch((err) => {
          console.log(err);
          self.$router.push("/");
        });
      },
    },
    mounted() {
      this.getStatistics();
    }
  }
</script>

<style>

</style>
