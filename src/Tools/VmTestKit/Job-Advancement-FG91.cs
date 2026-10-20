using System;using System.IO;
class Offline70JobAdvancementFG91 {
 // Native command36's grow-type branch reads one grow byte and two skill
 // banks (count u8 + slot/id/level triples), unlike the item reward branch.
 // Kind 1 = [grow type] first-promotion ring, kind 2 = [awakening type] (STQuestScript.cpp
 // reward-type table; the original writes the same shape for both, quest_reward.inc 336-374).
 // The grow byte is the character's grow byte after the change (first|second<<4), so the
 // promotion ring passes 1..5 and the first awakening passes first|0x10.
 internal static byte[] Body(ushort id,byte grow,uint gold,Offline70EconomyState before,Offline70EconomyState after,Offline70SkillSlotState skills){return Body(id,1,grow,gold,before,after,skills);}
 internal static byte[] Body(ushort id,byte kind,byte grow,uint gold,Offline70EconomyState before,Offline70EconomyState after,Offline70SkillSlotState skills){
  if(kind!=1&&kind!=2)throw new InvalidDataException("Growth reward kind");
  if(!Offline70RewardTypesFG183.IsGrowByte(grow)||after.GrowType!=grow)throw new InvalidDataException("Promotion reply identity");Offline70SkillSlots.Validate(skills);
  byte[] item=Offline70QuestWireFG.FinishRewardBody(id,gold,before,after);
  int prefix=9+item[8]*7;
  // Promotion quests must not silently omit ordinary item rewards.
  if(item[prefix]!=0||item[prefix+1]!=0)throw new InvalidDataException("Promotion mixed item rewards");
  using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){
   w.Write(item,0,prefix);w.Write(kind);w.Write(grow);w.Write((byte)Offline70SkillSlots.VisibleCountFG139(skills));
   for(int i=0;i<skills.Ids.Length;i++){if(skills.Levels[i]==0)continue;w.Write(skills.Slots[i]);w.Write(skills.Ids[i]);w.Write(skills.Levels[i]);}
   w.Write((byte)0);return m.ToArray();
  }
 }
 internal static byte[] Reply(ushort id,byte grow,uint gold,Offline70EconomyState before,Offline70EconomyState after,Offline70SkillSlotState skills,byte[] keys){return Reply(id,1,grow,gold,before,after,skills,keys);}
 internal static byte[] Reply(ushort id,byte kind,byte grow,uint gold,Offline70EconomyState before,Offline70EconomyState after,Offline70SkillSlotState skills,byte[] keys){return Offline70RoleProtocol.BatchPacketFG(1,36,Body(id,kind,grow,gold,before,after,skills),keys,false);}
}
