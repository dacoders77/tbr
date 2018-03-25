
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
    el: '#app',

    created() {
        Echo.channel('tbrChannel')
            .listen('TbrAppSearchResponse', (e) => {
            //alert('The event has been triggered! Here is the alert box for proofe!');

        //var d = new Date();
        //document.getElementById("app").innerHTML = d;
            var jsonParsedResponse = JSON.parse(e.update);
        //document.getElementById("text").innerHTML = jsonParsedResponse[0];

        demoP = document.getElementById("demo");

        //for (i = 0; i < 5; i++)
        //    //document.getElementById("demo").append("Some appended text" + [i] + "<br>");
        //    demoP.innerHTML = demoP.innerHTML + i + "<br>";

        $.each(jsonParsedResponse, function( index, value ) {
            //alert( index + ": " + value );
            demoP.innerHTML = demoP.innerHTML + value[1] + "<br>";
        });

        console.log(jsonParsedResponse);

    });
    }
}); // new Wue

// Buttons handlers
$('#search').click(function () {
    console.log("Search button clicked");
    //alert($("#searchInputTextField").val());

    var request1 = $.get('addmsgws/hellodude'); // Controller call
    request1.done(function(response) { // When the request is done
        console.log("request1 is done");

    });
});
