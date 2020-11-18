using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace NakedManhunt
{
	protected override void GiveCombatEnhancingDrugs(Pawn pawn)
	{
		if (Rand.Value >= pawn.kindDef.combatEnhancingDrugsChance)
		{
			return;
		}
		if (pawn.IsTeetotaler())
		{
			return;
		}
		for (int i = 0; i < pawn.inventory.innerContainer.Count; i++)
		{
			CompDrug compDrug = pawn.inventory.innerContainer[i].TryGetComp<CompDrug>();
			if (compDrug != null && compDrug.Props.isCombatEnhancingDrug)
			{
				return;
			}
		}
		int randomInRange = pawn.kindDef.combatEnhancingDrugsCount.RandomInRange;
		if (randomInRange <= 0)
		{
			return;
		}
		IEnumerable<ThingDef> source = DefDatabase<ThingDef>.AllDefsListForReading.Where(delegate (ThingDef x)
		{
			if (x.category != ThingCategory.Item)
			{
				return false;
			}
			if (pawn.Faction != null && x.techLevel > pawn.Faction.def.techLevel)
			{
				return false;
			}
			CompProperties_Drug compProperties = x.GetCompProperties<CompProperties_Drug>();
			return compProperties != null && compProperties.isCombatEnhancingDrug;
		});
		int num = 0;
		ThingDef def;
		while (num < randomInRange && source.TryRandomElement(out def))
		{
			pawn.inventory.innerContainer.TryAdd(ThingMaker.MakeThing(def, null), true);
			num++;
		}
	}
}
}
}
