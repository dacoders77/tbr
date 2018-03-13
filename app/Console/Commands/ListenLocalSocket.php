<?php

namespace App\Console\Commands;

use Illuminate\Console\Command;
use App\Events\eventTrigger; // Linked the events
use Illuminate\Support\Facades\DB;

class ListenLocalSocket extends Command
{
    /**
     * The name and signature of the console command.
     *
     * @var string
     */
    protected $signature = 'ratchet:socket';

    /**
     * The console command description.
     *
     * @var string
     */
    protected $description = 'Listens to local c# web socket server at 0.0.0.0:8181';

    /**
     * Create a new command instance.
     *
     * @return void
     */
    public function __construct()
    {
        parent::__construct();
    }


    /**
     * Execute the console command.
     *
     * @return mixed
     */
    public function handle()
    {
        echo "*****Ratchet websocket c# local listener started!*****\n";

        // The code from: https://github.com/ratchetphp/Pawl

        $loop = \React\EventLoop\Factory::create();
        $reactConnector = new \React\Socket\Connector($loop, [
            'dns' => '0.0.0.0',
            'timeout' => 10
        ]);


        $counter = 0;
        // timer
        // http://sergeyzhuk.me/2017/06/06/phpreact-event-loop/
        $loop->addPeriodicTimer(0.5, function() use(&$counter) { // addPeriodicTimer($interval, callable $callback)
            $counter++;
            //echo "$counter\n";
            // read messages from db
            $z = DB::table('socket_que')->select('*')
                ->where('is_new', 0)
                ->get();


            foreach ($z as $record){
                //print_r($z);
                echo "xx: " . $record->text_message . "         id: " . $record->id . "\n";

                //$conn->send('hello world!');

                // flag = new
                DB::table('socket_que')
                    ->where('id', $record->id)
                    ->update([
                        'is_new' => 0,
                    ]);
            }

        });




        $connector = new \Ratchet\Client\Connector($loop, $reactConnector);

        $connector('ws://localhost:8181', [], ['Origin' => '127.0.0.1:7451'])
            ->then( function(\Ratchet\Client\WebSocket $conn) {

                $conn->on('message', function(\Ratchet\RFC6455\Messaging\MessageInterface $msg) use ($conn) {
                    //RatchetWebSocket::out($msg); // Call the function when the event is received
                    echo $msg . "\n";
                });


                $conn->on('close', function($code = null, $reason = null) {
                    echo "Connection closed ({$code} - {$reason})\n";
                });

                $conn->send(['event' => 'ping']);


                $this->conn->send('hello world!');


            },
                function(\Exception $e) use ($loop) {
                echo "Could not connect: {$e->getMessage()}\n";

                $loop->stop();
            });


        $loop->run();
    }
}
