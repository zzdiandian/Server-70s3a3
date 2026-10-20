using System;using System.IO;
// Grow 0 reproduces the client's [initial value]+[growtype 1] curve byte for byte (verified at
// every level 1..70); a promoted character gets its own [growtype firstGrow+1] row from
// Offline70GrowthTableFG200, matching the native segmented CalcStat model. Equipment bonuses and
// the remaining eleven wire columns stay fixed (they never appear in any growtype block).
class Offline70SwordmanGrowthFG {
 internal static byte[] Stats(byte growByte,byte level){if(level<1||level>70||!Offline70GrowthTableFG200.SupportedGrowFG200(growByte))throw new InvalidDataException("Swordman growth bounds");int[] g=Offline70GrowthTableFG200.GrowthAtFG200(0,growByte,level);using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){
  w.Write((uint)g[0]);w.Write((uint)g[1]);w.Write((short)g[2]);w.Write((short)g[3]);w.Write((short)g[4]);w.Write((short)g[5]);
  w.Write((short)120);w.Write((short)120);w.Write((short)320);w.Write((short)-80);for(int i=0;i<17;i++)w.Write((short)0);w.Write((uint)g[6]);w.Write((short)0);w.Write((short)g[7]);w.Write(Offline70TestRatesFG114.Movement(8700));w.Write((ushort)8500);w.Write((ushort)7000);w.Write((short)g[8]);w.Write((short)4300);w.Write((uint)680000);w.Write((byte)100);w.Write((float)0);
  byte[] b=m.ToArray();if(b.Length!=87)throw new InvalidDataException("Native stat record bounds");return b;
 }}
}
