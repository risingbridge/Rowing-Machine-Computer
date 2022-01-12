using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SocketServer
{
    internal class RecieveCommands : WebSocketBehavior
    {
        public bool SendAllData = false;
        protected override void OnMessage(MessageEventArgs e)
        {
            switch (e.Data)
            {
                case "Stop":
                    Console.WriteLine("Stopping!");
                    SendAllData = false;
                    break;
                case "Start":
                    Console.WriteLine("Starting");
                    SendAllData = true;
                    break;
                default:
                    Console.WriteLine("Recieved unknown command");
                    break;
            }
        }
    }
}
