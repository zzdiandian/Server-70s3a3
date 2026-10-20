using System;using System.Collections.Generic;
// Condition monster tables derived from PVF [monster condition] tags.
// A condition monster is an extra monster that must be killed before the boss spawns.
// Level = basis + lvl when rel=1 (same as regular monster derivation).
internal static class ConditionMonstersFG {
 internal static readonly Dictionary<ushort,List<ushort[]>> Table =
  new Dictionary<ushort,List<ushort[]>>{
  {35,new List<ushort[]>{new ushort[]{5402,50073,44,0},new ushort[]{5402,50074,44,0},new ushort[]{5402,50075,44,0},new ushort[]{5402,50076,44,0},new ushort[]{5403,50073,44,0},new ushort[]{5403,50074,44,0},new ushort[]{5403,50075,44,0},new ushort[]{5403,50076,44,0},new ushort[]{5404,50073,44,0},new ushort[]{5404,50074,44,0},new ushort[]{5404,50075,44,0},new ushort[]{5404,50076,44,0},new ushort[]{5405,50073,44,0},new ushort[]{5405,50074,44,0},new ushort[]{5405,50075,44,0},new ushort[]{5405,50076,44,0},new ushort[]{5406,50073,44,0},new ushort[]{5406,50074,44,0},new ushort[]{5406,50075,44,0},new ushort[]{5406,50076,44,0},new ushort[]{5407,50073,44,0},new ushort[]{5407,50074,44,0},new ushort[]{5407,50075,44,0},new ushort[]{5407,50076,44,0},new ushort[]{5408,50073,44,0},new ushort[]{5408,50074,44,0},new ushort[]{5408,50075,44,0},new ushort[]{5408,50076,44,0},new ushort[]{5409,50073,44,0},new ushort[]{5409,50074,44,0},new ushort[]{5409,50075,44,0},new ushort[]{5409,50076,44,0},new ushort[]{5410,50073,44,0},new ushort[]{5410,50074,44,0},new ushort[]{5410,50075,44,0},new ushort[]{5410,50076,44,0},new ushort[]{5411,50073,44,0},new ushort[]{5411,50074,44,0},new ushort[]{5411,50075,44,0},new ushort[]{5411,50076,44,0},new ushort[]{5412,50073,44,0},new ushort[]{5412,50074,44,0},new ushort[]{5412,50075,44,0},new ushort[]{5412,50076,44,0},new ushort[]{5413,50073,44,0},new ushort[]{5413,50074,44,0},new ushort[]{5413,50075,44,0},new ushort[]{5413,50076,44,0},new ushort[]{5414,50073,44,0},new ushort[]{5414,50074,44,0},new ushort[]{5414,50075,44,0},new ushort[]{5414,50076,44,0},new ushort[]{5415,50073,44,0},new ushort[]{5415,50074,44,0},new ushort[]{5415,50075,44,0},new ushort[]{5415,50076,44,0},new ushort[]{5416,50073,44,0},new ushort[]{5416,50074,44,0},new ushort[]{5416,50075,44,0},new ushort[]{5416,50076,44,0},new ushort[]{5417,50073,44,0},new ushort[]{5417,50074,44,0},new ushort[]{5417,50075,44,0},new ushort[]{5417,50076,44,0},new ushort[]{5418,50073,44,0},new ushort[]{5418,50074,44,0},new ushort[]{5418,50075,44,0},new ushort[]{5418,50076,44,0},new ushort[]{5419,50073,44,0},new ushort[]{5419,50074,44,0},new ushort[]{5419,50075,44,0},new ushort[]{5419,50076,44,0},new ushort[]{5420,50073,44,0},new ushort[]{5420,50074,44,0},new ushort[]{5420,50075,44,0},new ushort[]{5420,50076,44,0},new ushort[]{5421,50073,44,0},new ushort[]{5421,50074,44,0},new ushort[]{5421,50075,44,0},new ushort[]{5421,50076,44,0},new ushort[]{5422,50073,44,0},new ushort[]{5422,50074,44,0},new ushort[]{5422,50075,44,0},new ushort[]{5422,50076,44,0},new ushort[]{5423,50073,44,0},new ushort[]{5423,50074,44,0},new ushort[]{5423,50075,44,0},new ushort[]{5423,50076,44,0}}},
  {37,new List<ushort[]>{new ushort[]{7015,50097,44,0},new ushort[]{7017,50097,44,0},new ushort[]{7020,50097,44,0},new ushort[]{7022,50097,44,0}}},
 };
 internal static bool HasConditionMonsters(ushort dungeonId){
  return Table.ContainsKey(dungeonId);}
 internal static List<ushort[]> GetConditionMonsters(ushort dungeonId, ushort mapIdx){
  if(!Table.ContainsKey(dungeonId))return new List<ushort[]>();
  var result=new List<ushort[]>();
  foreach(var entry in Table[dungeonId])
   if(entry[0]==mapIdx)result.Add(entry);
  return result;}
 internal static readonly Dictionary<ushort,ushort[][]> HuntConditions =
  new Dictionary<ushort,ushort[][]>{
  {35,new ushort[][]{new ushort[]{50073,1,1},new ushort[]{50074,1,1},new ushort[]{50075,1,1},new ushort[]{50076,1,1}}},
  {37,new ushort[][]{new ushort[]{50097,1,1}}},
 };
 internal static ushort[][] GetHuntConditions(ushort dungeonId){
  ushort[][] rows;return HuntConditions.TryGetValue(dungeonId,out rows)?rows:new ushort[0][];}
 internal static bool DeclaresMonster(ushort dungeonId,ushort mapIdx,ushort monsterId){
  foreach(ushort[] row in GetConditionMonsters(dungeonId,mapIdx))if(row[1]==monsterId)return true;
  return false;}
}