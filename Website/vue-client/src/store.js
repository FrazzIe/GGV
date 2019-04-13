import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({ //Stores data that can be accessed from component
  state: {
  	navigation: [ //Array of pages that i have in my navigation
  		{icon: "home", title: "Home", route: "/"},
  		{icon: "info", title: "About", route: "/about"},
      {icon: "search", title: "Search", route: "/search"},
  	],
  	userNavigation: [ //Array of pages that i have in my user navigation
  		{icon: "account_box", title: "Dashboard", route: "/dashboard"},
  	],
  	user: false, //The user object
    search: false //The search object
  },
  mutations: {
  	setUser(_, userState) { //Used to set the user object
  		this.state.user = userState;
  	},
    setSearch(_, searchResult) { //Used to set the search object
      this.state.search = searchResult;
    }
  },
  actions: {

  }
})
