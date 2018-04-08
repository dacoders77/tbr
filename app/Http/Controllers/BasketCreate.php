<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class basketCreate extends Controller
{

    public function index(Request $formRequest){

        DB::table('baskets')->insert(array(
            'basket_execution_time' => date("Y-m-d G:i:s"),
            'basket_name' => "New",
            'basket_allocated_funds' => 0,
            'basket_status' => "new",
            'basket_is_deleted' => 0
        ));

        session()->flash('basket_created', 'New basket created!');

        return redirect('home'); // Go to url

    } // public function

} // Class
