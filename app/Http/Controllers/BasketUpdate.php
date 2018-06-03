<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

/**
 * Class BasketUpdate
 * Updates basket content. Update action is taken when any changes are made at the basket content page
 * This controller is called from CompForm.vue
 * Basket's name, execution time or one of basket's assets
 * @package App\Http\Controllers
 */
class BasketUpdate extends Controller
{
    public function index(Request $request){

        /** Update basket name, exec time and allocated funds for trading */
        DB::table('baskets')
            ->where('id', $request->get('basketId'))
            ->update([
                'name' => $request->get('basketName'),
                'execution_time' => date("Y-m-d G:i:s", strtotime($request->get('basketExecTime'))),
                'allocated_funds' => $request->get('allocatedFunds')
            ]);

        /** Loop through all json records. These records are the content of the assets table from */
        $assets = $request->get('basketAssets');
        foreach ($assets as $asset ){
            DB::table('assets')
                ->where('id', $asset['id'])
                ->update([
                    'allocated_percent' => $asset['allocated_percent'],
                ]);
        }
    }
}
