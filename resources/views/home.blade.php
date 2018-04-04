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

                <!-- Display flash message -->
                @if(session()->has('status'))
                    <div class="alert alert-primary" role="alert">
                        <i class="fas fa-check-circle"></i>&nbsp;{{session()->get('status')}}
                    </div>

                @endif




                <div class="panel panel-default">
                    <!-- Default panel contents -->


                    <table class="table table-striped">
                        <thead>
                        <tr>
                            <th scope="col">Date</th>
                            <th scope="col">Bskt</th>
                            <th scope="col">Fund</th>
                            <th scope="col">Stat</th>
                            <th scope="col">Act</th>
                        </tr>
                        </thead>
                        <tbody>
                        <!-- Table
                        <tr>
                            <td>18.02.18</td>
                            <td>BSK1</td>
                            <td>16.200</td>
                            <td class="text-danger mx-auto"><span class="badge badge-warning">Pend</span></td>
                            <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                        </tr>

                        <tr>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
                            <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                        </tr>

                        <tr>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
                            <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                        </tr>

                        <tr>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td class="text-danger mx-auto"><span class="badge badge-danger">Error</span></td>
                            <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                        </tr>

                        <tr>
                            <td>18.02.17</td>
                            <td>BSK0</td>
                            <td>8.721</td>
                            <td class="text-danger mx-auto"><span class="badge badge-success">Filled</span></td>
                            <td class="text-danger mx-auto"><i class="fas fa-trash-alt"></i></td>
                        </tr>
                        -->


                        @php
                            $allDbRows = DB::table('baskets')->orderBy('basket_id', 'desc')->get();

                            foreach ($allDbRows as $dbRecord){

                                if(($dbRecord->basket_is_deleted == 0)){
                                    $shortDate = date("m-d G:i", strtotime($dbRecord->basket_execution_time));
                                    if ($dbRecord->basket_status == "filled") $status = "badge badge-success";
                                    if ($dbRecord->basket_status == "new") $status = "badge badge-warning";
                                    if ($dbRecord->basket_status == "error") $status = "badge badge-danger";

                                    echo "<tr>";
                                    echo "<td><a href=\"basket/$dbRecord->basket_id\">$shortDate</a></td>";
                                    echo "<td>$dbRecord->basket_name</td>";
                                    echo "<td>$dbRecord->basket_allocated_funds</td>";
                                    echo "<td class=\"text-danger mx-auto\"><span class=\"$status\">$dbRecord->basket_status</span></td>";
                                    echo "<td class=\"text-danger mx-auto\"><a href=\"basketdelete/$dbRecord->basket_id\"><i class=\"fas fa-trash-alt\" style=\"color: tomato\"></a></i></td>";
                                    echo "</tr>";
                                }
                            }
                        @endphp





                        </tbody>
                    </table>

                    <div class="alert alert-success" role="alert">
                        <a href="basketcreate">
                        <i class="fas fa-plus-square"></i>&nbsp;Add new basked
                        </a>
                    </div>



                </div>

            </div>
        </div>
    </div>
@endsection



