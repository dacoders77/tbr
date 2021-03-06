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

Auth::routes();

Route::get('/', function () {
    return view('home');
})->middleware('auth');


Route::get('/home', 'HomeController@index')->name('home');

Route::get('/admin', 'AdminController@index'); // is the name correct?


Route::get('/search', function () {
    return view('search');
});

// basket.blade.php
Route::get('/basket/{z}', function ($basketId) {
    return View::make('basket')->with('basket_id', $basketId);
});

// report.blade.php
Route::get('/report/{z}', function ($basketId) {
    return View::make('report')->with('basket_id', $basketId);
});

Route::get('/settings', function () {
    return view('settings');
});



// Add a record (message) to websocket que DB. Later these messages are sent to C#
Route::get('/addmsgws/{requestType}/{searchRequestString}', 'AddMessageToSocketQue@index');

// Test rnis
Route::get('/rnis', 'RnisApiTestConnect@index');

// Test search view and controller
Route::get('/searchRequestTest', function () {
    return view('searchRequestTest');
});

// Search response pusher event test
Route::get('event', function () {
    event(new \App\Events\TbrAppSearchResponse('How are you?'));
});

// Add new basket
Route::get('/basketcreate', 'BasketCreate@index');

Route::get('/basketdelete/{param}', 'BasketDelete@index'); // Controller is called using the given name and passing {param} to it

// Basket update
Route::post('/basketupdate', 'BasketUpdate@index')->name('basketupdate.post');

// Delete asset from the basket
Route::get('/assetdelete/{z}/{x}', 'AssetDelete@index')->name('assetdelete');

// Add asset to DB
Route::get('/assetcreate/{basketId}/{assetSymbol}/{longName}/{assetExchange}/{assetCurrency}/{assetAllocatedPercent}', 'AssetCreate@index')->name('assetcreate');

// Get basket name. Axios request controller
Route::post('/basketgetdetails', 'BasketGetDetails@index');

// Get the list of baskets for the home page
Route::get('homegetbasketslist', 'HomeGetBasketsList@index');

// Get server timr
Route::get('getservertime', 'GetServerTime'); // Single action controller

// Get the report
Route::post('/report', 'GetReport@index');

// Test table view. Delete it
Route::view('/table', 'table');