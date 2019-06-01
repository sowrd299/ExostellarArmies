using System.Xml;

namespace SFB.Game.Management{


    // a class to request input needed from a player
    // usually, this mean a card's ability gives the player a choice
    public abstract class InputRequest : Sendable, IIDed {

        // setup Sendable
        protected static new string XmlNodeName{
            get{ return "inputRequest"; }
        }

        // setup IIDed
        protected static IdIssuer<InputRequest> idIssuer = new IdIssuer<InputRequest>();
        public string ID { get; private set; }

        // returns whether or not the player has made their choice
        public abstract bool Made { get; }

        // NOTE: Any request you want handled needs to be assigned to a player. See Player.AssignInputRequest
        // NOTE: Input requests are currenlty NOT supported for use durring the planning phase
        public InputRequest(string id = null){
            // ID it
            if(id != null){
                ID = id;
                idIssuer.RegisterId(id, this);
            }else{
                ID = idIssuer.IssueId(this);
            }
        }

        // TO BE CALLED BY FROM XML ONLY.
        // CALLING THIS MANUAL WILL CAUSE PROBLEMS FOR ID's
        public InputRequest(XmlElement e) {
            // find and register the ID
            string id = e.Attributes["id"].Value;
            idIssuer.RegisterId(id, this);
        }

        // call this to set the choice to whatever choice made by the request
        //      described in the given XML
        // important for correlating success states between devices
        // returns the success state of the copy
        protected abstract bool MakeChoiceFrom(XmlElement e);

        public static InputRequest FromXml(XmlElement e){
            // find in there is already an Input Request with the same ID
            string id = e.Attributes["id"].Value;
            InputRequest sharedId = idIssuer.GetByID(id);
            // ...if there is, just migrate data to that one
            if(sharedId != null && sharedId.MakeChoiceFrom(e)){
                return sharedId;
            }
            // if that rought failed to return...
            object[] args = new object[]{e};
            return SendableFactory<InputRequest>.FromXml(e, args);
        }

        public override XmlElement ToXml(XmlDocument doc){
            XmlElement e = base.ToXml(doc);
            XmlAttribute a = doc.CreateAttribute("id");
            a.Value = ID;
            e.SetAttributeNode(a);
            return e;
        }

        public abstract Delta[] GetDeltas(); // get the deltas after the choice has been made

    }


    // a subclass that implements having a senable target
    public abstract class InputRequest<T> : InputRequest where T : IIDed{

        // TODO: add in some conception of what are the legal choices?
        protected SendableTarget<T> chosen;  // the choice the player made

        public override bool Made{
            get{ return chosen.Target != null; }
        }

        public InputRequest(string id = null) :
                base(id) 
        {
            chosen = new SendableTarget<T>("chosen", default(T));
        }

        // TO BE CALLED BY FROM XML ONLY.
        // CALLING THIS MANUAL WILL CAUSE PROBLEMS FOR ID's
        public InputRequest(XmlElement e, IdIssuer<T> targetIdIssuer)
            : base(e) 
        {
            // get the choice, if there is one
            chosen = new SendableTarget<T>("chosen", e, targetIdIssuer);
        }

        // NOTE: this does not override the abstract MakeChoiceFrom
        // you will need to override that one in any child class.
        // This is is here to be called in that method; to make writing it easier
        protected bool MakeChoiceFrom(XmlElement e, IdIssuer<T> targetIdIssuer){
            chosen = new SendableTarget<T>("chosen", e, targetIdIssuer);
            if(!object.Equals(chosen.Target, default(T))){
                MakeChoice(chosen.Target);
                return true;
            }
            return false;
        }

        public override XmlElement ToXml(XmlDocument doc){
            XmlElement e = base.ToXml(doc);
            e.SetAttributeNode(chosen.ToXml(doc));
            return e;
        }

        
        // make the choice
        // NOTE: Correctly, this is from InputRequest<T> and not InputRequest
        //       you will need to cast to a subtype to make a choice
        public bool MakeChoice(T chosen){
            if(IsLegalChoice(chosen)){
                this.chosen = new SendableTarget<T>("chosen", chosen);
                return true;
            }
            return false;
        }

        // override this to put further restrictions on what may be
        //      entered as a choice for a given request
        public virtual bool IsLegalChoice(T chosen){
            return true;
        }

    }

}