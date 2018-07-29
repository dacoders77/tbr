<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use App\Events\eventTrigger;

class AssetDelete extends Controller
{
    public function index(int $basketId, int $assetId) {

        // Delete record
        DB::table('assets')
            ->where('id', $assetId)
            ->where('basket_id', $basketId)
            ->delete();

        // Get all assets from DB after the asset was deleted
        $basketContentObject =
            DB::table('assets')
                ->where('basket_id', $basketId) // $request->get('basketid')
                ->get();

        return($basketContentObject);

        //$messageArray = array('messageType' => "showBasketContent", "body" => $basketContentObject);
        //event(new \App\Events\TbrAppSearchResponse(json_encode($messageArray))); // showBasketContent

    }
}
