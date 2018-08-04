<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

/**
 * Class GetReport
 * Gets a reort out of the DB.
 * When an asset is executed - the resulted information is written into a json field 'info'.
 * report.blade.php has Report.Vue component which makes a post request and call this controller.
 * @package App\Http\Controllers
 */


class GetReport extends Controller
{
    public function index(Request $request)
    {
        $reportContentObject =
            DB::table('assets')
                ->where('basket_id', $request->get('basketId'))
                ->get();

        return($reportContentObject);

    }
}
