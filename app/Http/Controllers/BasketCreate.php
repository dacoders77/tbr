<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class basketCreate extends Controller
{
    /**
     * This controller is called from the first page where all baskets are listed
     * @param    Request $formRequest
     * @return   string $basketContentObject
     */
    public function index(Request $formRequest)
    {
        DB::table('baskets')->insert(array(
            //'execution_time' => date("Y-m-d G:i:s"),
            'execution_time' => date('Y-m-d', strtotime('+1 year')),
            'name' => "New",
            'allocated_funds' => 0,
            'status' => "new",
            'is_deleted' => 0
        ));

        $basketContentObject =
            DB::table('baskets')
                ->where('is_deleted', 0) // Show baskets that has not been deleted
                ->get();

        return($basketContentObject);

        //app('App\Http\Controllers\HomeGetBasketsList')->index();

    }
}
