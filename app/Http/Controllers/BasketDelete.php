<?php

namespace App\Http\Controllers;

use Illuminate\Support\Facades\DB;
use Illuminate\Http\Request;

class BasketDelete extends Controller
{
    public function index($param) {

        //echo "Basket id:" . $param . " is being deleted";

        DB::table('baskets')
            ->where('basket_id', $param) //
            ->update([
                'basket_is_deleted' => 1
            ]);

        session()->flash('basket_deleted', 'Basket deleted!');

        return redirect('home'); // Go to url

    }
}
