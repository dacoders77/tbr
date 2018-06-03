<template>
    <div>

        <form>
            <br>

            Basket name:
            <input class="form-control" v-model="basketName" @input="onControlValueChanged"/>
            Basket execution time:
            <input type="datetime-local" class="form-control" v-model="basketExecTime" @input="onControlValueChanged"/>
            Funds to use. Availible: <span class="badge badge-info">{{ availableFunds }}$</span>
            <input class="form-control" v-model="allocatedFunds" @input="onControlValueChanged"/>
            <br>


        </form>



            <span>Basket contents:</span>

            <div class="container">
                <template v-for="asset in basketAssets">

                    <div class="row">
                        <div style="border: 0px solid red">
                            <a href="" v-on:click.prevent="assetDelete(asset.id)"><i class="fas fa-trash-alt"
                                                                                     style="color: tomato"></i></a>
                        </div>
                        <div class="col" style="border: 0px solid red">
                            <small>
                                {{ asset.long_name }}
                                Symb: <span class="badge badge-warning">{{ asset.symbol }}</span>
                                Exch: {{ asset.exchange}} Curr: <span class="badge badge-secondary">{{ asset.currency }}</span>
                                Price: <span class="badge badge-info">{{ asset.price }}</span>
                            </small>
                        </div>
                        <div style="border: 0px solid red">
                            <small>%</small>
                        </div>
                        <div style="border: 0px solid red">
                            <input style="width: 24px; padding: 0px; font-size: 12px; margin: 2px 5px" type="text"
                                   class="form-control input-group-lg reg_name"
                                   aria-describedby="passwordHelpInline" maxlength="3" size="3"
                                   v-model="asset.allocated_percent" value="122" @input="onControlValueChanged">
                        </div>
                        <div style="border: 0px solid red; width: 55px">
                            <small>{{ allocatedFunds  * asset.allocated_percent / 100 }}$</small>
                        </div>
                    </div>

                </template>
                <div class="row">
                    <div class="col text-right" style="border: 0px solid green">
                        <small>Total:</small>
                    </div>
                    <div style="border: 0px solid green; width: 36px; padding: 0px 5px">
                        <small>{{ totalPercent }}</small>
                    </div>
                    <div style="border: 0px solid green; width: 55px">
                        <small>{{ totalPrice }}$</small>
                    </div>
                </div>
            </div>




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
                allocatedFunds: ''
            }
        },
        methods: {
            saveBasket() {
                axios.post('/basketupdate', this.$data)
                    .then(response => {
                        console.log(response);
                    })
                    .catch(error => {
                        console.log(error.response);
                    })
            },
            assetDelete: function (assetId) {

                axios.get('/assetdelete/' + this.basketId + '/' + assetId)
                    .then(response => {
                        console.log(response);
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
                    amount += this.allocatedFunds * asset.allocated_percent / 100 // 10000 * asset.allocated_percent / 100 || asset.price * asset.allocated_percent
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
            axios.post('/basketgetdetails', this.$data)
                .then(response => {
                    this.basketName = response.data['basketName'];
                    this.basketExecTime = response.data['basketExecTime'];
                    this.allocatedFunds = response.data['allocated_funds'];


                    var jsonParsedResponse = JSON.parse(response.data['basketAssets']);
                    this.basketAssets = jsonParsedResponse;

                }) // Output returned data by controller
                .catch(error => {
                    console.log('getbasketname error: ');
                    console.log(error.response);
                })

            Echo.channel('tbrChannel').listen('TbrAppSearchResponse', (e) => {
                // SHOW BASKET CONTENT
                var jsonParsedResponse = JSON.parse(e.update);

                if (jsonParsedResponse.messageType == 'AvailableFundsResponse') {
                    this.availableFunds = jsonParsedResponse.funds;
                }

                // When + icon is clicked in the search results this event is triggered
                if (jsonParsedResponse.messageType == 'showBasketContent') {
                    this.basketAssets = jsonParsedResponse.body;
                }
            });

            axios.get('/addmsgws/getAvailableFunds/0') // Get available funds for trading
                .then(response => {
                    console.log('/addmsgws/getAvailableFunds/0 response:');
                    console.log(response);
                }) // Output returned data by controller
                .catch(error => {
                    console.log('CompForm.vue. /addmsgws/getAvailableFunds/0 error: ');
                    console.log(error.response);
                })

        },
    }
</script>