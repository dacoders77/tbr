<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

/**
 * Class HomeGetBasketsList
 * @package App\Http\Controllers
 *
 * Outputs the list of baskets to the home page
 */
class HomeGetBasketsList extends Controller
{
    public function index() // public function __invoke()
    {
        $basketContentobject =
            DB::table('baskets')
                ->where('is_deleted', 0) // Show baskets that has not been deleted
                ->get();

        $basketContentJson = json_encode($basketContentobject);

        event(new \App\Events\TbrAppSearchResponse(['eventType' => 'showBasketsList', $basketContentJson]));

    }
}
