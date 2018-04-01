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
            $table->increments('asset_id');
            $table->integer('basket_id');
            $table->string('asset_symbol')->nullable();
            $table->string('asset_exchange')->nullable();
            $table->string('asset_currency')->nullable();
            $table->string('asset_allocated_percent')->nullable();
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
