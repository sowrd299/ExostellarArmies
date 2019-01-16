using System.Xml;

namespace Server{

    // a superclass of objects responsible for handling client messages
    public abstract class MessageHandler{

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
                    from.Send("<file type='error'><msg>Unexpect message type durring gameplay: "+type+"</msg></file>");
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