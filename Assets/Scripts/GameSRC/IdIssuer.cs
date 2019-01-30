using System.Collections.Generic;

namespace SFB.Game.Management{

    // a class to issue unique id strings to objects that need them
    // id's to associate corresponding objects across clients/servers
    // objects that
    // 1) have multiple instances
    // 2) store gamestate
    // 3) change durring the course of gameplay
    // should be issued ID's
    // this also provides lookup based on those ID's
    public class IdIssuer<T> {

        private int i;
        public string NullId{ //a placeholder ID for client objects that do not have ID's yet; don't know if this is actually usefull
            get{ return "0"; }
        }

        private string idPrefix; // a prefix to give to ID's; all ID prefixes w/i a given context should be the same length

        private object writeLock; // a lock on editing or issuing ids, for thread safety

        // stores all the ID's that where issued and what they were issued to
        // TODO: this has the problem of storing "dead" gameobjects that otherwise should be garbage collected
        private Dictionary<string, T> idLookup;

        public IdIssuer(){
            this.idPrefix = "";
            this.idLookup = new Dictionary<string, T>();
            i = 1;
            writeLock = new object();
        }

        // generates and gives out a new ID
        public string IssueId(T issuee){
            string id;
            lock(writeLock){
                do{
                    id = idPrefix + i.ToString();
                    i++;
                }while(idLookup.ContainsKey(id));
                RegisterId(id, issuee);
            }
            return id;
        }

        // stores the given object at the given ID
        public void RegisterId(string id, T issuee){
            lock(writeLock){
                idLookup[id] = issuee;
            }
        }

        // returns the object to whom the ID was issued
        public T GetByID(string id){
            return idLookup[id];
        }

    }
    
}