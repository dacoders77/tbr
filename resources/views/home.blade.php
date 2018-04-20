@extends('layouts.app_tbr')

@section('content')

    <!-- CSRF Token -->
    <meta name="csrf-token" content="{{ csrf_token() }}">

    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8">

                <div id="vueJsContainer></div>



                <!-- Display flash message -->

                @if(session()->has('basket_created'))
                    <div class="alert alert-success" role="alert">
                        <i class="fas fa-check-circle"></i>&nbsp;{{session()->get('basket_created')}}
                    </div>
                @endif

                @if(session()->has('basket_deleted'))
                    <div class="alert alert-success" role="alert">
                        <i class="fas fa-check-circle"></i>&nbsp;{{session()->get('basket_deleted')}}
                    </div>
                @endif

                @if(session()->has('basket_saved'))
                    <div class="alert alert-success" role="alert">
                        <i class="fas fa-check-circle"></i>&nbsp;{{session()->get('basket_saved')}}
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



                        @php
                            $allDbRows = DB::table('baskets')->orderBy('id', 'desc')->get();

                            foreach ($allDbRows as $dbRecord){

                                if(($dbRecord->is_deleted == 0)){
                                    $shortDate = date("m-d G:i", strtotime($dbRecord->execution_time));
                                    if ($dbRecord->status == "filled") $status = "badge badge-success";
                                    if ($dbRecord->status == "new") $status = "badge badge-warning";
                                    if ($dbRecord->status == "error") $status = "badge badge-danger";

                                    echo "<tr>";
                                    echo "<td><a href=\"basket/$dbRecord->id\">$shortDate</a></td>";
                                    echo "<td>$dbRecord->name</td>";
                                    echo "<td>$dbRecord->allocated_funds</td>";
                                    echo "<td class=\"text-danger mx-auto\"><span class=\"$status\">$dbRecord->status</span></td>";
                                    echo "<td class=\"text-danger mx-auto\"><a href=\"basketdelete/$dbRecord->id\"><i class=\"fas fa-trash-alt\" style=\"color: tomato\"></a></i></td>";
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
                        <br>
                    </div>



                </div>

            </div>
        </div>
    </div>
@endsection



