using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SFB.Game.Content;

namespace SFB.Game.Management
{
	public class TurnPhase
	{
		public const string TAG_NAME = "phase";

		public string Name { get; private set; }
		public List<Delta> Deltas { get; private set; }

		public TurnPhase(string name)
		{
			Name = name;
			Deltas = new List<Delta>();
		}

		public TurnPhase(XmlElement from, CardLoader loader)
		{
			Name = from.GetAttribute("name");
			Deltas = from
				.GetElementsByTagName("delta")
				.OfType<XmlElement>()
				.Select(element => Delta.FromXml(element, loader))
				.ToList();
		}

		public XmlElement ToXml(XmlDocument document, Player player)
		{
			XmlElement element = document.CreateElement(TAG_NAME);
			element.SetAttribute("name", Name);
			foreach (Delta delta in Deltas.Where(delta => delta.VisibleTo(player)))
			{
				element.AppendChild(delta.ToXml(document));
			}
			return element;
		}
	}
}