using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System;
using System.Linq;

namespace Server{

    //manages an individual connection with a client
    //TODO: manage socket closing and cleanup after client disconnects
    //TODO: (maybe) queue and store messages better? so can be read multiple times/by multiple sources?
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

        //since every file is going to be an XML doc anyways,
        //might as well have a method to just convert it automatically
        //TODO: handle recieving things that aren't XML w/o crashing
        public XmlDocument ReceiveXml(int microseconds = 1000){
            string text = Recieve(microseconds);
            if(text != null){
                Console.WriteLine("Parsing XML: {0}", text); //TESTING
                //filter only valid XML characters; querry from https://stackoverflow.com/questions/8331119/escape-invalid-xml-characters-in-c-sharp
                var validXmlText = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
                //get and return the XML document
                XmlDocument r = new XmlDocument();
                r.LoadXml(new string(validXmlText));
                return r;
            }
            return null;
        }

        //send a given message to the client
        public void Send(string msg){
            byte[] data = Encoding.UTF8.GetBytes(msg);
            socket.Send(data);
        }


    }

}