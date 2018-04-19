<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;


class BasketUpdate extends Controller
{

    public function index(Request $request){

        DB::table('baskets')
            ->where('basket_id', $request->get('basketId'))
            ->update([
                'name' => $request->get('basketName'),
                'execution_time' => date("Y-m-d G:i:s", strtotime($request->get('basketExecTime')))
            ]);

        // Loop through all json records
        // These records are the content of the assets table from

        $assets = $request->get('basketAssets');

                // $assets as $asset
        foreach ($assets as $asset ){

            DB::table('assets')
                ->where('basket_id', $asset['basket_id'])
                ->where('id', $asset['id'])
                ->update([
                    'allocated_percent' => $asset['allocated_percent'],
                ]);
        }

    }
}
