using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;
using SFB.Game;
using SFB.Game.Content;

// this is part of Server, and not Game.Deck, because it (in theory) needs contact to
// databases, account information, etc.
// also the client shouldn't have access to it
// at minimum it is not part of the pure abstract gamelogic

namespace SFB.Net.Server
{

	class DeckListManager
	{
		private CardLoader cardLoader;
		private Dictionary<string, DeckList> decks;

		public DeckListManager()
		{
			cardLoader = new CardLoader();

			decks = new Dictionary<string, DeckList>();
			LoadDecks();
		}

		private void LoadDecks()
		{
			string path = Path.Combine("Assets", "Resources", "Decks");
			foreach (string fileName in Directory.EnumerateFiles(path, "*.yaml"))
			{
				YamlStream yaml = new YamlStream();
				yaml.Load(File.OpenText(fileName));

				YamlMappingNode root = yaml.Documents[0].RootNode as YamlMappingNode;
				string deckId = root.Children[new YamlScalarNode("id")].ToString();
				string deckName = root.Children[new YamlScalarNode("name")].ToString();
				
				DeckList deck = new DeckList();
				YamlSequenceNode cardSequence = root.Children[new YamlScalarNode("cards")] as YamlSequenceNode;
				foreach (YamlMappingNode cardItem in cardSequence)
				{
					deck.AddCard(
						cardLoader.GetByID(cardItem.Children[new YamlScalarNode("id")].ToString()),
						int.Parse(cardItem.Children[new YamlScalarNode("count")].ToString())
					);
				}

				decks.Add(deckId, deck);
				int deckCardCount = deck.GetCards().Select(deck.GetCopiesOf).Sum();
				Console.WriteLine($"Loaded deck {deckName} ({deckCardCount} cards).");
			}
		}

		// return a decklist object of the deck list with the
		// given ID
		public DeckList LoadFromID(string id)
		{
			Console.WriteLine($"Loading deck: {id}");
			if (decks.ContainsKey(id))
			{
				return decks[id];
			}

			//TESTING IMPLEMENTATION
			if (id.StartsWith("TEST "))
			{
				DeckList d = new DeckList();
				d.AddCard(cardLoader.GetByID(id.Substring(5)), 8);
				d.AddCard(cardLoader.GetByID("Resist Token"), 6);
				d.AddCard(cardLoader.GetByID("Mana Token"), 6);
				Console.WriteLine("Loaded test deck");
				return d;
			}
			else
			{
				// TODO: What's the default deck?
				switch(id) {
					case "tokensAtk":
						DeckList a = new DeckList();
						a.AddCard(cardLoader.GetByID("Attack Token 1"), 4);
						a.AddCard(cardLoader.GetByID("Attack Token 2"), 4);
						a.AddCard(cardLoader.GetByID("Attack Token 3"), 4);
						a.AddCard(cardLoader.GetByID("Attack Token 4"), 4);
						a.AddCard(cardLoader.GetByID("Attack Token 5"), 4);
						return a;
					case "testing":
					default:
						DeckList d = new DeckList();
						d.AddCard(cardLoader.GetByID("Resist Token"), 20);
						return d;
				}
			}
		}
	}
}