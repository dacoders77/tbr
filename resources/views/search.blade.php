@extends('layouts.app_tbr')

@section('content')
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8" id="vueJsContainer">

                <div class="form-inline" style="border-style: solid; border-width: thin; border-color: transparent;">
                    <div class="form-group mx-auto mb-2" style="width:60%; border-style: solid; border-width: thin; border-color: transparent;">
                        <input id="searchInputTextField" style="width: 100%" type="text" class="form-control" value="AAPL">
                    </div>
                    <div style="border-style: solid; border-width: thin; border-color: transparent;">
                    <button v-on:click="greet" id="search" type="submit" class="btn btn-secondary mb-2">Find symbol</button>
                    </div>
                </div>

                <div class="panel panel-default">
                    <!-- Default panel contents -->

                    <table class="table table-striped" id="myTable">
                        <thead>
                        <tr>
                            <th scope="col">Symb</th>
                            <th scope="col">Exch</th>
                            <th scope="col">PExc</th>
                            <th scope="col">Type</th>
                            <th scope="col">Curr</th>

                        </tr>
                        </thead>

                        <tbody>

                        <tr v-for="i in quantityOfRecords">
                            <th>@{{ i[1] }}</th>
                            <th>@{{ i[0] }}</th>
                            <th>@{{ i[4] }}</th>
                            <th>@{{ i[2] }}</th>
                            <th>@{{ i[3] }}</th>
                        </tr>

                        </tbody>

                    </table>



                    <!--
                    <div class="alert alert-danger" role="alert">
                        <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>
                        <span class="sr-only">Error:</span>
                        Please select an action
                    </div>
                    !-->

                    <!--
                <div id="app-10">
                    @{{ message }}
                    <br>
                    <button v-on:click="reverse">buton</button>

                </div>
                -->


                </div>

            </div>
        </div>
    </div>

@endsection



