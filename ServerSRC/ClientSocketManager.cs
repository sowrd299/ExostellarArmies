using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Server{

    //manages an individual connection with a client
    class ClientSocketManager{

        private int bufferSize = 1024;

        private Socket socket;

        private string eof;
        public string EOF{
            get{
                return eof;
            }
        }

        private string textBuffer; //the stub of the paritally recieved message

        //takes the socket to manage
        //optionally takes the End of Message tag to look for
        public ClientSocketManager(Socket socket, string eof = ""){
            this.socket = socket;
            this.eof = eof;
        }

        //read in data from the socket, if there is any
        public string Recieve(int microseconds = 1000){
            List<Socket> l = new List<Socket>();
            l.Add(socket);
            Socket.Select(l, null, null, microseconds);
            if(l.Count > 0){
                byte[] bytes = new byte[bufferSize];
                l[0].Receive(bytes);
                string text = Encoding.UTF8.GetString(bytes);
                //TODO: handle message reassembly
                return text;
            }
            return null;
        }

    }

}