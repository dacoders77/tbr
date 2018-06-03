<?php

namespace App\Http\Controllers;

use Illuminate\Support\Facades\DB;
use Illuminate\Http\Request;

/**
 * Class BasketDelete
 * Deletes a basket. Basket itself is not deleted, it is marked with is_deleted flag and remains in the DB
 * Baskets which have is_deleted flag equal to 1 are not shown in the application
 * @package App\Http\Controllers
 * @param int $param Basket id
 */

class BasketDelete extends Controller
{
    public function index(int $param) {

        DB::table('baskets')
            ->where('id', $param) //
            ->update([
                'is_deleted' => 1
            ]);

        $basketContentObject =
            DB::table('baskets')
                ->where('is_deleted', 0) // Show baskets that has not been deleted
                ->get();

        return($basketContentObject);

        //app('App\Http\Controllers\HomeGetBasketsList')->index();

    }
}
