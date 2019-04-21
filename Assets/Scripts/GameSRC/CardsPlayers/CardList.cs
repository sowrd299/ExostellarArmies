using System;
using System.Collections.Generic;
using SFB.Game.Management;

namespace SFB.Game
{
	public delegate void CardEvent(Card card);

	// A parent class for decks and hands
	public abstract class CardList : List<Card>, IIDed
	{
		public abstract string ID
		{
			get;
		}

		public event CardEvent afterInsert;

		public new virtual void Insert(int index, Card item)
		{
			base.Insert(index, item);
			afterInsert(item);
		}
	}
}