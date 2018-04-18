<?php

namespace App\Http\Controllers;
use Illuminate\Support\Facades\DB;

use Illuminate\Http\Request;

class AssetCreate extends Controller
{
    public function index(int $basketId, string $assetSymbol, string $assetExchange, string $assetCurrency, int $assetAllocatedPercent){

        // Add asset to DB
        DB::table('assets')->insert(array(
            'basket_id' => $basketId,
            'symbol' => $assetSymbol,
            'exchange' => $assetExchange,
            'currency' => $assetCurrency,
            'allocated_percent' => $assetAllocatedPercent

        ));

        // Get all assets from DB after the asset was deleted
        $basketContentObject =
            DB::table('assets')
                ->where('basket_id', $basketId) // $request->get('basketid')
                ->get();

        $assets = json_encode($basketContentObject);

        //session()->flash('asset_deleted', 'Symbol deleted!');
        //return redirect('basket/' . $basketId); // Go to url

        // Throw an event
        event(new \App\Events\TbrAppSearchResponse(['eventType' => 'showBasketContent', $assets]));

        //session()->flash('asset_added', 'Symbol added!');

    } // public function
}
