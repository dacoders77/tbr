
/**
 * First we will load all of this project's JavaScript dependencies which
 * includes Vue and other libraries. It is a great starting point when
 * building robust, powerful web applications using Vue and Laravel.
 */

require('./bootstrap');
window.Vue = require('vue');
Vue.use(require('vue-moment')); // https://github.com/brockpetrie/vue-moment

/**
 * Next, we will create a fresh Vue application instance and attach it to
 * the page. Then, you may begin adding components to this application
 * or customize the JavaScript scaffolding to fit your unique needs.
 */

Vue.component('example-component', require('./components/ExampleComponent.vue')); // Example component

const app = new Vue({

    el: '#vueJsContainer',

    data: {

        // Variables
        todos: [], // Direction for v-for tag. array 1
        quantityOfRecords: null, // quantity of records
        name: '',
        errors: ''

    },

    methods: {

        // Button event handler
        greet: function(event){
            //console.log("Search button clicked. Vue even handler2");

            // Ajax request. Axios
            axios.get('/addmsgws/' + document.getElementById("searchInputTextField").value)
                .then(function (response) {
                    //console.log(response);
                })
                .catch(function (error) {
                    console.log('axios addmsgws error: ' + error);
                });

        },

        message: function(message){

            // Ajax request. Axios
            axios.get('/assetcreate/' + message[0] + '/' + message[1] + '/' + message[2] + '/' + message[3] + '/' + message[4])
                .then(function (response) {
                    //console.log(response);
                })
                .catch(function (error) {
                    console.log('axios assetcreate error: ' + error);
                });
        }

    },

    created() {

        Echo.channel('tbrChannel')

            .listen('TbrAppSearchResponse', (e) => {

            //console.log(e.update);
            var jsonParsedResponse = JSON.parse(e.update[0]);

            if (e.update['eventType'] == 'searchJsonResponse')

            {
                var jsonParsedResponse = jsonParsedResponse;
                this.quantityOfRecords = jsonParsedResponse;
            }

        });
    },

}); // new Vue


// Vue basket component
Vue.component('search-block', require('./components/CompForm.vue'));
const app2 = new Vue({
    el: '#vueJsForm',
});

// Vue home page component
Vue.component('home-block', require('./components/HomeForm.vue'));
const app3 = new Vue({
    el: '#vueHomeForm',
});


