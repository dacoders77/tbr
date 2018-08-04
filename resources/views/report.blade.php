@extends('layouts.app_tbr')

@section('content')

    <!-- CSRF Token -->
    <meta name="csrf-token" content="{{ csrf_token() }}">

    <div class="container">
        <div class="row justify-content-center">
            <div class="col-md-8">

                <!-- Vue container. Must be wrapped in a parent vue div -->
                <div id="reportForm">
                    <!-- Vue component -->
                    <report-block basketid="{{ $basket_id }}"></report-block>
                </div>

            </div>

        </div>
    </div>

@endsection



