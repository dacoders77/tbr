
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

Vue.component('example-component', require('./components/ExampleComponent.vue'));

const app = new Vue({

    el: '#vueJsContainer',

    data: {

        // Variables
        todos: [], // Direction for v-for tag. array 1
        quantityOfRecords: null // quantity of records
    },

    methods: {

        // Button event handler
        greet: function(event){
            console.log("Search button clicked. Vue even handler2");

            // Ajax request. Pure JS
            var xhr = new XMLHttpRequest();
            xhr.open('GET', '/public/addmsgws/' + document.getElementById("searchInputTextField").value);
            xhr.onload = function() {
                if (xhr.status === 200) {
                    console.log('Request is done: ' + xhr.responseText);
                    //console.log(xhr.response);
                }
                else {
                    alert('Request failed.  Returned status of: ' + xhr.status);
                }
            };
            xhr.send();
        }, // greet

        message: function(message){
            //alert(message);

            // Ajax request. Pure JS
            var xhr = new XMLHttpRequest();

            xhr.open('GET', '/public/assetcreate/' + message[0] + '/' + message[1] + '/' + message[2] + '/' + message[3] + '/' + message[4]);
            xhr.onload = function() {
                if (xhr.status === 200) {
                    console.log('Request is done: ' + xhr.responseText);
                    //console.log(xhr.response);
                }
                else {
                    alert('Request failed.  Returned status of: ' + xhr.status);
                }
            };
            xhr.send();

        }

    },

    created() {
        Echo.channel('tbrChannel')

            .listen('TbrAppSearchResponse', (e) => {
            var jsonParsedResponse = JSON.parse(e.update);
            console.log("search responce: " + jsonParsedResponse);
            this.quantityOfRecords = jsonParsedResponse; // Loop through the length or received json

        }); // echo.listen
    }, // created


}); // new Wue


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



