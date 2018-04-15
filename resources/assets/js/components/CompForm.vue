<template>
<div>

    <form>

        <!-- <h3>basket id: {{ basketid }}</h3> -->
        Basket name:
        <input class="form-control" v-model="basketName" />
        <br>
        Basket execution time:
        <input type="datetime-local" class="form-control" v-model="basketExecTime" />
        <br><br>

        <table class="table table-striped">

            <thead>
            <tr>
                <th>Symb</th>
                <th>Exch</th>
                <th>Curr</th>
                <th>%</th>
                <th>Action</th>

            </tr>
            </thead>

            <tbody>

                <tr v-for="(i, index) in basketContentJson">
                    <td>{{ i['asset_symbol'] }}</td>
                    <td>{{ i['asset_exchange'] }}</td>
                    <td>{{ i['asset_currency'] }}</td>
                    <td><input class="form-control" v-model="i['asset_allocated_percent']" size="1" type="text"></td>
                    <td><a href="" v-on:click.prevent="assetDelete([i['basket_id'], i['asset_id']])"><i class="fas fa-trash-alt" style="color: tomato"></i></a></td>
                    <td>{{ i['asset_id'] }}</td>
                </tr>

            </tbody>

        </table>

        <button type="submit" class="btn btn-success mb-2" @click.prevent="saveBasket"><i class="far fa-save"></i>&nbsp;Save basket</button>

    </form>

</div>
</template>



<script>
export default {

    props: ['basketid'],
    data() {
       return {
           idbasket: this.basketid, // Put a property here. This is need in order to send all variables in this.$data to the BasketUpdate.php controller
           basketName: '',
           basketExecTime: '',
           basketContentJson: null,
       }
    },
    methods: {
        saveBasket() {
            //alert('CompVue.vue. Save basket button is clicked');
            console.log(this.$data);
            axios.post('/basketupdate', this.$data)
                .then(response => {console.log(response.data);})
                .catch(error => {console.log(error.response);})

        },
        assetDelete: function(param) {
            //alert(param[1]);
            axios.get('/assetdelete/' + param[0] + '/' + param[1])
                .then(response => {console.log(response.data);})
                .catch(error => {console.log(error.response);})
        },

    },

    mounted() {
        //console.log(this.title);
        //console.log(this.$props);

        axios.post('/getbasketname', this.$props)
            .then(response => {
                //console.log(response.data['basketContentJson']);

                this.basketName = response.data['basketName'];
                this.basketExecTime = response.data['basketExecTime'];

                var jsonParsedResponse = JSON.parse(response.data['basketContentJson']);
                this.basketContentJson = jsonParsedResponse;

            }) // Output returned data by controller
            .catch(error => {
                console.log(error.response);
            })
    },
    created() {

        Echo.channel('tbrChannel')

            .listen('TbrAppSearchResponse', (e) => {
                var jsonParsedResponse = JSON.parse(e.update);

                // Second element is goona be - Table Content
                if (jsonParsedResponse['eventType'] == 'showBasketContent')
                {
                    console.log(jsonParsedResponse[0]); // First element is key => value, second is another json object
                    var jsonParsedResponse = jsonParsedResponse[0];
                    this.basketContentJson = jsonParsedResponse;
                }

                //this.quantityOfRecords = jsonParsedResponse;

            });
    },

}
</script>