@extends('layouts.app_tbr')

@section('content')
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8">



                    <!-- Flash messages -->
                    @if(session()->has('asset_added'))
                        <div class="alert alert-success" role="alert">
                            <i class="fas fa-check-circle"></i>&nbsp;{{session()->get('asset_added')}}
                        </div>
                    @endif

                    @if(session()->has('asset_deleted'))
                        <div class="alert alert-success" role="alert">
                            <i class="fas fa-check-circle"></i>&nbsp;{{session()->get('asset_deleted')}}
                        </div>
                    @endif


                    <div class="container text-center" style="border-style: solid; border-width: thin; border-color: transparent;">
                        <div class="row align-items-center">
                            <div class="container col" style="border-style: solid; border-width: thin; border-color: transparent">
                                <p style="font-size: 1.5em;">USD: <span class="badge badge-warning">10.000</span></p>
                            </div>
                            <div class="container col">
                                <p style="font-size: 1.5em;">Avail: <span class="badge badge-success">52.856</span></p>
                            </div>
                        </div>
                    </div>


                    <!-- Vue container. Must be wrapped in a parent vue div -->
                    <div id="vueJsForm">
                        <!-- Vue component -->
                        <search-block basketid="{{ $basket_id }}"></search-block>
                    </div>

                    <br>
                    <br>

                     <div id="vueJsContainer">

                    <!-- Search field and button -->

                        <div class="form-inline" style="border-style: solid; border-width: thin; border-color: transparent;">
                            <div class="form-group mx-auto mb-2" style="width:60%; border-style: solid; border-width: thin; border-color: transparent;">
                                <input id="searchInputTextField" style="width: 100%" type="text" class="form-control" value="AAPL">
                            </div>
                            <div style="border-style: solid; border-width: thin; border-color: transparent;">
                                <button v-on:click="greet" id="search" type="submit" class="btn btn-secondary mb-2">Find symbol</button>
                            </div>
                        </div>

                        <!-- Search results table -->

                        <table class="table table-striped" id="myTable">

                            <thead>
                            <tr>
                                <th scope="col">Symb</th>
                                <th scope="col">Exch</th>
                                <th scope="col">Type</th>
                                <th scope="col">Curr</th>
                                <th scope="col">Add</th>

                            </tr>
                            </thead>

                            <tbody>

                            <tr v-for="(i, index) in quantityOfRecords">
                                <th>@{{ i[1] }}</th> <!-- symbol -->
                                <th>@{{ i[0] }}</th> <!-- exchange -->
                                <!--<th>@{{ i[4] }}</th>  primary exchange -->
                                <th>@{{ i[2] }}</th> <!-- type -->
                                <th>@{{ i[3] }}</th> <!-- currency -->
                                <th>
                                    <!-- <button v-on:click="message(i[0])" type="button" class="btn btn-link">Basket @{{ index }}</button> -->
                                    <a href="" v-on:click.prevent='message([{{$basket_id}},i[1],i[0],i[3],0])'><i class="fas fa-plus-square"></i></a>
                                </th>

                            </tr>

                            </tbody>

                        </table>

                    </div>
                    <!-- vue js container -->

                </div>

            </div>
        </div>
    </div>
@endsection



