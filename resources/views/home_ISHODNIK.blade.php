@extends('layouts.app')

@section('content')
<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-8">
            <div class="card">
                <div class="card-header">Ticker search</div>

                <div class="card-body">
                    @if (session('status'))
                        <div class="alert alert-success">
                            {{ session('status') }}
                        </div>
                    @endif

                        <form class="navbar-form navbar-left" role="search">
                            <div class="form-group">
                                <input type="text" class="form-control" placeholder="Search">
                            </div>
                            <button type="submit" class="btn btn-default">Submit</button>
                        </form>

                </div>
            </div>

            <div class="panel panel-default">
                <!-- Default panel contents -->
                <div class="panel-heading">Panel heading</div>

                <!-- Table -->
                <table class="table">
                    <table class="table"> <thead>
                        <tr> <th>#</th> <th>Symbol</th> <th>Name</th> <th>Type</th> <th>Currency</th> <th>Add</th> </tr>
                        </thead> <tbody>
                        <tr> <th scope="row">1</th> <td>AAPL</td> <td>Apple inc.</td> <td>Stock</td> <td>USD</td> <td>add</td> </tr>
                        <tr> <th scope="row">2</th> <td>PHOT</td> <td>Photo corp.</td> <td>Stck</td> <td>USD</td> <td>add</td> </tr>
                        <tr> <th scope="row">2</th> <td>TYSE</td> <td>Yuse info.</td> <td>ETF</td> <td>SGD</td> <td>add</td> </tr>
                        <tr> <th scope="row">1</th> <td>AAPL</td> <td>Apple inc.</td> <td>Stock</td> <td>USD</td> <td>add</td> </tr>
                        <tr> <th scope="row">2</th> <td>PHOT</td> <td>Photo corp.</td> <td>Stck</td> <td>USD</td> <td>add</td> </tr>

                        </tbody> </table>
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
