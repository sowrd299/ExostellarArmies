using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace Server{

    //manages an individual connection with a client
    class SocketManager{

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
        public SocketManager(Socket socket, string eof = ""){
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
                //handle EOF
                if(eof == "" && text.Length > 0){ //if no EOF set, just spit out the message
                    return text;
                }
                //wait for the end of the file
                textBuffer += text;
                int eofIndex = textBuffer.IndexOf(eof); //search for EOF in the entire cached string
                                                //in case got a second eof in an early read that was never processes
                                                //also helps if EOF gets broken over the divide
                                                //scanning for all and breaking on arival has more overhead time
                if(eofIndex >= 0){ //if we have an eof, return the first file
                    string r = textBuffer.Substring(0, eofIndex + eof.Length); //split from the end of the EOF
                    textBuffer = textBuffer.Substring(eofIndex + eof.Length);
                    return r;
                }
            }
            //if nothing to read, or nothing to return, return null
            return null;
        }

        public void Send(string msg){
            byte[] data = Encoding.UTF8.GetBytes(msg);
            socket.Send(data);
        }


    }

}