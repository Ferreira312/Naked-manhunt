using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace NakedManhunt
{
    public class IncidentWorker_NakedManhunt : IncidentWorker
    {

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Pawn NakedPawn = null;
            PawnKindDef NakedPawnKindDef = PawnKindDef.Named("NakedPeople");
            bool flag = NakedPawnKindDef == null;
            bool flag2 = flag;
            bool result;
            if (flag2)
            {
                Log.Error("Can't spawn any Naked People");
                result = false;
            }
            else
            {
                IEnumerable<Pawn> source = map.mapPawns.AllPawns.Cast<Pawn>();
                Func<Pawn, bool> predicate;
                bool flag3 = (predicate = IncidentWorker_NakedManhunt.NakedPeople.isNaked) == null;
                if (flag3)
                {
                    predicate = (IncidentWorker_NakedManhunt.NakedPeople.isNaked = new Func<Pawn, bool>(IncidentWorker_NakedManhunt.NakedPeople.Naked.TryExecute));
                }
                List<Pawn> source2 = source.Where(predicate).ToList<Pawn>();
                double value = (double)(source2.Count<Pawn>() / 3);
                int num = (int)Math.Round(value, 1);
                bool flag4 = num <= 1;
                bool flag5 = flag4;
                if (flag5)
                {
                    num = 2;
                }
                IntVec3 intVec = new IntVec3();
                bool flag6 = !RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, 0f);
                bool flag7 = flag6;
                if (flag7)
                {
                    result = false;
                }
                else
                {
                    IntVec3 intVec2 = CellFinder.RandomClosewalkCellNear(intVec, map, 10);
                    Rot4 rot = Rot4.FromAngleFlat((map.Center - intVec).AngleFlat);
                    int num2;
                    for (int i = 0; i < num; i = num2 + 1)
                    {
                        NakedPawn = PawnGenerator.GeneratePawn(NakedPawnKindDef);
                        GenSpawn.Spawn(NakedPawn, intVec2, map, rot, WipeMode.Vanish, false);
                        NakedPawn.needs.food.CurLevel = 0.01f;
                        NakedPawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.ManhunterPermanent, null, false, false, null, false);
                        NakedPawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + Rand.Range(90000, 130000);
                        num2 = i;
                    }

                    Find.LetterStack.ReceiveLetter("Naked People", "A group of hungry naked people have entered your area. They'll do anything to get your food!", LetterDefOf.ThreatBig, new TargetInfo(intVec, map, false), null);
                    Find.TickManager.slower.SignalForceNormalSpeedShort();
                    result = true;
                }
            }
            return result;
        }

        private sealed class NakedPeople
        {
            public bool TryExecute(Pawn p)
            {
                return (p.RaceProps.Humanlike && !GenHostility.HostileTo(p, Faction.OfPlayer) && !p.IsColonist) || p.IsPrisonerOfColony;
            }

            public static readonly IncidentWorker_NakedManhunt.NakedPeople Naked = new IncidentWorker_NakedManhunt.NakedPeople();

            public static Func<Pawn, bool> isNaked;
        }
    }
}