using System;using System.IO;using System.Text;using System.Security.Cryptography;using System.Collections.Generic;
// Original TW8535ED2 ordinary solo pools1/3 with current CN70 PVF inputs.
// Pools2/4, magic-seal, premium/team modifiers and equipment luck are separate.
class Offline70GeneralStackFG126 {
 internal const string CatalogSha256="f9d738efa9cbc74057649a1641b1c4cd68db438bb1082646f458be137d7b1ccf";
 static readonly Dictionary<string,uint[][]> Tables=new Dictionary<string,uint[][]>();
 static readonly float[] Difficulty=new float[]{1f,1.2f,1.4f,1.6f,2f},Kinds=new float[]{1f,2f,3f,4f};
 static readonly int[] StackRarity=new int[]{700000,964900,990000,1000000,1000001,1000002},RecipeRarity=new int[]{500000,944900,999900,1000000,1000001,1000002};
 static string Key(int pool,int rarity,int level){return pool+":"+rarity+":"+level;}
 internal static void Load(string path){
  if(Tables.Count!=0)return;byte[] b=File.ReadAllBytes(path);if(b.Length>2097152)throw new InvalidDataException("Floor stack catalog size");
  StringBuilder hash=new StringBuilder();using(SHA256 h=SHA256.Create())foreach(byte n in h.ComputeHash(b))hash.Append(n.ToString("x2"));if(hash.ToString()!=CatalogSha256)throw new InvalidDataException("Floor stack catalog evidence hash");
  using(BinaryReader r=new BinaryReader(new MemoryStream(b),Encoding.ASCII)){
   if(Encoding.ASCII.GetString(r.ReadBytes(8))!="FG126FS1"||Encoding.ASCII.GetString(r.ReadBytes(64)).ToUpperInvariant()!=Offline70QuestCatalogFG.CurrentPvf)throw new InvalidDataException("Floor stack catalog PVF");
   int count=r.ReadUInt16();if(count!=840)throw new InvalidDataException("Floor stack table count");
   for(int i=0;i<count;i++){
    int pool=r.ReadByte(),rarity=r.ReadByte(),level=r.ReadByte(),rows=r.ReadUInt16();string key=Key(pool,rarity,level);if(pool!=1&&pool!=3||rarity>5||level<1||level>70||rows>2048||Tables.ContainsKey(key))throw new InvalidDataException("Floor stack table profile");
    uint[][] entries=new uint[rows][];uint previous=0;
    for(int j=0;j<rows;j++){uint weight=r.ReadUInt32(),id=r.ReadUInt32();if(weight<=previous||id<3)throw new InvalidDataException("Floor stack CDF profile");Offline70EconomyItemDefinition d=Offline70Economy.Definition(id);if(d.Kind!=1&&d.Kind!=2||d.MaxDurability!=0)throw new InvalidDataException("Floor stack item category");entries[j]=new uint[]{weight,id};previous=weight;}
    Tables.Add(key,entries);
   }
   if(r.BaseStream.Position!=r.BaseStream.Length)throw new InvalidDataException("Floor stack catalog trailing bytes");
  }
 }
 internal static int Threshold(int basis,byte diff,byte kind,int pool){
  if(basis<1||basis>99||diff>3||kind>3||pool!=1&&pool!=3)throw new InvalidDataException("Ordinary floor stack profile");int probability=0;
  foreach(int[] row in Offline70FloorCatalogFG119.Probability)if(row[0]<=basis&&basis<=row[1]){probability=row[pool+2];break;}
  // Original script stores f32 rates, then x87 multiplies before truncation.
  // E.g.60*1.4f is83.999998, so the inclusive threshold is83, not84.
  return (int)((double)probability*Difficulty[diff]*Kinds[kind]);
 }
 internal static int Rarity(int pool,uint draw){if(draw>1000000||pool!=1&&pool!=3)throw new InvalidDataException("Floor stack rarity draw");int[] cuts=pool==1?StackRarity:RecipeRarity;for(int i=0;i<cuts.Length;i++)if(cuts[i]>=draw)return i;return 0;}
 internal static uint Total(int pool,int rarity,int level){uint[][] rows;if(!Tables.TryGetValue(Key(pool,rarity,level),out rows))throw new InvalidDataException("Unproven floor stack level");return rows.Length==0?0:rows[rows.Length-1][0];}
 internal static uint Choose(int pool,int rarity,int level,uint draw){uint[][] rows;if(!Tables.TryGetValue(Key(pool,rarity,level),out rows))throw new InvalidDataException("Unproven floor stack table");if(rows.Length==0)return 0;if(draw>rows[rows.Length-1][0])throw new InvalidDataException("Floor stack CDF draw");int low=0,high=rows.Length-1;while(low<high){int middle=(low+high)/2;if(rows[middle][0]>=draw)high=middle;else low=middle+1;}return rows[low][1];}
 internal static uint Select(int level,int basis,byte diff,byte kind,int pool,uint chance,uint rarity,uint cdf){if(chance>10000)throw new InvalidDataException("Floor stack chance draw");if(chance>Threshold(basis,diff,kind,pool))return 0;return Choose(pool,Rarity(pool,rarity),level,cdf);}
 internal static void Append(List<Offline70FloorDropsFG119.Reward> rewards,int level,int basis,byte diff,byte kind,byte rolls,string receipt){
  // The catalog covers pool1/3 rarity0..5 levels1..70 (840 tables, data/floor-stack-FG126.dat), regenerated
  // from the CN70 PVF with CItemGenRateTable::generateTable; see local-run/recon/floor-stack-4170-report.md.
  // A monster above level 70 is outside that evidence domain, so that source is skipped instead of
  // rejecting the kill. Levels 1..40 keep the exact behavior of the previously shipped tables.
  if(level>70)return;
  if(level<1||rolls>32||receipt==null||receipt.Length>200)throw new InvalidDataException("Floor stack roster context");
  for(int i=0;i<rolls;i++)foreach(int pool in new int[]{1,3}){
   string purpose="floor126:"+i+":"+pool;uint chance=Offline70ClearItemsFG125.Draw(receipt,purpose+":chance",10000);if(chance>Threshold(basis,diff,kind,pool))continue;
   int rarity=Rarity(pool,Offline70ClearItemsFG125.Draw(receipt,purpose+":rarity",1000000));uint total=Total(pool,rarity,level);if(total==0)continue;
   uint id=Choose(pool,rarity,level,Offline70ClearItemsFG125.Draw(receipt,purpose+":cdf",total));if(id!=0)rewards.Add(new Offline70FloorDropsFG119.Reward{Item=id,Quantity=1,Source=pool==1?"general-stack-original126":"general-recipe-original126"});
  }
 }
 internal static int Quotes(string[] args){if(args.Length!=3)throw new InvalidDataException("Floor stack quote arguments");using(StreamWriter w=new StreamWriter(args[2],false,Encoding.UTF8)){foreach(string line in File.ReadAllLines(args[1])){if(line.Length==0||line.StartsWith("level"))continue;string[] c=line.Split('\t');if(c.Length!=8)throw new InvalidDataException("Floor quote columns");uint id=Select(Int32.Parse(c[0]),Int32.Parse(c[1]),Byte.Parse(c[2]),Byte.Parse(c[3]),Int32.Parse(c[4]),UInt32.Parse(c[5]),UInt32.Parse(c[6]),UInt32.Parse(c[7]));w.WriteLine(id);}}return 0;}
}
