using System;using System.IO;using System.Collections.Generic;
// The FG service uses separate state; it keeps previous bounded profiles intact.
partial class Offline70RoleProtocol {
 internal static byte[] BatchCryptFG(byte[] data,byte[] keys,int index,bool encrypt){
  if(index==0){byte[] b=(byte[])data.Clone();for(int i=0;i<b.Length;i++)b[i]^=keys[4+i%4];return b;}
  if(index==1)return ProbeLoginCandidate.Crypt(data,keys,encrypt);
  if(index==2)return encrypt?new Blowfish(keys).Encode(data):new Blowfish(keys).Decode(data);
  if(index==3)return Offline70KeySettings.Crypt(data,keys,encrypt);
  if(index==4||index==11||index==13)return Offline70GoldCipher.Crypt(data,keys,index,encrypt);
  if(index>=5&&index<=8)return Offline70FlowCipherFG.Crypt(data,keys,index,encrypt);
  if(index==9||index==10)return Offline70AreaCipher.Crypt(data,keys,index,encrypt);
  if(index==12){if(data.Length%8!=0)throw new InvalidDataException("Batch MULTI2 bounds");byte[] b=new byte[data.Length];for(int at=0;at<data.Length;at+=8){byte[] chunk=new byte[8];Buffer.BlockCopy(data,at,chunk,0,8);DungeonLoadCrypt(chunk,keys,encrypt).CopyTo(b,at);}return b;}
  throw new InvalidDataException("Unregistered batch cipher");
 }
 internal static byte[] BatchPacketFG(byte kind,ushort opcode,byte[] plain,byte[] keys,bool integrity){if(integrity)plain=WithDungeonIntegrity(plain);int index=opcode%14,block=index==0?4:index==1||index==3||index==4||index==8||index==13?16:8;return Packet(kind,opcode,BatchCryptFG(Offline70GoldLoot.Pad(plain,block),keys,index,true));}
 internal static byte[] BatchTownReplyFG(ushort destination,ushort op,Offline70Role role,byte[] keys,int x,int y){BatchBaseRoleFG(role);if(destination!=2)throw new InvalidDataException("Unsupported return area");if(op==3)return Offline70TownProtocol.Reply(op,BatchBaseRoleFG(role),keys);return new Offline70Gate{Area=2,X=(ushort)x,Y=(ushort)y,Direction=5}.Reply(op,keys);}
 internal static Offline70Role BatchBaseRoleFG(Offline70Role role){if(role.Id!=1||role.Job!=0||role.Name!="yoyo"||role.Level<1||role.Level>70)throw new InvalidDataException("FG saved ghost-knight profile");return new Offline70Role{Id=role.Id,Job=role.Job,Level=1,Name=role.Name,CreatedUtc=role.CreatedUtc};}
 internal static byte[] BatchRosterFG(Offline70Role role,byte[] keys){Offline70Role basic=BatchBaseRoleFG(role);byte[] b=RosterBody(new List<Offline70Role>(new Offline70Role[]{basic}));b[15+NameEncoding.GetByteCount(role.Name)]=role.Level;return Packet(0,2,new Blowfish(keys).Encode(Pad(Zlib(b))));}
 internal static byte[] BatchRosterFG(Offline70Role role,Offline70EconomyState assets,byte[] keys){
  Offline70Economy.Validate(assets);if(role.Id!=assets.RoleId||role.Job!=assets.Job||role.Level!=assets.Level)throw new InvalidDataException("Roster asset identity");
  byte[] basic=RosterBody(new List<Offline70Role>(new Offline70Role[]{BatchBaseRoleFG(role)}));basic[14+NameEncoding.GetByteCount(role.Name)]=assets.GrowType;basic[15+NameEncoding.GetByteCount(role.Name)]=role.Level;
  int at=18+NameEncoding.GetByteCount(role.Name);if(basic[at]!=0)throw new InvalidDataException("Roster worn row boundary");
  List<Offline70EconomyEntry> worn=new List<Offline70EconomyEntry>();foreach(Offline70EconomyEntry e in assets.Inventory)if(e.List==3)worn.Add(e);worn.Sort(delegate(Offline70EconomyEntry a,Offline70EconomyEntry b){return a.Slot.CompareTo(b.Slot);});
  using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write(basic,0,at);w.Write((byte)worn.Count);foreach(Offline70EconomyEntry e in worn){w.Write((byte)e.Slot);w.Write(e.ItemId);w.Write((byte)0);w.Write((uint)0);w.Write((uint)0);}w.Write(basic,at+1,basic.Length-at-1);return Packet(0,2,new Blowfish(keys).Encode(Pad(Zlib(m.ToArray()))));}
 }
 internal static byte[] BatchAppearanceFG(Offline70Role role,byte[] keys){byte[] b=AppearanceBody(BatchBaseRoleFG(role));b[15]=role.Level;return Packet(0,2,new Blowfish(keys).Encode(Pad(Zlib(b))));}
 internal static byte[] BatchAdditionBodyFG(Offline70Role role,uint total,byte growByte){if(role.Level!=Offline70ProgressionFG.Level(total))throw new InvalidDataException("FG role/experience mismatch");byte[] b=AdditionBody(BatchBaseRoleFG(role),0,true);BitConverter.GetBytes(total).CopyTo(b,5);Offline70SwordmanGrowthFG.Stats(growByte,role.Level).CopyTo(b,13);return b;}
 internal static byte[] BatchAdditionFG(Offline70Role role,uint total,byte growByte,byte[] keys){return Packet(0,2,new Blowfish(keys).Encode(Pad(Zlib(BatchAdditionBodyFG(role,total,growByte)))));}
 // Original notify48 (46D5C0): remaining, used, three auxiliary counters,
 // two flags. The HUD uses remaining/(remaining+used), not select-ACK values.
 // Auxiliary counters and flags remain zero in the existing offline profile.
 internal static byte[] BatchFatigueFG(int used,byte[] keys){if(used<0||used>156)throw new InvalidDataException("Diagnostic fatigue bounds");byte[] b=new byte[22];BitConverter.GetBytes((uint)(156-used)).CopyTo(b,0);BitConverter.GetBytes((uint)used).CopyTo(b,4);return BatchPacketFG(0,48,b,keys,false);}
 internal static byte[] BatchSelectFG(Offline70Role role,Offline70ProgressionStateFG state,Offline70QuestCatalogFG catalog,Offline70QuestEngineFG engine,byte[] keys){if(state.Active.Count>20)throw new InvalidDataException("Native active quest limit");byte[] b=Offline70SelectCipher.Body(BatchBaseRoleFG(role),true,false);int slot=0;for(int i=0;i<state.Active.Count;i++){Offline70QuestActiveFG q=state.Active[i];if(!catalog.ActiveVisible(catalog.Get(q.Id),state))continue;BitConverter.GetBytes(q.Id).CopyTo(b,22+6*slot);BitConverter.GetBytes(engine.WireRemaining(catalog.Get(q.Id),q,state)).CopyTo(b,24+6*slot);slot++;}return BatchPacketFG(1,4,b,keys,false);}
 // Original senders serialize inventory/equipment and skill state summaries.
 // These vary with player assets; 101 was an old empty-inventory observation.
 // This offline service records both summaries without claiming an authoritative
 // recomputation. The caller admits only a selected player awaiting a room load.
 internal static byte[] BatchLoadRequestFG(byte[] encrypted,byte[] keys){if(encrypted==null||encrypted.Length!=8)throw new InvalidDataException("Native room-load request must contain two u32 summaries");return DungeonLoadCrypt(encrypted,keys,false);}
 // Single-player native42: 64 logical bytes plus fold4. The support/statistics
 // fields at 42/46 vary with the monster/map; 1/32 were only first-room
 // observations. Keep them as client reports, never as award authority.
 // Support the current native variable contributor list.
 // Integrity, owner membership and live victim roster remain checked.
 // Client statistics never determine XP, drops or the dungeon-clear award.
 internal static ushort BatchDeathRequestFG(byte[] encrypted,Offline70Role role,byte[] keys){return BatchDeathRequestFG(encrypted,role,keys,false);}
 internal static ushort BatchDeathRequestFG(byte[] encrypted,Offline70Role role,byte[] keys,bool liveGsd){return BatchDeathRequestFG(encrypted,role,keys,delegate(ushort id){return id==role.Id||liveGsd&&id==Offline70DungeonFlowFG.GsdUidFG130;});}
 // The current CN writer carries a byte count and ten bytes per contributor.
 // Owner/contributor keys do not set rewards. The service validates the exact
 // announced victim and its live room before committing death, XP and drops.
 // Actual CN70 no-attacker deaths: ownerFFFF, zero contributor rows, full original fold/padding.
 // Only ordinary announced victims qualify; Kill still requires the live loaded room roster.
 // No absent owner is accepted for GSD or nonempty contributor lists.
 internal static ushort BatchDeathRequestFG(byte[] encrypted,Offline70Role role,byte[] keys,Func<ushort,bool> ownerAllowed){
  Offline70TownProtocol.Validate(BatchBaseRoleFG(role),keys);if(encrypted==null||encrypted.Length<60||encrypted.Length>2612||ownerAllowed==null)throw new InvalidDataException("Batch death native size");byte[] plain=BatchCryptFG(encrypted,keys,0,false);int count=plain[20],logical=54+10*count,padded=(logical+7)&~3;if(encrypted.Length!=padded)throw new InvalidDataException("Batch death layout");byte[] body=new byte[logical];Buffer.BlockCopy(plain,0,body,0,logical);byte[] folded=WithDungeonIntegrity(body);for(int i=0;i<4;i++)if(plain[logical+i]!=folded[logical+i])throw new InvalidDataException("Batch death integrity");for(int i=logical+4;i<plain.Length;i++)if(plain[i]!=0)throw new InvalidDataException("Batch death padding");ushort uid=BitConverter.ToUInt16(body,0),attacker=BitConverter.ToUInt16(body,2);bool unowned=count==0&&attacker==UInt16.MaxValue&&uid!=Offline70DungeonFlowFG.GsdUidFG130;if((uid<100||uid>163)&&uid!=Offline70DungeonFlowFG.GsdUidFG130||!unowned&&!ownerAllowed(attacker))throw new InvalidDataException("Batch death actor bounds");HashSet<ushort> contributors=new HashSet<ushort>();bool ownerPresent=count==0;
  for(int i=0;i<count;i++){int at=21+10*i;ushort contributor=BitConverter.ToUInt16(body,at+2);if(BitConverter.ToUInt16(body,at)!=0x111||!ownerAllowed(contributor)||BitConverter.ToUInt16(body,at+8)==0||!contributors.Add(contributor))throw new InvalidDataException("Batch death contributor profile");if(contributor==attacker)ownerPresent=true;}
  if(!ownerPresent)throw new InvalidDataException("Batch death owner absent from contributors");int tail=21+10*count;for(int i=tail+4;i<tail+8;i++)if(body[i]!=0)throw new InvalidDataException("Batch death room flags");if(body[tail+10]>1)throw new InvalidDataException("Batch death support flag");return uid;
 }
 internal static byte[] BatchPickupRequestFG(byte[] encrypted,byte[] keys){if(encrypted.Length!=16)throw new InvalidDataException("Batch pickup size");byte[] b=BatchCryptFG(encrypted,keys,4,false);ushort slot=BitConverter.ToUInt16(b,0);if(slot<2000||slot>Offline70FloorDropsFG119.LastSlot||b[2]!=0||b[3]>1)throw new InvalidDataException("Batch pickup routing");foreach(int at in new int[]{4,6,10,12})if(BitConverter.ToUInt16(b,at)>4096)throw new InvalidDataException("Batch pickup position");return b;}
}
class Offline70GroundFG {internal ushort Slot;internal uint ItemId,Quantity;internal string Receipt;}
class Offline70LootWireFG {
 // Native _MakeNotiPacketDieMonster (ORIG 0x0830b8cc) tail is put_byte(0), then
 // convertAPCRoleType2MonsterRoleType(roleType) != 0 ? m_apcKillCount : -1, then flag34.
 // Monster kinds 1..3 and APC roles 6..8 convert non-zero (their deaths carry the count);
 // kind 0 and APC role 5 carry -1. The old code always wrote -1, which is what a boss death
 // must not do.
 internal static byte[] Death(ushort uid,IList<Offline70GroundFG> drops){return Death(uid,drops,-1);}
 internal static byte[] Death(ushort uid,IList<Offline70GroundFG> drops,int apcKillCount){if((uid<100||uid>163)&&uid!=Offline70DungeonFlowFG.GsdUidFG130||drops.Count>Offline70FloorDropsFG119.MaximumRows||uid==Offline70DungeonFlowFG.GsdUidFG130&&drops.Count!=0||apcKillCount>255)throw new InvalidDataException("Batch death rows");using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write(uid);w.Write((byte)drops.Count);foreach(Offline70GroundFG d in drops){if(d.Slot<2000||d.Slot>Offline70FloorDropsFG119.LastSlot||d.Quantity==0)throw new InvalidDataException("Ground record");w.Write(d.Slot);w.Write(d.ItemId);w.Write(d.Quantity);w.Write((ushort)0);w.Write((byte)0);w.Write((ushort)0);w.Write((uint)0);w.Write(new byte[6]);w.Write((ushort)65535);}w.Write(new byte[]{0,(byte)(apcKillCount<0?255:apcKillCount),0});return m.ToArray();}}
 internal static byte[] Pickup(Offline70GroundFG d,ushort destination){if(d.ItemId==0){if(d.Slot<2000||d.Slot>Offline70FloorDropsFG119.LastSlot||d.Quantity==0||d.Quantity>Offline70GoldLoot.Limit)throw new InvalidDataException("Floor gold pickup");byte[] gold=new byte[25];BitConverter.GetBytes(d.Slot).CopyTo(gold,0);BitConverter.GetBytes((ushort)1).CopyTo(gold,2);gold[4]=1;BitConverter.GetBytes(d.Quantity).CopyTo(gold,5);return gold;}byte[] b=new byte[13];BitConverter.GetBytes(d.Slot).CopyTo(b,0);BitConverter.GetBytes((ushort)1).CopyTo(b,2);BitConverter.GetBytes((ushort)1).CopyTo(b,8);BitConverter.GetBytes(destination).CopyTo(b,10);return b;}
}
