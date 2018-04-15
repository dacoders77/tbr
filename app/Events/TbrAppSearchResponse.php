<?php

namespace App\Events;

use Illuminate\Broadcasting\Channel;
use Illuminate\Queue\SerializesModels;
use Illuminate\Broadcasting\PrivateChannel;
use Illuminate\Broadcasting\PresenceChannel;
use Illuminate\Foundation\Events\Dispatchable;
use Illuminate\Broadcasting\InteractsWithSockets;
use Illuminate\Contracts\Broadcasting\ShouldBroadcast;

class TbrAppSearchResponse implements ShouldBroadcast
{
    use Dispatchable, InteractsWithSockets, SerializesModels;
    public $update; // The public variable which can be read in the event listener as e._variable_name. e.update in js

    /**
     * Create a new event instance.
     *
     * @return void
     */
    public function __construct($z)
    {
        $this->update = $z; // Passing a parameter
    }

    /**
     * Get the channels the event should broadcast on.
     *
     * @return \Illuminate\Broadcasting\Channel|array
     */
    public function broadcastOn()
    {
        return new Channel('tbrChannel'); // Channel name
    }
}
