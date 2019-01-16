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

        private EotCallback eotCallback;

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
        public PlayerManager(SocketManager socket, Player player, GameState gs, EotCallback eotCallback){
            this.socket = socket;
            this.player = player;
            this.gameState = gs;
            this.eotCallback = eotCallback;
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

        // TODO: this probably should get broken up into many smaller functions
        public override void handleMessage(XmlDocument msg, SocketManager from){
            switch(messageTypeOf(msg)){
                case "gameAction":
                    if(state == State.ACTING){
                        Action a = new Action(msg.DocumentElement["action"]);
                        if(gameState.IsLegalAction(player, a)){
                            Delta[] ds =  gameState.GetActionDeltas(player, a);
                            XmlDocument resp = NewEmptyMessage("actionDeltas");
                            // TODO: should break into three loops so spend less time in each lock and respond to the message faster
                            lock(gameState){
                                lock(turnDeltas){
                                    foreach(Delta d in ds){
                                        turnDeltas.Add(d);
                                        gameState.ApplyDelta(d);
                                        resp.DocumentElement.AppendChild(d.ToXml().DocumentElement);
                                        //also get it ready to send
                                        //TODO: send back deltas
                                    }
                                }
                            }
                            socket.SendXml(resp);
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
                        eotCallback();
                    }else{
                        from.Send("<file type='error'><msg>You probably sent 'lockInTurn' multiple times</msg></file>");
                    }
                    break;
                default:
                    base.handleMessage(msg,from);
                    break;
            }
        }

        // a delegate for methods to be called after locking in a turn
        public delegate void EotCallback();

    }

}