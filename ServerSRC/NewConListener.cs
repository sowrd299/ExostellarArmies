
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Server{

    //a socket-rapper that listens for and accepts new client connections
    public class NewConListener{
        private IPAddress addr;
        private int port;
        private Socket socket; //a socket that listens for new connections to the server

        public NewConListener(IPAddress addr, int port){
            self.addr = addr;
            self.port = port;
            setup();
        }

        //setup the socket
        private void setup(){
            socket = newSocket(AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(addr, port));
            socket.Listen(10);
        }
         
        //accept connections
        //microseconds should be small: when expecting to get lots of new cons, can afford to loop back regularly; when not 
        public Socket Accept(int microseconds = 50){
            List<Socket> l = new List<Socket>(new {socket});
            Socket.Select(l, null, null, microseconds);
            //if have a new connection, accept and return it
            //may be able to accept multiple, but only accept one so can return to gameplay quickly
            if(l.Count > 0){
                return l[0].Accept();
            }
            //if no connections, return null
            return null;
        }

    }

}