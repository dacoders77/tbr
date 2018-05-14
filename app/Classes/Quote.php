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
        echo "\n********* Quote.Update method!" . $symbol . " " . $basketNumber . " " . $price . "\n";

        DB::table('assets')
            ->where('basket_id', $basketNumber)
            ->where('symbol', $symbol)
            ->update([
                'price' => $price,
            ]);

        // Get all assets from DB after the asset was added
        $basketContentObject =
            DB::table('assets')
                ->where('basket_id', $basketNumber)
                ->get();

        $messageArray = array('messageType' => "showBasketContent", "body" => $basketContentObject);


        // Trigger an event
        event(new \App\Events\TbrAppSearchResponse(json_encode($messageArray))); // showBasketContent

    }
}