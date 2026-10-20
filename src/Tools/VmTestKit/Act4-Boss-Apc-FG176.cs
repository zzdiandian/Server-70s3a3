using System;using System.IO;using System.Collections.Generic;
// Act4 ShadowMaze (dungeon 37) has no [monster] boss: map/shadowmaze/b1..b4.map carry
//   [ai character]
//       1301 <x> <y> 0 `[monster]` `[boss]` <cx> <cy>
//   [/ai character]
// with an empty [monster] section, and the .dgn boss cell (2,0) resolves to b1.map
// ([greed] "EE EE" -> gridValue 4 = the cell's mask 4). The native server spawns that
// actor as an APC and the client reports its death like the GSD quest APC, so the room
// body uses the same APC slot (uid 900) that Dungeon-QuestCompanion-FG130 already writes.
// Level 47 is the [minimum info] level of aicharacter/swordman/Sayoung/Sayoung.aic
// (apc id 1301); the role byte mirrors the APC roleType table (5 normal / 6 champion /
// 7 super champion / 8 boss) used by the GSD write, which sends 5 for `[normal]`.
//
// r-apc (2026-10-17): the same side channel is what every other enemy APC needs, because a
// [ai character] actor has no .mob drop profile and the room roster (MapRolls/MonsterPools) has no
// pool for it -- putting it in the roster would make Floor-Drops-FG119 throw "Floor monster profile"
// / "Floor map roster". So the d37-only hard coding is generalised to the generated
// ApcMonstersFG table ((dungeon, map) -> {apcId, level, roleNum}), one side-channel slot per
// declared actor, addressed by a per-run/per-room registry (ApcSlotFG186). The shipped d37 fields
// (BossApc*FG176, ApcSpawnedUidFG176, KillBossApcFG176) stay authoritative for the 刹影 row so its
// room bytes, its clear handshake and its A/B switches do not move; every other row is served by
// the registry. [special passive object] rows are never in the table, so destruction objectives
// (e.g. [hunt enemy] 4471 monster 815) stay outside this path.
partial class Offline70DungeonFlowFG {
 // Native 0x8301d76 add_monster_APC_AI assigns mob.m_instId = seq++, the SAME counter that
 // numbers the room's monsters, so an APC instance id sits in the monster range (100..163).
 // The client keys actors by that id: an APC sent as 900 is never death-reported (0 op42 in a
 // whole shadowmaze run) and the clear/score flow never starts. Legacy boss 900 stays accepted only inside its boss room; quest GSD keeps its own900 UID.
 internal ushort ApcUidFG176 {get{return (ushort)(100+TotalCount);}}   // first APC slot, after every actor incl. condition/event monsters
 internal ushort ApcSpawnedUidFG176;
 // The uid of the index-th side-channel APC of the current room. It deliberately sits ABOVE
 // 100+TotalCount, i.e. outside ValidateKill/IsDead's monster range: that is what keeps these
 // actors out of the drop planner and out of RoomCleared. It must never be folded into TotalCount.
 internal ushort ApcUidFG186(int index){return (ushort)(100+TotalCount+index);}
 // One written side-channel APC actor. Registered when RoomBody() emits it, so the uid -> id
 // mapping survives room changes (a later uid 104 in another room is a different actor).
 internal sealed class ApcSlotFG186{internal string RunId;internal int Room;internal ushort Uid,Id,Level;internal byte Role;internal bool Spawned,Dead;}
 readonly List<ApcSlotFG186> apcSlotsFG186=new List<ApcSlotFG186>();
 internal bool ApcRoutingFG176(ushort uid){return ApcRegisteredFG186(uid);}
 internal void ApcArmFG176(){ApcSpawnedUidFG176=ApcUidFG176;}
 // 2026-10-15 (second pass): the client-side roleType>=8 theory was refuted and the
 // silent exit funnel was located (secure-CRT invalid-parameter handler 0x128E513 ->
 // TerminateProcess 0xC0000417); the death notification is now byte-faithful to rednf
 // _MakeNotiPacketDieMonster, which writes the APC kill count (not -1) for boss roles.
 // Default is the real 刹影 APC; set DNF70_ACT4_APC_BOSS=0 to fall back to the stand-in
 // normal boss (50098 kind 3) without a rebuild.
 internal static readonly bool Act4ApcBossEnabled=Environment.GetEnvironmentVariable("DNF70_ACT4_APC_BOSS")!="0";
 internal const ushort BossApcIdFG176=1301;
 internal const byte BossApcLevelFG176=47;
 // A/B switch for the client-side roleType>=8 branch (0x00935CF8 cmp [esi+0x1d8],8):
 // DNF70_ACT4_APC_ROLE=5|6|7|8 overrides the emitted role byte (default 8 = [boss]).
 internal static readonly byte BossApcRoleFG176=ApcRoleFromEnvironmentFG176();
 static byte ApcRoleFromEnvironmentFG176(){byte role;string value=Environment.GetEnvironmentVariable("DNF70_ACT4_APC_ROLE");return value!=null&&byte.TryParse(value,out role)&&role>=5&&role<=8?role:(byte)8;}
 string bossApcRunFG176;bool bossApcSpawnedFG176,bossApcDeadFG176;
 // RunId identifies the run, so a fresh Begin() clears the state without a hook there.
 internal bool BossApcRoomFG176 {get{if(!Act4ApcBossEnabled)return false;return Active&&DungeonId==37&&IsBossRoom;}}
 internal bool BossApcPendingFG176 {get{return BossApcRoomFG176&&bossApcRunFG176!=RunId;}}
 internal bool BossApcAliveFG176 {get{return BossApcRoomFG176&&bossApcRunFG176==RunId&&bossApcSpawnedFG176&&!bossApcDeadFG176;}}
 internal bool BossApcSlainFG176 {get{return BossApcRoomFG176&&bossApcRunFG176==RunId&&bossApcDeadFG176;}}
 internal void BossApcSpawnFG176(){bossApcRunFG176=RunId;bossApcSpawnedFG176=true;bossApcDeadFG176=false;}
 internal bool KillBossApcFG176(ushort uid){if(!Active||!Loaded||ClearPublished||!ApcRoutingFG176(uid)||bossApcRunFG176!=RunId||!bossApcSpawnedFG176)throw new InvalidDataException("Act4 boss APC death outside its spawned boss room");if(bossApcDeadFG176)return false;bossApcDeadFG176=true;return true;}
 // ---- generalised side-channel registry (r-apc) ----------------------------------------------
 internal bool BossApcSpawnedThisRunFG176 {get{return bossApcRunFG176==RunId&&bossApcSpawnedFG176;}}
 // The APC actors declared for the current room, in wire order. d37's 刹影 keeps its dedicated
 // boss-room gate (so DNF70_ACT4_APC_BOSS and the elite-table strip in Elite-Monsters-FG118 keep
 // working unchanged); every other declared actor spawns wherever its map is used, as PVF says.
 internal List<ushort[]> CurrentApcMonsters(){
  List<ushort[]> result=new List<ushort[]>();
  if(!Active||DungeonId<3)return result;
  foreach(ushort[] row in ApcMonstersFG.GetApcMonsters(DungeonId,CurrentMap))
   if(row[1]!=BossApcIdFG176||BossApcRoomFG176)result.Add(row);
  return result;
 }
 // roleNum 8 is the boss role the Act4 A/B switch was built for, so a boss-role row honours it.
 internal static byte ApcRoleByteFG186(ushort[] row){return row[3]==8?BossApcRoleFG176:(byte)row[3];}
 ApcSlotFG186 FindApcSlotFG186(ushort uid){
  foreach(ApcSlotFG186 s in apcSlotsFG186)if(s.RunId==RunId&&s.Room==RoomIndex&&s.Uid==uid&&s.Spawned)return s;
  return null;
 }
 internal bool ApcRegisteredFG186(ushort uid){
  // shipped legacy: the Act4 boss room also accepts the quest GSD uid as the boss APC's death key
  if(uid==GsdUidFG130)return BossApcRoomFG176&&BossApcSpawnedThisRunFG176;
  return FindApcSlotFG186(uid)!=null;
 }
 internal bool ApcPendingFG186(ushort uid){
  if(uid==GsdUidFG130)return false;
  return FindApcSlotFG186(uid)==null;
 }
 internal bool ApcAliveFG186(ushort uid){
  if(uid==GsdUidFG130)return BossApcAliveFG176;
  ApcSlotFG186 s=FindApcSlotFG186(uid);return s!=null&&!s.Dead;
 }
 // the hunted monster id this uid stands for, or 0 when the uid is no registered APC
 internal ushort ApcIdAtUid(ushort uid){
  if(uid==GsdUidFG130)return BossApcRoomFG176&&BossApcSpawnedThisRunFG176?BossApcIdFG176:(ushort)0;
  ApcSlotFG186 s=FindApcSlotFG186(uid);return s==null?(ushort)0:s.Id;
 }
 internal byte ApcRoleAtUid(ushort uid){
  if(uid==GsdUidFG130)return BossApcRoleFG176;
  ApcSlotFG186 s=FindApcSlotFG186(uid);return s==null?(byte)0:s.Role;
 }
 internal bool ApcIsBossChannelFG186(ushort uid){return BossApcRoomFG176&&(uid==ApcSpawnedUidFG176||uid==GsdUidFG130);}
 // RoomBody() writes the actor and registers it here; the 刹影 row keeps driving the shipped
 // single-slot fields so BossApcAliveFG176/SlainFG176/BossKilled() stay exactly as before.
 internal void ApcRegisterSpawnFG186(ushort uid,ushort[] row){
  if(apcSlotsFG186.Count>=1024)throw new InvalidDataException("APC slot registry bounds");
  apcSlotsFG186.Add(new ApcSlotFG186{RunId=RunId,Room=RoomIndex,Uid=uid,Id=row[1],Level=row[2],Role=ApcRoleByteFG186(row),Spawned=true});
  if(row[1]==BossApcIdFG176){BossApcSpawnFG176();ApcArmFG176();}
 }
 // true only for the first death report of that uid in this run (replays return false), which is
 // what makes the quest kill feed write-once without any extra flag.
 internal bool KillApcFG186(ushort uid){
  if(uid==GsdUidFG130||BossApcRoomFG176&&uid==ApcSpawnedUidFG176)return KillBossApcFG176(uid);
  if(!Active||!Loaded||ClearPublished)throw new InvalidDataException("APC death outside its spawned room");
  ApcSlotFG186 slot=FindApcSlotFG186(uid);
  if(slot==null)throw new InvalidDataException("APC death outside its spawned room");
  if(slot.Dead)return false;
  slot.Dead=true;return true;
 }
 // uids of the APCs currently alive in this room, for the room sweep test aid
 internal List<ushort> ApcAliveUidsFG186(){
  List<ushort> alive=new List<ushort>();
  foreach(ApcSlotFG186 s in apcSlotsFG186)if(s.RunId==RunId&&s.Room==RoomIndex&&s.Spawned&&!s.Dead)alive.Add(s.Uid);
  return alive;
 }
}
