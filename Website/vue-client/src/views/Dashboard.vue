<template>
	<v-container grid-list-md text-xs-center>
		<span class="title">Welcome Frazzle.!</span>
		<v-layout align-center justify-center row fill-height>
			<v-flex xs3>
				<img src="../assets/logo.png">
			</v-flex>
			<v-flex xs3>
				<img src="../assets/logo.png">
			</v-flex>
			<v-flex xs3>
				<img src="../assets/logo.png">
			</v-flex>
			<v-flex xs3>
				<img src="../assets/logo.png">
			</v-flex>
		</v-layout>
		<v-layout align-center justify-center row fill-height>
			<v-flex xs3>
				<span class="caption">50</span>
			</v-flex>
			<v-flex xs3>
				<span class="caption">50</span>
			</v-flex>
			<v-flex xs3>
				<span class="caption">50</span>
			</v-flex>
			<v-flex xs3>
				<span class="caption">50</span>
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
  	...mapState(['user'])
  },
  methods: {

  }
}

</script>

<style>

</style>