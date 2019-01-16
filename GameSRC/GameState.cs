namespace Game{

    // a class to reprsent the state of the game at a give point in time
    public class GameState{

        // returns if a move is legal for the given player to make
        public bool IsLegalAction(Player player, Action a){
            // dummy implementation
            return true;
        }

        // returns the outcomes of a player taking a give action
        public Delta[] GetActionDeltas(Player player, Action a){
            // dummy implementation
            return new Delta[0];
        }

        public void ApplyDelta(Delta deltas){

        }

        public Delta[] GetTurnDeltas(){
            // dummy implementation
            return new Delta[0];
        }


    }

}