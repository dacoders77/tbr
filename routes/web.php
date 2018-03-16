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
Route::get('/admin', 'AdminController@index')->name('home'); // is the name correct?


Route::get('/search', function () {
    return view('search');
});

Route::get('/basket', function () {
    return view('basket');
});

Route::get('/settings', function () {
    return view('settings');
});

// Add a record to websocket que DB
Route::get('/addmsgws', 'AddMessageToSocketQue@index');

// Test rnis
Route::get('/rnis', 'RnisApiTestConnect@index');

