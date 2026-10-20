using System;using System.IO;using System.Collections.Generic;using System.Diagnostics;

// CN70 notify32 is actor ushort + life state byte. The ordinary cmd43 request
// contains death coordinates; cmd44 contains the target actor, without a fold.
// Only the sixteen currently implemented ordinary solo dungeons are admitted.
partial class Offline70BatchServiceFG {
 const int DeathReturnSecondsFG117=15;
 bool playerDeadFG117;
 uint deathNumberFG117,usedRevivalCoinsFG117;
 readonly Stopwatch deathWaitFG117=new Stopwatch();
 readonly Dictionary<ushort,string> deathRequestsFG117=new Dictionary<ushort,string>();
 sealed class RevivalReplyFG117 {internal string Fingerprint;internal byte[] Packet;}
 readonly Dictionary<ushort,RevivalReplyFG117> revivalRequestsFG117=new Dictionary<ushort,RevivalReplyFG117>();

 void ResetDeathFG117(){playerDeadFG117=false;deathNumberFG117=usedRevivalCoinsFG117=0;deathWaitFG117.Reset();}
 static byte[] LifeBodyFG117(ushort actor,byte state){if(state>2)throw new InvalidDataException("Life state");return new byte[]{(byte)actor,(byte)(actor>>8),state};}
 void LifeFG117(byte state,string label){Send(label,Offline70RoleProtocol.BatchPacketFG(0,32,LifeBodyFG117(role.Id,state),keys,false));}
 static byte[] DeathPlainFG117(byte[] cipher,byte[] key,int logical,int index){
  int padded=index==1?16:8;if(cipher==null||cipher.Length!=padded)throw new InvalidDataException("Death/coin cipher size");
  byte[] b=Offline70RoleProtocol.BatchCryptFG(cipher,key,index,false);for(int i=logical;i<b.Length;i++)if(b[i]!=0)throw new InvalidDataException("Death/coin reserved padding");return b;
 }
 bool LifeDungeonFG117(){return selected&&dungeon.Active&&dungeon.Loaded&&!dungeon.ClearPublished;}
 byte[] CoinReplyFG117(bool success,ushort actor,byte error){return Offline70RoleProtocol.BatchPacketFG(1,44,success?new byte[]{1,(byte)actor,(byte)(actor>>8)}:new byte[]{0,error},keys,false);}
 void DieFG117(byte[] cipher,ushort sequence){
  try{
   byte[] b=DeathPlainFG117(cipher,keys,4,1);ushort x=BitConverter.ToUInt16(b,0),y=BitConverter.ToUInt16(b,2);
   if(!LifeDungeonFG117()){Send("death-state-failed",Offline70RoleProtocol.BatchPacketFG(1,43,new byte[]{0,0x13},keys,false));return;}
   string fingerprint=Convert.ToBase64String(cipher),old;
   if(deathRequestsFG117.TryGetValue(sequence,out old)){if(old!=fingerprint)throw new InvalidDataException("Death sequence reused");Note("CHARACTER_DEATH_REPEAT seq="+sequence+" noLifeChange=True noTimerExtension=True");return;}
   if(playerDeadFG117){Send("death-already-dead",Offline70RoleProtocol.BatchPacketFG(1,43,new byte[]{0,0x12},keys,false));return;}
   if(deathRequestsFG117.Count>=4096||deathNumberFG117==UInt32.MaxValue)throw new InvalidDataException("Death receipt capacity");
   deathRequestsFG117.Add(sequence,fingerprint);deathNumberFG117++;playerDeadFG117=true;deathWaitFG117.Reset();deathWaitFG117.Start();
   LifeFG117(0,"character-dead32");
   Note("CHARACTER_DEATH_ACCEPTED run="+dungeon.RunId+" actor="+role.Id+" x="+x+" y="+y+" death="+deathNumberFG117+" seq="+sequence+" timeoutSeconds="+DeathReturnSecondsFG117+" originalLifeState=0");
  }catch(InvalidDataException e){Send("death-invalid",Offline70RoleProtocol.BatchPacketFG(1,43,new byte[]{0,0x13},keys,false));Note("CHARACTER_DEATH_REJECTED "+e.Message);}
 }
 void ReviveFG117(byte[] cipher,ushort sequence){
  ushort actor=role.Id;
  try{
   byte[] b=DeathPlainFG117(cipher,keys,2,2);actor=BitConverter.ToUInt16(b,0);string fingerprint=Convert.ToBase64String(cipher);RevivalReplyFG117 previous;
   if(revivalRequestsFG117.TryGetValue(sequence,out previous)){
    if(previous.Fingerprint!=fingerprint)throw new InvalidDataException("Coin sequence reused");Send("revival-repeat44",previous.Packet);Note("REVIVAL_REPEAT seq="+sequence+" noCharge=True noLifeChange=True");return;
   }
   if(revivalRequestsFG117.Count>=4096)throw new InvalidDataException("Coin receipt capacity");
   byte error=0;
   if(!LifeDungeonFG117())error=0x13;
   else if(actor!=role.Id)error=0x15;
   else if(!playerDeadFG117)error=0x12;
   else if(deathWaitFG117.ElapsedMilliseconds>=DeathReturnSecondsFG117*1000)error=0x13;
   else{Sync();if(assets.RevivalCoins==0)error=0x11;else{uint revivalTicketItemFG185;uint revivalTicketCountFG185;int revivalCoinLimitFG185;if(Offline70EntryCostFG185.TryCost(dungeon.DungeonId,out revivalTicketItemFG185,out revivalTicketCountFG185,out revivalCoinLimitFG185)&&revivalCoinLimitFG185>0&&usedRevivalCoinsFG117>=revivalCoinLimitFG185)error=0x16;}}
   if(error!=0){byte[] failure=CoinReplyFG117(false,actor,error);revivalRequestsFG117.Add(sequence,new RevivalReplyFG117{Fingerprint=fingerprint,Packet=failure});Send("revival-failed44",failure);Note("REVIVAL_REJECTED seq="+sequence+" error="+error+" coinBalance="+assets.RevivalCoins+" noCharge=True");return;}
   Offline70EconomyState before=assets;bool duplicate;
   assets=Offline70Economy.Apply(economyPath,dungeon.RunId+"-revival-"+deathNumberFG117,"revival-coin",role.Id+":"+deathNumberFG117,delegate(Offline70EconomyState s){if(s.RevivalCoins==0)throw new InvalidDataException("Revival coin balance changed");s.RevivalCoins--;},out duplicate);
   if(duplicate)throw new InvalidDataException("Death coin receipt already committed");
   playerDeadFG117=false;deathWaitFG117.Reset();usedRevivalCoinsFG117++;Sync();
   byte[] reply=CoinReplyFG117(true,actor,0);revivalRequestsFG117.Add(sequence,new RevivalReplyFG117{Fingerprint=fingerprint,Packet=reply});
   LifeFG117(1,"character-revived32");Send("revival-used44",reply);Send("revival-coin-balance",Offline70EconomyProtocol.InventoryChangeReply(before,assets,keys));
   Note("REVIVAL_COMMITTED run="+dungeon.RunId+" death="+deathNumberFG117+" usedCoins="+usedRevivalCoinsFG117+" remaining="+assets.RevivalCoins+" actor="+actor+" seq="+sequence+" originalLifeState=1 fullHPMP=True");
  }catch(InvalidDataException e){Send("revival-invalid44",CoinReplyFG117(false,actor,0x13));Note("REVIVAL_INVALID "+e.Message);}
 }
 void DeathTimeoutFG117(){
  if(!playerDeadFG117||!selected||!dungeon.Active||!deathWaitFG117.IsRunning||deathWaitFG117.ElapsedMilliseconds<DeathReturnSecondsFG117*1000)return;
  string run=dungeon.RunId;uint coins=assets.RevivalCoins;dungeon.Abort();
  Send("death-timeout-exit45",Offline70DungeonFlowFG.Reply(1,45,new byte[]{1},keys));LifeFG117(1,"death-timeout-reset32");ReturnToEntry();
  Note("DEATH_TIMEOUT_RETURN run="+run+" timeoutSeconds="+DeathReturnSecondsFG117+" remainingCoins="+coins+" noClearAward=True noCharge=True");
 }
}
