<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;


class BasketUpdate extends Controller
{

    public function index(Request $request){

        DB::table('baskets')
            ->where('basket_id', $request->get('basket-id')) //
            ->update([
                'basket_name' => $request->get('basket-name'),
                'basket_execution_time' => date("Y-m-d G:i:s", strtotime($request->get('basket-execution-time')))
            ]);

        return redirect('home'); // Go to url

    }
}
