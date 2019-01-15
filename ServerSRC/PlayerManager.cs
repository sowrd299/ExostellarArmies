using Game.Decks;
using System.Xml;

namespace Server.Matches{

    // a class to manage one specific player durring a match
    class PlayerManager{

        private enum State{ACTING, WAITING} 

        // the socket to the player's client
        private SocketManager socket;

        // tracks what the player is current supposed to be doing
        private State state;

        // returns whether or not the players has locked in their current turn
        public bool TurnLockedIn{
            get{
                // this algorithm is an oversimplification,
                // but at VGDC quarter-long scale it should work perfectly
                return state == State.WAITING;
            }
        }

        // TODO: I am not convinced this should actually take a decklist
        //      that should probably be handled by something that manages game state
        //      not game networking
        public PlayerManager(SocketManager socket, DeckList list){
            this.socket = socket;
        }

        // to be called at the start of the game
        public void Start(){
            //TODO: maybe match start should be sent by the match itself?
            socket.Send("<file type='matchStart'></file>");
        }

        // handle everything that happens at the start of a new turn
        public void StartTurn(){
            // ...
            socket.Send("<file type='turnStart'></file>");
            state = State.ACTING;
        }

        // to be called once per main-loop-ish
        public void Update(){
            // recieve messages from the player
            XmlDocument msg = socket.ReceiveXml();
            if(msg != null){
                string type = msg.DocumentElement.Attributes["type"].Value;
                switch(type){
                    case "gameAction":
                        if(state == State.ACTING){
                            // TODO: handle the player taking an action
                        }else{
                            socket.Send("<file type='error'><msg>Cannot take game actions now</msg></file>");
                        }
                        break;
                    case "lockInTurn":
                        if(state == State.ACTING){
                            // ...
                            state = State.WAITING; // after locking in, the player may make no more actions
                        }else{
                            socket.Send("<file type='error'><msg>You probably sent 'lockInTurn' multiple times</msg></file>");
                        }
                        break;
                    default:
                        socket.Send("<file type='error'><msg>Unexpect message type durring gameplay: "+type+"</msg></file>");
                        break;
                }
            }
        }
    }

}