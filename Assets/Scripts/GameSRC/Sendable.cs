using System;
using System.Xml;
using System.Reflection;

namespace SFB.Game.Management{

    // an error for when you try to call FromXml with mismatched arguments
    public class IllegalSendableFactoryCallException : Exception{
        public IllegalSendableFactoryCallException(){}
        public IllegalSendableFactoryCallException(string message) : base(message){}
        public IllegalSendableFactoryCallException(string message, Exception inner): base(message, inner){}
    }

    // a superclass for objects that can be sent across the network
    public abstract class Sendable{

        protected static string XmlNodeName{
            get{ return "sendable"; }
        }

        public virtual XmlElement ToXml(XmlDocument doc){
            XmlElement e = doc.CreateElement(XmlNodeName);
            // attach the type
            XmlAttribute typeAttr = doc.CreateAttribute("type");
            typeAttr.Value = this.GetType().ToString();
            e.SetAttributeNode(typeAttr);
            return e;
        }

        // a class to contain static factory methods
        // having this be it's own less class allows us to use template to make the
        //      factory methods more generic
        protected static class SendableFactory<T> where T : Sendable{
            // this will return a new instance of the Delta type specified in the XML
            // and return it
            public static T FromXml(XmlElement from, object[] constructionArgs = null, Type[] argTypes = null){
                // get the type
                string t = from.Attributes["type"].Value;
                Type type = Type.GetType(t);
                // get and validate the constructor arguments and their types
                constructionArgs = constructionArgs ?? new object[]{from};
                argTypes = argTypes ?? new Type[]{from.GetType()};
                for(int i = 0; i < constructionArgs.Length; i++){
                    // error if lengths aren't the same or if types don't match
                    if(i >= argTypes.Length || !(constructionArgs[i].GetType() == argTypes[i])){
                        throw new IllegalSendableFactoryCallException("Types of: "+constructionArgs.ToString()+" do not match "+argTypes.ToString());
                    }
                }
                // build
                if(type.IsSubclassOf(typeof(T))){ //refuse to construct types that aren't Deltas; do this for safety
                    ConstructorInfo con = type.GetConstructor(argTypes);
                    return con?.Invoke(constructionArgs) as T;
                }else{
                    return null;
                }
            }
        }

    }

}