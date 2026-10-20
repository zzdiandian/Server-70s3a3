using System;using System.IO;
// Verified native70 fields and fixed-key fixtures for role1, Lv1 Lorien normal selection.
// Native packet readers are retained. This does not initialize a dungeon or combat.
class Offline70DungeonSelection {
 // User-authorized difficulty override for every implemented dungeon profile.
 internal const byte TestMaximumDifficulty=3;
 internal static readonly ushort[] Opcodes=new ushort[]{3,26,5,27};
 internal static byte[] Body(ushort opcode){
  if(opcode==3)return new byte[]{1,1,0,1}; // u8 count, u16 actor, u8 state=selection
  if(opcode==26)return new byte[]{0}; // local party host seat
  if(opcode==5)return new byte[]{2,0,1,0,TestMaximumDifficulty,2,0,TestMaximumDifficulty};
  if(opcode==27)return new byte[13]; // u32 hell state; 3 flag/count bytes; empty item group count; u32 guide; u8 share
  throw new InvalidDataException("Unverified dungeon selection opcode");
 }
 internal static byte[] Reply(ushort opcode,Offline70Role role,byte[] keys){
  return Reply(opcode,role,keys,false);
 }
 internal static byte[] Reply(ushort opcode,Offline70Role role,byte[] keys,bool forest){
  return Reply(opcode,role,keys,forest?2:1);
 }
 internal static byte[] Reply(ushort opcode,Offline70Role role,byte[] keys,int town){
  return Reply(opcode,role,keys,town,3);
 }
 internal static byte[] Reply(ushort opcode,Offline70Role role,byte[] keys,int town,int area){
  Offline70TownProtocol.Validate(role,keys);string cipher;
  if(opcode==3)cipher="107e8e0553966c9b9ffbc7f26f9b2855";
  else if(opcode==26)cipher="e55bd513598bfde3";
  else if(opcode==5)return Offline70RoleProtocol.Packet(0,5,Offline70RoleProtocol.BatchCryptFG(Offline70RoleProtocol.Pad(ProfileForArea(town,area)),keys,5,true));
  else if(opcode==27)cipher="67dda5765662d5df55ca73c1454fae2f";
  else throw new InvalidDataException("Unverified dungeon selection opcode");
  return Offline70RoleProtocol.Packet(0,opcode,Offline70TownProtocol.Hex(cipher));
 }
 // op5 profile per gate area, i.e. which `[dungeon gate]` worldmap this area opens. Only areas
 // answering op15 (DungeonGate) reach here; everything else keeps the historical Body(5) fallback.
 internal static byte[] ProfileForArea(int town,int area){
  if(town==1)return LorienBody();
  if(town==2)return area==9?NorthmyreBody():area==4?PubBody():area==7?HillsideBody():ForestBody();
  if(town==3)return (area==4||area==6)?GblBody():area==9?FallenBody():area==10?StrangerBody():SkyBody();
  if(town==4)return area==3?AncientBody():Act4Body();
  if(town==5)return StormPassBody();
  if(town==6)return (area==2||area==3)?AntberrBody():Body(5);
  if(town==9)return area==1?LufthafenBody():Body(5);
  return Body(5);}
 internal static byte[] ProfileBody(ushort[] ids){using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write((ushort)ids.Length);foreach(ushort id in ids){w.Write(id);w.Write(TestMaximumDifficulty);}return m.ToArray();}}
 // 格兰之森门 (town2 area6, worldmap/granfloris.wdm) 还带 503/506/514 三行 `[in progress]` 转职修炼场，
 // 同行第 3 列就是开门任务 (503->7824/7834/7873/7884/7894，506->7865，514->7864)；PVF 内容齐全
 // (3 房 / 29-30 个 [monster] 行 / 无 [necessary party] / minLv 20)，证据 local-run/pvf/export/training-rooms.txt。
 internal static byte[] ForestBody(){return ProfileBody(new ushort[]{3,4,5,6,7,8,9,1000,503,506,514});}
 // 诺斯玛尔 (northmyre, PVF worldmap/northmyre.wdm `[dungeon]` = 50 51 52 53 1506) is served by
 // the HendonMyre(town2) area9 gate: town/hendonmyre.twn area9 = `HendonMyre/Hendon2North.map`
 // with `[dungeon gate]` 7, and that area's `[town movable area]` has the -1/-1 dungeon entrance
 // rectangle already carried by Town-Content-FG75 DungeonPortal(2,9). Evidence + re-derivation:
 // local-run/pvf/port_northmyre_gen.py (gate-evidence section).
 internal static byte[] NorthmyreBody(){return ProfileBody(new ushort[]{50,51,52,53,1506});}
 internal static byte[] SkyBody(){return ProfileBody(new ushort[]{11,12,13,14,15,17,504});}   // 504 = middleocean 转职修炼场 (skycastle.wdm `[in progress]`，任务 7859/7889)
 // 天帷巨兽 (Behemoth, town3 area4/area6 `[dungeon gate]` 4) lists 21 22 23 24 25 91 26 90 27 in
 // worldmap/behemoth.wdm.  90 树精繁殖地 (dungeon/act3/breeding.dgn, rooms map/gbl_breeding) and
 // 91 GBL女神殿 (dungeon/act3/goddesstemple.dgn, rooms map/gbl_goddess_temple) both carry `-1` in
 // the quest column, i.e. no task prerequisite.  Evidence: local-run/pvf/export/gap-dgn-probe.txt.
 internal static byte[] GblBody(){return ProfileBody(new ushort[]{21,22,23,24,25,26,27,90,91});}
 // Lorien (town1) area2 gate carries the ancient dungeons too: lorien.wdm lists {1,2,1500,1501}
 // and both .dgns use WorldMap/Act1/elvengard slot 2/3 (PVF evidence, port-1500-prep.md).
 internal static byte[] LorienBody(){return ProfileBody(new ushort[]{1,2,1500,1501});}
 // Ticket consumption/coin limit for33(4083) and1502(4343) are integrated (Entry-Cost-FG185.cs):
 // op16 admission checks the [required item] (4083/4343 x1) before the fatigue spend, consumes it
 // through Offline70Economy.Apply with a RunId-scoped receipt, and [coin limit]3 caps coin revivals
 // at 3 per run (op44 error0x16) - native checkInoutConditionDungeon/ReviveUserByCoin + CN70 client
 // op16 dispatcher branch 0x11. Both entries stay admitted.
 // Act4 (暗黑城) is offered by 阿法利亚 (town4) gates: area2 is `[dungeon gate] 5`, area3/area4 are
 // gates 11/13 which hold nothing else in this build, so all three areas serve the client list.
 internal static byte[] Act4Body(){return ProfileBody(new ushort[]{31,32,33,34,35,36,37});}
 // Ancient dungeon (诺伊佩拉, 1502) is offered by the 阿法利亚 area3 gate; its profile body carries
 // both the Act4 group and 1502 because the client shows the 暗黑城 list at that gate too.
 internal static byte[] AncientBody(){return ProfileBody(new ushort[]{31,32,33,34,35,36,37,1502});}
 // 万年雪山 (StormPass, town5) serves 40..45 and 141 from the client's own gate areas area2/area3.
 // 3503 逆袭之谷 (dungeon/act5/counterattackarea.dgn, 2 rooms in map/counterattackarea) is the eighth
 // row of worldmap/charnak.wdm; its `[pathgate object]` and `[village attack revenge dungeon]` wave
 // mechanics are not implemented here, and `[gold card use] 0` is enforced by the client's own PVF.
 internal static byte[] StormPassBody(){return ProfileBody(new ushort[]{40,41,42,43,44,45,141,3503});}
 // 安特贝鲁峡谷 (Antberr Canyon, town6 根特) is offered by the two Gent dungeon gates:
 // town/gent.twn area2 = `Gent/Gent2Ant1.map` and area3 = `Gent/Gent2Ant2.map`, both
 // `[dungeon gate]` 12, and worldmap.lst #12 = `Antberr.wdm` whose `[dungeon]` block lists
 // 80 81 82 83 84 85 88 89 (every row `-1`, i.e. no quest/ticket prerequisite).  86/87/1504
 // live on worldmap/lufthafen.wdm (town9) and are NOT part of this batch.
 // Evidence and re-derivation: local-run/pvf/r2_gen_gent_lufthafen.py (export/r2-port2).
 internal static byte[] AntberrBody(){return ProfileBody(new ushort[]{80,81,82,83,84,85,88,89});}
 // 月光酒馆 (town2 area4 `HendonMyre/Hendon_Pub.map`, `[dungeon gate]` 8 = worldmap/moonlightpub.wdm)
 // offers only 140 月光酒馆 (dungeon/village/moonlightpub/aganzo.dgn, rooms map/aganzo); the other
 // rows of that worldmap are all `[in progress]` event dungeons (500/505/507/508/511).
 internal static byte[] PubBody(){return ProfileBody(new ushort[]{140,500});}   // 500 = quest/training 转职修炼场 (moonlightpub.wdm `[in progress]`，任务 7829/7830/7837/7868/7869/7876)
 // 格兰之森山麓 (town2 area7 `HendonMyre/D_Hendon2.map`, `[dungeon gate]` 14 = granflorishillside.wdm)
 // = 100 南部溪谷 (dungeon/southerndale/grimseeker.dgn, rooms map/grim).
 internal static byte[] HillsideBody(){return ProfileBody(new ushort[]{100});}
 // 堕落之殿 (town3 area9 `HendonMyre/West2Fallen.map`, `[dungeon gate]` 9 = worldmap/gblgrim.wdm)
 // = 110 GBL教阿拉德分部 / 111 GBL教研究所 / 112 GBL教孵化场. 110's worldmap row asks for quest
 // 3802, which the client enforces before it offers the entry.
 internal static byte[] FallenBody(){return ProfileBody(new ushort[]{110,111,112});}
 // 陌路者要塞 (town3 area10 `HendonMyre/West2Stranger.map`, `[dungeon gate]` 10 = aradcartel.wdm)
 // = 60 热血八番街 (quest 3901) / 61 绿都格罗兹尼 (quest 3907); rooms map/odesa2 and
 // map/groznyoutside + map/groznyinside.
 internal static byte[] StrangerBody(){return ProfileBody(new ushort[]{60,61});}
 // 海上列车 (town9 area1 `LuftHafen/LuftHafent_Platform.map`, `[dungeon gate]` 15 = lufthafen.wdm):
 // 86 列车上的海贼 / 87 夺回西部线 / 92 雾都赫伊斯 / 93 阿登高地 / 1504 幽灵列车. The worldmap rows
 // ask for quest 4039 (92/93) and 6003 (1504), and 1504's .dgn has [required item] 4421 x1 plus
 // [coin limit] 3, which Offline70EntryCostFG185 enforces the same way as 1502/1506.
 internal static byte[] LufthafenBody(){return ProfileBody(new ushort[]{86,87,92,93,1504});}
 internal static bool SupportedInTown(int town,ushort id){return SupportedInArea(town,3,id);}
 internal static bool SupportedInArea(int town,int area,ushort id){return town==1&&(id==1||id==2||(area==2&&(id==1500||id==1501)))||town==2&&(id>=3&&id<=9||id==1000||(area==9&&(id>=50&&id<=53||id==1506))||area==4&&(id==140||id==500)||area==7&&id==100||area==6&&(id==503||id==506||id==514))||town==3&&((area==3||area==5)&&(id>=11&&id<=15||id==17||id==504)||(area==4||area==6)&&(id>=21&&id<=27||id==90||id==91)||area==9&&(id>=110&&id<=112)||area==10&&(id==60||id==61))||town==4&&(area==2&&id>=31&&id<=37||area==3&&(id>=31&&id<=37||id==1502))||town==5&&(area==2||area==3)&&(id>=40&&id<=45||id==141||id==3503)||town==6&&(area==2||area==3)&&(id>=80&&id<=85||id==88||id==89)||town==9&&area==1&&(id==86||id==87||id==92||id==93||id==1504);}
}
