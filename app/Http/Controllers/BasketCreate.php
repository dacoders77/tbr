<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class basketCreate extends Controller
{

    public function index(Request $formRequest){
        echo "basket controller hello!" . $formRequest['basket-name'] . "<br>";
        echo "basket time: " . $formRequest['basket-execution-time'] . "<br>";
        echo $formRequest['basket-name'];
        echo "time '2018-03-31 10:00:00': " . date("Y-m-d G:i:s", strtotime($formRequest['basket-execution-time'])) . "<br>";

        // Datetime converted to MySQL format after receiving from the from
        // date("Y-m-d G:i:s", strtotime($formRequest['basket-execution-time']))

        DB::table('baskets')->insert(array(
            'basket_execution_time' => null,
            'basket_name' => "New",
            'basket_allocated_funds' => 0,
            'basket_status' => "new",
            'basket_is_deleted' => 0
        ));

        //return redirect()->route('home');
        return View::make('home');

    } // public function

} // Class
