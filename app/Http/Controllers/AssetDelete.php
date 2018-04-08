<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class AssetDelete extends Controller
{
    public function index(int $basketId, int $assetId) {

        DB::table('assets')
            ->where('asset_id', $assetId)
            ->where('basket_id', $basketId)
            ->delete();

        session()->flash('asset_deleted', 'Symbol deleted!');

        return redirect('basket/' . $basketId); // Go to url

    }
}
