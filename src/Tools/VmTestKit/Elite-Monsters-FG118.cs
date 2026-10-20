using System;using System.IO;using System.Collections.Generic;
// Current CN70 five-value [champion] plus original TW getAddChampionMobCnt.
// Ordinary selected mazes only; named-dungeon lists are absent in this catalog.
class Offline70EliteMonstersFG118 {
 internal static int DungeonBasis(ushort dungeon,byte difficulty){
  if(difficulty>4)throw new InvalidDataException("Champion difficulty");
  int[] values;switch(dungeon){
   case 1:case 2:values=new int[]{0,0,0,0,0};break;
   case 3:values=new int[]{1,2,3,4,5};break;
   case 4:case 5:case 9:values=new int[]{2,4,6,8,10};break;
   case 6:case 7:case 1000:values=new int[]{3,6,9,12,14};break;
   case 8:case 11:case 12:case 13:case 14:case 15:values=new int[]{4,8,12,16,18};break;
   case 17:values=new int[]{6,12,18,24,26};break;
   case 21:values=new int[]{6,12,18,24,26};break;
   case 22:case 23:values=new int[]{8,16,24,32,36};break;
   case 24:values=new int[]{6,9,12,15,18};break;
   case 25:values=new int[]{3,6,9,12,15};break;
   case 26:values=new int[]{7,14,21,28,30};break;
   case 27:values=new int[]{6,9,12,15,18};break;
   case 31:values=new int[]{7,14,21,28,30};break; case 32:values=new int[]{7,14,21,28,30};break; case 33:values=new int[]{0,0,0,0,0};break; case 34:values=new int[]{6,12,18,24,26};break; case 35:values=new int[]{6,12,18,24,26};break; case 36:values=new int[]{6,12,18,24,26};break; case 37:values=new int[]{6,12,18,24,26};break; case 1502:values=new int[]{0,0,0,0,0};break; case 40:values=new int[]{6,12,18,24,26};break; case 41:values=new int[]{0,0,0,0,0};break; case 42:values=new int[]{6,12,18,24,26};break; case 43:values=new int[]{6,12,18,24,26};break; case 44:values=new int[]{6,12,18,24,26};break; case 45:values=new int[]{6,12,18,24,26};break; case 141:values=new int[]{6,12,18,24,26};break; case 1500:values=new int[]{0,0,0,0,0};break; case 1501:values=new int[]{0,0,0,0,0};break; case 50:values=new int[]{6,12,18,24,26};break; case 51:values=new int[]{6,12,18,24,26};break; case 52:values=new int[]{3,6,9,12,9};break; case 53:values=new int[]{3,6,9,12,9};break; case 1506:values=new int[]{3,6,9,12,9};break; case 80:values=new int[]{6,12,18,24,26};break; case 81:values=new int[]{6,12,18,24,26};break; case 82:values=new int[]{6,12,18,24,26};break; case 83:values=new int[]{3,6,9,12,9};break; case 84:values=new int[]{3,6,9,12,9};break; case 85:values=new int[]{3,6,9,12,9};break; case 88:values=new int[]{6,12,18,24,26};break; case 89:values=new int[]{3,6,9,12,9};break; case 90:values=new int[]{6,12,18,24,26};break; case 91:values=new int[]{3,6,9,12,15};break; case 60:values=new int[]{4,8,12,16,18};break; case 61:values=new int[]{4,8,12,16,18};break; case 100:values=new int[]{4,8,12,16,18};break; case 110:values=new int[]{3,6,9,12,14};break; case 111:values=new int[]{3,6,9,12,14};break; case 112:values=new int[]{3,6,9,12,14};break; case 86:values=new int[]{3,6,9,12,9};break; case 87:values=new int[]{3,6,9,12,9};break; case 92:values=new int[]{3,6,9,12,9};break; case 93:values=new int[]{3,6,9,12,9};break; case 140:values=new int[]{6,12,18,24,26};break; case 1504:values=new int[]{0,0,0,0,0};break; case 3503:values=new int[]{0,0,0,0,0};break; case 500:values=new int[]{6,12,18,24,26};break; case 503:values=new int[]{3,6,9,12,14};break; case 504:values=new int[]{4,8,12,16,18};break; case 506:values=new int[]{3,6,9,12,14};break; case 514:values=new int[]{3,6,9,12,14};break; default:throw new InvalidDataException("Unproven champion dungeon");
  }return values[difficulty];
 }
 internal static int Quota(int basis,byte difficulty,int width,int height,int fixedCount,bool noRandom,int draw){
  if(basis<0||basis>1000||difficulty>4||width<1||width>16||height<1||height>16||fixedCount<0||fixedCount>64||draw<0||draw>=100)throw new InvalidDataException("Champion quota input");
  int adjusted=basis;if(difficulty==1)adjusted=basis*150/100;else if(difficulty==2)adjusted=basis*250/100;else if(difficulty==3)adjusted=basis*500/100;
  return fixedCount+(!noRandom&&100*adjusted/(width*height)>draw?1:0);
 }
 internal static int FixedCount(ushort map){return map==1411||map==1417||map==2814?2:0;}
 internal static bool Eligible(ushort code,ushort kind){return kind==0&&!(code>=50000&&code<=51000||code>=56000&&code<=60000);}
}
partial class Offline70DungeonFlowFG {
 readonly Dictionary<int,ushort[][]> eliteRoomsFG118=new Dictionary<int,ushort[][]>();
 internal int MazeWidthFG118 {get{return DungeonId==1?1:DungeonId==2?3:Offline70ForestFG75.Width(DungeonId);}}
 internal int MazeHeightFG118 {get{return DungeonId==1?3:DungeonId==2?3:Offline70ForestFG75.Height(DungeonId);}}
 void ResetElitesFG118(){eliteRoomsFG118.Clear();}
 ushort[][] RoomMonstersFG118(){
  ushort[][] cached;if(eliteRoomsFG118.TryGetValue(RoomIndex,out cached))return cached;
  ushort[][] original;
  if(DungeonId==1){original=new ushort[Counts[Y]][];for(int i=0;i<original.Length;i++)original[i]=new ushort[]{(ushort)(IsBossRoom&&i==4?2:1),1,(ushort)(IsBossRoom&&i==4?3:0)};}
  else original=GsdMonstersFG130()??(DungeonId==2?Offline70DeepLorienFG72.Monsters(X,Y):Offline70ForestFG75.Monsters(DungeonId,X,Y));
  // The 37 boss room keeps a stand-in normal boss (50098 kind 3) in the PVF-derived table so
  // the APC fallback stays available; when the real 刹影 APC is active it must not also spawn.
  if(BossApcRoomFG176){List<ushort[]> keep=new List<ushort[]>();foreach(ushort[] row in original)if(!(row[0]==50098&&row[2]==3))keep.Add(row);original=keep.ToArray();}
  cached=new ushort[original.Length][];List<int> candidates=new List<int>();
  for(int i=0;i<original.Length;i++){cached[i]=(ushort[])original[i].Clone();if(Offline70EliteMonstersFG118.Eligible(cached[i][0],cached[i][2]))candidates.Add(i);}
  Func<int,int> draw=Offline70QuestDropsFG.StableDraws(RunId+"-elite-room"+RoomIndex);
  int quota=Offline70EliteMonstersFG118.Quota(Offline70EliteMonstersFG118.DungeonBasis(DungeonId,Difficulty),Difficulty,MazeWidthFG118,MazeHeightFG118,Offline70EliteMonstersFG118.FixedCount(CurrentMap),false,draw(100));
  for(int i=0;i<quota&&candidates.Count>0;i++){int chosen=draw(candidates.Count),index=candidates[chosen];candidates.RemoveAt(chosen);cached[index][2]=1;}
  eliteRoomsFG118.Add(RoomIndex,cached);return cached;
 }
}
