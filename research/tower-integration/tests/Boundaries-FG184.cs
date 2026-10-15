using System;using System.IO;using System.Collections.Generic;
class TowerRebaseBoundariesFG184 {
 static int checks;static void Check(bool value,string name){checks++;if(!value)throw new Exception(name);}
 static bool Rejected(Action action){try{action();return false;}catch(InvalidDataException){return true;}}
 static byte[] Death(ushort uid,ushort owner,byte[] keys){byte[] body=new byte[54],padded=new byte[60];BitConverter.GetBytes(uid).CopyTo(body,0);BitConverter.GetBytes(owner).CopyTo(body,2);Offline70RoleProtocol.WithDungeonIntegrity(body).CopyTo(padded,0);return Offline70RoleProtocol.BatchCryptFG(padded,keys,0,true);}
 static int Main(string[] args){try{
  Offline70WishBoxFG84.Register();Offline70BoosterRewardsFG47.Register();Offline70BoosterOpenFG46.Register();Offline70CashPurchaseFG45.Register();Offline70NpcPurchaseFG44.Register();
  foreach(string name in new string[]{"item-metadata.xml","world-item-metadata-FG182.xml","tower-item-metadata-FG184.xml"})Offline70ItemMetadataFG.Register(Path.Combine(args[0],name));
  Offline70TowerRewardsFG174.Load(Path.Combine(args[0],"tower-rewards-FG174.dat"));
  byte[] keys=new byte[334];for(int i=0;i<keys.Length;i++)keys[i]=(byte)i;
  Offline70Role role=new Offline70Role{Id=1,Name="yoyo",Job=0,Level=50};
  Check(Rejected(delegate{Offline70RoleProtocol.BatchDeathRequestFG(Death(50000,1,keys),role,keys,delegate(ushort owner){return owner==1;});}),"Tower UID cannot enter ordinary parser");
  Check(Offline70RoleProtocol.BatchDeathRequestFG(Death(50000,1,keys),role,keys,delegate(ushort owner){return owner==1;},delegate(ushort uid){return uid==50000;})==50000,"Tower current roster permits its UID");
  Check(Rejected(delegate{Offline70RoleProtocol.BatchDeathRequestFG(Death(50001,1,keys),role,keys,delegate(ushort owner){return owner==1;},delegate(ushort uid){return uid==50000;});}),"Unannounced tower UID refused");
  Check(Rejected(delegate{Offline70RoleProtocol.BatchDeathRequestFG(Death(900,65535,keys),role,keys,delegate(ushort owner){return owner==1;});}),"No-attacker GSD remains refused");
  Check(Offline70RoleProtocol.BatchDeathRequestFG(Death(100,65535,keys),role,keys,delegate(ushort owner){return owner==1;})==100,"Accepted ordinary no-attacker format retained");
  Check(Rejected(delegate{Offline70RoleProtocol.BatchDeathRequestFG(Death(100,1,keys),role,keys,delegate(ushort owner){return true;},null);}),"Null roster predicate refused");
  foreach(byte level in new byte[]{50,60,70}){
   Offline70DeathTowerFloorsFG171 tower=new Offline70DeathTowerFloorsFG171();tower.Begin(level);HashSet<ushort> all=new HashSet<ushort>();
   Check(tower.Info().Length==7&&BitConverter.ToUInt32(tower.Info(),0)==11000,"Current info remains7 bytes");
   for(int floor=1;floor<=45;floor++){
    Check(tower.Floor==floor&&tower.Map==30000+floor,"Original floor/map identity");
    byte[] room=tower.Room();Check(room.Length==10+12*room[8],"Current ushort map/12-byte actor records");
    Check(Rejected(delegate{tower.Advance();}),"Cannot skip live floor");
    Check(tower.Start()&&!tower.Start(),"Start duplicate does not advance");
    foreach(Offline70TowerActorFG171 actor in tower.Actors){Check(actor.Uid<60000&&actor.Uid!=1&&actor.Uid!=900&&all.Add(actor.Uid),"No actor UID reuse/GSD collision");Check(tower.Kill(actor.Uid)&&!tower.Kill(actor.Uid),"Death is idempotent");}
    Check(tower.Cleared&&tower.FinishDialog()&&!tower.FinishDialog(),"Only fully cleared roster finishes");
    if(floor<45)tower.Advance();else Check(tower.ReadyToFinish,"Only45 final");
   }
  }
  Offline70QuestCatalogFG catalog=Offline70QuestCatalogFG.Load(Path.Combine(args[0],"quest-catalog.xml"));
  foreach(ushort id in new ushort[]{3500,3501,3502,3588,3589,3590,3591,3592,3606,3612})Check(catalog.Get(id).Supported,"Original tower quest enabled");
  foreach(ushort id in new ushort[]{7313,7314,7325,7326})Check(Rejected(delegate{catalog.Get(id);}),"Training qualification remains unavailable");
  Check(catalog.Get(453).Selection.Length==0,"Diary fix retained");Check(catalog.Get(39).Supported&&catalog.Get(40).Supported&&Rejected(delegate{catalog.Get(41);}),"DarkCity task boundary retained");
  Check(Offline70TownContentFG75.DungeonGate(3,7)&&Offline70TownContentFG75.DungeonGate(4,2),"Tower and DarkCity return corridors coexist");
  Check(Offline70TownContentFG75.Linked(3,1,3,7)&&Offline70TownContentFG75.Linked(3,7,3,1),"Tower original portals both ways");
  Console.WriteLine("{\"allPassed\":true,\"checks\":"+checks+",\"releaseAllowed\":false}");return 0;
 }catch(Exception e){Console.Error.WriteLine(e);return 1;}}
}
