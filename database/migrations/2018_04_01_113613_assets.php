<?php

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class Assets extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {

        Schema::create('assets', function (Blueprint $table) {
            $table->increments('id');
            $table->integer('basket_id');
            $table->string('symbol')->nullable();
            $table->string('long_name')->nullable();
            $table->string('exchange')->nullable();
            $table->string('currency')->nullable();
            $table->string('allocated_percent')->nullable();
            $table->double('stock_quote')->nullable();
            $table->double('fx_quote')->nullable();
            $table->double('fx_quote_processed')->nullable();
            $table->double('volume')->nullable();
            $table->bigInteger('request_id')->nullable();
            $table->double('order_placed')->nullable();
            $table->json('info')->nullable();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('assets');
    }
}
