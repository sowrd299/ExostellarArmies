using Game.Decks;

namespace Server.Matches{

    // a class to manage one specific player
    // durring a match
    class PlayerManager{

        // the socket to the player's client
        private SocketManager socket;

        public PlayerManager(SocketManager socket, DeckList list){
            this.socket = socket;
        }

        public void Start(){
            //TODO: maybe match start should be sent by the match itself?
            socket.Send("<file type='matchStart'></file>");
        }
    }

}