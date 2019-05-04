using System.Xml;
using SFB.Game.Management;

namespace SFB.Game.Content
{
	public class Tower : IIDed
	{
		public const string TAG_NAME = "tower";

		private static IdIssuer<Tower> idIssuer = new IdIssuer<Tower>();
		public static IdIssuer<Tower> IdIssuer {
			get { return idIssuer; }
		}

		public int MaxHP { get; private set; }
		public int HP { get; private set; }

		private const int BASE_DAMAGE = 1;
		public int Damage { get; set; }

		readonly private string id;
		public string ID
		{
			get { return id; }
		}

		public Tower()
		{
			MaxHP = 2;
			HP = MaxHP;
			Damage = BASE_DAMAGE;

			id = idIssuer.IssueId(this);
		}

		public Tower(XmlElement from)
		{
			MaxHP = 2;
			HP = MaxHP;
			Damage = BASE_DAMAGE;

			id = from.Attributes["id"].Value;
			idIssuer.RegisterId(id, this);
		}

		public XmlElement ToXml(XmlDocument doc, int index)
		{
			XmlElement element = doc.CreateElement(TAG_NAME);

			element.SetAttribute("id", ID);
			element.SetAttribute("index", index.ToString());

			return element;
		}
		public void TakeDamage(int amount)
		{
			HP -= amount;
		}

		public void UndoTakeDamage(int amount)
		{
			HP += amount;
		}

		public void Revive()
		{
			if (HP <= 0)
			{
				HP = ++MaxHP;
			}
			else
			{
				throw new System.Exception("Tried to revive tower when HP wasn't 0.");
			}
		}

		public void ResetDamage()
		{
			Damage = BASE_DAMAGE;
		}

		public TowerDamageDelta[] GetDamageDeltas(int amt, Damage.Type dmgType)
		{
			return new TowerDamageDelta[] { new TowerDamageDelta(this, amt, dmgType) };
		}

		public TowerReviveDelta[] GetReviveDeltas()
		{
			return new TowerReviveDelta[] { new TowerReviveDelta(this) };
		}
	}
}