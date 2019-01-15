namespace Game{

    // a class to reprsent the state of the game at a give point in time
    public class GameState{

        // returns if a move is legal for the given player to make
        public bool IsLegalMove(Player player, Move m){
            // dummy implementation
            return true;
        }

        // returns the outcomes of a player taking a give action
        public Delta[] GetMoveDeltas(Player player, Move m){
            // dummy implementation
            return new Delta[0];
        }

        public void ApplyDeltas(Delta[] deltas){

        }

        public Delta[] GetTurnDeltas(){
            // dummy implementation
            return new Delta[0];
        }


    }

}