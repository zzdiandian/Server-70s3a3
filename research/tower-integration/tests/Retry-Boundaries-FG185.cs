using System;using System.IO;using System.Collections.Generic;using System.Reflection;using System.Runtime.Serialization;using System.Xml.Serialization;
// Exercises the real maintenance method on a full disposable economic store.
// No socket is supplied: capacity must refuse before any reply could be sent.
class TowerRetryBoundariesFG185 {
 static int checks;static void Check(bool value,string message){checks++;if(!value)throw new Exception(message);}
 static FieldInfo Field(string name){return typeof(Offline70BatchServiceFG).GetField(name,BindingFlags.Instance|BindingFlags.NonPublic);}
 static int Main(string[] args){try{
  string output=Path.GetFullPath(args[0]);if(output.IndexOf(Path.Combine("build","tower-integration-research"),StringComparison.OrdinalIgnoreCase)<0)throw new Exception("Disposable test scope");
  string directory=Path.Combine(output,"retry-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(directory);
  Offline70WishBoxFG84.Register();Offline70BoosterRewardsFG47.Register();Offline70BoosterOpenFG46.Register();Offline70CashPurchaseFG45.Register();Offline70NpcPurchaseFG44.Register();
  foreach(string name in new string[]{"item-metadata.xml","world-item-metadata-FG182.xml","tower-item-metadata-FG184.xml"})Offline70ItemMetadataFG.Register(Path.Combine(output,name));
  Offline70TowerRewardsFG174.Load(Path.Combine(output,"tower-rewards-FG174.dat"));
  foreach(bool failure in new bool[]{false,true}){
   Offline70DeathTowerFloorsFG171 tower=new Offline70DeathTowerFloorsFG171();tower.Begin(50);
   Offline70TowerRewardFG174 offer=null;for(int attempt=0;attempt<100;attempt++){
    tower.RunId=Guid.NewGuid().ToString("N");long old=DateTime.UtcNow.Ticks-30*TimeSpan.TicksPerSecond;
    offer=failure?Offline70TowerRewardsFG174.PlanFailure(tower.RunId,50,12,old):Offline70TowerRewardsFG174.Plan(tower.RunId,50,old);
    if(offer.Items.Exists(delegate(Offline70TowerRewardItemFG174 item){return item.ItemId>0&&Offline70Economy.Definition(item.ItemId).Kind==4;}))break;offer=null;
   }
   Check(offer!=null,"Bounded test offer must contain equipment");
   Offline70EconomyState state=new Offline70EconomyState{Level=50,Experience=Offline70ProgressionFG.Threshold(50),Gold=100000};
   for(ushort slot=9;slot<=40;slot++)state.Inventory.Add(new Offline70EconomyEntry{List=0,Slot=slot,ItemId=10050,Quantity=1,EquipmentSeed=slot,Durability=28});
   for(ushort slot=3;slot<=8;slot++)state.Inventory.Add(new Offline70EconomyEntry{List=0,Slot=slot,ItemId=3015,Quantity=1});
   Offline70TowerRewardsFG174.Record(state,offer);string path=Path.Combine(directory,failure?"failure.xml":"success.xml");
   using(FileStream file=File.Create(path))new XmlSerializer(typeof(Offline70EconomyState)).Serialize(file,state);
   byte[] before=File.ReadAllBytes(path);Offline70BatchServiceFG service=(Offline70BatchServiceFG)FormatterServices.GetUninitializedObject(typeof(Offline70BatchServiceFG));
   Field("role").SetValue(service,new Offline70Role{Id=1,Name="yoyo",Job=0,Level=50});Field("economyPath").SetValue(service,path);Field("assets").SetValue(service,Offline70Economy.Load(path));Field("towerFG172").SetValue(service,tower);Field("towerFinishingFG174").SetValue(service,true);Field("towerRewardShownFG174").SetValue(service,true);Field("towerDisplayDueFG174").SetValue(service,1L);
   string log=Path.Combine(directory,failure?"failure.txt":"success.txt");
   using(Offline70EvidenceWriterFG171 writer=new Offline70EvidenceWriterFG171(log)){
    Field("evidenceFG171").SetValue(service,writer);MethodInfo maintain=typeof(Offline70BatchServiceFG).GetMethod("MaintainTowerRewardFG174",BindingFlags.Instance|BindingFlags.NonPublic);
    long now=DateTime.UtcNow.Ticks;maintain.Invoke(service,null);
    Check((long)Field("towerDisplayDueFG174").GetValue(service)==1L,"Pending reward polling was disabled after capacity failure");
    long retry=(long)Field("towerRewardRetryFG185").GetValue(service);Check(retry>=now+4*TimeSpan.TicksPerSecond&&retry<=DateTime.UtcNow.Ticks+5*TimeSpan.TicksPerSecond,"Capacity retry must be bounded and deferred");
    for(int i=0;i<200;i++)maintain.Invoke(service,null);
    Check(Convert.ToBase64String(before)==Convert.ToBase64String(File.ReadAllBytes(path)),"Polling changed fixed reward or wallet/inventory");
    Check(!(bool)Field("towerRewardCreditedFG174").GetValue(service),"Full bag falsely credited");
   }
   int pending=0;foreach(string line in File.ReadAllLines(log))if(line.IndexOf("TOWER_FINAL_PENDING_CAPACITY",StringComparison.Ordinal)>=0)pending++;
   Check(pending==1,"Capacity polling caused an unbounded diagnostic loop");
   Check(!Offline70Economy.Load(path).TowerRewardsFG174[0].Delivered,"Pending plan was discarded");
  }
  Console.WriteLine("{\"allPassed\":true,\"checks\":"+checks+",\"cases\":2,\"maintenanceCalls\":402,\"releaseAllowed\":false}");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
