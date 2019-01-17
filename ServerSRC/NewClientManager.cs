using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace SFB.Net.Server{

    //a socket-rapper that listens for and accepts new client connections
    public class NewClientManager{
        private IPAddress addr;
        private int port;
        private Socket socket; //a socket that listens for new connections to the server

        public NewClientManager(IPAddress addr, int port){
            //this.addr = addr;
            this.addr = addr;
            this.port = port;
            setup();
        }

        //setup the socket
        private void setup(){
            socket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(addr, port));
            socket.Listen(10);
            Console.WriteLine("Opening connect socket on: {0}:{1}",addr.ToString(), port.ToString());
        }
         
        //accept connections
        //microseconds should be small: when expecting to get lots of new cons, can afford to loop back regularly; when not 
        public Socket Accept(int microseconds = 50){
            List<Socket> l = new List<Socket>();
            l.Add(socket);
            Socket.Select(l, null, null, microseconds);
            //if have a new connection, accept and return it
            //may be able to accept multiple, but only accept one so can return to gameplay quickly
            if(l.Count > 0){
                return l[0].Accept();
            }
            //if no connections, return null
            return null;
        }

        // accept connections asychronously
        public void AsynchAccept(HandleNewClient handler){
            socket.BeginAccept(new AsyncCallback(endAsyncAccept), handler);
        }

        // handles a successful async accept
        private void endAsyncAccept(IAsyncResult ar){
            Socket newClient = socket.EndAccept(ar);
            ((HandleNewClient)ar.AsyncState)(newClient);
        }

        public delegate void HandleNewClient(Socket s);

    }

}