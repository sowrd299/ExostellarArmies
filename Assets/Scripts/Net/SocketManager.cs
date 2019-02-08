using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System;
using System.Linq;

namespace SFB.Net{

    //manages an individual connection with a client
    //TODO: (maybe) queue and store messages better? so can be read multiple times/by multiple sources?
    //TODO: add a child of this class that associates verified account data
    //TODO: handle resuming sessions
    //TODO: allow multiple handlers to receive at once? Seems like a powerful feature to have
    /*TODO: HANDLE THIS ERROR:
    Unhandled Exception:
        System.Net.Sockets.SocketException (0x80004005): Connection reset by peer
        at System.Net.Sockets.Socket.EndReceive (System.IAsyncResult asyncResult) [0x00012] in <5bf358e735be486487282a37cb3bce80>:0 
        at SFB.Net.SocketManager.endAsynchReceiveXml (System.IAsyncResult ar) [0x0001b] in <4fec1bc975604ce0aca66f4d8c8202c0>:0 
        at System.Net.Sockets.SocketAsyncResult+<>c.<Complete>b__27_0 (System.Object state) [0x0000b] in <5bf358e735be486487282a37cb3bce80>:0 
        at System.Threading.QueueUserWorkItemCallback.System.Threading.IThreadPoolWorkItem.ExecuteWorkItem () <0x7f251d219ba0 + 0x0003a> in <04750267503a43e5929c1d1ba19daf3e>:0 
        at System.Threading.ThreadPoolWorkQueue.Dispatch () [0x00074] in <04750267503a43e5929c1d1ba19daf3e>:0 
        at System.Threading._ThreadPoolWaitCallback.PerformWaitCallback () <0x7f251d219a50 + 0x00005> in <04750267503a43e5929c1d1ba19daf3e>:0 
     */
    public class SocketManager{

        private int bufferSize = 1024;

        private Socket socket;

        private string eof;
        public string EOF{
            get{
                return eof;
            }
        }

        // if the socket is connected to the other end
        // may stop reporting alive while still have buffered messages
        private bool alive;
        public bool Alive{
            get{
                return alive; 
            }
        }

        private string textBuffer; //the stub of the paritally recieved message

        private bool asynchReceiving; //whether or not we are currently receiving asynchronously
        private object asynchReceivingLock; //a lock for asych receiving; doubles as the lock for handleAsynchXmlMessage

        // store the method to be called to handle asynch Xml messages
        // stored here so can change it more dynamically as states change in other threads
        private HandleMessage<XmlDocument> handleAsynchXmlMessage;

        // what to do when the socket disconnects
        private HandleDeath handleAsyncDeath;

        //takes the socket to manage
        //optionally takes the End of Message tag to look for
        public SocketManager(Socket socket, string eof = ""){
            this.socket = socket;
            this.eof = eof;
            alive = true; // assume the socket is allive until proven otherwise
            asynchReceiving = false;
            asynchReceivingLock = new object();
        }

        //read in data from the socket, if there is any
        public string Receive(int microseconds = 1000){
            List<Socket> l = new List<Socket>();
            l.Add(socket);
            Socket.Select(l, null, null, microseconds);
            if(l.Count > 0){
                byte[] bytes = new byte[bufferSize];
                int i = l[0].Receive(bytes);
                return parseMessage(bytes, i);
            }
            //if nothing to read, or nothing to return, return null
            return null;
        }

        // parses data recieved from a socket
        private string parseMessage(byte[] bytes, int i){
            string text = Encoding.UTF8.GetString(bytes);
            //handle dead connection
            if(i == 0){
                die();
            }
            //handle EOF
            if(eof == ""){ //if no EOF set, just spit out the message
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
            return null;
        }

        //since every file is going to be an XML doc anyways,
        //might as well have a method to just convert it automatically
        //TODO: handle recieving things that aren't XML w/o crashing
        public XmlDocument ReceiveXml(int microseconds = 1000){
            string text = Receive(microseconds);
            if(text != null){
                Console.WriteLine("Parsing XML: {0}", text); //TESTING
                return parseXml(text);
            }
            return null;
        }

        // recieve a message from the socket asynchronously
        // NOTE: calling a second time will override previous calls
        // TODO: there seem to be some really ugly race-cases with managing only one asyncReceiving at a time
        public void AsynchReceiveXml(HandleMessage<XmlDocument> handler, HandleDeath deathHandler, int bufferLen = 256){
            lock(asynchReceivingLock){
                handleAsynchXmlMessage = handler;
                handleAsyncDeath = deathHandler;
                if(!asynchReceiving){
                    byte[] buffer = new byte[bufferLen];
                    socket.BeginReceive(buffer, 0, buffer.Length, 0,
                            new AsyncCallback(endAsynchReceiveXml),
                            new AsyncState<XmlDocument>{buffer = buffer, handler = handler});
                    asynchReceiving = true;
                }
                /* TESTING
                else{
                    Console.WriteLine("Changing Ansync Callback For Socket!");
                }
                // */
            }
        }

        private void endAsynchReceiveXml(IAsyncResult ar){
            lock(asynchReceivingLock){
                // no long have an outstanding async receive
                asynchReceiving = false;
                // reading stuff
                int i = socket.EndReceive(ar);
                AsyncState<XmlDocument> state = (AsyncState<XmlDocument>)ar.AsyncState;
                string text = parseMessage(state.buffer, i);
                // have a full message, deal with it
                if(text != null){
                    XmlDocument msg = parseXml(text);
                    handleAsynchXmlMessage(msg, this);
                // if do not have a full message...
                }else{ 
                    if(!Alive){ // ...may have died ...
                        handleAsyncDeath(this);
                    }else{ // ...may need to continue reading
                        AsynchReceiveXml(handleAsynchXmlMessage, handleAsyncDeath);
                    }
                }
            }
        }

        // returns an XML document from the given recieved message
        private XmlDocument parseXml(string text){
            //filter only valid XML characters; querry from https://stackoverflow.com/questions/8331119/escape-invalid-xml-characters-in-c-sharp
            var validXmlText = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            //get and return the XML document
            XmlDocument r = new XmlDocument();
            r.LoadXml(new string(validXmlText));
            return r;
        }

        //send a given message to the client
        public void Send(string msg){
            byte[] data = Encoding.UTF8.GetBytes(msg);
            socket.Send(data);
        }

        public void SendXml(XmlDocument xml){
            Send(xml.InnerXml);
        }

        // to be called once the socket disconnects
        private void die(){
            alive = false;
            socket.Close();
        }

        // make sure everything get's cleaned up
        ~SocketManager(){
            // some of the most poetic code I have ever written
            if(alive){
                die();
            }
        }

        public delegate void HandleDeath(SocketManager from);
        public delegate void HandleMessage<T>(T msg, SocketManager from);
        private class AsyncState<T>{
            public HandleMessage<T> handler; //DEPRICATED
            public byte[] buffer;
        }

    }

}