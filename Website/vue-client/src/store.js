import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
  	navigation: [
  		{icon: "home", title: "Home", route: "/"},
  		{icon: "info", title: "About", route: "/about"},
  	],
  	userNavigation: [
  		{icon: "account_box", title: "Dashboard", route: "/dashboard"},
  	],
  	user: false
  },
  mutations: {
  	setUser(_, userState) {
  		this.state.user = userState;
  	}
  },
  actions: {

  }
})
