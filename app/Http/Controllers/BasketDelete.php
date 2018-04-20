<?php

namespace App\Http\Controllers;

use Illuminate\Support\Facades\DB;
use Illuminate\Http\Request;

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
