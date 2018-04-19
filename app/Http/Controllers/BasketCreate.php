<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class basketCreate extends Controller
{

    public function index(Request $formRequest){

        DB::table('baskets')->insert(array(
            'execution_time' => date("Y-m-d G:i:s"),
            'name' => "New",
            'allocated_funds' => 0,
            'status' => "new",
            'is_deleted' => 0
        ));

        session()->flash('basket_created', 'New basket created!');

        return redirect('home'); // Go to url

    } // public function

} // Class
