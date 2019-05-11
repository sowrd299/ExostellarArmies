using SFB.Game.Content;

namespace SFB.Game.Management
{
	public class GameStateLocation
	{
		public GameState GameState { get; private set; }
		public int Lane { get; private set; }
		public int Side { get; private set; }
		public int Pos { get; private set; }

		public Player[] Players { get { return GameState.Players; } }
		public Lane[] Lanes { get { return GameState.Lanes; } }

		public Player SubjectPlayer { get { return Players[Side]; } }
		public Lane SubjectLane { get { return Lanes[Lane]; } }
		public Unit SubjectUnit { get { return SubjectLane.Units[Side, Pos]; } }

		public GameStateLocation(GameState gameState, int lane, int side, int pos)
		{
			GameState = gameState;
			Lane = lane;
			Side = side;
			Pos = pos;
		}
	}
}