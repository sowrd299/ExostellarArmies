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

		public event InsertCardEvent afterInsert;

		public new virtual void Insert(int index, Card item)
		{
			base.Insert(index, item);
			if (afterInsert != null) afterInsert(index, item);
		}
	}
}