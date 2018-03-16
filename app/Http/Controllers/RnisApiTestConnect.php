<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use GuzzleHttp\Client; // Guzzle is used to send http headers http://docs.guzzlephp.org/en/stable/

class RnisApiTestConnect extends Controller
{

    public function index(){

        $client = new Client(); // Create guzzle http client

        $response = $client->request('POST', 'https://rnis.mosreg.ru/ajax/request', [
            'headers' => [
                'Content-Type'     => 'application/x-www-form-urlencoded',
                'X-Requested-With' => 'XMLHttpRequest',
                'Subject'          => 'com.rnis.auth.action.login',
            ],
            'json' => [
                'headers' => [
                    'version'   => '1.0.0',
                    'requester' => 'web',
                    'timestamp' => '',
                ],
                'payload' => [
                    'login'    => 'admin',
                    'password' => 't1rnis2018',
                ],
            ],
        ]);

        echo "json_decode(\$response->getBody()->getContents()): ";
        dump(json_decode($response->getBody()->getContents()));

        echo "\$response->getBody(): ";
        dump ($response->getBody()); // Get the body out of the request
        echo "\$response->getReasonPhrase(): ";
        dump($response->getReasonPhrase());


        /*
        $body = $response->getBody(); // Get the body out of the request
        $json = json_decode($body, true); // Decode JSON. Associative array will be outputed

        if ($response->getStatusCode() == 200) // Request successful
        {
            // Add candles to DB
            foreach (array_reverse($json) as $z) { // The first element in array is the youngest - first from the left on the chart. Go through the array backwards. This is the order how points will be read from DB and outputed to the chart

            }

        } // if 200

        else // Request is not successful. Error code is not 200

        {
            //echo "<script>alert('Request error: too many requests!' )</script>"; // $response->getReasonPhrase()
        }
        */

    }
}
