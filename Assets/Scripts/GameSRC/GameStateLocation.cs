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

		// returns null if already in front
		public Unit FrontUnit { get {
				return (Pos == 0 ? null : SubjectLane.Units[Side, 0]);
		} }
		// returns null if already in back
		public Unit BackUnit {
			get {
				return (Pos == 1 ? null : SubjectLane.Units[Side, 1]);
			}
		}

		// returns null if nonexistent
		public Lane LeftLane {
			get {
				return (Lane==0 ? null : Lanes[Lane-1]);
			}
		}
		// returns null if nonexistent
		public Lane RightLane {
			get {
				return (Lane == 2 ? null : Lanes[Lane + 1]);
			}
		}

		public bool IsSupporting(string[] types)
		{
			Unit front = FrontUnit;
			if(Pos == 1 && front != null) {
				foreach(string type in types)
					if(!front.Card.UnitType.Contains(type))
						return false;
				return true;
			}
			return false;
		}

		public GameStateLocation(GameState gameState, int lane, int side, int pos)
		{
			GameState = gameState;
			Lane = lane;
			Side = side;
			Pos = pos;
		}
	}
}