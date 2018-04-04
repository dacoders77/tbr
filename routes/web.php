<?php

/*
|--------------------------------------------------------------------------
| Web Routes
|--------------------------------------------------------------------------
|
| Here is where you can register web routes for your application. These
| routes are loaded by the RouteServiceProvider within a group which
| contains the "web" middleware group. Now create something great!
|
*/

Route::get('/', function () {
    return view('admin');
});

Auth::routes();

Route::get('/home', 'HomeController@index')->name('home');



Route::get('/admin', 'AdminController@index'); // is the name correct?


Route::get('/search', function () {
    return view('search');
});


Route::get('/basket/{z}', function ($basketId)
{
    return View::make('basket')->with('basket_id', $basketId);
})->name('basket.view'); // basket.blade.php works good



Route::get('/settings', function () {
    return view('settings');
});

// Add a record to websocket que DB
Route::get('/addmsgws/{searchRequestString}', 'AddMessageToSocketQue@index');

// Test rnis
Route::get('/rnis', 'RnisApiTestConnect@index');

// Test search view and controller
Route::get('/searchRequestTest', function () {
    return view('searchRequestTest');
});

// Search response pusher event
Route::get('event', function () {
    event(new \App\Events\TbrAppSearchResponse('How are you?'));
});

// Add new basket
Route::get('/basketcreate', 'BasketCreate@index');

Route::get('/basketdelete/{param}', 'BasketDelete@index'); // Controller is called using the given name and passing {param} to it

// Basket update
Route::post('/basketupdate', 'BasketUpdate@index');