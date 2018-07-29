<?php

namespace App\Http\Controllers;
use Illuminate\Support\Facades\DB;

use Illuminate\Http\Request;

class AssetCreate extends Controller
{
    public function index(int $basketId, string $assetSymbol, string $longName, string $assetExchange, string $assetCurrency, int $assetAllocatedPercent){

        // Add asset to DB
        DB::table('assets')->insert(array(
            'basket_id' => $basketId,
            'symbol' => $assetSymbol,
            'long_name' => $longName,
            'exchange' => $assetExchange,
            'currency' => $assetCurrency,
            'allocated_percent' => $assetAllocatedPercent,
            'info' => '{ "placeOrder": { "log": "", "status": "" }, "executeOrder": { "log": "", "status": "", "filled": "", "orderStatus": "", "avgFillprice": "" }, "fxQuoteRequest": { "log": "", "status": "" }, "volumeCalculate": { "log": "", "status": "" }, "stockQuoteRequest": { "log": "", "status": "" } }'

        ));

        // Get all assets from DB after the asset was added
        $basketContentObject =
            DB::table('assets')
                ->where('basket_id', $basketId) // $request->get('basketid')
                ->get();

        // DELETE ALL THIS. This event is received in CompForm.vue line 197
        //$messageArray = array('messageType' => "showBasketContent", "body" => $basketContentObject);
        // Trigger an event
        //event(new \App\Events\TbrAppSearchResponse(json_encode($messageArray)));

    }
}
