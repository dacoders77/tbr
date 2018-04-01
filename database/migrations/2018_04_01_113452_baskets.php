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
            $table->dateTime('basket_execution_time')->nullable();
            $table->string('basket_name')->nullable();
            $table->integer('basket_allocated_funds')->nullable();
            $table->string('basket_status')->nullable();
            $table->boolean('basket_is_deleted'); // Is message new flag

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
