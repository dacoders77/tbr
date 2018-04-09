<!DOCTYPE html>
<html lang="{{ app()->getLocale() }}">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <!-- CSRF Token -->
    <meta name="csrf-token" content="{{ csrf_token() }}">

    <title>{{ config('app.name', 'Laravel') }}</title>
    <!-- External links -->
    <script defer src="https://use.fontawesome.com/releases/v5.0.8/js/all.js"></script>
    <script src="https://unpkg.com/axios/dist/axios.min.js"></script>

    <!-- Styles -->
    <link href="{{ asset('css/app.css') }}" rel="stylesheet">

</head>

<!-- Ekko menu is used to generate dynamic urls. https://packagist.org/packages/laravelista/ekko -->

<p>{{ isActiveURL('/') }}</p>
<body>
    <div id="app">
        <nav class="navbar navbar-expand-md navbar-light navbar-laravel" >

            <div class="col-md-12 text-center" style="border-style: solid; border-width: thin; border-color: transparent;">
            <ul class="nav nav-pills center-pills nav-justified">
                <li style="width: 33%; border-style: solid; border-width: thin; border-color: transparent;">
                    <a class="nav-link {{ isActiveURL('/home') }}" href="{{ route('home') }}"><i class="fas fa-home"></i>&nbspHome</a>
                </li>
                <li style="width: 33%;">
                    <a class="nav-link {{ isActiveURL('/search') }}" href="{{ url('/search/')}}"><i class="fas fa-search"></i>&nbspSearch</a>
                </li>
                <li style="width: 33%;">
                    <a class="nav-link {{ isActiveURL('/basket') }}" href="{{ url('/home/')}}"><i class="fas fa-shopping-basket"></i>&nbspBasket</a>
                </li>
            </ul>
            </div>


        </nav>

        <main class="py-4">
            @yield('content')
        </main>
    </div>

    <!-- Scripts -->
    <script>window.siteUrl = "{{ url('/') }}"</script>
    <script src="{{ asset('js/app.js') }}"></script>


</body>
</html>

<style>
    .center-pills {
        display: flex;
        justify-content: center;
    }

</style>





<!--
<div>
    <a class="dropdown-item" href="{{ route('logout') }}"
       onclick="event.preventDefault();
       document.getElementById('logout-form').submit();">
        {{ __('Logout') }}
        </a>

        <form id="logout-form" action="{{ route('logout') }}" method="POST" style="display: none;">
        @csrf
    </form>
</div>
