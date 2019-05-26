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
    /*TODO: HANDLE THIS ERROR: (being delt with)
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

        private StringBuilder textBuffer; // The stub of the partially received message
		private object textBufferLock;

        private bool asyncReceiving; //whether or not we are currently receiving asynchronously
        private object asyncReceivingLock; //a lock for async receiving; doubles as the lock for HandleAsyncXmlMessage

        // store the method to be called to handle async Xml messages
        // stored here so can change it more dynamically as states change in other threads
        private Action<XmlDocument, SocketManager> HandleAsyncXmlMessage;

        // what to do when the socket disconnects
        private Action<SocketManager> HandleAsyncDeath;

        //takes the socket to manage
        //optionally takes the End of Message tag to look for
        public SocketManager(Socket socket, string eof = ""){
            this.socket = socket;
            this.eof = eof;
            alive = true; // assume the socket is alive until proven otherwise
			textBuffer = new StringBuilder();
			textBufferLock = new object();
            asyncReceiving = false;
            asyncReceivingLock = new object();
        }

        //read in data from the socket, if there is any
        public string Receive(int microseconds = 1000){
            List<Socket> l = new List<Socket>();
            l.Add(socket);
            Socket.Select(l, null, null, microseconds);
            if(l.Count > 0){
                byte[] bytes = new byte[bufferSize];
                int i = l[0].Receive(bytes);
                return ParseMessage(bytes, i);
            }
            //if nothing to read, or nothing to return, return null
            return null;
        }

        // parses data recieved from a socket
        private string ParseMessage(byte[] bytes, int i){
            string text = Encoding.UTF8.GetString(bytes);
			#if UNITY_EDITOR
				UnityEngine.Debug.Log($"Received text\n{text}");
			#endif
            //handle dead connection
            if(i == 0){
                Console.WriteLine("Socket Received Empty 'End of Connnection' Packet; Dying");
                // Console.WriteLine("Circumventing death for testing");
                //* TODO: TESTING do in fact need something here
                Die();
                //*/
            }
            //handle EOF
            // TODO: handle text buffer better without EOF
            if(eof == ""){ //if no EOF set, just spit out the message
                return text;
            }
            //wait for the end of the file
            textBuffer.Append(text);
			return ExtractMessage();
        }

		private string ExtractMessage()
		{
			lock (textBufferLock)
			{
				string text = textBuffer.ToString();
				int eofIndex = text.IndexOf(eof);
				if (eofIndex >= 0)
				{ //if we have an eof, return the first file
					string r = text.Substring(0, eofIndex + eof.Length); //split from the end of the EOF
					textBuffer.Remove(0, eofIndex + eof.Length);
					return r;
				}
				return null;
			}
		}

        //since every file is going to be an XML doc anyways,
        //might as well have a method to just convert it automatically
        //TODO: handle recieving things that aren't XML w/o crashing
        public XmlDocument ReceiveXml(int microseconds = 1000){
            string text = Receive(microseconds);
            if(text != null){
                Console.WriteLine("Parsing XML: {0}", text); //TESTING
                return ParseXml(text);
            }
            return null;
        }

        // recieve a message from the socket asynchronously
        // NOTE: calling a second time will override previous calls
        // TODO: there seem to be some really ugly race-cases with managing only one asyncReceiving at a time
        public void AsyncReceiveXml(Action<XmlDocument, SocketManager> handler, Action<SocketManager> deathHandler, int bufferLen = 256){
            lock(asyncReceivingLock) {
				// The message may have already been received in an earlier call, because TCP sometimes bundles the messages together.
				string existingMessage = ExtractMessage();
				if (existingMessage != null) {
					handler(ParseXml(existingMessage), this);
					return;
				}

                HandleAsyncXmlMessage = handler;
                HandleAsyncDeath = deathHandler;
                if(!asyncReceiving){
                    byte[] buffer = new byte[bufferLen];
                    socket.BeginReceive(
						buffer, 0, buffer.Length, 0,
						new AsyncCallback(EndAsyncReceiveXml),
						new AsyncState<XmlDocument>{buffer = buffer, handler = handler}
					);
                    asyncReceiving = true;
                }
                /* TESTING
                else{
                    Console.WriteLine("Changing Ansync Callback For Socket!");
                }
                // */
            }
        }

        private void EndAsyncReceiveXml(IAsyncResult ar){
            Console.WriteLine("Data incoming...");
            lock(asyncReceivingLock){
                // no long have an outstanding async receive
                asyncReceiving = false;
                // reading stuff
                int i;
                try{ // this may error; not really sure why but eh, this works I think
                    i = socket.EndReceive(ar);
                }catch(SocketException e){
                    Console.WriteLine("Socket died from an error; Alive: {0}; Error: ", alive, e);
                    Die();
                    HandleAsyncDeath(this);
                    return;
                }
                AsyncState<XmlDocument> state = ar.AsyncState as AsyncState<XmlDocument>;
                string text = ParseMessage(state.buffer, i);
                // have a full message, deal with it
                if(text != null){
                    XmlDocument msg = ParseXml(text);
                    HandleAsyncXmlMessage(msg, this);
                // if do not have a full message...
                }else{ 
                    if(!Alive){ // ...may have died ...
                        HandleAsyncDeath(this);
                    }else{ // ...may need to continue reading
                        AsyncReceiveXml(HandleAsyncXmlMessage, HandleAsyncDeath);
                    }
                }
            }
        }

        // returns an XML document from the given recieved message
        private XmlDocument ParseXml(string text){
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
            // textBuffer += Receive(); // get and buffer any text that may be clogging up the system
            socket.Send(data);
        }

        public void SendXml(XmlDocument xml){
            Send(xml.InnerXml);
        }

        // returns whether or not the socket is still connected 
        private bool Connected(){
            bool poll = socket.Poll(1000, SelectMode.SelectRead);
            bool data = (socket.Available == 0);
            return poll && data;
        }

        // to be called once the socket disconnects
        private void Die(){
            Console.WriteLine("Socket {0} Dying", socket.RemoteEndPoint); // TESTING
            alive = false;
            socket.Close();
        }

        // make sure everything gets cleaned up
        ~SocketManager(){
            // some of the most poetic code I have ever written
            if(alive){
                Console.WriteLine("Socket Manager Deconstructing; Dying");
                Die();
            }
        }
        private class AsyncState<T>{
            public Action<T, SocketManager> handler; //DEPRICATED
            public byte[] buffer;
        }

    }

}