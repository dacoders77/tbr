<?php

namespace App\Http\Controllers;

use Illuminate\Support\Facades\DB;
use Illuminate\Http\Request;

/**
 * Class BasketDelete
 * @package App\Http\Controllers
 * Deletes a basket. Basket itself is not deleted, it is marked with is_deleted flag and remains in the DB
 * Baskets which have is_deleted flag equal to 1 are not shown in the application
 */

class BasketDelete extends Controller
{
    public function index($param) {

        DB::table('baskets')
            ->where('id', $param) //
            ->update([
                'is_deleted' => 1
            ]);

        app('App\Http\Controllers\HomeGetBasketsList')->index();

    }
}
