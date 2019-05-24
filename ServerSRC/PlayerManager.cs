using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System;
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

        private enum State {ACTING, WAITING} 

        // the socket to the player's client
        public SocketManager Socket { get; private set; }

        // tracks what the player is current supposed to be doing
        private State state;

        // a private copy of the gameState
        private GameManager gameManager;

        // the moves taken this turn
        private List<Delta> turnDeltas;
        public Delta[] TurnDeltas => turnDeltas.ToArray();

        // to be called at end of turn
        private Action eotCallback;

        // to be called if/when the connection dies
        private Action deathCallback;

        // the client's player
        private Player player;

        public string Name { get; private set; }

        // returns whether or not the players has locked in their current turn
        public bool TurnLockedIn => state == State.WAITING || player.DeployPhases <= 0;

        // TODO: I am not convinced this should actually take a decklist
        //      that should probably be handled by something that manages game state
        //      not game networking
        public PlayerManager(SocketManager socket, int playerIndex, GameManager gm, Action eotCallback, Action deathCallback){
            Socket = socket;
            player = gm.Players[playerIndex];
            gameManager = gm;
            this.eotCallback = eotCallback;
            this.deathCallback = deathCallback;
            turnDeltas = new List<Delta>();
            Name = "Player " + playerIndex;
        }

        // to be called at the start of the game
        public void Start(XmlElement[] otherPlayersIDs, XmlElement[] laneIds, int sideIndex){
            //TODO: maybe match start should be sent by the match itself?
            XmlDocument doc = NewEmptyMessage("matchStart");

            doc.DocumentElement.SetAttribute("sideIndex", sideIndex.ToString());

            // and in local player IDs
            XmlElement friendlyIDs = player.GetPlayerIDs(doc);
            friendlyIDs.SetAttribute("side", "local");
            doc.DocumentElement.AppendChild(friendlyIDs);

            // add in the other players
            foreach(XmlElement ids in otherPlayersIDs){
                XmlElement e = doc.ImportNode(ids, true) as XmlElement;
                e?.SetAttribute("side", "opponent");
                doc.DocumentElement.AppendChild(e);
            }
            //add the lanes
            foreach(XmlElement ids in laneIds){
                XmlElement e = doc.ImportNode(ids, true) as XmlElement;
                doc.DocumentElement.AppendChild(e);
            }
            Socket.SendXml(doc);
        }

        public XmlElement GetPlayerIDs(XmlDocument doc) => player.GetPlayerIDs(doc);

        // handle everything that happens at the start of a new turn
        public void StartTurn(TurnPhase[] phases){
            XmlDocument msg = NewEmptyMessage("turnStart");
            foreach (TurnPhase phase in phases)
            {
                XmlElement element = phase.ToXml(msg, player);
                msg.DocumentElement.AppendChild(element);
            }
            // add in input requests
            foreach (InputRequest req in player.InputRequests){
                XmlElement element = req.ToXml(msg);
                msg.DocumentElement.AppendChild(element);
            }
            // send
			socket.SendXml(msg);
            // reset for next turn
            if(player.InputRequests.Count == 0){
                // do not resume acting if have outstanding input requests
                // TODO: this will probably cause problems for other players... oops? but also maybe not B/C deploy phase count?
                state = State.ACTING;
            }
            turnDeltas = new List<Delta>();
        }

        public void EndMatch(){
            // TODO: send more helpful information, e.g. winner
            // TODO: the if statement is just here because if it weren't
            //          disconnecting would 100% of the time trigger sending a message to a dead socket
            //          and thus a fatal error ... HANDLE THIS BETTER PLEASE
            if(Socket.Alive) Socket.SendXml(NewEmptyMessage("matchEnd"));
        }

        // to be called once per main-loop-ish
        public void Update(){
            // recieve messages from the player
            HandleSocket(Socket);
        }

        public void StartAsyncReceive(){
            StartAsyncReceive(Socket);
        }

        // what to do when a socket dies
        protected override void HandleSocketDeath(SocketManager _){
            deathCallback();
        }

        // TODO: this probably should get broken up into many smaller functions
        public override void HandleMessage(XmlDocument msg, SocketManager from){
            switch(MessageTypeOf(msg)){
                // handle player taking action
                case "gameAction":
                    if(state == State.ACTING){
                        // forloop to handle all actions in the message
                        // will handle up-to but not past any illegal actions given
                        XmlDocument resp = NewEmptyMessage("actionDeltas");
                        foreach (XmlElement actionElement in msg.GetElementsByTagName("action")){
                            PlayerAction a = PlayerAction.FromXml(actionElement, cardLoader, Lane.IdIssuer);
                            if(gameManager.IsLegalAction(player, a)){
                                // using three different for loops to:
                                //  1) send message faster
                                //  2) spend less time in each lock
                                handleDeltas(gameManager.GetActionDeltas(player, a), resp);
                            }else{
                                from.Send("<file type='error'><msg>Illegal game action</msg></file>");
                                break;
                            }
                        }
                        Socket.SendXml(resp);
                    }else{
                        from.Send("<file type='error'><msg>Cannot take game actions now</msg></file>");
                    }
                    break;
                // handle ending turn
                case "lockInTurn":
                    if(state == State.ACTING){
                        // ...
                        // TES
                        Console.WriteLine("Turn locked in...");
                        state = State.WAITING; // after locking in, the player may makthough maybe I should fixe no more actions
                        eotCallback();
                    }else{
                        from.Send("<file type='error'><msg>You probably sent 'lockInTurn' multiple times</msg></file>");
                    }
                    break;
                // handle answers to input requests
                case "inputRequestResponse":
                    XmlDocument irResp = NewEmptyMessage("actionDeltas");
                    foreach (XmlElement reqElement in msg.GetElementsByTagName("inputRequest")){
                        InputRequest req = InputRequest.FromXml(reqElement);
                        if(player.ContainsInputRequest(req) && req.Made){
                            // if the player is actually answering an input request they were assigned to
                            handleDeltas(req.GetDeltas(), irResp);
                            player.FinishInputRequest(req);
                        }else{
                            from.Send("<file type='error'<msg>Either: You were not assigned this Input Request, or you did not make a valid choice: "
                                    +reqElement.OuterXml+"</msg></file>");
                        }
                    }
                    socket.SendXml(irResp);
                    break;
                default:
                    base.HandleMessage(msg,from);
                    break;
            }
        }

        // what to do with deltas generated by handling messages
        // TODO: this send some now, send some later architecture is way more convoluted than it should be
        //          it really should just be send all later
        private void handleDeltas(Delta[] ds, XmlDocument resp){
            // update the gamestate
            lock(gameManager){
                foreach(Delta d in ds){
                    gameManager.ApplyDelta(d);
                }
            }
            // build and send the reponse
            foreach(Delta d in ds){
                XmlElement e = d.ToXml(resp);
                resp.DocumentElement.AppendChild(e);
            }
            // log the turn deltas
            lock(turnDeltas){
                foreach(Delta d in ds){
                    turnDeltas.Add(d);
                }
            }
        }
    }

}