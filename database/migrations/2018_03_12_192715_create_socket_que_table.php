<?php

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class CreateSocketQueTable extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('socket_que', function (Blueprint $table) {
            $table->increments('id');
            $table->dateTime('date')->nullable();
            $table->boolean('is_new'); // Is message new flag
            $table->string('text_message')->nullable();
            $table->json('json_message')->nullable();

        });

        $z = json_encode([
            //'event' => 'ping', // 'event' => 'ping'
            'event' => 'subscribe',
            'channel' => 'trades',
            'symbol' => 'tETHUSD' // tBTCUSD
        ]);

        DB::table('socket_que')->insert(array(
            'date' => date("Y-m-d H:i:s"),
            'is_new' => 1,
            'text_message' => 'hello',
            'json_message' => $z,
        ));

        sleep(5);

        DB::table('socket_que')->insert(array(
            'date' => date("Y-m-d H:i:s"),
            'is_new' => 1,
            'text_message' => 'this is a second record',
            'json_message' => json_encode(['event' => 'subscribe'])
        ));

    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('socket_que');
    }
}
