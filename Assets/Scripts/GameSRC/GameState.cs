using SFB.Game.Decks;

namespace SFB.Game.Management{

    // a class to reprsent the state of the game at a give point in time
    public class GameState{

        public bool Over{
            get{
                return false;
            }
        }

        Player[] players;
        // an array of all the players
        // MUST BE IN THE SAME ORDER AS THEIR DECKLISTS WERE PROVIDED
        public Player[] Players{
            get{
                return players;
            }
        }

        public GameState(DeckList[] lists){
            players = new Player[lists.Length];
            for(int i = 0; i < players.Length; i++){
                players[i] = new Player("Player " + (i+1), lists[i]);
            }
        }

        // returns if a move is legal for the given player to make
        public bool IsLegalAction(Player player, Action a){
            // dummy implementation
            return true;
        }

        // returns the outcomes of a player taking a give action
        public Delta[] GetActionDeltas(Player player, Action a){
            // dummy implementation
            return new Delta[]{};
        }

        // return the resaults if the turn were to end right then
        public Delta[] GetTurnDeltas(){
            // dummy implementation
            return new Delta[0];
        }

        public void ApplyDeltas(Delta[] deltas){
            foreach(Delta d in deltas){
                d.Apply();
            }
        }

    }

}