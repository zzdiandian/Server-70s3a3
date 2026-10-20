using System;using System.IO;using System.Collections.Generic;using System.Xml;using System.Xml.Serialization;
public class Offline70QuestIdsFG { public ushort[] Ids=new ushort[0]; }
public class Offline70QuestDefinitionFG {
 public ushort Id;public string Name,Path,Grade,Type,Job,Reason,Difficulty;public int SubType,Npc,CompleteNpc,MinLevel,MaxLevel,GrowType,Relation,GoldMultiple=100;public bool Supported,Repeatable,IgnoreRewardLevel;
 public int[] Condition=new int[0],Rewards=new int[0],Selection=new int[0],Collisions=new int[0],MonsterDrops=new int[0],ClearDrops=new int[0];public Offline70QuestIdsFG[] Prerequisites=new Offline70QuestIdsFG[0];
 public ushort[] AlternateRewardQuests=new ushort[0];public bool AlternateRewardAny;public int[] AlternateRewards=new int[0];
 public int[] StartItems=new int[0];
 public bool EnforceCurrentLevel;public bool CanAbandon=true;public bool TitleRewardFG157;public bool JobChange;public byte RewardGrowType,JobChangeStage,RewardSecondGrow,RewardExpertJob;
 // PVF [job change quest] ring number: 1 = first-promotion ring, 2 = awakening ring, 0 = unset.
 // JobChangeStage carries the literal PVF value; the legacy bool keeps the shipped
 // <JobChange>true</JobChange> records parsing, and the reward kind implies its ring when the
 // element is absent, so a catalog landed without it still awakens instead of silently
 // completing the quest as an ordinary item reward.
 internal int ChangeStage(){return JobChangeStage!=0?JobChangeStage:JobChange?1:RewardSecondGrow>0?2:0;}
}
public class Offline70QuestCatalogDocumentFG {public int Format=1,RewardProfileVersion;public string PvfSha256;public List<Offline70QuestDefinitionFG> Quests=new List<Offline70QuestDefinitionFG>();}
class Offline70QuestCatalogFG {
 internal const string CurrentPvf="61CCC511C0B3804741387344DC6862C3D6731AAD3D4334142A35E095141AAE0D";
 internal readonly Dictionary<ushort,Offline70QuestDefinitionFG> Definitions=new Dictionary<ushort,Offline70QuestDefinitionFG>();
 internal static Offline70QuestCatalogFG Load(string path){
  if(new FileInfo(path).Length>8388608)throw new InvalidDataException("Quest catalog size");Offline70QuestCatalogDocumentFG document;
  using(FileStream f=File.OpenRead(path))using(XmlTextReader r=new XmlTextReader(f)){r.ProhibitDtd=true;r.XmlResolver=null;document=(Offline70QuestCatalogDocumentFG)new XmlSerializer(typeof(Offline70QuestCatalogDocumentFG)).Deserialize(r);}
  // Bounded, not exact: the shipped legacy tree is 3051 records; the appended current-tree
  // families raise it to 3212 (T-NEWJOB: n_quest/job/*, 106 records, plus T-CURRENT: 55 n_quest
  // records that passed the item/dungeon/prerequisite/NPC reachability gates in
  // local-run/pvf/gen_current_quests.py). The ceiling keeps rejecting a scrambled or truncated file.
  if(document.Format!=1||document.RewardProfileVersion!=1||document.PvfSha256!=CurrentPvf||document.Quests.Count<3051||document.Quests.Count>3300)throw new InvalidDataException("Quest catalog version/reward profile");
  Offline70QuestCatalogFG result=new Offline70QuestCatalogFG();foreach(Offline70QuestDefinitionFG q in document.Quests){if(q.Id==0||result.Definitions.ContainsKey(q.Id)||q.Condition==null||q.Rewards==null||q.Selection==null||q.Prerequisites==null||q.Collisions==null||q.StartItems==null||q.StartItems.Length%2!=0||q.StartItems.Length>64||q.RewardSecondGrow>1||q.JobChangeStage>2||q.RewardExpertJob>4||q.AlternateRewardQuests==null||q.AlternateRewards==null||q.AlternateRewards.Length%2!=0||q.AlternateRewardQuests.Length>256)throw new InvalidDataException("Quest definition bounds");for(int i=0;i<q.StartItems.Length;i+=2)if(q.StartItems[i]<=2||q.StartItems[i+1]<=0||q.StartItems[i+1]>1000000)throw new InvalidDataException("Quest start item pair");foreach(ushort id in q.AlternateRewardQuests)if(id==0)throw new InvalidDataException("Alternate reward prerequisite");for(int i=0;i<q.AlternateRewards.Length;i+=2)if(q.AlternateRewards[i]<0||q.AlternateRewards[i+1]<=0)throw new InvalidDataException("Alternate reward pair");Offline70TitleBookFG157.CheckProfile(q);result.Definitions.Add(q.Id,q);}return result;
 }
 internal Offline70QuestDefinitionFG Get(ushort id){Offline70QuestDefinitionFG q;if(!Definitions.TryGetValue(id,out q)||!q.Supported)throw new InvalidDataException("Unsupported quest "+id);return q;}
 internal Dictionary<int,uint> CompletionRequirements(Offline70QuestDefinitionFG q){Dictionary<int,uint> result=new Dictionary<int,uint>(),given=new Dictionary<int,uint>();if(q.Type=="[seeking]")for(int i=0;i<q.Condition.Length;i+=2){uint amount;result.TryGetValue(q.Condition[i],out amount);result[q.Condition[i]]=checked(amount+(uint)q.Condition[i+1]);}for(int i=0;i<q.StartItems.Length;i+=2){uint amount;given.TryGetValue(q.StartItems[i],out amount);given[q.StartItems[i]]=checked(amount+(uint)q.StartItems[i+1]);}foreach(KeyValuePair<int,uint> gift in given){bool retain=false;foreach(Offline70QuestDefinitionFG next in Definitions.Values){bool successor=false;foreach(Offline70QuestIdsFG group in next.Prerequisites)if(Array.IndexOf(group.Ids,q.Id)>=0)successor=true;if(!successor||next.Type!="[seeking]"||next.Condition.Length%2!=0)continue;ulong needed=0,provided=0;for(int i=0;i<next.Condition.Length;i+=2)if(next.Condition[i]==gift.Key)needed+=(uint)next.Condition[i+1];for(int i=0;i<next.StartItems.Length;i+=2)if(next.StartItems[i]==gift.Key)provided+=(uint)next.StartItems[i+1];if(needed>provided)retain=true;}if(retain)continue;uint required;result.TryGetValue(gift.Key,out required);result[gift.Key]=Math.Max(required,gift.Value);}return result;}
 internal static int[] ResolvedRewards(Offline70QuestDefinitionFG q,Offline70ProgressionStateFG s){if(q.AlternateRewardQuests.Length==0)return q.Rewards;bool satisfied=!q.AlternateRewardAny;foreach(ushort id in q.AlternateRewardQuests){bool complete=s.Completed.Contains(id);satisfied=q.AlternateRewardAny?satisfied||complete:satisfied&&complete;}return satisfied?q.AlternateRewards:q.Rewards;}
 internal bool ActiveVisible(Offline70QuestDefinitionFG q,Offline70ProgressionStateFG s){byte level=Offline70ProgressionFG.Level(s.Experience);return !q.EnforceCurrentLevel||level>=q.MinLevel&&level<=q.MaxLevel;}
 internal ushort[] Offers(Offline70ProgressionStateFG s){
  // Native21's quest list must retain accepted quests, as both reference engines do.
  List<ushort> offers=new List<ushort>();foreach(Offline70QuestActiveFG a in s.Active)if(ActiveVisible(Get(a.Id),s))offers.Add(a.Id);foreach(Offline70QuestDefinitionFG q in Definitions.Values)if(Available(q,s))offers.Add(q.Id);offers.Sort();return offers.ToArray();
 }
 internal bool Available(Offline70QuestDefinitionFG q,Offline70ProgressionStateFG s){
  byte level=Offline70ProgressionFG.Level(s.Experience);if(q.ChangeStage()==1&&Offline70RewardTypesFG183.FirstGrow(s.GrowType)!=0)return false;if(!q.Supported||level<q.MinLevel||level>q.MaxLevel||!JobMatches(q.Job,s.Job)||q.GrowType>=0&&q.GrowType!=Offline70RewardTypesFG183.FirstGrow(s.GrowType)||s.Find(q.Id)!=null||s.Completed.Contains(q.Id)&&!q.Repeatable)return false;
  foreach(int collision in q.Collisions)if(s.Completed.Contains((ushort)collision)||s.Find((ushort)collision)!=null)return false;
  // Expert job quests ([reward type] [expert job], [job change quest] 20): the original gates only on
  // the level floor (quest_reward.inc:386 get_charac_level()<m_levelMin), which MinLevel already
  // carries, and SetCurCharacExpertJobType overwrites silently. The four official rings are mutually
  // excluded by their PVF [collision quest] rows above; this state check covers the four *_test
  // twins (2703-2706) which ship with empty [collision quest] and would otherwise let a character
  // hold two different expert jobs. Re-granting the SAME type stays offered (the original does).
  if(q.RewardExpertJob!=0&&s.ExpertJobTypeFG183!=0&&s.ExpertJobTypeFG183!=q.RewardExpertJob)return false;
  // Reference engines agree: AND within one prerequisite group, alternatives across groups.
  bool prerequisites=q.Prerequisites.Length==0;foreach(Offline70QuestIdsFG group in q.Prerequisites){bool satisfied=true;foreach(ushort id in group.Ids)if(!s.Completed.Contains(id))satisfied=false;if(satisfied)prerequisites=true;}return prerequisites;
 }
 static bool JobMatches(string job,byte characterJob){return job=="[all]"||job=="[swordman]"&&characterJob==0||job=="[fighter]"&&characterJob==1||job=="[gunner]"&&characterJob==2||job=="[mage]"&&characterJob==3||job=="[priest]"&&characterJob==4;}
 internal static byte[] OfferBody(byte level,ushort[] ids){if(level<1||level>70||ids==null||ids.Length>2048)throw new InvalidDataException("Quest offer body bounds");using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write(level);w.Write((ushort)ids.Length);foreach(ushort id in ids)w.Write(id);return m.ToArray();}}
 internal static byte[] AcceptBody(ushort id,uint remaining){byte[] b=new byte[8];b[0]=1;BitConverter.GetBytes(id).CopyTo(b,1);BitConverter.GetBytes(remaining).CopyTo(b,3);return b;}
 internal static byte[] ProgressBody(ushort id,uint remaining){byte[] b=new byte[7];b[0]=1;BitConverter.GetBytes(id).CopyTo(b,1);BitConverter.GetBytes(remaining).CopyTo(b,3);return b;}
 // Same 11-byte fixed successful native36 reader shape as verified quest101; wider IDs need native replay.
 internal static byte[] FinishBody(ushort id){byte[] b=new byte[11];b[0]=1;BitConverter.GetBytes(id).CopyTo(b,1);return b;}
}
