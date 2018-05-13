<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

/* This class records messages to DB, socket_que table. Then these messages are read from ListenLocalSocket.php controller
 * and transmitted to the websocket channel. The this messages are picked up from C# application
 */

class AddMessageToSocketQue extends Controller
{
    public function index(string $requestType, string $param)
    {
        DB::table('socket_que')->insert(array(
            'date' => date("Y-m-d H:i:s"),
            'is_new' => 1,
            'message_type' => $requestType,
            'text_message' => $param
            //'json_message' => json_encode(['a1key' => 'some_json', 'b' => 2, 'c' => 3])
        ));
    }
}
