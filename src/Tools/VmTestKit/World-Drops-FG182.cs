using System;using System.IO;using System.Xml;using System.Xml.Serialization;using System.Collections.Generic;using System.Security.Cryptography;using System.Text;
public class WorldDropEntryFG182 {[XmlAttribute]public uint Item;[XmlAttribute]public uint Weight;}
public class WorldDropProfileFG182 {public int Id;public List<WorldDropEntryFG182> Entries=new List<WorldDropEntryFG182>();}
public class WorldDropCatalogFG182 {public int Format;public string PvfSha256,DocumentSha256;public List<WorldDropProfileFG182> Profiles=new List<WorldDropProfileFG182>();public List<int> Levels=new List<int>();}
// Current CN table with complete reference089022BE/085383B8 selection evidence.
// Ordinary solo, neutral100 drop rate, one draw for non-boss monster deaths.
// Other event/team/war-area trigger policies are not implemented here.
class Offline70WorldDropsFG182 {
 internal const string CatalogSha256="c102e106c6f30864c7b014b49d055ffc076bade56d7180b0e84e9c6313f108c9";
 static WorldDropCatalogFG182 Data;static uint[] Totals;
 internal static void Load(string path){
  if(Data!=null)return;byte[] bytes=File.ReadAllBytes(path);if(bytes.Length>1048576)throw new InvalidDataException("World catalog size");
  StringBuilder hash=new StringBuilder();using(SHA256 h=SHA256.Create())foreach(byte b in h.ComputeHash(bytes))hash.Append(b.ToString("x2"));if(hash.ToString()!=CatalogSha256)throw new InvalidDataException("World catalog identity");
  WorldDropCatalogFG182 d;using(XmlTextReader r=new XmlTextReader(new MemoryStream(bytes))){r.ProhibitDtd=true;r.XmlResolver=null;d=(WorldDropCatalogFG182)new XmlSerializer(typeof(WorldDropCatalogFG182)).Deserialize(r);}
  if(d.Format!=1||d.PvfSha256==null||d.PvfSha256.ToUpperInvariant()!=Offline70QuestCatalogFG.CurrentPvf||d.Profiles==null||d.Profiles.Count!=24||d.Levels==null||d.Levels.Count!=99)throw new InvalidDataException("World catalog profile");
  uint[] totals=new uint[d.Profiles.Count];for(int i=0;i<d.Profiles.Count;i++){WorldDropProfileFG182 p=d.Profiles[i];if(p==null||p.Id!=i||p.Entries==null||p.Entries.Count>256)throw new InvalidDataException("World profile index");uint total=0;foreach(WorldDropEntryFG182 e in p.Entries){if(e==null||e.Item<3||e.Weight>100000)throw new InvalidDataException("World row bounds");total=checked(total+e.Weight);if(e.Weight>0){byte kind=Offline70Economy.Definition(e.Item).Kind;if(kind!=1&&kind!=2)throw new InvalidDataException("World stack item kind");}}if(total==0||total>100000)throw new InvalidDataException("World total");totals[i]=total;}
  foreach(int id in d.Levels)if(id<0||id>=d.Profiles.Count)throw new InvalidDataException("World level profile");Data=d;Totals=totals;
 }
 internal static uint Total(int level){if(Data==null)throw new InvalidDataException("World catalog not loaded");return level<1||level>99?0:Totals[Data.Levels[level-1]];}
 internal static uint Select(int level,uint chance,uint cdf){if(level<0||level>255||chance>100000)throw new InvalidDataException("World draw bounds");uint total=Total(level);if(cdf>total)throw new InvalidDataException("World CDF bounds");if(total==0||chance>total)return 0;uint sum=0;foreach(WorldDropEntryFG182 e in Data.Profiles[Data.Levels[level-1]].Entries){sum+=e.Weight;if(cdf<sum)return e.Item;}return 0;}
 internal static void Append(List<Offline70FloorDropsFG119.Reward> rewards,int level,byte kind,string receipt){
  if(rewards==null||level<1||level>99||kind>3||String.IsNullOrEmpty(receipt)||receipt.Length>200)throw new InvalidDataException("World death context");if(kind==3)return;
  uint chance=Offline70ClearItemsFG125.Draw(receipt,"world182:chance",100000);if(chance>Total(level))return;uint cdf=Offline70ClearItemsFG125.Draw(receipt,"world182:cdf",Total(level));uint id=Select(level,chance,cdf);if(id!=0)rewards.Add(new Offline70FloorDropsFG119.Reward{Item=id,Quantity=1,Source="world-original182"});
 }
}
