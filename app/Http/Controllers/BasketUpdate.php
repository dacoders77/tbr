<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;


class BasketUpdate extends Controller
{

    public function index(Request $request){


        DB::table('baskets')
            ->where('basket_id', $request->get('basket-id'))
            ->update([
                'basket_name' => $request->get('basket-name'),
                'basket_execution_time' => date("Y-m-d G:i:s", strtotime($request->get('basket-execution-time')))
            ]);

        // Loop through the list of all assets shown at the page
        // Where i >= 5. Starting from 5th variable
            // DB:table('baskets')...
            // 'asset_id' => 'value'

        $allFormVariables = $request->all();
        $counter = 1;

        foreach ($allFormVariables as $x => $x_value) {

           // echo "NO IF. key: " . $x . " value: " . $x_value . "<br>";

            if($counter > 4){
                //echo "key: " . $x . " value: " . $x_value . "<br>";


                DB::table('assets')
                    ->where('basket_id', $request->get('basket-id'))
                    ->where('asset_id', $x)
                    ->update([
                        'asset_allocated_percent' => $x_value,
                    ]);

            }
            $counter++;
        }

        session()->flash('basket_saved', 'Basked saved!');

        return redirect('home'); // Go to url

    }
}
