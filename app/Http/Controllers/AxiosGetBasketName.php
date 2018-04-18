<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class AxiosGetBasketName extends Controller
{
    public function index(Request $request){ //

        //echo "Hello from Axios get basket name controller";
        //dump($request->all());

        $basketName =
            DB::table('baskets')
                ->where('basket_id', $request->get('basketId'))
                ->value('basket_name');

        $basketExecTime =
            DB::table('baskets')
                ->where('basket_id', $request->get('basketId'))
                ->value('basket_execution_time');

        $basketContentobject =
            DB::table('assets')
                ->where('basket_id', $request->get('basketId')) // $request->get('basketid')
                ->get();

        $basketContentJson = json_encode($basketContentobject);

        // Make associative array and return it to VUE js component where these values will be outputed to the form
        $basketProps = array(
            'basketName' => $basketName,
            'basketExecTime' => date("Y-m-d\TH:i", strtotime($basketExecTime)),
            'basketAssets' => $basketContentJson
        );

        //$basketExecutionTime = date("Y-m-d\TH:i", strtotime($basketExecTime));

        return $basketProps;

    }
}
