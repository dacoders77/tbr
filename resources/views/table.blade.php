<!DOCTYPE html>
<html lang="{{ app()->getLocale() }}">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- CSRF Token -->
    <meta name="csrf-token" content="{{ csrf_token() }}">

    <title>{{ config('app.name', 'Laravel') }}</title>

    <!-- Styles -->
    <link href="{{ asset('css/app.css') }}" rel="stylesheet">
</head>
<body>

<br><br><br>

<div class="container">
    <div class="row">
        <div style="border: 1px solid red">
            22
        </div>
        <div class="col" style="border: 1px solid red">
            VANGUARD GL MOMENTUM FAC ETF Symb: AAPL Exch: SMART Curr: USD Price: 185
        </div>
        <div style="border: 1px solid red">
            %
        </div>
        <div style="border: 1px solid red">
            <input style="width: 50px" type="text" class="form-control input-group-lg reg_name" aria-describedby="passwordHelpInline" maxlength="3" size="3">
        </div>
        <div style="border: 1px solid red">
            12,800.27
        </div>
    </div>
</div>


<!-- Scripts -->
<script src="{{ asset('js/app.js') }}"></script>
</body>
</html>



