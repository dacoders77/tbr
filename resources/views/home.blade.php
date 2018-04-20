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

                    <!-- Vue container. Must be wrapped in a parent vue div -->
                    <div id="vueHomeForm">
                        <!-- Vue component -->
                        <home-block></home-block>
                    </div>




                </div>

            </div>
        </div>
    </div>
@endsection



