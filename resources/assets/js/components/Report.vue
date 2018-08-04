<template>
    <div>
        <h3>Basket №{{ basketId }}. Execution report </h3>


        <template v-for="(element, index) in reportArray">
            <br>
            <b>Asset №: {{ index + 1 }} </b><br>
            Symbol: {{ element.symbol }}<br>
            Currency: {{ element.currency }}<br>
            Allocated percent: {{ element.allocated_percent }}%<br>
            Volume: {{ element.volume }} <br>
            Order placed: {{ arr[index].executeOrder.status }}<br>
            Average fill price: {{ arr[index].executeOrder.avgFillPrice }} <br><br>

            FX quote request status: {{ arr[index].fxQuoteRequest.status }} <br>
            <span v-html="arr[index].fxQuoteRequest.log"></span> <br>
            Stock quote request status: {{ arr[index].stockQuoteRequest.status }} <br>
            <span v-html="arr[index].stockQuoteRequest.log"></span> <br>
            Volume calculation status: {{ arr[index].volumeCalculate.status }} <br>
            <span v-html="arr[index].volumeCalculate.log"></span> <br>
            Place order status:: {{ arr[index].placeOrder.status }} <br>
            <span v-html="arr[index].placeOrder.log"></span> <br><br>


             <!-- {{ element.symbol }} - {{ element.currency }} - {{ arr[index].placeOrder.log }} -->
        </template>

        <!--
        <template v-for="z in arr">
            <pre> {{ z.placeOrder.log }} </pre>
        </template>


        <template v-for="element in reportArray">
            <template v-for="dbRecord in element">
                {{ dbRecord }}
             </template>
        </template>
        -->



    </div>
</template>


<script>


    export default {

        props: ['basketid'], // basketid is passed as a parameter from a vue js component
        data() {
            return {
                basketId: this.basketid, // Put a property here. This is needed in order to send all variables in this.$data to the BasketUpdate.php controller
                basketName: '',
                basketExecTime: '',
                basketAssets: null,
                availableFunds: '',
                allocatedFunds: 'sss',
                serverTime: null,

                reportArray: null,
                arr: []
            }
        },
        methods: {
            saveBasket() {
                axios.post('/basketupdate', this.$data)
                    .then(response => {
                        //console.log(response);
                    })
                    .catch(error => {
                        console.log(error.response);
                    })
            },
            assetDelete: function (assetId) {

                axios.get('/assetdelete/' + this.basketId + '/' + assetId)
                    .then(response => {
                        //console.log(response);
                        this.basketAssets = response.data
                    })
                    .catch(error => {
                        console.log(error.response);
                    })
            },
            onControlValueChanged() {
                //console.log('CompForm.vue. Form filed value changed event:');
                //console.log(this.$data);
                // Record to the DB all values of the form: name, exec time, allocated funds
                axios.post('/basketupdate', this.$data)
                    .then(response => {
                        //console.log(response);
                    })
                    .catch(error => {
                        console.log(error.response);
                    })
            },
        },

        computed: {
            totalPrice() {
                let amount = 0; // Like var in func boundry
                _.each(this.basketAssets, (asset) => {
                    amount += this.allocatedFunds * asset.allocated_percent / 100
                }); // lodash lib. chech it!
                return amount; // outpuit it to the form
            },
            totalPercent() {
                let amount2 = 0;
                _.each(this.basketAssets, (asset) => {
                    amount2 += parseInt(asset.allocated_percent)
                });
                return amount2;
            },

        },

        created() { // mounted is not used anymore
            //console.log(this.title);
            //console.log(this.$props);


            axios.post('/report', this.$data)
                .then(response => {

                    this.reportArray = response.data;
                    var me = this;

                    response.data.forEach(function(element) {
                        me.arr.push(JSON.parse(element.info));
                    });

                    //console.log(this.arr);


                    //this.basketName = response.data['basketName'];
                    //this.basketExecTime = response.data['basketExecTime'];
                    //this.allocatedFunds = response.data['allocated_funds'];

                    var jsonParsedResponse = JSON.parse(response.data['basketAssets']);
                    this.basketAssets = jsonParsedResponse;

                }) // Output returned data by controller
                .catch(error => {
                    console.log('getbasketname error: ');
                    console.log(error.response);
                })


        },
    }
</script>