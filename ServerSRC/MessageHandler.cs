using System.Xml;
using System.Collections.Generic;

namespace Server{

    // a superclass of objects responsible for handling client messages
    public abstract class MessageHandler{

        // begins accepting new messages asynchronously
        // will continue to accept new messages ad infinitum
        public void StartAsyncReceive(SocketManager socket){
            socket.AsynchReceiveXml(endAsyncReceiveXml);
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
                handleMessage(msg, from);
            }
        }

        // what to do when a socket dies
        protected abstract void handleSocketDeath(SocketManager socket);

        // gets the type of a message
        protected string messageTypeOf(XmlDocument msg){
            return msg.DocumentElement.Attributes["type"].Value;
        }

        public virtual void handleSocket(SocketManager socket){
            XmlDocument msg = socket.ReceiveXml();
            if(msg != null){
                handleMessage(msg, socket);
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

        public static XmlDocument NewEmptyMessage(string type){
            XmlDocument r = new XmlDocument();
            XmlElement e = r.CreateElement("file");
            r.AppendChild(e);
            XmlAttribute a = r.CreateAttribute("type");
            a.Value = type;
            e.SetAttributeNode(a);
            return r;
        }

    }

}