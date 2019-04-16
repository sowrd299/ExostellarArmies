using System;
using System.Collections.Generic;

namespace SFB.Game
{    public class AbilityList : List<Ability> {
		
		// abilities must be in a set order because of filtering targets to damage
		// otherwise, say if you had flying and lob, depending on the order, the targets
		//     would be filtered differently - do you skip a target for
		//     lob before or after looking at only flying targets?
		// currently no clashes bc flying doesn't exist, but it could happen
		/*
		private static Type[] order = new Type[] {
			typeof(RangedShield), typeof(MeleeShield), typeof(Lob)
		};

		public AbilityList(params Ability[] abilities) : base(abilities) {
			// TODO: implement sorting here and in Add
		}*/

		public AbilityList() { }

		public AbilityList(Ability[] list) :
				base(list) { }

		public new void Add(Ability a) {
			if(this.hasType(a.GetType())) {
				if(a.Num == -1) {
					// error: trying to add a numberless ability to a unit that already has it
				} else {
					foreach(Ability ability in this) {
						if(ability.GetType() == a.GetType()) {
							ability.Num += a.Num;
							break;
						}
					}
				}
			} else {
				base.Add(a);
			}
		}

		public new void Remove(Ability a) {
			if(a.Num == -1) {
				base.Remove(a);
			} else {
				for(int i = 0; i < this.Count; i++) {
					if(this[i].GetType() == a.GetType()) {
						this[i].Num -= a.Num;
						if(this[i].Num <= 0)
							base.RemoveAt(i);
						break;
					}
				}
			}
		}

		public bool hasType(Type type) {
			return !this.TrueForAll(ability => ability.GetType() != type);
		}
	}
}
