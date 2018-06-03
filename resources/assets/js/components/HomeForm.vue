<template>
    <div>

        <form>

            <table class="table table-striped">

                <thead>
                <tr>

                    <th>Date&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp</th>
                    <th>Name</th>
                    <th>Fund</th>
                    <th>Stat</th>
                    <th>Act</th>

                </tr>
                </thead>

                <tbody>

                <tr v-for="asset in basketAssets">
                    <td><span>{{ asset.execution_time | moment("MM-DD-YY HH:mm") }}</span></td>
                    <td><a v-bind:href="'basket/' + asset.id">{{ asset.name }}</a></td>
                    <td>{{ asset.allocated_funds }}$</td>
                    <td class="text-danger mx-auto"><span class="badge"
                                                          v-bind:class="{ 'badge-warning': (asset.status == 'new'), 'badge-success': (asset.status == 'filled'), 'badge-danger': (asset.status == 'error')}">{{ asset.status
                        }}</span></td>
                    <td><a href="" v-on:click.prevent="deleteBasket(asset.id)"><i class="fas fa-trash-alt"
                                                                                  style="color: tomato"></i></a></td>
                </tr>

                </tbody>

                <tfoot>
                <tr>
                    <td colspan="2" class="text-right">Total:</td>
                    <td>{{ totalFunds }}$</td>
                </tr>
                </tfoot>

            </table>

            <div class="alert alert-success" role="alert">
                <!-- <a href="basketcreate"></i>&nbsp;Add new basked</a> -->
                <a href="" v-on:click.prevent="createBasket()"><i class="fas fa-plus-square"></i>&nbspCreate new basket</a>
                &nbsp
                <a href="" v-on:click.prevent="reloadBasketList()"><i class="fas fa-sync-alt"></i>&nbspReload basket list</a>
                <br>
            </div>

        </form>
    </div>
</template>
<script>

    // Date Bskt Fund Stat Act
    export default {

        props: [], // basketid is passed as a parameter from vue js component
        data() {
            return {
                basketAssets: null,
                isActive: true,
                href: 'xxxx'
            }
        },
        methods: {
            deleteBasket: function (basketId) {
                axios.get('/basketdelete/' + basketId)
                    .then(response => {
                        console.log(response);
                        this.basketAssets = response.data
                    })
                    .catch(error => {
                        console.log(error.response);
                    })
            },
            createBasket: function () {
                axios.get('/basketcreate')
                    .then(response => {
                        console.log(response);
                        this.basketAssets = response.data
                    })
                    .catch(error => {
                        console.log(error.response);
                    })
            },
            // Used as an option when page contents did not load via websocket
            reloadBasketList: function () {
                axios.get('/homegetbasketslist')
                    .then(response => {
                        console.log('HomeForm.vue. homegetbasketslist response: ');
                        console.log(response);
                    })
                    .catch(error => {
                        console.log('HomeForm.vue. homegetbasketslist error:');
                        console.log(error.response);
                    })
            }
        },
        computed: {
            totalFunds() {
                let amount = 0;
                _.each(this.basketAssets, (asset) => {
                    amount += asset.allocated_funds;
                    console.log(asset);
                });
                return amount;
            },
        },
        created() {
            axios.get('/homegetbasketslist')
                .then(response => {
                    //console.log('HomeForm.vue. homegetbasketslist response: ');
                    //console.log(response);
                    this.basketAssets = response.data;
                })
                .catch(error => {
                    console.log('HomeForm.vue. homegetbasketslist error: ');
                    console.log(error.response);
                })

            // No websocket is needed
            /*
            Echo.channel('tbrChannel')
                .listen('TbrAppSearchResponse', (e) => {
                    var jsonParsedResponse = JSON.parse(e.update);
                    //console.log('HomeForm.vue event listener in created()' + JSON.stringify(jsonParsedResponse));
                    // BASKET LIST AT THE START PAGE
                    if (jsonParsedResponse.messageType == 'basketsList') // First element is key => value, second is a json object
                    {
                        this.basketAssets = jsonParsedResponse.body; // Assign only the collection from json object. This collection is the same as it was pulled out from DB, baskets table. HomeGetBasketsList controller
                        //console.log(moment('01/12/2016', 'DD/MM/YYYY', true).format()); // Moment JS lib
                    }
                });
                */
        },
    }
</script>