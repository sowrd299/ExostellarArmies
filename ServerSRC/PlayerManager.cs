using Game.Decks;
using Game;
using System.Xml;
using System.Collections.Generic;

namespace Server.Matches{

    // a class to manage one specific player durring a match
    class PlayerManager : MessageHandler{

        private enum State{ACTING, WAITING} 

        // the socket to the player's client
        private SocketManager socket;

        // tracks what the player is current supposed to be doing
        private State state;

        // a private copy of the gameState
        private GameState gameState;

        // the moves taken this turn
        private List<Delta> turnDeltas;
        public Delta[] TurnDeltas{
            get{
                return turnDeltas.ToArray();
            }
        }

        // the client's player
        private Player player;

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
        public PlayerManager(SocketManager socket, Player player, GameState gs){
            this.socket = socket;
            this.player = player;
            this.gameState = gs;
            turnDeltas = new List<Delta>();
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
            turnDeltas = new List<Delta>();
        }

        // to be called once per main-loop-ish
        public void Update(){
            // recieve messages from the player
            handleSocket(socket);
        }

        public void StartAsyncReceive(){
            socket.AsynchReceiveXml(endAsyncReceive);
        }

        private void endAsyncReceive(XmlDocument msg, SocketManager from){
            handleMessage(msg, from);
            StartAsyncReceive();
        }

        public override void handleMessage(XmlDocument msg, SocketManager from){
            switch(messageTypeOf(msg)){
                case "gameAction":
                    if(state == State.ACTING){
                        Move m = new Move(msg.DocumentElement["move"]);
                        if(gameState.IsLegalMove(player, m)){
                            Delta[] ds =  gameState.GetMoveDeltas(player, m);
                            foreach(Delta d in ds){
                                turnDeltas.Add(d);
                                //also get it ready to send
                                //TODO: send back deltas
                            }
                        }else{
                            from.Send("<file type='error'><msg>Illegal game action</msg></file>");
                        }
                    }else{
                        from.Send("<file type='error'><msg>Cannot take game actions now</msg></file>");
                    }
                    break;
                case "lockInTurn":
                    if(state == State.ACTING){
                        // ...
                        state = State.WAITING; // after locking in, the player may make no more actions
                    }else{
                        from.Send("<file type='error'><msg>You probably sent 'lockInTurn' multiple times</msg></file>");
                    }
                    break;
                default:
                    base.handleMessage(msg,from);
                    break;
            }
        }
    }

}