
/**
 * First we will load all of this project's JavaScript dependencies which
 * includes Vue and other libraries. It is a great starting point when
 * building robust, powerful web applications using Vue and Laravel.
 */

require('./bootstrap');

window.Vue = require('vue');

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
            console.log("Search button clicked. Vue even handler2");

            // Ajax request. Axios
            axios.get('/addmsgws/' + document.getElementById("searchInputTextField").value)
                .then(function (response) {
                    console.log(response);
                })
                .catch(function (error) {
                    console.log(error);
                });

        },

        message: function(message){

            // Ajax request. Axios
            axios.get('/assetcreate/' + message[0] + '/' + message[1] + '/' + message[2] + '/' + message[3] + '/' + message[4])
                .then(function (response) {
                    console.log(response);
                })
                .catch(function (error) {
                    console.log(error);
                });
        }

    },

    created() {

        Echo.channel('tbrChannel')

            .listen('TbrAppSearchResponse', (e) => {
            var jsonParsedResponse = JSON.parse(e.update);
            console.log("app.js: search responce: " + jsonParsedResponse);
            this.quantityOfRecords = jsonParsedResponse;

        });
    },

}); // new Vue










Vue.component('search-block', require('./components/CompForm.vue')); // Vue test component

const app2 = new Vue({

    el: '#vueJsForm',

}); // Vue


/*
// Buttons handlers
$('#search').click(function () {
    console.log("Search button clicked");
    // //alert($("#searchInputTextField").val());

    //$("tbody").remove();
    var request1 = $.get('/public/addmsgws/' + $("#searchInputTextField").val() + ''); // Controller call
    request1.done(function(response) { // When the request is done
        console.log("Request1 is done");
    });
});
*/



