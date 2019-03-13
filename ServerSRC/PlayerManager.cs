using System.Xml;
using System.Collections.Generic;
using System.Linq;
using SFB.Game.Content;
using SFB.Game.Management;
using SFB.Game;

namespace SFB.Net.Server.Matches{

    // a class to manage one specific player durring a match
    class PlayerManager : MessageHandler{

        // TODO: this is a SUPER DUMB way to do this. I'm just lazy and it's week 10.
        // but actually, this will need more meaningfull access to a card loader at some point
        // most likely getting one passed in from Match, which gets one from the server
        private static CardLoader cardLoader = new CardLoader();

        private enum State{ACTING, WAITING} 

        // the socket to the player's client
        private SocketManager socket;
        public SocketManager Socket{
            get{
                return socket;
            }
        }

        // tracks what the player is current supposed to be doing
        private State state;

        // a private copy of the gameState
        private GameManager gameManager;

        // the moves taken this turn
        private List<Delta> turnDeltas;
        public Delta[] TurnDeltas{
            get{
                return turnDeltas.ToArray();
            }
        }

        // to be called at end of turn
        private EotCallback eotCallback;

        // to be called if/when the connection dies
        private DeathCallback deathCallback;

        // the client's player
        private Player player;

        public string Name{
            get{ return player.Name; }
        }

        // returns whether or not the players has locked in their current turn
        public bool TurnLockedIn{
            get{
                // this algorithm is an oversimplification,
                // but at VGDC quarter-long scale it should work perfectly
                return state == State.WAITING || player.DeployPhases <= 0;
            }
        }

        // TODO: I am not convinced this should actually take a decklist
        //      that should probably be handled by something that manages game state
        //      not game networking
        public PlayerManager(SocketManager socket, Player player, GameManager gm, EotCallback eotCallback, DeathCallback deathCallback){
            this.socket = socket;
            this.player = player;
            this.gameManager = gm;
            this.eotCallback = eotCallback;
            this.deathCallback = deathCallback;
            turnDeltas = new List<Delta>();
        }

        // to be called at the start of the game
        public void Start(XmlElement[] otherPlayersIDs, XmlElement[] laneIds){
            //TODO: maybe match start should be sent by the match itself?
            XmlDocument doc = NewEmptyMessage("matchStart");
            XmlElement friendlyIDs = player.GetPlayerIDs(doc);
            XmlAttribute side = doc.CreateAttribute("side");
            side.Value = "local";
            friendlyIDs.SetAttributeNode(side); 
            doc.DocumentElement.AppendChild(friendlyIDs);
            // add in the other players
            foreach(XmlElement ids in otherPlayersIDs){
                XmlElement e = doc.ImportNode(ids, true) as XmlElement;
                XmlAttribute enemySide = doc.CreateAttribute("side");
                enemySide.Value = "opponent";
                e?.SetAttributeNode(enemySide); 
                doc.DocumentElement.AppendChild(e);
            }
            //add the lanes
            foreach(XmlElement ids in laneIds){
                XmlElement e = doc.ImportNode(ids, true) as XmlElement;
                doc.DocumentElement.AppendChild(e);
            }
            socket.SendXml(doc);
        }

        public XmlElement GetPlayerIDs(XmlDocument doc){
            return player.GetPlayerIDs(doc);
        }

        // handle everything that happens at the start of a new turn
        public void StartTurn(Delta[] deltas){
            XmlDocument msg = NewEmptyMessage("turnStart");
            foreach(Delta d in deltas.Where((d) => d.VisibleTo(player))){
                // TODO: filter down to only the deltas the player gets to know about
                // e.g. remove cards drawn by opponent
                XmlElement e = d.ToXml(msg);
                msg.DocumentElement.AppendChild(e);
            }
            socket.SendXml(msg);
            state = State.ACTING;
            turnDeltas = new List<Delta>();
        }

        public void EndMatch(){
            // TODO: send more helpful information, e.g. winner
            // TODO: the if statement is just here because if it weren't
            //          disconnecting would 100% of the time trigger sending a message to a dead socket
            //          and thus a fatal error ... HANDLE THIS BETTER PLEASE
            if(socket.Alive) socket.SendXml(NewEmptyMessage("matchEnd"));
        }

        // to be called once per main-loop-ish
        public void Update(){
            // recieve messages from the player
            handleSocket(socket);
        }

        public void StartAsyncReceive(){
            StartAsyncReceive(socket);
        }

        // what to do when a socket dies
        protected override void handleSocketDeath(SocketManager _){
            deathCallback();
        }

        // TODO: this probably should get broken up into many smaller functions
        public override void handleMessage(XmlDocument msg, SocketManager from){
            switch(messageTypeOf(msg)){
                // handle player taking action
                case "gameAction":
                    if(state == State.ACTING){
                        PlayerAction a = PlayerAction.FromXml(msg.DocumentElement["action"], cardLoader, Lane.IdIssuer);
                        if(gameManager.IsLegalAction(player, a)){
                            Delta[] ds =  gameManager.GetActionDeltas(player, a);
                            // using three different for loops to:
                            //  1) send message faster
                            //  2) spend less time in each lock

                            // update the gamestate
                            lock(gameManager){
                                foreach(Delta d in ds){
                                    gameManager.ApplyDelta(d);
                                }
                            }
                            // build and send the reponse
                            XmlDocument resp = NewEmptyMessage("actionDeltas");
                            foreach(Delta d in ds){
                                XmlElement e = d.ToXml(resp);
                                resp.DocumentElement.AppendChild(e);
                            }
                            socket.SendXml(resp);
                            // log the turn deltas
                            lock(turnDeltas){
                                foreach(Delta d in ds){
                                    turnDeltas.Add(d);
                                }
                            }
                        }else{
                            from.Send("<file type='error'><msg>Illegal game action</msg></file>");
                        }
                    }else{
                        from.Send("<file type='error'><msg>Cannot take game actions now</msg></file>");
                    }
                    break;
                // handle ending turn
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

        // a delegate for methods to be called when the socket dies
        public delegate void DeathCallback();

    }

}