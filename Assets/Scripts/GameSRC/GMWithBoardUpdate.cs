using SFB.Game.Content;

namespace SFB.Game.Management
{
	public class GMWithBoardUpdate
	{
		public GameManager GameManager { get; private set; }
		public BoardUpdate BoardUpdate { get; private set; }

		public Player[] Players { get { return GameManager.Players; } }
		public Lane[] Lanes { get { return GameManager.Lanes; } }
		public int Lane { get { return BoardUpdate.Lane; } }
		public int Side { get { return BoardUpdate.Side; } }
		public int Pos { get { return BoardUpdate.Pos; } }

		public Player SubjectPlayer { get { return Players[Side]; } }
		public Lane SubjectLane { get { return Lanes[Lane]; } }
		public Unit SubjectUnit { get { return SubjectLane.Units[Side, Pos]; } }


		public GMWithBoardUpdate(GameManager gm, BoardUpdate boardUpdate)
		{
			GameManager = gm;
			BoardUpdate = boardUpdate;
		}
	}
}