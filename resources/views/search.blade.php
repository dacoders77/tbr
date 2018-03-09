@extends('layouts.app_tbr')

@section('content')
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8">

                <form class="form-inline" style="border-style: solid; border-width: thin; border-color: transparent;">
                    <div class="form-group mx-auto mb-2" style="width:60%; border-style: solid; border-width: thin; border-color: transparent;">
                        <input style="width: 100%" type="text" class="form-control" id="inputPassword2">
                    </div>
                    <div style="border-style: solid; border-width: thin; border-color: transparent;">
                    <button type="submit" class="btn btn-secondary mb-2">Find symbol</button>
                    </div>
                </form>

                <div class="panel panel-default">
                    <!-- Default panel contents -->
                    
                    <!-- Table -->

                    <table class="table table-striped">
                        <thead>
                        <tr>
                            <th scope="col">#</th>
                            <th scope="col">Symb</th>
                            <th scope="col">Name</th>
                            <th scope="col">Type</th>
                            <th scope="col">Curr</th>
                            <th scope="col">Add</th>
                        </tr>
                        </thead>
                        <tbody>
                        <tr>
                            <th scope="row">1</th>
                            <td>AAPL</td>
                            <td>Apple. inc</td>
                            <td>STK</td>
                            <td>USD</td>
                            <td class="text-danger mx-auto"><i class="fas fa-plus-circle"></i></td>
                        </tr>
                        <tr>
                            <th scope="row">2</th>
                            <td>AAPL</td>
                            <td>Apple. inc</td>
                            <td>STK</td>
                            <td>USD</td>
                            <td class="text-danger mx-auto"><i class="fas fa-plus-circle"></i></td>
                        </tr>
                        <tr>
                            <th scope="row">3</th>
                            <td>AAPL</td>
                            <td>Apple. inc</td>
                            <td>STK</td>
                            <td>USD</td>
                            <td class="text-danger mx-auto"><i class="fas fa-plus-circle"></i></td>
                        </tr>
                        <tr>
                            <th scope="row">4</th>
                            <td>AAPL</td>
                            <td>Apple. inc</td>
                            <td>STK</td>
                            <td>USD</td>
                            <td class="text-danger mx-auto"><i class="fas fa-plus-circle"></i></td>
                        </tr>
                        <tr>
                            <th scope="row">5</th>
                            <td>AAPL</td>
                            <td>Apple. inc</td>
                            <td>STK</td>
                            <td>USD</td>
                            <td class="text-danger mx-auto"><i class="fas fa-plus-circle"></i></td>
                        </tr>
                        <tr>
                            <th scope="row">6</th>
                            <td>AAPL</td>
                            <td>Apple. inc</td>
                            <td>STK</td>
                            <td>USD</td>
                            <td class="text-danger mx-auto"><i class="fas fa-plus-circle"></i></td>
                        </tr>

                        </tbody>
                    </table>

                    <div class="alert alert-danger" role="alert">
                        <span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>
                        <span class="sr-only">Error:</span>
                        Please select an action
                    </div>

                </div>

            </div>
        </div>
    </div>
@endsection



