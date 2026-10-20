using System;using System.IO;using System.Collections.Generic;
partial class Offline70SkillSlots {
 internal static byte ListStartFG123(byte id){switch(id){
  case 56:case 61:case 93:case 94:case 172:case 187:return 102;
  case 29:case 33:case 35:case 37:case 55:case 197:return 54;
  // First-awakening passives 89..92 sit in the same tab as the branch passives above. Evidence:
  // the original allocates a granted skill's slot from CSkill::get_group() (rednf
  // CListTables/init.inc:105-107 reform_ui_group_no([skill class], special,
  // [skill fitness growtype] row count) + SkillSlot/addSkillOnCreateCharacter.inc:57-79, whose
  // group ranges are g0 [6,54) / g1 [54,102) / g2 [102,150) / g3 [150,198) - exactly this file's
  // tab bases). PVF measured (local-run/pvf/awakening_pvf_truth.py -> awakening-pvf-truth.tsv):
  // 89 class 3, 90 class 2, 91 class 1, 92 class 0, every one with an EMPTY [skill fitness
  // growtype] row list (0 rows <= 2 => group 1) and [type] [passive], so all four land in 54..101.
  // The same rule reproduces 120 of the 121 ids this switch already groups (only 197 conversion
  // reads 6 instead of the 54 captured from the live client), and it is why 191..194 ([skill class]
  // 4 => group 2) went to 102 rather than here.
  case 89:case 90:case 91:case 92:return 54;
  case 1:case 5:case 8:case 11:case 12:case 13:case 14:case 15:case 16:case 17:case 20:case 25:case 26:case 28:case 45:case 46:case 48:case 58:case 59:case 64:case 65:case 77:return 6;
  case 2:case 3:case 4:case 6:case 7:case 9:case 10:case 18:case 19:case 21:case 22:case 23:case 24:case 27:case 30:case 31:case 32:case 34:case 36:case 38:case 39:case 40:case 41:case 44:case 47:case 49:case 50:case 51:case 52:case 53:case 54:case 57:case 60:case 62:case 63:case 66:case 67:case 68:case 69:case 70:case 71:case 72:case 73:case 74:case 75:case 76:case 78:case 79:case 80:case 81:case 82:case 83:case 84:case 103:case 104:case 105:case 106:case 107:case 108:case 109:case 110:case 111:case 112:case 113:case 114:return 54;
  case 169:case 170:case 171:case 173:case 174:case 175:case 176:case 177:case 178:case 179:case 180:case 181:case 182:case 184:case 186:case 188:case 189:case 190:case 191:case 192:case 193:case 194:return 102;
  default:throw new InvalidDataException("Skill placement outside original SP catalog");}}
 internal static bool ListSlotFG123(byte id,int slot){int first=ListStartFG123(id);return slot>=first&&slot<first+48;}
 internal static bool PassiveSlotFG123(Offline70SkillSlotState s,byte id,int slot){return ListSlotFG123(id,slot)||slot>=100&&slot<150;}
 internal static byte FindListFG123(Offline70SkillSlotState s,byte id){int first=ListStartFG123(id);for(int slot=first;slot<first+48;slot++)if(Array.IndexOf(s.Slots,(byte)slot)<0)return (byte)slot;throw new InvalidDataException("Original skill tab is full");}
 internal static Offline70SkillSlotState NormalizePlacementFG123(string path){string full=Path.GetFullPath(path);Directory.CreateDirectory(Path.GetDirectoryName(full));using(FileStream held=new FileStream(full+".lock",FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None)){Offline70SkillSlotState s=Load(full);List<int> move=new List<int>();for(int i=0;i<s.Ids.Length;i++)if(!Quick(s.Slots[i])&&!ListSlotFG123(s.Ids[i],s.Slots[i])){if(i<11&&!Active(s.Ids[i]))throw new InvalidDataException("Initial passive slot changed");move.Add(i);s.Slots[i]=255;}if(move.Count==0)return s;foreach(int i in move)s.Slots[i]=FindListFG123(s,s.Ids[i]);Validate(s);string temp=full+".pending-"+Guid.NewGuid().ToString("N"),backup=full+".backup-"+Guid.NewGuid().ToString("N");try{using(FileStream f=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None)){Serializer.Serialize(f,s);f.Flush();}Validate(Load(temp));if(File.Exists(full))File.Replace(temp,full,backup);else File.Move(temp,full);return Load(full);}finally{if(File.Exists(temp))File.Delete(temp);}}}
}
