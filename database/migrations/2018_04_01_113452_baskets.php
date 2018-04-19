<?php

use Illuminate\Support\Facades\Schema;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Database\Migrations\Migration;

class Baskets extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {

        Schema::create('baskets', function (Blueprint $table) {
            $table->increments('basket_id');
            $table->dateTime('execution_time')->nullable();
            $table->string('name')->nullable();
            $table->integer('allocated_funds')->nullable();
            $table->string('status')->nullable();
            $table->boolean('is_deleted'); // Is message new flag

        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('baskets');
    }
}
