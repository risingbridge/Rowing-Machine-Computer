using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp.Server;
using WebSocketSharp;

namespace SocketServer
{
    public class SendData : WebSocketBehavior
    {
        protected override void OnOpen()
        {
            while (true)
            {
                Send(DateTime.Now.ToString());
                Thread.Sleep(100);
            }
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.WriteLine($"Recieved message from client");
            Console.WriteLine(e.Data);
            Send(e.Data);
        }
    }
}
