
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
 * or customize the JavaScript scaffolding to fit your unique needs. ok.
 */

Vue.component('example-component', require('./components/ExampleComponent.vue')); // Example component

const app = new Vue({

    el: '#vueJsContainer',
    data: {
        todos: [],
        quantityOfRecords: null, // quantity of records
        name: '',
        errors: ''
    },
    methods: {

        // Search button event handler
        greet: function(event){
            // Symbol search request. C#
            var symbolSearchJsonString = {"symbol" : document.getElementById("searchInputTextField").value}; // Prepare a JSON string which consits only of one parameter - symbol
            var symbolSearchJson = JSON.stringify(symbolSearchJsonString);

            axios.get('/addmsgws/symbolSearch/' + symbolSearchJson) // document.getElementById("searchInputTextField").value
                .then(function (response) {
                    //console.log(response);
                })
                .catch(function (error) {
                    console.log('axios addmsgws error (symbolSearch): ' + error);
                    concole.log('');
                });

        },
        message: function(message){

            // When + is clicked from the basket content page, two requests are made: Create asset and Get quote
            // {basketId}/{assetSymbol}/{longName}/{assetExchange}/{assetCurrency}/{assetAllocatedPercent}
            // Actually two controllers are called. This one and getQuote controller
            // One creates a record in assets table, a second one is getting a quote and updates its value in DB (stock_quote)

            axios.get('/assetcreate/' + message[0] + '/' + message[1] + '/' + message[2] + '/' + message[3] + '/' + message[4] + '/' + message[5])
                .then(function (response) {
                    //console.log('/assetcreate response:');
                    //console.log(response);
                })
                .catch(function (error) {
                    console.log('app.js line 62. /assetcreate error: ' + error);
                });

            // MAKE C# QUOTE REQUEST GOES FROM HERE

            var getQuoteJsonString = {"symbol" : message[1], "basketNumber" : message[0], "currency" : message[4] };
            var getQuoteJson = JSON.stringify(getQuoteJsonString);

            // This json string will be passed directley to web-socket channel, processed in ListenLocalSocket.php (ratchet)
            axios.get('/addmsgws/getQuote/' + getQuoteJson)
                .then(function (response) {
                    //console.log('app.js. getQuote response: ');
                    //console.log(response);
                })
                .catch(function (error) {
                    console.log('app.js line 77. getQuote error: ');
                    console.log(error);
                });
        }

    },

    created() {

        // Web-socket listener. Listens for search results returened from web-socket
        Echo.channel('tbrChannel').listen('TbrAppSearchResponse', (e) => {
            var jsonParsedResponse = JSON.parse(e.update);
            if (jsonParsedResponse.messageType == 'SearchResponse')
            {
                this.quantityOfRecords = jsonParsedResponse.searchList;
            }
        });
    },

});


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


