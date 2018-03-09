@extends('layouts.app_tbr')

@section('content')
    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8">

                <div class="alert alert-success" role="alert">
                    <i class="fas fa-check-circle"></i>&nbsp;AAPL was successfully added to BS1 basket!
                </div>


                <div class="input-group mb-3">
                    <input type="text" class="form-control" placeholder="BSK1" aria-label="Recipient's username" aria-describedby="basic-addon2">
                    <div class="input-group-append">
                        <span class="input-group-text" id="basic-addon2">&nbsp;&nbsp;&nbsp;Basket name</span>
                    </div>
                </div>

                <div class="input-group mb-3">
                    <input type="date" class="form-control" placeholder="BSK1" aria-label="Recipient's username" aria-describedby="basic-addon2">
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
                    <!-- Default panel contents -->

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
                        <tr>
                            <th scope="row">4</th>
                            <td>AAPL</td>
                            <td>NYSE</td>
                            <td>USD</td>
                            <td>
                                <input style="" type="text" class="form-control" id="inputPassword2" placeholder="12%">
                            </td>
                            <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                        </tr>
                        <tr>
                            <th scope="row"></th>
                            <td></td>
                            <td></td>

                            <td><b>Toral:</b></td>
                            <td><b>&nbsp;&nbsp;&nbsp;&nbsp;34%</b></td>
                            <td></td>

                        </tr>

                        </tbody>
                    </table>


                    <div class="container text-center" style="border-style: solid; border-width: thin; border-color: transparent;">
                        <button type="submit" class="btn btn-success mb-2"><i class="far fa-save"></i>&nbsp;Save busket</button>
                    </div>



                </div>

            </div>
        </div>
    </div>
@endsection



