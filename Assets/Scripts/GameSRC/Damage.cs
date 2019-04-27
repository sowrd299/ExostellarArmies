
namespace SFB.Game {
	public class Damage {
		public enum Type {
			RANGED, MELEE, TOWER, HEAL
		}

		public static string DamageTypeToString(Type type) {
			switch(type) {
				case Type.RANGED:
					return "R";
				case Type.MELEE:
					return "M";
				case Type.TOWER:
					return "T";
				case Type.HEAL:
					return "H";
				default:
					return "";
			}
		}

		public static Type StringToDamageType(string type) {
			switch(type) {
				case "R":
					return Type.RANGED;
				case "M":
					return Type.MELEE;
				case "T":
					return Type.TOWER;
				case "H":
					return Type.HEAL;
				default:
					throw new System.Exception($"Type string \"{type}\" is invalid");
			}
		}
	}
}