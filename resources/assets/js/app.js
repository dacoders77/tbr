
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

    el: '#testVue',

    data: {
        quantityOfRecords: null // quantity of records
    },

    created() {

        Echo.channel('tbrChannel')
            .listen('TbrAppSearchResponse', (e) => {
            var jsonParsedResponse = JSON.parse(e.update);
            console.log("search responce: " + jsonParsedResponse);
            this.quantityOfRecords = jsonParsedResponse; // Loop through the length or received json

    }); // echo.listen
    }, // created

    // Another vue element
    /*
    el: '#testVue_xxxxxxxx',

    data: {
        todos: [1,2,3,4], // Direction for v-for tag. array 1
        quant: 6 // array 2
    },
    methods: {
        tableButton: function () { // Button with tag tableButton click
            //alert('hello');
            //this.todos.push(998); // Works good
            this.todos = [11,22,33,]; // Works good. Refresh data set with new values

        } // function
    }
    */

}); // new Wue


// Buttons handlers
$('#search').click(function () {
    console.log("Search button clicked");
    //alert($("#searchInputTextField").val());

    //$("tbody").remove();
    var request1 = $.get('addmsgws/' + $("#searchInputTextField").val() + ''); // Controller call
    request1.done(function(response) { // When the request is done
        console.log("request1 is done");
    });
});



