@extends('layouts.app_tbr')

@section('content')
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8">

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


                <!-- <form action="/public/basketupdate" method="post"> -->
                    {{ Form::open(['route' => 'basketupdate.post']) }}
                    @csrf

                    <input type="hidden" name="basket-id" value="{{$basket_id}}">

                    <div class="input-group mb-3">
                        <input type="text" name="basket-name" class="form-control" value="@php

                            $basketName =
                                DB::table('baskets')
                                    ->where('basket_id', $basket_id)
                                    ->value('basket_name');

                            echo $basketName;

                        @endphp" aria-label="basket name text" aria-describedby="basic-addon2">
                        <div class="input-group-append">
                            <span class="input-group-text" id="basic-addon2">&nbsp;&nbsp;&nbsp;Basket name</span>
                        </div>
                    </div>

                    <div class="input-group mb-3">
                        <input type="datetime-local" name="basket-execution-time" class="form-control" value="@php

                            $basketExecutionTime =
                                DB::table('baskets')
                                    ->where('basket_id', $basket_id)
                                    ->value('basket_execution_time'); // datetime-local

                        $basketExecutionTime = date("Y-m-d\TH:i", strtotime($basketExecutionTime));;
                        echo $basketExecutionTime;

                            @endphp" placeholder="BSK1" aria-label="execution time" aria-describedby="basic-addon2">
                        <div class="input-group-append">
                            <span class="input-group-text" id="basic-addon2">Execution time</span>
                        </div>
                    </div>

                    <div class="container text-center" style="border-style: solid; border-width: thin; border-color: transparent;">
                        <div class="row align-items-center">
                            <div class="container col" style="border-style: solid; border-width: thin; border-color: transparent">
                                <p style="font-size: 1.5em;">USD: <span class="badge badge-warning">10.000</span></p>
                            </div>
                            <div class="container col">
                                <p style="font-size: 1.5em;">Availible: <span class="badge badge-success">52.856</span></p>
                            </div>
                        </div>
                    </div>




                    <div class="panel panel-default">

                        <!-- Table -->

                        <table class="table table-striped">
                            <thead>
                            <tr>
                                <th scope="col">Symb</th>
                                <th scope="col">Exch</th>
                                <th scope="col">Curr</th>
                                <th scope="col">%</th>
                                <th scope="col">Action</th>
                            </tr>
                            </thead>
                            <tbody>

                            @php
                                $allDbRows = DB::table('assets')->orderBy('basket_id', 'desc')->get();

                                foreach ($allDbRows as $dbRecord){

                                    if(($dbRecord->basket_id == $basket_id)){
                                        echo "<tr>";
                                            //echo "<td><a href=\"basket/$dbRecord->basket_id\">$shortDate</a></td>";
                                            // echo "<td>$dbRecord->asset_id</td>";
                                            echo "<td>$dbRecord->asset_symbol</td>";
                                            echo "<td>$dbRecord->asset_exchange</td>";
                                            echo "<td>$dbRecord->asset_currency</td>";
                                            echo "<td><input type=\"text\" name=\"$dbRecord->asset_id\" class=\"form-control\" placeholder=\"$dbRecord->asset_allocated_percent\"></td>";
                                            $url = URL::route('assetdelete', array('zz'=>$basket_id, 'xx'=>$dbRecord->asset_id));
                                            echo "<td class=\"text-danger mx-auto\"><a href=\"$url\"><i class=\"fas fa-trash-alt\" style=\"color: tomato\"></a></i></td>";
                                        echo "</tr>";
                                    }
                                }
                            @endphp



                            </tbody>
                        </table>



                            <div class="container text-center" style="border-style: solid; border-width: thin; border-color: transparent;">
                                <button type="submit" class="btn btn-success mb-2"><i class="far fa-save"></i>&nbsp;Save basket</button>
                            </div>

                    </div>

                </form>




                <!-- Search feild and button -->

                <div id="vueJsContainer">

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
                                <a href="" v-on:click='message([{{$basket_id}},i[1],i[0],i[3],0])'><i class="fas fa-plus-square"></i></a>
                            </th>

                        </tr>

                        </tbody>

                    </table>

                </div>



                </div>

            </div>
        </div>
    </div>
@endsection



