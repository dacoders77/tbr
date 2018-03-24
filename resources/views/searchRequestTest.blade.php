<!doctype html>
<html lang="{{ app()->getLocale() }}">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">

    <meta name="csrf-token" content="{{ csrf_token() }}">
    <title>Search request test view and controller</title>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>

</head>

<body>

<h1>Hellow world!</h1>
<button id="search">send search test request</button>
<p></p>


<div id="app"> <!-- VueJS container -->
</div>

<div id="text"> <!-- Test container -->
</div>



</body>

<script>

    // Button handlers
    $('#search22').click(function () {
        for (i = 0; i < 10; i++)
        $("p").append("Some appended text" + [i] + "<br>");

        // call the controller. controller adds a message to db
    });

</script>

<script src={{ asset('js/app.js') }}></script>


</html>
