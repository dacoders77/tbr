<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class AddMessageToSocketQue extends Controller
{
    public function index()
    {
        echo "hello all!";

        DB::table('socket_que')->insert(array(
            'date' => date("Y-m-d H:i:s"),
            'is_new' => 1,
            'text_message' => '***********YEAH BABY!!!!',
            'json_message' => json_encode(['a1key' => 'znackenie', 'b' => 2, 'c' => 3, 'd' => 4, 'e' => 5])
        ));

        //echo json_encode(['a' => 1, 'b' => 2, 'c' => 3, 'd' => 4, 'e' => 5]);
    }
}
