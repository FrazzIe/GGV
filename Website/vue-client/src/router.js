import Vue from 'vue'
import Router from 'vue-router' //import required libs

import Home from './views/Home.vue'
import About from './views/About.vue'
import Dashboard from './views/Dashboard.vue'
import Search from './views/Search.vue' //Import router vues

Vue.use(Router) //Make Vue use Vue-Router

export default new Router({ //Define a new router
  //mode: 'history',
  //base: process.env.BASE_URL,
  routes: [ //A list of paths and names that link to components
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/about',
      name: 'about',
      component: About
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard
    },
    {
      path: '/search',
      name: 'search',
      component: Search
    },
  ]
})
