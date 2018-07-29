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
            $table->increments('id');
            $table->dateTime('execution_time')->nullable();

            $table->string('elapsed_time')->nullable();
            $table->boolean('executed')->default(0);
            $table->string('name')->nullable();
            $table->integer('allocated_funds')->default(0);
            $table->string('status')->nullable();
            $table->boolean('is_deleted')->default(0); // Is message new flag
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
