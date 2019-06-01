using SFB.Game.Content;

namespace SFB.Game.Management
{
	public class BoardUpdate
	{
		public BoardUpdateType BoardUpdateType { get; private set; }
		public int Lane { get; private set; }
		public int Side { get; private set; }
		public int Pos { get; private set; }

		public static BoardUpdate GetAdd(int l, int side, int pos) {
			return new BoardUpdate(BoardUpdateType.ADD, l, side, pos);
		}

		public static BoardUpdate GetRemove(int l, int side, int pos) {
			return new BoardUpdate(BoardUpdateType.REMOVE, l, side, pos);
		}

		public static BoardUpdate GetSwap(int l, int side) {
			return new BoardUpdate(BoardUpdateType.SWAP, l, side);
		}

		private BoardUpdate(BoardUpdateType t, int l, int s, int p=-1)
		{
			BoardUpdateType = t;
			Lane = l;
			Side = s;
			Pos = p;
		}
	}

	public enum BoardUpdateType { ADD, REMOVE, SWAP }
}