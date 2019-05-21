using System;
using System.Collections.Generic;
using SFB.Game.Management;

namespace SFB.Game
{
	public delegate void InsertCardEvent(int index, Card item);

	// A parent class for decks and hands
	public abstract class CardList : List<Card>, IIDed
	{
		public abstract string ID
		{
			get;
		}
	}
}