using System;using System.IO;using System.Text;using System.Security.Cryptography;using System.Collections.Generic;
// Current CN70 ordinary solo card rules. Native source/proofs are in FG125.
// Magic-seal, premium events, luck accrual and high-level amplification are
// separate mechanics; the original clear constructor disables magic-seal.
public class Offline70CardItemFG125 {
 public uint ItemId;public int Seed=-1;public ushort Durability;public bool Sealed;
 internal uint Data {get{return Seed<0?1:(uint)Seed;}}
 internal string Fingerprint {get{return ItemId+":"+Seed+":"+Durability+":"+Sealed;}}
}
public class Offline70CardOfferFG125 {
 public string RunId;public ushort Dungeon;public byte Difficulty;public uint Fee;
 public Offline70CardItemFG125 Free=new Offline70CardItemFG125(),Paid=new Offline70CardItemFG125();
 public bool FreeDelivered,PaidPurchased;
}
class Offline70ClearItemsFG125 {
 // Regenerated 2026-10-16 from the CN70 PVF with local-run/pvf/gen_card_items.py: the clear-reward
 // generateTable recipe (etc/itemdropinfo_clearreward.etc [item drop ref table], keepRandomOption
 // =false) reproduces every shipped 1..39 cell row-for-row, so the domain now spans levels 1..70.
 internal const string CatalogSha256="7eac246ca195ae4a34161d9fc5c16015ed0144b28815d5f1db6d233936c461e1";
 static readonly Dictionary<string,uint[][]> Tables=new Dictionary<string,uint[][]>();
 static readonly Dictionary<uint,Offline70CardItemFG125> Properties=new Dictionary<uint,Offline70CardItemFG125>();
 static string Key(int type,int rarity,int level){return type+":"+rarity+":"+level;}
 internal static void Load(string path){
  if(Tables.Count!=0)return;byte[] b=File.ReadAllBytes(path);if(b.Length>2097152)throw new InvalidDataException("Card catalog length");
  StringBuilder hash=new StringBuilder();using(SHA256 h=SHA256.Create())foreach(byte n in h.ComputeHash(b))hash.Append(n.ToString("x2"));
  if(hash.ToString()!=CatalogSha256)throw new InvalidDataException("Card catalog evidence hash");
  using(BinaryReader r=new BinaryReader(new MemoryStream(b),Encoding.ASCII)){
   if(Encoding.ASCII.GetString(r.ReadBytes(8))!="FG125CF1"||Encoding.ASCII.GetString(r.ReadBytes(64)).ToUpperInvariant()!=Offline70QuestCatalogFG.CurrentPvf)throw new InvalidDataException("Card catalog PVF");
   int count=r.ReadUInt16();if(count!=1680)throw new InvalidDataException("Card catalog table count");
   for(int i=0;i<count;i++){
    int type=r.ReadByte(),rarity=r.ReadByte(),level=r.ReadByte(),rows=r.ReadUInt16();string key=Key(type,rarity,level);
    if(type<1||type>4||rarity>5||level<1||level>70||rows>2048||Tables.ContainsKey(key))throw new InvalidDataException("Card table profile");
    uint[][] entries=new uint[rows][];uint previous=0;
    for(int j=0;j<rows;j++){uint weight=r.ReadUInt32(),id=r.ReadUInt32();if(weight<=previous||id<3)throw new InvalidDataException("Card CDF profile");Offline70Economy.Definition(id);entries[j]=new uint[]{weight,id};previous=weight;}
    Tables.Add(key,entries);
   }
   int items=r.ReadUInt16();if(items!=4820)throw new InvalidDataException("Card item properties count");
   for(int i=0;i<items;i++){uint id=r.ReadUInt32();byte type=r.ReadByte();ushort durability=r.ReadUInt16();bool sealedItem=r.ReadBoolean();Offline70EconomyItemDefinition d=Offline70Economy.Definition(id);if(type!=d.Kind||d.MinimumLevel>70||durability!=d.MaxDurability||sealedItem&&d.Kind!=4&&d.Kind!=8)throw new InvalidDataException("Card creation profile");Properties.Add(id,new Offline70CardItemFG125{ItemId=id,Seed=d.Kind==4||d.Kind==8?0:-1,Durability=durability,Sealed=sealedItem});}
   if(r.BaseStream.Position!=r.BaseStream.Length)throw new InvalidDataException("Card catalog trailing bytes");
  }
  foreach(uint[][] table in Tables.Values)foreach(uint[] row in table)if(!Properties.ContainsKey(row[1]))throw new InvalidDataException("Card property coverage");
 }
 // Force original f32 stores for exploration before the next calculation.
 static float Store(double v){return BitConverter.ToSingle(BitConverter.GetBytes((float)v),0);}
 internal static float Exploration(int passed,int total){
  if(total<1||total>256||passed<1||passed>total)throw new InvalidDataException("Card room accounting");
  float ratio=Store((double)passed/total);int rate=total==1?2000:total==2?3000:total==3?4000:total==4?5000:total==5?6000:0;
  return rate==0?ratio:Store((double)rate*ratio/10000);
 }
 internal static int Threshold(int level,byte difficulty,int passed,int total,bool paid){
  if(level<1||level>70||difficulty>3)throw new InvalidDataException("Current solo card inputs");
  int prob=paid||level<=15?9000:8000;int bonus=difficulty==0?0:difficulty==1?300:difficulty==2?600:1000;
  // Original stGenerateRefData standard rate100. Solo cannot clear while dead;
  // its dead-at-room-clear counter is zero (revival itself is not that counter).
  return checked((int)((double)prob*(paid?1f:Exploration(passed,total)))+bonus);
 }
 internal static int Type(uint draw){if(draw>10000)throw new InvalidDataException("Card type draw");return draw<9000?2:draw<9500?3:draw<10000?4:1;}
 internal static int Rarity(int type,uint draw){
  if(draw>1000000||type<1||type>4)throw new InvalidDataException("Card rarity draw");
  int[] basis=new int[]{800000,989900,999900,1000000,1000001,1000002};
  if(type==2){int delta=-unchecked((basis[2]-basis[1])*3000)/10000;basis[0]+=unchecked(basis[0]*delta)/basis[1];basis[1]+=delta;}
  for(int i=0;i<basis.Length;i++)if(basis[i]>=draw)return i;return 0;
 }
 internal static bool HasTable(int level){for(int type=1;type<=4;type++)for(int rarity=0;rarity<=5;rarity++)if(Tables.ContainsKey(Key(type,rarity,level)))return true;return false;}
 internal static uint Total(int type,int rarity,int level){uint[][] rows;if(!Tables.TryGetValue(Key(type,rarity,level),out rows))throw new InvalidDataException("Unproven card table");return rows.Length==0?0:rows[rows.Length-1][0];}
 internal static uint Choose(int type,int rarity,int level,uint draw){
  uint[][] rows;if(!Tables.TryGetValue(Key(type,rarity,level),out rows))throw new InvalidDataException("Unproven card table");if(rows.Length==0)return 0;if(draw>rows[rows.Length-1][0])throw new InvalidDataException("Card CDF draw");
  int low=0,high=rows.Length-1;while(low<high){int middle=(low+high)/2;if(rows[middle][0]>=draw)high=middle;else low=middle+1;}return rows[low][1];
 }
 internal static Offline70CardItemFG125 Select(int level,byte difficulty,int passed,int total,bool paid,uint chance,uint typeDraw,uint rarityDraw,uint cdfDraw,int seed){
  if(chance>10000||seed<0)throw new InvalidDataException("Card creation draw");if(chance>=Threshold(level,difficulty,passed,total,paid))return new Offline70CardItemFG125();
  int type=Type(typeDraw),rarity=Rarity(type,rarityDraw);uint id=Choose(type,rarity,level,cdfDraw);if(id==0)return new Offline70CardItemFG125();
  Offline70CardItemFG125 p=Properties[id];return new Offline70CardItemFG125{ItemId=id,Seed=p.Seed<0?-1:seed,Durability=p.Durability,Sealed=p.Sealed};
 }
 // Equal inclusive draws with rejection. Persisted run plans prevent a request,
 // full bag or duplicate selection from rerolling an offered item.
 internal static uint Draw(string run,string purpose,uint maximum){
  ulong span=(ulong)maximum+1,limit=4294967296UL/span*span;
  for(int attempt=0;attempt<1024;attempt++){byte[] digest;using(SHA256 h=SHA256.Create())digest=h.ComputeHash(Encoding.UTF8.GetBytes(run+":fg125:"+purpose+":"+attempt));uint v=BitConverter.ToUInt32(digest,0);if(v<limit)return (uint)(v%span);}
  throw new InvalidDataException("Card random rejection limit");
 }
 static Offline70CardItemFG125 Plan(string run,int level,byte difficulty,int passed,int total,bool paid){
  string key=paid?"paid":"free";uint chance=Draw(run,key+":chance",10000);if(chance>=Threshold(level,difficulty,passed,total,paid))return new Offline70CardItemFG125();
  uint typeDraw=Draw(run,key+":type",10000),rarityDraw=Draw(run,key+":rarity",1000000);int type=Type(typeDraw),rarity=Rarity(type,rarityDraw);uint weight=Total(type,rarity,level);if(weight==0)return new Offline70CardItemFG125();
  return Select(level,difficulty,passed,total,paid,chance,typeDraw,rarityDraw,Draw(run,key+":cdf",weight),(int)Draw(run,key+":seed",Int32.MaxValue));
 }
 internal static Offline70CardOfferFG125 Offer(string run,ushort dungeon,byte difficulty,int passed,int total){
  Guid parsed=new Guid(run);if(parsed.ToString("N")!=run)throw new InvalidDataException("Card run identifier");int level=Offline70ClearExperienceFG116.Basis(dungeon);
  // The catalog now carries the complete CN70 grid: every (type,rarity,level) cell for levels
  // 1..70, so every dungeon basis (Snow 40..49, Act4/5 up to 64) resolves to a real card pool.
  // The empty-offer branch stays as the guard for a basis outside that domain: no invented item,
  // and Claim skips the fee for an empty paid card.
  if(!HasTable(level))return new Offline70CardOfferFG125{RunId=run,Dungeon=dungeon,Difficulty=difficulty,Fee=Offline70PaidCostFG120.ForDungeon(dungeon),Free=new Offline70CardItemFG125(),Paid=new Offline70CardItemFG125()};
  return new Offline70CardOfferFG125{RunId=run,Dungeon=dungeon,Difficulty=difficulty,Fee=Offline70PaidCostFG120.ForDungeon(dungeon),Free=Plan(run,level,difficulty,passed,total,false),Paid=Plan(run,level,difficulty,passed,total,true)};
 }
 internal static void ValidateItem(Offline70CardItemFG125 item){if(item==null||item.ItemId==0&&(item.Seed!=-1||item.Durability!=0||item.Sealed))throw new InvalidDataException("Empty card item state");if(item.ItemId==0)return;Offline70CardItemFG125 p;if(!Properties.TryGetValue(item.ItemId,out p)||item.Seed<-1||(p.Seed<0)!=(item.Seed<0)||item.Durability!=p.Durability||item.Sealed!=p.Sealed)throw new InvalidDataException("Card item creation state");}
 internal static void Validate(List<Offline70CardOfferFG125> offers){if(offers==null||offers.Count>256)throw new InvalidDataException("Card offers bounds");HashSet<string> runs=new HashSet<string>();foreach(Offline70CardOfferFG125 o in offers){Guid run;if(o==null||o.RunId==null||o.RunId.Length!=32)throw new InvalidDataException("Card offer identifier");try{run=new Guid(o.RunId);}catch(Exception e){throw new InvalidDataException("Card offer identifier",e);}if(run.ToString("N")!=o.RunId||!runs.Add(o.RunId)||o.Fee!=Offline70PaidCostFG120.ForDungeon(o.Dungeon)||o.Difficulty>3)throw new InvalidDataException("Card offer profile");ValidateItem(o.Free);ValidateItem(o.Paid);}}
 internal static Offline70CardOfferFG125 Find(Offline70EconomyState s,string run){foreach(Offline70CardOfferFG125 o in s.CardOffersFG125)if(o.RunId==run)return o;throw new InvalidDataException("Card offer not persisted");}
 internal static void Record(Offline70EconomyState s,Offline70CardOfferFG125 offer){ValidateItem(offer.Free);ValidateItem(offer.Paid);foreach(Offline70CardOfferFG125 old in s.CardOffersFG125)if(old.RunId==offer.RunId)throw new InvalidDataException("Card offer repeated mutation");while(s.CardOffersFG125.Count>=128){int old=s.CardOffersFG125.FindIndex(delegate(Offline70CardOfferFG125 o){return o.FreeDelivered;});if(old<0)throw new InvalidDataException("Claim pending free cards before another run");s.CardOffersFG125.RemoveAt(old);}s.CardOffersFG125.Add(offer);}
 internal static void Add(Offline70EconomyState s,Offline70CardItemFG125 item){
  ValidateItem(item);if(item.ItemId==0)return;HashSet<string> before=new HashSet<string>();foreach(Offline70EconomyEntry e in s.Inventory)before.Add(e.List+":"+e.Slot);
  Offline70Economy.AddAssetItem(s,item.ItemId,1);if(item.Seed<0)return;
  foreach(Offline70EconomyEntry e in s.Inventory)if(e.ItemId==item.ItemId&&!before.Contains(e.List+":"+e.Slot)){e.EquipmentSeed=item.Seed;e.Durability=item.Durability;e.Sealed=item.Sealed;return;}
  throw new InvalidDataException("Card equipment new instance missing");
 }
 internal static Offline70EconomyState Claim(string path,string run,bool paid,out bool duplicate){
  Offline70CardOfferFG125 expected=Find(Offline70Economy.Load(path),run);Offline70CardItemFG125 item=paid?expected.Paid:expected.Free;
  return Offline70Economy.Apply(path,run+(paid?"-paid-card-125":"-free-card-125"),paid?"paid-card":"free-card",expected.Dungeon+":"+expected.Difficulty+":"+(paid?expected.Fee:0)+":"+item.Fingerprint,delegate(Offline70EconomyState s){Offline70CardOfferFG125 o=Find(s,run);if(paid?o.PaidPurchased:o.FreeDelivered)throw new InvalidDataException("Card delivery state conflict");if(paid&&o.Paid.ItemId!=0&&s.Gold<o.Fee)throw new InvalidDataException("Insufficient paid-card gold");Add(s,paid?o.Paid:o.Free);if(paid){if(o.Paid.ItemId!=0)s.Gold-=o.Fee;o.PaidPurchased=true;}else o.FreeDelivered=true;},out duplicate);
 }
 // Keep a full-bag free result pending across leaving the dungeon and login.
 // Its seed and identity are fixed. No item is silently discarded or rerolled.
 internal static Offline70EconomyState Recover(string path){Offline70EconomyState s=Offline70Economy.Load(path);List<string> runs=new List<string>();foreach(Offline70CardOfferFG125 o in s.CardOffersFG125)if(!o.FreeDelivered)runs.Add(o.RunId);foreach(string run in runs)try{bool duplicate;s=Claim(path,run,false,out duplicate);}catch(InvalidDataException){s=Offline70Economy.Load(path);}return s;}
 internal static byte[] ClearBody(byte[] legacy,uint freeGold,uint fee,Offline70CardItemFG125 item){
  if(legacy==null||legacy.Length!=91)throw new InvalidDataException("Clear XP wire baseline");ValidateItem(item);
  using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write(legacy,0,62);w.Write((byte)(item.ItemId==0?1:2));w.Write((uint)0);w.Write(freeGold);if(item.ItemId!=0){w.Write(item.ItemId);w.Write(item.Data);}w.Write((byte)0);w.Write((byte)0);w.Write((byte)0);w.Write(fee);w.Write(legacy,78,13);return m.ToArray();}
 }
 internal static byte[] SelectionBody(byte free,byte paid,Offline70CardItemFG125 item){
  ValidateItem(item);using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)){w.Write((byte)1);for(int card=0;card<4;card++){w.Write(card==free?(byte)0:(byte)255);w.Write(card==paid?(byte)0:(byte)255);bool chosen=card==paid;w.Write(chosen?(item.ItemId==0?(byte)1:(byte)2):(byte)0);if(chosen){w.Write((uint)0);w.Write((uint)0);if(item.ItemId!=0){w.Write(item.ItemId);w.Write(item.Data);}}w.Write((byte)0);}return m.ToArray();}
 }
 internal static int Quotes(string[] args){if(args.Length!=3)return 2;Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"card-items-FG125.dat"));using(StreamWriter w=new StreamWriter(args[2]))foreach(string line in File.ReadAllLines(args[1])){string[] v=line.Split(',');if(v.Length!=10)throw new InvalidDataException("Card quote input");int level=Int32.Parse(v[0]),passed=Int32.Parse(v[2]),total=Int32.Parse(v[3]);byte diff=Byte.Parse(v[1]);bool paid=v[4]=="1";Offline70CardItemFG125 item=Select(level,diff,passed,total,paid,UInt32.Parse(v[5]),UInt32.Parse(v[6]),UInt32.Parse(v[7]),UInt32.Parse(v[8]),Int32.Parse(v[9]));w.WriteLine(Threshold(level,diff,passed,total,paid)+","+item.ItemId+","+item.Seed+","+item.Durability+","+(item.Sealed?1:0));}return 0;}
}
