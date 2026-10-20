using System;using System.Collections.Generic;using System.IO;using System.Xml;using System.Xml.Serialization;
// Current70 offline diagnostic Lorien route: (0,0) map3, (0,1) map5, (0,2) map13.
// Exact current-PVF populations are retained. It is not an all-dungeon catalog.
partial class Offline70DungeonFlowFG {
 // Explicit diagnostic awards for the bounded Lorien route. These are not the
 // production PVF/server clear-reward formula and must not be presented as one.
 internal const uint DiagnosticClearExperience=100,DiagnosticClearGold=50;
 internal ushort DungeonId=1;internal string RunId;internal byte X,Y,Difficulty;internal bool Active,Loaded,ClearPublished;
 bool movePending;
 readonly HashSet<byte> visitedRooms=new HashSet<byte>();
 // Boss room entrance condition (PVF [boss room entrance condition], native check_boss_map):
 // one status per hunted monster id. Native SetGridPath picks spawnCnt random candidate rooms
 // at dungeon start (candidate = every candidate map of that cell declares the monster) and
 // merges that room's [monster condition] rows into the mob list; a kill of the monster id
 // anywhere increments the counter, and the boss cell refuses entry until every counter
 // reached maxKillCnt (check_boss_map returns 7 otherwise).
 internal sealed class ConditionStatusFG181{internal ushort MonsterId;internal List<int> Cells=new List<int>();internal int MaxKill,CurKill;}
 readonly List<ConditionStatusFG181> conditionStatusFG181=new List<ConditionStatusFG181>();
 internal bool BossRoomUnlockedFG181{get{foreach(ConditionStatusFG181 s in conditionStatusFG181)if(s.CurKill<s.MaxKill)return false;return true;}}
 // Event monsters: PVF [event monster]/[event monster3] rows are rolled once per room and the
 // monster ids are kept per room, so uids stay in the same 100..163 numbering as the room's
 // other actors (native: one shared instance counter). The run total is capped by the table's
 // attempt counts, which is what "1-2 event monsters per dungeon" means in the PVF.
 readonly Dictionary<int,List<ushort>> eventRoomFG174=new Dictionary<int,List<ushort>>();
 int eventSpawnedRunFG174;
 // Native SetGridPath rolls [event monster] only while the repeat event 0x24 is active
 // (CEventManager::GetRepeatEvent(0x24)->IsEventing), and the [event monster3] roster is a
 // China event monster list (chn_golden_goblin_junior). This server has no event manager, so
 // both tables stay off unless DNF70_EVENT_MONSTERS=1 is set explicitly - otherwise every
 // dungeon run happily spawns golden goblins in the first rooms (live report 2026-10-15).
 internal static readonly bool EventMonstersEnabledFG182=Environment.GetEnvironmentVariable("DNF70_EVENT_MONSTERS")=="1";
 internal int DiagnosticFatigueUsed {get{return visitedRooms.Count;}}
 internal int CardClearedRoomsFG125 {get{if(!ReadyToClear)throw new InvalidDataException("Card exploration requires boss clear");return visitedRooms.Count;}}
 internal int CardTotalRoomsFG125 {get{int total=0;if(DungeonId==1)return Maps.Length;if(DungeonId==2){for(byte y=0;y<3;y++)for(byte x=0;x<3;x++)if(Offline70DeepLorienFG72.Exists(x,y))total++;}else for(byte y=0;y<Offline70ForestFG75.Height(DungeonId);y++)for(byte x=0;x<Offline70ForestFG75.Width(DungeonId);x++)if(MapFG130(x,y)!=0)total++;if(total<1)throw new InvalidDataException("Card maze count");return total;}}
 readonly System.Diagnostics.Stopwatch clearTimer=new System.Diagnostics.Stopwatch();
 readonly System.Diagnostics.Stopwatch settlementTimer=new System.Diagnostics.Stopwatch();
 internal const int AutoReturnSeconds=30;
 internal bool AutoReturnDue {get{return Active&&ClearPublished&&settlementTimer.IsRunning&&settlementTimer.ElapsedMilliseconds>=AutoReturnSeconds*1000;}}
 internal uint ClearMilliseconds {get{return (uint)Math.Min(86400000L,clearTimer.ElapsedMilliseconds);}}
 // Original C03D40 treats any zero member word as end eligibility reason1;
 // 105EF50 then displays resource1DB (a member has zero fatigue). This is
 // independent of card selection. The bounded single-player three-room Lorien
 // uses the persisted ordinary fatigue balance; unused members remain FFFF.
 // Other dungeon costs and premium fatigue remain outside this route.
 internal byte[] EndEligibilityBody(int remaining){if(remaining<0||remaining>188)throw new InvalidDataException("Fatigue eligibility bounds");if(!Active||!ClearPublished||!IsBossRoom)throw new InvalidDataException("End eligibility requires completed Lorien");return new byte[]{1,(byte)remaining,0,255,255,255,255,255,255};}
 internal int VerifiedKillCount,VerifiedChampionCount,VerifiedSuperChampionCount,VerifiedBossCount;
 readonly HashSet<ushort>[] dead=new HashSet<ushort>[256];
 internal Offline70DungeonFlowFG(){for(int i=0;i<dead.Length;i++)dead[i]=new HashSet<ushort>();}
 internal int RoomIndex {get{return DungeonId==1?Y:DungeonId==2?Y*3+X:Y*Offline70ForestFG75.Width(DungeonId)+X;}}
 internal byte BossX {get{if(GsdRunFG130&&DungeonId==12)return 0;return DungeonId>=3?Offline70ForestFG75.BossX(DungeonId):(byte)(DungeonId==1?0:2);}}
 internal byte BossY {get{if(GsdRunFG130&&DungeonId==12)return 1;return DungeonId>=3?Offline70ForestFG75.BossY(DungeonId):(byte)(DungeonId==1?2:1);}}
 internal bool IsBossRoom {get{return X==BossX&&Y==BossY;}}
 // Native28 stores bytes4/5 as the minimap boss position. Use the same
 // coordinates as combat/clear admission, including re-entry after retry.
 internal byte[] EntryBody(){if(!Active)throw new InvalidDataException("No active dungeon entry");byte[] body=new byte[12];BitConverter.GetBytes(DungeonId).CopyTo(body,0);body[2]=Difficulty;body[3]=(byte)(GsdRunFG130?1:0);body[4]=BossX;body[5]=BossY;return body;}
 internal ushort CurrentMap {get{if(SealDoorOpenFG181&&IsSealDoor(X,Y))return SealDoorMapIdx;return DungeonId==1?Maps[Y]:DungeonId==2?Offline70DeepLorienFG72.Map(X,Y):MapFG130(X,Y);}}
 internal int CurrentCount {get{return RoomMonstersFG118().Length;}}
 // Total monster count including condition and event monsters.
 internal int TotalCount {get{int total=CurrentCount;total+=CurrentConditionMonsters().Count;total+=EventIdsForCurrentRoomFG174().Count;return total;}}
 internal ushort[] Monster(ushort uid){int i=uid-100;if(i>=0&&i<CurrentCount)return RoomMonstersFG118()[i];if(i>=CurrentCount){int ci=i-CurrentCount;var conds=CurrentConditionMonsters();if(ci>=0&&ci<conds.Count){var c=conds[ci];return new ushort[]{c[1],c[2],c[3]};}var evs=EventIdsForCurrentRoomFG174();if(i>=0&&i-CurrentCount-CurrentConditionMonsters().Count<evs.Count){return new ushort[]{evs[i-CurrentCount-CurrentConditionMonsters().Count],(byte)Offline70ClearExperienceFG116.Basis(DungeonId),(byte)0};}}throw new InvalidDataException("Room monster bounds");}
 // Condition monsters spawning in the current room: the room's own [monster condition] rows,
 // filtered to the run's rolled spawn rooms (native add_monster_list_bossroom_condition).
 internal List<ushort[]> CurrentConditionMonsters(){
  List<ushort[]> result=new List<ushort[]>();
  if(!Active||DungeonId<3||conditionStatusFG181.Count==0)return result;
  List<ushort[]> declared=ConditionMonstersFG.GetConditionMonsters(DungeonId,CurrentMap);
  if(declared.Count==0)return result;
  int cell=Y*Offline70ForestFG75.Width(DungeonId)+X;
  foreach(ConditionStatusFG181 status in conditionStatusFG181){
   int instances=0;foreach(int c in status.Cells)if(c==cell)instances++;
   for(int k=0;k<instances;k++)foreach(ushort[] row in declared)if(row[1]==status.MonsterId)result.Add(row);
  }
  return result;
 }
 // Native SetGridPath: candidates are non-start/non-boss cells whose maps declare the hunted
 // monster; spawnCnt of them are drawn at random (duplicates allowed, as in the original).
 void BuildConditionStatusFG181(){
  conditionStatusFG181.Clear();
  if(DungeonId<3)return;
  ushort[][] hunt=ConditionMonstersFG.GetHuntConditions(DungeonId);
  if(hunt.Length==0)return;
  int width=Offline70ForestFG75.Width(DungeonId),height=Offline70ForestFG75.Height(DungeonId);
  byte startX=Offline70ForestFG75.StartX(DungeonId),startY=Offline70ForestFG75.StartY(DungeonId);
  Random roll=new Random(unchecked(Environment.TickCount*17+DungeonId*101+Difficulty));
  foreach(ushort[] triple in hunt){
   ConditionStatusFG181 status=new ConditionStatusFG181{MonsterId=triple[0],MaxKill=triple[2]};
   List<int> candidates=new List<int>();
   for(byte y=0;y<height;y++)for(byte x=0;x<width;x++){
    if(x==BossX&&y==BossY||x==startX&&y==startY)continue;
    ushort map=MapFG130(x,y);
    if(map==0)continue;
    if(ConditionMonstersFG.DeclaresMonster(DungeonId,map,status.MonsterId))candidates.Add(y*width+x);
   }
   int spawnCount=Math.Max(1,(int)triple[1]);
   for(int k=0;k<spawnCount&&candidates.Count>0;k++)status.Cells.Add(candidates[roll.Next(candidates.Count)]);
   conditionStatusFG181.Add(status);
  }
 }
 internal bool BossConditionGateFG181(byte x,byte y){return x==BossX&&y==BossY&&!BossRoomUnlockedFG181;}
 internal bool ConditionMonstersCleared(){return BossRoomUnlockedFG181;}
 // Event monster generation: roll probability and spawn if successful.
 // Returns list of spawned event monster UIDs (empty if none spawned).

