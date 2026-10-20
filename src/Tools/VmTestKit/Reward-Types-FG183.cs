using System;using System.IO;using System.Collections.Generic;
// FG183: the four quest reward kinds the engine still refuses, isolated as pure builders + tables.
// Wire evidence is the original server's CUser::quest_reward (reference/rednf/remake/source/DNFServer/
// GameServer/Game/CUserOnlyOrig/quest_reward.inc, lines 239-462). It writes the reward-type byte at
// the exact slot our Offline70QuestWireFG.FinishRewardBody leaves as 0 (ack.put_byte(q->m_fieldB8)),
// then per kind: 1 [grow type] -> grow byte + two skill banks (set_grow_type(&ack)); 2 [awakening
// type] -> second-grow byte + two skill banks; 3 [hell challenge] -> nothing; 0x14 [expert job] ->
// expert byte + two banks; 0x15 [hero] -> u16 dungeon id, plus a second op5 packet [1][dungeon][4].
// Our kind byte values are the same numbers STQuestScript.cpp assigns in its `[reward type]` parser
// (1 grow / 2 awakening / 3 hell challenge / 0x14 expert job / 0x15 hero).
class Offline70RewardTypesFG183 {
 // First-awakening skill per (job,grow): character/<job>/<job>.chr `[growtype N]` blocks, N = grow+1
 // (chr growtype 1 is the base class: chr [growtype name] = base, 剑魂/鬼泣/狂战士/阿修罗 as gt2..gt5,
 // matching skill/swordmanskill.lst: gt2 weaponmasteryup/equiplightsword/armormasterylightwm = 剑魂).
 // `[awakening 1] [awakening skill]` value returned here is the skill id; level is always 1
 // (AwakeningSkillLevel). Re-measured 2026-10-17 over the running PVF HTTP API: swordman
 // [growtype 2..5][awakening 1][awakening skill] = 91 1 / 89 1 / 90 1 / 92 1, and the same four
 // blocks' [skill] rows equal AutoFG139[1..4] one for one (27,33,37,172,8,25,65,20,94,197 /
 // 35,29,187,8,25,65,20,93,197 / 56,34,181x2,182x2,8,25,65,20,76,197 / 47,55,61,8,25,65,20,197),
 // which is the proof that chr [growtype N] carries grow N-1. Row 0 (swordman) is the only row
 // this engine can serve: Offline70SkillSlots pins the swordman catalog and s.Job/role.Job never
 // leave 0 (recon/job-advancement-report.md §2), so AwakeningSkill(0,grow) is what a grant uses
 // and rows 1..5 are the standby tables of the other five character scripts (also re-measured:
 // fighter 72/70/65/66, gunner 66/65/68/69, mage 73/75/84/110, priest 86/87/89/122, thief 56/58
 // - all at level 1, all with an empty [awakening 2] skill row).
 // 0 = the client script grants nothing for that pair (thief gt4/gt5 have empty awakening lists).
 internal static readonly byte[][] AwakeningTable=new byte[][]{
  new byte[]{0,91,89,90,92},   // swordman : skill/swordman/{defensedownoncritical,darkaura,bleedingmastery,stuckuponcounter}.skl
  new byte[]{0,72,70,65,66},   // fighter
  new byte[]{0,66,65,68,69},   // gunner
  new byte[]{0,73,75,84,110},  // mage
  new byte[]{0,86,87,89,122},  // priest
  new byte[]{0,56,58,0,0}};    // thief (female): gt4/gt5 `[awakening skill]` empty in the CN70 script
 // Expert job table: character/expertjob.lst (1 enchanter / 2 alchemist / 3 disjointer / 4 doll) and
 // each character/expertjob/*.exj `[clear quests]` + `[skill]` (single skill, level 1).
 internal static readonly byte[] ExpertJobSkills=new byte[]{191,193,194,192};     // index expertType-1
 internal static readonly ushort[] ExpertJobQuests=new ushort[]{2702,2708,2710,2712};
 // [grow type]/[awakening type] grow byte packs first grow in bits0..3 and the awakening level in
 // bits4..6 (rednf Game/CUserCharacInfo.h `union GrowTypeBits` + CUser/set_grow_type.inc).
 internal static byte EncodeGrow(byte first,byte second){if(first>5||second>2)throw new InvalidDataException("Grow type field bounds");return (byte)(first|second<<4);}
 internal static byte FirstGrow(byte combined){return (byte)(combined&0x0F);}
 internal static byte SecondGrow(byte combined){return (byte)(combined>>4&0x07);}
 // A grow byte the engine accepts: first grow 1..5 in bits0..3 (the CN70 trees use 1..5) and
 // the first awakening level 0..1 in bits4..6. Zero itself means 'not promoted' and is only
 // legal where a caller explicitly treats zero as unset.
 internal static bool IsGrowByte(byte value){byte first=FirstGrow(value);return first>=1&&first<=5&&SecondGrow(value)<=1;}
 internal static byte AwakeningSkill(byte job,byte grow){if(job>=AwakeningTable.Length||grow<1||grow>4)return 0;return AwakeningTable[job][grow];}
 // Level the character scripts attach to every awakening skill row. PVF measured 2026-10-17: all
 // 24 non-empty [awakening 1][awakening skill] rows of the six CN70 character scripts are (id,1);
 // no script grants an awakening skill at another level, and no awakening skill comes from the
 // client side (the server writes it in set_grow_type's second-grow branch, see Promotion-Skills-FG139).
 internal const byte AwakeningSkillLevel=1;
 // The skill a grow byte actually grants: only a byte that carries the awakening nibble does,
 // because the original writes the awakening skill inside the [awakening type] reward branch
 // (quest_reward.inc b8==2 -> set_grow_type(firstGrow, 1)). A bare first grow (1..4) and 0 both
 // map to 0 = nothing to grant, which is what Offline70BatchServiceFG.Sync and the probe check.
 internal static byte AwakeningSkillFor(byte job,byte growByte){return SecondGrow(growByte)==0?(byte)0:AwakeningSkill(job,FirstGrow(growByte));}
 internal static byte ExpertJobSkill(byte expertType){if(expertType<1||expertType>4)throw new InvalidDataException("Expert job type bounds");return ExpertJobSkills[expertType-1];}
 internal static ushort ExpertJobQuest(byte expertType){if(expertType<1||expertType>4)throw new InvalidDataException("Expert job type bounds");return ExpertJobQuests[expertType-1];}
 // The original writes the two skill banks of set_grow_type(&ack): bank0 = tree0 count + (slot,id,level)
 // triples, bank1 = tree1. Our promotion path (Offline70JobAdvancementFG91.Body) already ships that
 // exact triple shape with an empty second bank, so the awakening/expert replies reuse it. CN70 has a
 // single skill tree, so bank1 is the empty second bank the promotion fixture still expects.
 internal static byte[] Bank(Offline70SkillSlotState skills){
  Offline70SkillSlots.Validate(skills);byte count=checked((byte)Offline70SkillSlots.VisibleCountFG139(skills));
  using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){
   w.Write(count);for(int i=0;i<skills.Ids.Length;i++){if(skills.Levels[i]==0)continue;w.Write(skills.Slots[i]);w.Write(skills.Ids[i]);w.Write(skills.Levels[i]);}
   w.Write((byte)0);return m.ToArray();}
 }
 // Shared head: the success header, gold and consumed rows. The reward-type byte must sit where
 // FinishRewardBody put its 0 and no received row may exist (mirrors the promotion guard verbatim).
 static byte[] Head(ushort id,uint gold,Offline70EconomyState before,Offline70EconomyState after){
  byte[] item=Offline70QuestWireFG.FinishRewardBody(id,gold,before,after);int prefix=9+item[8]*7;
  if(item[prefix]!=0||item[prefix+1]!=0)throw new InvalidDataException("Mixed item reward kind");
  byte[] head=new byte[prefix];Array.Copy(item,head,prefix);return head;
 }
 public const byte KindAwakening=2,KindHellChallenge=3,KindExpertJob=0x14,KindHero=0x15;
 internal static byte[] AwakeningBody(ushort id,uint gold,byte secondGrow,Offline70EconomyState before,Offline70EconomyState after,byte[] bank){
  if(secondGrow!=1)throw new InvalidDataException("Only the first awakening is supported");if(bank==null||bank.Length<1)throw new InvalidDataException("Awakening skill bank");
  byte[] head=Head(id,gold,before,after);using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write(head);w.Write(KindAwakening);w.Write(secondGrow);w.Write(bank);return m.ToArray();}
 }
 internal static byte[] HellChallengeBody(ushort id,uint gold,Offline70EconomyState before,Offline70EconomyState after){
  byte[] head=Head(id,gold,before,after);using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write(head);w.Write(KindHellChallenge);return m.ToArray();}
 }
 internal static byte[] ExpertJobBody(ushort id,uint gold,byte expertType,Offline70EconomyState before,Offline70EconomyState after,byte[] bank){
  ExpertJobSkill(expertType);if(bank==null||bank.Length<1)throw new InvalidDataException("Expert job skill bank");
  byte[] head=Head(id,gold,before,after);using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write(head);w.Write(KindExpertJob);w.Write(expertType);w.Write(bank);return m.ToArray();}
 }
 internal static byte[] HeroBody(ushort id,uint gold,ushort dungeon,Offline70EconomyState before,Offline70EconomyState after){
  if(dungeon==0)throw new InvalidDataException("Hero dungeon bounds");
  byte[] head=Head(id,gold,before,after);using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write(head);w.Write(KindHero);w.Write(dungeon);return m.ToArray();}
 }
 internal static byte[] AwakeningReply(ushort id,uint gold,byte secondGrow,Offline70EconomyState before,Offline70EconomyState after,byte[] bank,byte[] keys){return Offline70RoleProtocol.BatchPacketFG(1,36,AwakeningBody(id,gold,secondGrow,before,after,bank),keys,false);}
 internal static byte[] HellChallengeReply(ushort id,uint gold,Offline70EconomyState before,Offline70EconomyState after,byte[] keys){return Offline70RoleProtocol.BatchPacketFG(1,36,HellChallengeBody(id,gold,before,after),keys,false);}
 internal static byte[] ExpertJobReply(ushort id,uint gold,byte expertType,Offline70EconomyState before,Offline70EconomyState after,byte[] bank,byte[] keys){return Offline70RoleProtocol.BatchPacketFG(1,36,ExpertJobBody(id,gold,expertType,before,after,bank),keys,false);}
 internal static byte[] HeroReply(ushort id,uint gold,ushort dungeon,Offline70EconomyState before,Offline70EconomyState after,byte[] keys){return Offline70RoleProtocol.BatchPacketFG(1,36,HeroBody(id,gold,dungeon,before,after),keys,false);}
 // op5 update after the hero quest. The original appends one entry [u16 dungeon][u16 4] to the
 // cleared-dungeon list (quest_reward.inc:456-460), but this client's per-entry value is a single
 // byte: the verified Dungeon-Selection-Protocol.Body(5)/ProfileBody fixtures ([u16 count] then
 // [u16 dungeon][u8 difficulty] per row) are live on towns 1..5, so value 4 (hero tier, above the
 // 3 = king our list advertises) is written as one byte here.
 internal static byte[] HeroUnlockBody(ushort dungeon){if(dungeon==0)throw new InvalidDataException("Hero dungeon bounds");return new byte[]{(byte)1,(byte)0,(byte)dungeon,(byte)(dungeon>>8),(byte)4};}
 internal static byte[] HeroUnlockPacket(ushort dungeon,byte[] keys){return Offline70RoleProtocol.BatchPacketFG(0,5,HeroUnlockBody(dungeon),keys,false);}
}
