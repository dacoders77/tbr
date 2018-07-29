<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;

class GetServerTime extends Controller
{
    public function __invoke(){

        return(date('d-m-Y H:i:s'));
    }
}
