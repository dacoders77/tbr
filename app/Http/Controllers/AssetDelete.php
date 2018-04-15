<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;
use App\Events\eventTrigger;

class AssetDelete extends Controller
{
    public function index(int $basketId, int $assetId) {

        // Delet record
        DB::table('assets')
            ->where('asset_id', $assetId)
            ->where('basket_id', $basketId)
            ->delete();

        // Get all assets from DB after the asset was deleted
        $basketContentObject =
            DB::table('assets')
                ->where('basket_id', $basketId) // $request->get('basketid')
                ->get();

        $basketContentJson = json_encode($basketContentObject);

        //session()->flash('asset_deleted', 'Symbol deleted!');
        //return redirect('basket/' . $basketId); // Go to url

        // Throw an event
        event(new \App\Events\TbrAppSearchResponse(json_encode(['eventType' => 'showBasketContent', $basketContentObject])));

        //return "AssetDelete.php: asset deleted";

    }
}