 internal List<ushort> RollEventMonsters(){
  var spawned=new List<ushort>();
  if(!Active)return spawned;   // the room packet is composed before the client confirms the load
  if(!EventMonstersEnabledFG182)return spawned;   // event monsters need an active event (native 0x24)
  if(eventRoomFG174.ContainsKey(RoomIndex))return eventRoomFG174[RoomIndex];   // once per room
  // PVF [event monster] row: <attempts> <triggerProb/10000> then tuples
  // <eventIdx> <spawnCount> <weight> <field0c> <field10> (rednf STDungeonScript 0x766 read order;
  // 61xxx is the monster id the client spawns for that entry, spawnCount is m_monsterIdx).
  Random roll=new Random(unchecked(Environment.TickCount*31+DungeonId*7+RoomIndex));
  int cap=EventCapFG174();
  // Native add_monster_event caps the room at the room's own monster count (spawned<m_monsterMap.size()).
  int roomCap=Math.Min(cap,CurrentCount);
  foreach(ushort[][] table in new ushort[][][]{EventMonstersFG.EventTable.ContainsKey(DungeonId)?EventMonstersFG.EventTable[DungeonId]:new ushort[0][],
                                               EventMonstersFG.EventTable3.ContainsKey(DungeonId)?EventMonstersFG.EventTable3[DungeonId]:new ushort[0][]}){
   foreach(ushort[] row in table){
    if(row.Length<7)continue;
    int attempts=row[0],trigger=row[1],spawnCount=Math.Max(1,(int)row[3]);
    for(int attempt=0;attempt<attempts;attempt++){
     if(eventSpawnedRunFG174>=cap||spawned.Count>=roomCap)break;
     if(roll.Next(10000)>=trigger)continue;
     for(int i=0;i<spawnCount&&eventSpawnedRunFG174<cap&&spawned.Count<roomCap;i++){spawned.Add(row[2]);eventSpawnedRunFG174++;}
    }
   }
  }
  eventRoomFG174[RoomIndex]=spawned;
  return spawned;
 }
 int EventCapFG174(){
  int cap=0;
  foreach(Dictionary<ushort,ushort[][]> table in new Dictionary<ushort,ushort[][]>[]{EventMonstersFG.EventTable,EventMonstersFG.EventTable3}){
   ushort[][] rows;
   if(!table.TryGetValue(DungeonId,out rows))continue;
   foreach(ushort[] row in rows)if(row.Length>=7)cap+=row[0];
  }
  return cap;
 }
 List<ushort> EventIdsForCurrentRoomFG174(){List<ushort> ids;return eventRoomFG174.TryGetValue(RoomIndex,out ids)?ids:emptyEventsFG174;}
 static readonly List<ushort> emptyEventsFG174=new List<ushort>();
 // Check if a position is a seal door for this dungeon.
 internal bool IsSealDoor(byte x,byte y){
  if(!SealDoorsFG.HasSealDoor(DungeonId))return false;
  return x==SealDoorsFG.GetSealDoorPosX(DungeonId)&&y==SealDoorsFG.GetSealDoorPosY(DungeonId);
 }
 // Get the seal door map index for this dungeon (0 if none).
 internal ushort SealDoorMapIdx{get{return SealDoorsFG.GetSealDoorMapIdx(DungeonId);}}
 // [seal door map index]/[seal door pos]/[seal door appear rate] route the seal door cell into
 // the hell room (kingsruins 60026 / underfoot 60020) only when (a) the run rolls the appear
 // rate and (b) hell-party mode is active — native dungeon_start also consumes seal door items
 // for every member. This server does not model hell-party mode yet, so the door stays closed
 // and the cell keeps its own map; the roll is kept so the wiring is ready and documented.
 internal static readonly bool HellPartyModeFG181=false;
 bool sealDoorOpenFG181;
 internal bool SealDoorOpenFG181{get{return sealDoorOpenFG181;}}
 void RollSealDoorFG181(){
  sealDoorOpenFG181=false;
  if(!SealDoorsFG.HasSealDoor(DungeonId))return;
  int rate=SealDoorsFG.GetSealDoorRate(DungeonId);
  Random roll=new Random(unchecked(Environment.TickCount*29+DungeonId*13+Difficulty));
  if(roll.Next(100)<rate)sealDoorOpenFG181=HellPartyModeFG181;
 }
 internal static readonly ushort[] Maps=new ushort[]{3,5,13};internal static readonly int[] Counts=new int[]{4,5,7};
 internal bool RoomCleared {get{return Active&&dead[RoomIndex].Count>=TotalCount;}}
 // The ordinary Lorien route ends on its only boss, not on unreported minions.
 internal bool ReadyToClear {get{return Active&&Loaded&&BossKilled();}}
 internal string RoomReceiptScope {get{return RunId+"-room"+RoomIndex;}}
 internal void Begin(){Begin(DungeonId,Difficulty);}
 internal void Begin(ushort id){Begin(id,0);}
 internal void Begin(ushort id,byte difficulty){if(id<1||id>2&&!Offline70ForestFG75.Supported(id)||difficulty>Offline70DungeonSelection.TestMaximumDifficulty)throw new InvalidDataException("Unsupported dungeon or difficulty");if(Active)throw new InvalidDataException("A dungeon run is already active");DungeonId=id;Difficulty=difficulty;RunId=Guid.NewGuid().ToString("N");X= id>=3?Offline70ForestFG75.StartX(id):(byte)0;Y=id>=3?Offline70ForestFG75.StartY(id):(byte)(id==1?0:2);Active=true;Loaded=false;movePending=false;ClearPublished=false;clearTimer.Reset();settlementTimer.Reset();visitedRooms.Clear();ResetElitesFG118();VerifiedKillCount=VerifiedChampionCount=VerifiedSuperChampionCount=VerifiedBossCount=0;foreach(HashSet<ushort> set in dead)set.Clear();BuildConditionStatusFG181();RollSealDoorFG181();eventRoomFG174.Clear();eventSpawnedRunFG174=0;}
 internal void MarkLoaded(){if(!Active||Loaded)throw new InvalidDataException("Dungeon load state");Loaded=true;movePending=false;visitedRooms.Add((byte)RoomIndex);if(!clearTimer.IsRunning)clearTimer.Start();}
 internal void ValidateKill(ushort uid){if(!Active||!Loaded||ClearPublished||ReadyToClear)throw new InvalidDataException("Dungeon actor state");if(uid<100||uid>=100+TotalCount)throw new InvalidDataException("Dungeon actor UID");}
 internal bool IsDead(ushort uid){if(!Active)throw new InvalidDataException("Dungeon actor query state");if(uid<100||uid>=100+TotalCount)throw new InvalidDataException("Dungeon actor query UID");return dead[RoomIndex].Contains(uid);}
 internal bool Kill(ushort uid){if(IsDead(uid))return false;ValidateKill(uid);if(!dead[RoomIndex].Add(uid))return false;if(uid>=100+CurrentCount){int ci=uid-100-CurrentCount;List<ushort[]> conditions=CurrentConditionMonsters();if(ci<conditions.Count)foreach(ConditionStatusFG181 status in conditionStatusFG181)if(status.MonsterId==conditions[ci][1]&&status.CurKill<status.MaxKill)status.CurKill++;}VerifiedKillCount++;byte kind=(byte)Monster(uid)[2];if(kind==1)VerifiedChampionCount++;else if(kind==2)VerifiedSuperChampionCount++;else if(kind==3)VerifiedBossCount++;return true;}
 internal bool IsBoss(ushort uid){return Active&&IsBossRoom&&Monster(uid)[2]==3;}
 bool BossKilled(){if(BossApcRoomFG176)return BossApcSlainFG176;if(!IsBossRoom)return false;if(!BossRoomUnlockedFG181)return false;bool found=false;for(ushort uid=100;uid<100+CurrentCount;uid++)if(IsBoss(uid)){found=true;if(!dead[RoomIndex].Contains(uid))return false;}return found;}
 internal ushort[] CleanupAfterBoss(){if(!ReadyToClear)throw new InvalidDataException("Boss cleanup state");List<ushort> removed=new List<ushort>();for(ushort uid=100;uid<100+CurrentCount;uid++)if(dead[RoomIndex].Add(uid))removed.Add(uid);return removed.ToArray();}
 internal bool IsPendingMove(byte x,byte y){return Active&&!Loaded&&movePending&&X==x&&Y==y;}
 internal void Move(byte x,byte y){if(!Active||!Loaded||ClearPublished||ReadyToClear||!RoomCleared||!(DungeonId==1?x==0&&y<=2: DungeonId==2?x<=2&&y<=2&&Offline70DeepLorienFG72.Exists(x,y):CanMoveFG130(x,y))||BossConditionGateFG181(x,y)||Math.Abs((int)y-Y)+Math.Abs((int)x-X)!=1)throw new InvalidDataException("Dungeon room admission");X=x;Y=y;Loaded=false;movePending=true;}
 internal void PublishClear(){if(!ReadyToClear||ClearPublished)throw new InvalidDataException("Dungeon clear state");ClearPublished=true;clearTimer.Stop();}
 // Start only after the client reaches end eligibility (after XP presentation),
 // so the 30-second wait does not cut short the preceding results animation.
 // Repeated eligibility requests must not extend the same run's deadline.
 internal void BeginEndWait(){if(!Active||!ClearPublished)throw new InvalidDataException("End wait outside settled dungeon");if(!settlementTimer.IsRunning)settlementTimer.Start();}
 internal void End(byte option){if(!Active||!ClearPublished||option>3)throw new InvalidDataException("Dungeon end state");if(option==3)return;Active=false;Loaded=false;movePending=false;settlementTimer.Stop();}
 // Voluntary return uses command45, rather than a fabricated pre-clear75 award.
 // This diagnostic abort retains already committed kills/items and grants no
 // clear award; the production fatigue/stamina failure penalty is not modeled.
 internal void Abort(){if(!Active)throw new InvalidDataException("No dungeon to abort");Active=false;Loaded=false;movePending=false;settlementTimer.Stop();}
 internal byte[] RoomBody(){if(!Active)throw new InvalidDataException("No active dungeon");using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){
  // Original native29 flag0 uses the retained client room, preserving broken objects.
  bool revisit=visitedRooms.Contains((byte)RoomIndex);w.Write(X);w.Write(Y);w.Write((uint)0);w.Write((byte)0);w.Write((byte)0);w.Write((byte)(revisit?0:1));if(revisit)return m.ToArray();
  bool spawnGsd=GsdRunFG130&&!GsdSpawnedFG130&&CurrentMap==GsdStartMapFG135;w.Write(CurrentMap);// Native 0x1d counts m_monsterMap.size(), i.e. the [ai character] APCs that
// add_monster_APC_AI pushed into the same map (roleType 5..8): an APC written
// outside the count is read by the client as the object section and never spawns.
  RollEventMonsters();            // must run before the room packet is composed, otherwise the spawns never reach the client
  var conds=CurrentConditionMonsters();
  // Side-channel APCs of this room (generated ApcMonstersFG rows, see Act4-Boss-Apc-FG176.cs). They
  // count towards the native m_monsterMap.size() byte, which is what makes the client spawn them,
  // but never towards TotalCount: their uids must stay outside ValidateKill's monster range so no
  // drop plan (MapRolls/MonsterPools) is ever attempted for an actor that has no .mob profile.
  var apcs=CurrentApcMonsters();int pendingApcsFG186=0;for(int ai=0;ai<apcs.Count;ai++)if(ApcPendingFG186(ApcUidFG186(ai)))pendingApcsFG186++;
w.Write((byte)(TotalCount-dead[RoomIndex].Count+(spawnGsd?1:0)+pendingApcsFG186));  // both APC slots count (native m_monsterMap.size())
  for(ushort i=0;i<CurrentCount;i++){ushort uid=(ushort)(100+i);if(dead[RoomIndex].Contains(uid))continue;ushort[] monster=Monster(uid);w.Write(i);w.Write((uint)i);w.Write(uid);w.Write(monster[0]);w.Write((byte)monster[1]);w.Write((byte)monster[2]);w.Write((byte)0);w.Write((byte)0);}
  // Condition monsters: extra monsters that must be killed before the boss spawns.
  for(int ci=0;ci<conds.Count;ci++){ushort uid=(ushort)(100+CurrentCount+ci);if(dead[RoomIndex].Contains(uid))continue;var c=conds[ci];w.Write((ushort)(CurrentCount+ci));w.Write((uint)(CurrentCount+ci));w.Write(uid);w.Write(c[1]);w.Write((byte)c[2]);w.Write((byte)c[3]);w.Write((byte)0);w.Write((byte)0);}
  // Event monsters (native add_monster_event): the wire mobId is the 10000+ instance base
  // (that range is what marks an event monster), mobIndex is the [event monster] entry value
  // and the level is the dungeon's standard level. Group index 0 mirrors the native memset.
  var evs=EventIdsForCurrentRoomFG174();for(int xi=0;xi<evs.Count;xi++){ushort uid=(ushort)(100+CurrentCount+conds.Count+xi);if(dead[RoomIndex].Contains(uid))continue;w.Write((ushort)0);w.Write((uint)(10000+xi));w.Write(uid);w.Write(evs[xi]);w.Write((byte)Offline70ClearExperienceFG116.Basis(DungeonId));w.Write((byte)0);w.Write((byte)0);w.Write((byte)0);}
  if(spawnGsd){w.Write((ushort)0);w.Write((uint)0);w.Write(GsdUidFG130);w.Write(GsdApcFG135);w.Write((byte)84);w.Write((byte)5);w.Write((byte)0);w.Write((byte)0);GsdSpawnedFG130=true;}for(int ai=0;ai<apcs.Count;ai++){ushort apcUidFG186=ApcUidFG186(ai);if(!ApcPendingFG186(apcUidFG186))continue;ushort[] apcRowFG186=apcs[ai];w.Write((ushort)0);w.Write((uint)0);w.Write(apcUidFG186);w.Write(apcRowFG186[1]);w.Write((byte)apcRowFG186[2]);w.Write(ApcRoleByteFG186(apcRowFG186));w.Write((byte)0);w.Write((byte)0);ApcRegisterSpawnFG186(apcUidFG186,apcRowFG186);}w.Write((byte)0);w.Write((byte)0);return m.ToArray();
 }}
 internal static byte[] ResultBody(byte rank,uint clearMilliseconds,byte technic,byte hit){if(rank>105||clearMilliseconds>86400000)throw new InvalidDataException("Clear score bounds");byte[] b=new byte[7];b[0]=rank;BitConverter.GetBytes(clearMilliseconds).CopyTo(b,1);b[5]=technic;b[6]=hit;return b;}
 // Native35 four-member projection with one local free gold card. The displayed
 // gold is the diagnostic clear award already committed by Clear(), not a new
 // grant. No paid card is offered. Native readers consume all91 logical bytes.
 internal static byte[] ClearRewardBody(uint totalExperience,uint baseExperience,uint previousExperience){if(totalExperience<previousExperience||totalExperience-previousExperience!=baseExperience)throw new InvalidDataException("Clear experience presentation mismatch");byte[] b=new byte[91];BitConverter.GetBytes(totalExperience).CopyTo(b,0);BitConverter.GetBytes(baseExperience).CopyTo(b,4);BitConverter.GetBytes(previousExperience).CopyTo(b,8);b[62]=1;BitConverter.GetBytes(DiagnosticClearGold).CopyTo(b,67);return b;}
 internal static byte[] BossBody(ushort uid){if(uid<100||uid>163)throw new InvalidDataException("Supported boss UID");return new byte[]{1,1,(byte)uid,(byte)(uid>>8)};}
 internal static byte[] Reply(byte kind,ushort opcode,byte[] body,byte[] keys){if(kind>1||body==null||keys==null||keys.Length!=334)throw new InvalidDataException("Dungeon frame bounds");byte[] padded,encrypted;int index=opcode%14;
  if(kind==0&&(opcode==34||opcode==35||opcode==127))body=Offline70RoleProtocol.WithDungeonIntegrity(body);
  if(opcode==29&&kind==0){padded=new byte[(body.Length+15)/16*16];body.CopyTo(padded,0);encrypted=ProbeLoginCandidate.Crypt(padded,keys,true);}
  else if(opcode==31&&kind==0||(opcode==3||opcode==45)&&kind==1){padded=new byte[16];body.CopyTo(padded,0);encrypted=Offline70KeySettings.Crypt(padded,keys,true);}
  else if((opcode==34||opcode==35)&&kind==0||opcode==48&&kind==1||opcode==49&&kind==1||opcode==75&&kind==1){padded=Offline70RoleProtocol.Pad(body);encrypted=Offline70FlowCipherFG.Crypt(padded,keys,index,true);}
  else if(opcode==127&&kind==0){padded=new byte[16];body.CopyTo(padded,0);encrypted=ProbeLoginCandidate.Crypt(padded,keys,true);}
  else throw new InvalidDataException("Dungeon wire opcode has no verified reader");return Offline70RoleProtocol.Packet(kind,opcode,encrypted);
 }
 internal static byte[] EndPlain(byte[] encrypted,byte[] keys){if(encrypted==null||(encrypted.Length!=8&&encrypted.Length!=16))throw new InvalidDataException("End request ciphertext size");byte[] plain=Offline70FlowCipherFG.Crypt(encrypted,keys,5,false);if(plain[0]>2||plain[1]>3)throw new InvalidDataException("End option bounds");for(int i=2;i<plain.Length;i++)if(plain[i]!=0)throw new InvalidDataException("End request padding");return plain;}
 // Current70 type1/op48 has a 70-byte logical body padded to 72 bytes.
 // Native 105D9A0 excludes this command from the folded-MD5 trailer. Coordinates
 // occupy bytes 0/1; bytes 68/69 are also client data, not zero padding.
 // The subsequent client counters are not authoritative input.
 // Run/load/death/adjacency admission is enforced separately by Move().
 internal static byte[] MovePlain(byte[] encrypted,byte[] keys){if(encrypted==null||encrypted.Length!=72)throw new InvalidDataException("Move request ciphertext size");byte[] plain=Offline70FlowCipherFG.Crypt(encrypted,keys,6,false);if(plain[0]>15||plain[1]>15)throw new InvalidDataException("Move coordinates outside bounded maze grid");for(int i=70;i<72;i++)if(plain[i]!=0)throw new InvalidDataException("Move request padding");return plain;}
 internal static void ValidateClearRequest(byte[] encrypted,byte[] keys){ResultPlain(encrypted,keys);}
 // Original current70 requests cover both values: 0 exits, 1 returns to channel
 // selection. After successful reply3 the native handler closes the old socket;
 // the server must stop processing this connection and permit a new ch10 login.
 internal static byte ExitPlain(byte[] encrypted,byte[] keys){if(encrypted==null||encrypted.Length!=16)throw new InvalidDataException("Channel exit ciphertext size");byte[] plain=Offline70KeySettings.Crypt(encrypted,keys,false);if(plain[0]>1)throw new InvalidDataException("Channel exit login state");for(int i=1;i<16;i++)if(plain[i]!=0)throw new InvalidDataException("Channel exit padding");return plain[0];}
 internal static void ValidateAbortRequest(byte[] body){if(body==null||body.Length!=0)throw new InvalidDataException("Dungeon abort request must have no body");}
 // The native sender truncates a float to u8 at byte0; it has no <=105 clamp.
 // Packed helper 1EE3875 adds four u16 fields after the first 12 bytes, so the
 // logical body is 40 bytes, followed by fold4 and four zero padding bytes.
 // These are reported statistics, not authority for the server-side award.
 internal static byte[] ResultPlain(byte[] encrypted,byte[] keys){if(encrypted==null||encrypted.Length!=48)throw new InvalidDataException("Clear result ciphertext size");byte[] plain=Offline70FlowCipherFG.Crypt(encrypted,keys,7,false),body=new byte[40];Buffer.BlockCopy(plain,0,body,0,40);byte[] integrity=Offline70RoleProtocol.WithDungeonIntegrity(body);for(int i=40;i<44;i++)if(plain[i]!=integrity[i])throw new InvalidDataException("Clear result checksum");for(int i=44;i<48;i++)if(plain[i]!=0)throw new InvalidDataException("Clear result padding");if(body[1]!=0)throw new InvalidDataException("Clear result reserved field");return body;}
}
// This journal guards clear idempotence independently of the role stores.
// The caller's consolidated transaction must commit asset deltas before publishing
// clear; this file alone is not an atomic multi-file XP/gold transaction.
public class Offline70ClearReceiptFG {public string RunId;public ushort DungeonId=1;public uint Experience,Gold;}
public class Offline70ClearLedgerFG {public int Format=1;public ushort RoleId=1;public List<Offline70ClearReceiptFG> Receipts=new List<Offline70ClearReceiptFG>();}
class Offline70ClearStoreFG {
 static readonly XmlSerializer Serializer=new XmlSerializer(typeof(Offline70ClearLedgerFG));
 static void Validate(Offline70ClearLedgerFG s){if(s==null||s.Format!=1||s.RoleId!=1||s.Receipts==null||s.Receipts.Count>10000)throw new InvalidDataException("Clear journal bounds");HashSet<string> ids=new HashSet<string>();foreach(Offline70ClearReceiptFG r in s.Receipts){Guid id;if(r==null||r.DungeonId!=1||r.RunId==null||r.RunId.Length!=32||!TryGuid(r.RunId,out id)||!ids.Add(r.RunId)||r.Experience>1000000||r.Gold>1000000)throw new InvalidDataException("Clear journal receipt");}}
 static bool TryGuid(string value,out Guid id){try{id=new Guid(value);return true;}catch(FormatException){id=Guid.Empty;return false;}}
 internal static Offline70ClearLedgerFG Load(string path){if(!File.Exists(path))return new Offline70ClearLedgerFG();if(new FileInfo(path).Length>2000000)throw new InvalidDataException("Clear journal file bounds");using(FileStream f=File.OpenRead(path))using(XmlTextReader x=new XmlTextReader(f)){x.ProhibitDtd=true;x.XmlResolver=null;Offline70ClearLedgerFG s=(Offline70ClearLedgerFG)Serializer.Deserialize(x);Validate(s);return s;}}
 internal static bool Commit(string path,string runId,uint experience,uint gold){string full=Path.GetFullPath(path);Directory.CreateDirectory(Path.GetDirectoryName(full));using(FileStream held=new FileStream(full+".lock",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None)){Offline70ClearLedgerFG s=Load(full);foreach(Offline70ClearReceiptFG r in s.Receipts)if(r.RunId==runId){if(r.Experience!=experience||r.Gold!=gold)throw new InvalidDataException("Conflicting repeated clear award");return false;}s.Receipts.Add(new Offline70ClearReceiptFG{RunId=runId,Experience=experience,Gold=gold});Validate(s);string tmp=full+".pending-"+Guid.NewGuid().ToString("N"),backup=full+".backup-"+Guid.NewGuid().ToString("N");try{using(FileStream f=new FileStream(tmp,FileMode.CreateNew,FileAccess.Write,FileShare.None)){Serializer.Serialize(f,s);f.Flush();}if(Load(tmp).Receipts.Count!=s.Receipts.Count)throw new InvalidDataException("Clear journal read-back mismatch");if(File.Exists(full))File.Replace(tmp,full,backup);else File.Move(tmp,full);if(Load(full).Receipts.Count!=s.Receipts.Count)throw new InvalidDataException("Clear journal commit mismatch");return true;}finally{if(File.Exists(tmp))File.Delete(tmp);}}}
}
