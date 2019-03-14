using System.Xml;
using System.Collections.Generic;
using System;

namespace SFB.Net{

    // a superclass of objects responsible for handling client messages
    public abstract class MessageHandler{

        // begins accepting new messages asynchronously
        // will continue to accept new messages ad infinitum
        public void StartAsyncReceive(SocketManager socket){
            socket.AsynchReceiveXml(endAsyncReceiveXml, handleSocketDeath);
        }

        protected virtual void endAsyncReceiveXml(XmlDocument msg, SocketManager from){
            // check if the connection is dead; if it is, do something about it
            // specifically doing socket and not from here, because "from" isn't necessarily
            // ... the player's main socket/responsibility
            if(!from.Alive){
                handleSocketDeath(from);
            }else{
                // resume receiving
                // we do this first so that if handle message changes the message handler
                // ... this one won't imediately grab it back
                // TODO: this is a dumb order of ops to care about what is my archetecture
                StartAsyncReceive(from);
                // handle the message
				try{
                	handleMessage(msg, from);
				}catch(Exception e){
					handleError(e, from);
                    Console.WriteLine("...was handling message: {0}", msg.OuterXml);
				}
            }
        }

        // what to do when a socket dies
        protected abstract void handleSocketDeath(SocketManager socket);

        // gets the type of a message
        protected string messageTypeOf(XmlDocument msg){
            return msg.DocumentElement.Attributes["type"].Value;
        }

        // synchronously read and handle date from a socket
        public virtual void handleSocket(SocketManager socket){
            XmlDocument msg = socket.ReceiveXml();
            if(msg != null){
                // try... catch... to avoid malicious actto
                try{
                    handleMessage(msg, socket);
                }catch(Exception e){
					handleError(e, socket);
                }
            }else if(!socket.Alive){
                handleSocketDeath(socket);
            }
        }

        // handle a recieved message
        // returns a message to be sent
        // TODO: add consistency about if it returns/takes strings or XML
        // TODO: ...eh less imporant with handleSocket being the main interface here
        public virtual void handleMessage(XmlDocument msg, SocketManager from){
            string type = messageTypeOf(msg);
            switch(type){
                default:
                    from.Send("<file type='error'><msg>Unexpect message type: "+type+"</msg></file>");
                    break;
            }
        }

        private void handleError(Exception e, SocketManager socket){
            socket.Send("<file type='error'><msg>Server handling message raised exception: "+e.Message+"</msg></file");
            Console.WriteLine("Recieced error: {0}\n{1}\n...while handling message. Sent error back to sender.", e.Message, e.StackTrace);
        }

        // returns an empty XML message in propper format, with type set to the given type
        public static XmlDocument NewEmptyMessage(string type){
            // create the document itself and the encompassing file tag
            XmlDocument r = new XmlDocument();
            XmlElement e = r.CreateElement("file");
            r.AppendChild(e);
            // create and set the type attribute
            XmlAttribute a = r.CreateAttribute("type");
            a.Value = type;
            e.SetAttributeNode(a);
            // need to add in this comment so it formats the end of the massage as
            //      <file...></file> and not <file../>
            // TODO: handle detecting the ends of XML documents better
            XmlComment c = r.CreateComment(type);
            e.AppendChild(c);
            return r;
        }

    }

}