<?php
/**
 * Created by PhpStorm.
 * User: slinger
 * Date: 5/13/2018
 * Time: 4:06 PM
 */

namespace App\Classes;
use Illuminate\Support\Facades\DB;

/**
 * This class updates an asset quote in DB. The quote is received from C# application.
 * Update method is called from ListenLocalSocket.php controller when a response message from C# app is received
 */

class Quote
{
    public function Update(string $symbol, float $price, int $basketNumber)
    {
        echo "\n*********Quote.Update!" . $symbol . " " . $basketNumber . " " . $price . "\n";
        // Update price
        // Where
        // basket num = $basketNumber
        // symbol = symbol

        
        DB::table('assets')
            ->where('basket_id', $basketNumber)
            ->where('symbol', $symbol)
            ->update([
                'price' => $price,
            ]);


        // Get all records from DB ->get

        // call event and pass table content as json
    }
}