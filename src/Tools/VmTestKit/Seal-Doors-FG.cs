using System;using System.Collections.Generic;
// Seal door tables derived from PVF [seal door ...] tags.
// A seal door is a special room transition when hell party is active at a specific grid position.
internal static class SealDoorsFG {
 internal static readonly Dictionary<ushort,ushort[]> Table =
  new Dictionary<ushort,ushort[]>{
  {33,new ushort[]{60026,1,0,10}},
  {36,new ushort[]{60020,3,2,1}},
  {1502,new ushort[]{60036,3,0,10}},
  {44,new ushort[]{60029,2,1,10}},
  {45,new ushort[]{60031,1,0,10}},
  {1500,new ushort[]{60038,4,0,10}},
  {1501,new ushort[]{60037,1,0,10}},
  {50,new ushort[]{60018,1,3,1}},
  {51,new ushort[]{60019,3,0,1}},
  {52,new ushort[]{60048,0,2,10}},
  {53,new ushort[]{60049,3,1,10}},
  {1506,new ushort[]{60053,0,1,10}},
  {80,new ushort[]{60034,3,3,10}},
  {81,new ushort[]{60033,2,0,10}},
  {82,new ushort[]{60035,0,0,10}},
  {83,new ushort[]{60045,3,0,10}},
  {84,new ushort[]{60046,5,1,10}},
  {85,new ushort[]{60047,3,1,10}},
  {88,new ushort[]{60054,1,1,10}},
  {89,new ushort[]{60055,1,1,10}},
  {60,new ushort[]{60044,3,2,10}},
  {61,new ushort[]{60043,4,0,10}},
  {110,new ushort[]{60040,1,0,10}},
  {111,new ushort[]{60041,2,1,10}},
  {112,new ushort[]{60042,1,1,10}},
  {86,new ushort[]{60050,8,0,10}},
  {87,new ushort[]{60051,6,0,10}},
  {92,new ushort[]{60056,2,2,10}},
  {93,new ushort[]{60057,2,2,10}},
  {140,new ushort[]{60039,3,1,10}},
  {1504,new ushort[]{60052,1,0,10}},
 };
 internal static bool HasSealDoor(ushort dungeonId){
  return Table.ContainsKey(dungeonId);}
 internal static ushort GetSealDoorMapIdx(ushort dungeonId){
  return Table.ContainsKey(dungeonId)?Table[dungeonId][0]: (ushort)0;}
 internal static byte GetSealDoorPosX(ushort dungeonId){
  return Table.ContainsKey(dungeonId)? (byte)Table[dungeonId][1]: (byte)0;}
 internal static byte GetSealDoorPosY(ushort dungeonId){
  return Table.ContainsKey(dungeonId)? (byte)Table[dungeonId][2]: (byte)0;}
 internal static ushort GetSealDoorRate(ushort dungeonId){
  return Table.ContainsKey(dungeonId)?Table[dungeonId][3]: (ushort)0;}
}