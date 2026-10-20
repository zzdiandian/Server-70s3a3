using System;using System.IO;
// r6 (2026-10-17): post-promotion attribute growth, measured from the six CN70 client
// scripts character/<job>/<job>.chr [growtype N] (native fixed-point10, GetRoundInt(10f*v)
// per reference/rednf ImportCharacterScript.inc:218-326). Grow row index = chr growtype-1 =
// first grow 0..5 (Reward-Types-FG183.EncodeGrow packs first|second<<4; reward grows measured
// 1..4, IsGrowByte bounds first 1..5). Field order: HPmax MPmax pAtk pDef mAtk mDef invLimit
// mpRegen hitRecovery. Only row set 0 (swordman) is reachable in this build: s.Job/role.Job
// never leave 0 (recon/job-advancement-report.md section 2); rows 1..5 are the standby tables
// of the other five character scripts, same convention as Offline70RewardTypesFG183.AwakeningTable.
// Growth is a DERIVED quantity (level + grow byte + this table); nothing new is persisted, so
// old archives load unchanged. Model: CalcStat.inc:5-33 / calc_lev_stat.inc:6-37 (levels 2..15
// baseRow, 16..50 first-grow branch, 51+ awakening block; the awakening1 blocks were measured
// equal to their growtype rows for every served branch EXCEPT where the manifest flags a
// difference, so growth never changes at awakening).
class Offline70GrowthTableFG200 {
 // [job][grow 0..5] per-level growth rows (fixed-point10).
 internal static readonly int[][][] GrowFG200=new int[][][]{
  new int[][]{new int[]{590,400,69,68,63,62,3000,25,15},new int[]{640,350,71,70,61,60,3000,25,20},new int[]{540,450,56,55,76,75,3000,50,10},new int[]{690,300,76,75,56,55,3000,25,20},new int[]{590,400,66,65,66,65,3000,40,20},new int[]{615,375,71,60,71,60,2700,35,20}}, // swordman  (served job)
  new int[][]{new int[]{640,350,71,70,61,60,2700,25,15},new int[]{540,450,56,55,76,75,2700,50,10},new int[]{665,325,76,75,56,55,2700,30,30},new int[]{615,375,71,60,71,60,2700,35,20},new int[]{690,300,71,80,56,55,2700,30,30},new int[]{0,0,0,0,0,0,0,0,0}}, // fighter
  new int[][]{new int[]{540,450,66,65,66,65,3000,25,10},new int[]{615,375,76,75,56,55,3000,25,20},new int[]{590,400,76,75,56,55,3000,25,20},new int[]{490,500,56,55,76,75,3000,50,10},new int[]{540,450,53,48,53,48,3000,30,20},new int[]{540,450,66,65,66,65,3000,25,10}}, // gunner
  new int[][]{new int[]{490,500,61,60,71,70,2500,40,10},new int[]{440,550,56,55,76,75,2500,50,10},new int[]{465,525,59,57,74,72,2500,50,10},new int[]{540,450,71,60,71,60,2500,40,30},new int[]{490,500,66,60,73,63,2500,50,20},new int[]{615,375,71,60,71,60,2700,35,20}}, // mage
  new int[][]{new int[]{640,350,68,68,61,65,3000,25,10},new int[]{690,300,56,75,56,75,3000,25,20},new int[]{640,350,73,70,60,59,3000,25,20},new int[]{540,450,71,60,71,60,3000,50,10},new int[]{540,450,56,55,76,75,3000,25,20},new int[]{615,375,71,60,71,60,2700,35,20}}, // priest
  new int[][]{new int[]{565,425,71,60,71,60,2700,25,15},new int[]{615,375,74,65,63,60,2700,30,20},new int[]{515,475,68,55,74,65,2700,40,10},new int[]{490,500,66,55,76,65,2700,50,10},new int[]{590,400,76,65,66,55,2700,30,20},new int[]{615,375,71,60,71,60,2700,35,20}}, // thief
};
 // [job] [initial value] growth columns (fixed-point10, same field order).
 internal static readonly int[][] InitialFG200=new int[][]{
  new int[]{1940,1550,96,95,66,65,480000,500,6000}, // swordman
  new int[]{2140,1350,96,95,66,65,430000,500,6000}, // fighter
  new int[]{1740,1750,86,85,76,75,400000,500,6000}, // gunner
  new int[]{1340,2150,66,65,96,95,340000,800,5000}, // mage
  new int[]{2340,1150,86,85,66,85,480000,500,5000}, // priest
  new int[]{1840,1650,96,75,86,65,430000,500,6000}, // thief
};
 // The four remaining wire constants come from the swordman [initial value] block only
 // (elemResist fire/water/dark/light, hpRegen, jump, weight, move/attack/cast speed, the
 // +0x52 byte and the +0x53 float); they never appear in any growtype block (guard in
 // r6_gen_growth.py), which is why Offline70SwordmanGrowthFG could keep them fixed.
 internal static bool SupportedGrowFG200(byte growByte){return growByte==0||Offline70RewardTypesFG183.IsGrowByte(growByte);}
 // Segmented per-level application (rednf): steps s=level-1; i<=14 baseRow, 15<=i<=49 branch,
 // i>=50 awakening (measured equal to the branch rows, so a single branch row is enough).
 internal static int[] GrowthAtFG200(byte job,byte growByte,byte level){
  if(job>=GrowFG200.Length||level<1||level>70||!SupportedGrowFG200(growByte))throw new InvalidDataException("Growth table bounds");
  byte first=Offline70RewardTypesFG183.FirstGrow(growByte);int[] grow=GrowFG200[job][first],baseRow=GrowFG200[job][0],init=InitialFG200[job];int[] values=new int[9];
  int steps=level-1,n1=Math.Min(steps,14),n2=Math.Min(Math.Max(steps-14,0),35),n3=Math.Max(steps-49,0);
  for(int i=0;i<9;i++)values[i]=init[i]+n1*baseRow[i]+(n2+n3)*grow[i];return values;}
}
