<template>
    <div>

        <form>

            <table class="table table-striped">

                <thead>
                <tr>
                    <th>Date</th>
                    <th>Bskt</th>
                    <th>Fund</th>
                    <th>Stat</th>
                    <th>Act</th>

                </tr>
                </thead>

                <tbody>

                <tr v-for="asset in basketAssets">
                    <td><span><a v-bind:href="'basket/' + asset.id"> {{ asset.execution_time | moment("MM-DD HH:mm") }}</a></span></td>
                    <td>{{ asset.name }}</td>
                    <td>--</td>
                    <td class="text-danger mx-auto"><span class="badge" v-bind:class="{ 'badge-warning': (asset.status == 'new'), 'badge-success': (asset.status == 'filled'), 'badge-danger': (asset.status == 'error')}">{{ asset.status }}</span></td>
                    <td><a href="" v-on:click.prevent="deleteBasket(asset.id)"><i class="fas fa-trash-alt" style="color: tomato"></i></a></td>

                </tr>

                </tbody>

            </table>

            <div class="alert alert-success" role="alert">
                <!-- <a href="basketcreate"></i>&nbsp;Add new basked</a> -->
                <a href="" v-on:click.prevent="createBasket()"><i class="fas fa-plus-square"></i>&nbspCreate new basket</a>
                <br>
            </div>

            <!--
            <button type="hidden" class="btn btn-success mb-2" @click.prevent="saveBasket"><i class="far fa-save"></i>&nbsp;Save basket</button>
            -->

        </form>

    </div>
</template>



<script>

    // Date Bskt Fund Stat Act

    export default {

        props: [], // basketid is passed as a parameter from a vue js component
        data() {
            return {
                basketAssets: null,
                isActive: true,
                href: 'xxxx'
            }
        },
        methods: {

            deleteBasket: function(basketId) {
                axios.get('/basketdelete/' + basketId)
                    //.then(response => {console.log(response.data);})
                    .catch(error => {console.log(error.response);})
            },
            createBasket: function() {
                axios.get('/basketcreate')
                //.then(response => {console.log(response.data);})
                .catch(error => {console.log(error.response);})
            }

        },

        mounted() {

            axios.get('/homegetbasketslist')
                .then(response => {

                    //var jsonParsedResponse = JSON.parse(response.data['basketAssets']);
                    console.log('HomeForm.vue response: ');
                    console.log(response);

                }) // Output returned data by controller
                .catch(error => {
                    console.log('HomeForm.vue error: ');
                    console.log(error.response);
                })
        },
        created() {

            Echo.channel('tbrChannel')

                .listen('TbrAppSearchResponse', (e) => {

                    var jsonParsedResponse = JSON.parse(e.update[0]);

                    if (e.update['eventType'] == 'showBasketsList') // First element is key => value, second is a json object
                    {
                        console.log('HomeForm.vue baskets list: ');
                        console.log(jsonParsedResponse);
                        this.basketAssets = jsonParsedResponse;
                        console.log(moment('01/12/2016', 'DD/MM/YYYY', true).format()); // Moment JS lib
                    }

                });
        },

    }
</script>