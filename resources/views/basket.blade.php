@extends('layouts.app_tbr')

@section('content')
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8">

                <div class="alert alert-success" role="alert">
                    <i class="fas fa-check-circle"></i>&nbsp;AAPL was successfully added to BS1 basket!
                </div>

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
                                <th scope="col">#</th>
                                <th scope="col">Symb</th>
                                <th scope="col">Exch</th>
                                <th scope="col">Curr</th>
                                <th scope="col">%</th>
                                <th scope="col">Action</th>
                            </tr>
                            </thead>
                            <tbody>
                            <tr>
                                <th scope="row">1</th>
                                <td>AAPL</td>
                                <td>NYSE</td>
                                <td>USD</td>
                                <td>
                                    <input style="" type="text" class="form-control" id="inputPassword2" placeholder="12%">
                                </td>
                                <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                            </tr>
                            <tr>
                                <th scope="row">2</th>
                                <td>AAPL</td>
                                <td>NYSE</td>
                                <td>USD</td>
                                <td>
                                    <input type="text" class="form-control" id="inputPassword2" placeholder="12%">
                                </td>
                                <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                            </tr>
                            <tr>
                                <th scope="row">3</th>
                                <td>AAPL</td>
                                <td>NYSE</td>
                                <td>USD</td>
                                <td>
                                    <input style="" type="text" class="form-control" id="inputPassword2" placeholder="12%">
                                </td>
                                <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                            </tr>


                            </tbody>
                        </table>



                            <div class="container text-center" style="border-style: solid; border-width: thin; border-color: transparent;">
                                <button type="submit" class="btn btn-success mb-2"><i class="far fa-save"></i>&nbsp;Save basket</button>
                            </div>

                    </div>

                </form>




                <!-- Search feild and button -->
                <div class="form-inline" style="border-style: solid; border-width: thin; border-color: transparent;">
                    <div class="form-group mx-auto mb-2" style="width:60%; border-style: solid; border-width: thin; border-color: transparent;">
                        <input id="searchInputTextField" style="width: 100%" type="text" class="form-control" value="AAPL">
                    </div>
                    <div id="search" style="border-style: solid; border-width: thin; border-color: transparent;">
                        <button type="submit" class="btn btn-secondary mb-2">Find symbol</button>
                    </div>
                </div>

                <!-- Search results table -->
                <div id="testVue">

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
                </div>



                </div>

            </div>
        </div>
    </div>
@endsection



