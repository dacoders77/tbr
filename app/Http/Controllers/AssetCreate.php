<?php

namespace App\Http\Controllers;
use Illuminate\Support\Facades\DB;

use Illuminate\Http\Request;

class AssetCreate extends Controller
{
    public function index(int $basketId, string $assetSymbol, string $assetExchange, string $assetCurrency, int $assetAllocatedPercent){


        DB::table('assets')->insert(array(
            'basket_id' => $basketId,
            'asset_symbol' => $assetSymbol,
            'asset_exchange' => $assetExchange,
            'asset_currency' => $assetCurrency,
            'asset_allocated_percent' => $assetAllocatedPercent

        ));

        session()->flash('asset_added', 'Symbol added!');

        //return redirect('basket/' . $basketId);

    } // public function
}
