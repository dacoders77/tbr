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



            <!-- Search field and button -->
                <div id="vueJsContainer">

                    <div class="form-inline"
                         style="border-style: solid; border-width: thin; border-color: transparent;">
                        <div class="form-group mx-auto mb-2"
                             style="width:60%; border-style: solid; border-width: thin; border-color: transparent;">
                            <input id="searchInputTextField" style="width: 100%" type="text" class="form-control"
                                   value="AAPL">
                        </div>
                        <div style="border-style: solid; border-width: thin; border-color: transparent;">
                            <button v-on:click="greet" id="search" type="submit" class="btn btn-secondary mb-2">Find
                                symbol
                            </button>
                        </div>
                    </div>


                    <!-- Search results table -->
                    <div class="container">

                            <template v-for="(i, index) in quantityOfRecords">

                                <div class="row">
                                    <div style="border: 0px solid red">
                                        <small><span class="badge badge-warning">@{{ i.symbol }}</span></small>
                                    </div>
                                    <div class="col" style="border: 0px solid red">
                                        <small>
                                            @{{ i.longName }}
                                        </small>
                                    </div>
                                    <div style="border: 0px solid red; padding: 0px 10px;">
                                        <small>@{{ i.exchange }}</small>
                                    </div>
                                    <div style="border: 0px solid red; padding: 0px 10px;">
                                        <small><span class="badge badge-secondary">@{{ i.currency }}</span></small>
                                    </div>
                                    <div style="border: 0px solid red">
                                        <a href=""
                                           v-on:click.prevent='message([{{$basket_id}},i.symbol, i.longName, i.exchange, i.currency, 0])'><i
                                                    class="fas fa-plus-square"></i></a>
                                    </div>

                                </div>

                            </template>


                    </div>

                </div>




                <!-- Vue container. Must be wrapped in a parent vue div -->
                <div id="vueJsForm">
                    <!-- Vue component -->
                    <search-block basketid="{{ $basket_id }}"></search-block>
                </div>

            </div>

        </div>
    </div>
    </div>
@endsection



