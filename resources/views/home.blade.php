@extends('layouts.app_tbr')

@section('content')

    <!-- CSRF Token -->
    <meta name="csrf-token" content="{{ csrf_token() }}">

    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8">

                <div class="container text-center" style="border-style: solid; border-width: thin; border-color: transparent;">
                    <div class="row align-items-center">
                        <div  style="display: inline-block; width: 20%; border-style: solid; border-width: thin; border-color: transparent; font-size:2em;">
                            <a style="color:Tomato" href="{{ url('/settings')}}">
                            <i class="fas fa-sliders-h"></i>
                            </a>
                        </div>
                        <div class="container col" style="border-style: solid; border-width: thin; border-color: transparent">
                            <form>
                                <div >
                                    <button style="width: 100%" type="submit" class="btn btn-success"><i class="fas fa-play"></i>&nbsp;Start Bot</button>
                                </div>
                            </form>
                        </div>
                        <div style="display: inline-block; width: 20%; font-size:35px; color:Tomato">

                            <!--
                            <a style="color:Tomato" href="{{ route('logout') }}">
                            <i class="fas fa-sign-out-alt"></i>
                            </a>
                            -->


                            <a style="color:Tomato" class="dropdown-item" href="{{ route('logout') }}"
                               onclick="event.preventDefault();
                                                     document.getElementById('logout-form').submit();">
                                <i class="fas fa-sign-out-alt"></i>
                            </a>

                            <form id="logout-form" action="{{ route('logout') }}" method="POST" style="display: none;">
                                @csrf
                            </form>


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
                            <th scope="col">Date</th>
                            <th scope="col">Bskt</th>
                            <th scope="col">Fund</th>
                            <th scope="col">%</th>
                            <th scope="col">Stat</th>
                        </tr>
                        </thead>
                        <tbody>
                        <tr>
                            <th scope="row">1</th>
                            <td>18.02.18</td>
                            <td>BSK1</td>
                            <td>16.200</td>
                            <td>15</td>
                            <td class="text-danger mx-auto"><span class="badge badge-warning">Pend</span></td>
                        </tr>

                        <tr>
                            <th scope="row">1</th>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td>76</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
                        </tr>

                        <tr>
                            <th scope="row">1</th>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td>76</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
                        </tr>

                        <tr>
                            <th scope="row">1</th>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td>76</td>
                            <td class="text-danger mx-auto"><span class="badge badge-danger">Error</span></td>
                        </tr>

                        <tr>
                            <th scope="row">1</th>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td>76</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
                        </tr>

                        <tr>
                            <th scope="row">1</th>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td>76</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
                        </tr>

                        <tr>
                            <th scope="row">1</th>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td>76</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
                        </tr>

                        <tr>
                            <th scope="row">1</th>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td>76</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
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



