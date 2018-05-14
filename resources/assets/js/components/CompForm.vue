<template>
<div>

    <form>

        <!-- <h3>basket id: {{ basketid }}</h3> -->
        Basket name:
        <input class="form-control" v-model="basketName" @input="onControlValueChanged" />
        <br>
        Basket execution time:
        <input type="datetime-local" class="form-control" v-model="basketExecTime" @input="onControlValueChanged" />
        <br><br>

        <table class="table table-striped">

            <thead>
            <tr>
                <th>Symb</th>
                <th>Exch</th>
                <th>Curr</th>
                <th>Price</th>
                <th>%</th>
                <th>Fund</th>
                <th>Action</th>

            </tr>
            </thead>

            <tbody>

                <tr v-for="asset in basketAssets">
                    <td>{{ asset.symbol }}</td>
                    <td>{{ asset.exchange }}</td>
                    <td>{{ asset.currency }}</td>
                    <td>{{ asset.price }}</td>
                    <td><input type="text" class="form-control" v-model="asset.allocated_percent" size="1" @input="onControlValueChanged"></td>
                    <td>0</td>
                    <td><a href="" v-on:click.prevent="assetDelete(asset.id)"><i class="fas fa-trash-alt" style="color: tomato"></i></a></td>
                </tr>

            </tbody>

            <tfoot>
            <tr>
                <th colspan="4">Total:</th>
                <th>85%</th>
                <th class="align-right">12,625$</th>

            </tr>
            </tfoot>



        </table>

        <!--
        <button type="hidden" class="btn btn-success mb-2" @click.prevent="saveBasket"><i class="far fa-save"></i>&nbsp;Save basket</button>
        -->

    </form>

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
       }
    },
    methods: {
        saveBasket() {
            axios.post('/basketupdate', this.$data)
                .then(response => {console.log(response.data);})
                .catch(error => {console.log(error.response);})
        },
        assetDelete: function(assetId) {
            //alert(param[1]);
            axios.get('/assetdelete/' + this.basketId + '/' + assetId) // axios.get('/assetdelete/' + param[0] + '/' + param[1])
                .then(response => {console.log(response.data);})
                .catch(error => {console.log(error.response);})
        },
        onControlValueChanged() {
            console.log('event is rised');
            console.log(this.$data);
            axios.post('/basketupdate', this.$data)
                .then(response => {console.log(response.data);})
                .catch(error => {console.log(error.response);})
        },

    },

    mounted() {
        //console.log(this.title);
        //console.log(this.$props);

        axios.post('/basketgetdetails', this.$data)
            .then(response => {

                this.basketName = response.data['basketName'];
                this.basketExecTime = response.data['basketExecTime'];

                var jsonParsedResponse = JSON.parse(response.data['basketAssets']);
                this.basketAssets = jsonParsedResponse;

                //console.log('jsonParseResponse: ');
                //console.log(jsonParsedResponse);

            }) // Output returned data by controller
            .catch(error => {
                console.log('getbasketname error: ');
                console.log(error.response);
            })
    },
    created() {

        Echo.channel('tbrChannel')

            .listen('TbrAppSearchResponse', (e) => {

                // SHOW BASKET CONTENT
                var jsonParsedResponse = JSON.parse(e.update);

                if (jsonParsedResponse.messageType == 'showBasketContent')
                {
                    this.basketAssets = jsonParsedResponse.body;
                }
            });
    },
}
</script>