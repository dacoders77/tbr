using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Schema;

namespace Fleck.Samples.ConsoleApp
{
    class Server
    {
 

        static void Main()
        {


            FleckLog.Level = LogLevel.Debug;
            var allSockets = new List<IWebSocketConnection>();
            var server = new WebSocketServer("ws://0.0.0.0:8181");
			server.SupportedSubProtocols = new[] { "superchat", "chat" };

			server.Start(socket =>
                {
                    socket.OnOpen = () =>
                        {
                            Console.WriteLine("Open!");
                            allSockets.Add(socket);
                        };
                    socket.OnClose = () =>
                        {
                            Console.WriteLine("Close!");
                            allSockets.Remove(socket);
                        };
                    socket.OnMessage = message =>
                        {
                            Console.WriteLine("----------------------: " + message);

							// Send message back to websocket 
                            //allSockets.ToList().ForEach(s => s.Send("Echo. Hellow from c#: " + message));
                        };
                });


			var input = Console.ReadLine();
            while (input != "exit")
            {
                foreach (var socket in allSockets.ToList())
                {
                    socket.Send(input);
					//Console.WriteLine("negot protocol prot: " + socket.ConnectionInfo.NegotiatedSubProtocol + " " + socket.ConnectionInfo.SubProtocol);
                }
                input = Console.ReadLine();
            }
        }
    }
}
