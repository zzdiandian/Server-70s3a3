using System;
// Entry costs of 33 (kingsruins, dungeon/act4/kingsruins.dgn), 1502 (noirpera,
// dungeon/ancient/noirpera.dgn), 1500 (vilmark), 1501 (screamingcave), 1506 (leshphon) and
// 1504 (ghosttrain, dungeon/ancient/ghosttrain.dgn):
// PVF [required item] 4083/4343/3209/4176/4430/4421 x1 per entry plus [coin limit] 3 coin revivals,
// re-derived by local-run/pvf/entry_cost_evidence.py and port-1500-prep.md; the 1506 row by
// local-run/pvf/port_northmyre_gen.py from dungeon/ancient/leshphon.dgn.
// Native CParty::checkInoutConditionDungeon sends the missing-ticket op16 reject as
// {0, 0x11, seat} and UseAncientDungeonItems failure as {0, uint32 1}; the CN70 client
// op16 dispatcher (0x8014B0) reads a uint32 error plus one seat byte on branch 0x11,
// so the reject body is written as {0, 0x11, 0, 0, 0, seat}. Consumption goes through
// the receipt-scoped economy ledger: a replayed or crashed entry request never debits
// twice, and a missing ticket is rejected before any fatigue is spent.
class Offline70EntryCostFG185 {
 internal static bool TryCost(ushort dungeonId,out uint itemId,out uint count,out int coinLimit){itemId=0;count=0;coinLimit=0;if(dungeonId==33){itemId=4083;count=1;coinLimit=3;return true;}if(dungeonId==1502){itemId=4343;count=1;coinLimit=3;return true;}if(dungeonId==1500){itemId=3209;count=1;coinLimit=3;return true;}if(dungeonId==1501){itemId=4176;count=1;coinLimit=3;return true;}if(dungeonId==1506){itemId=4430;count=1;coinLimit=3;return true;}if(dungeonId==1504){itemId=4421;count=1;coinLimit=3;return true;}return false;}
 internal static uint Owned(Offline70EconomyState s,uint itemId){uint total=0;foreach(Offline70EconomyEntry e in s.Inventory)if(e.List!=3&&e.ItemId==itemId)total+=e.Quantity;return total;}
 internal static Offline70EconomyState Consume(string path,string receipt,ushort dungeonId,uint itemId,uint count,out bool duplicate){return Offline70Economy.Apply(path,receipt,"entry-ticket",dungeonId+":"+itemId+":"+count,delegate(Offline70EconomyState s){Offline70Economy.RemoveAssetItem(s,itemId,count);},out duplicate);}
 internal static byte[] TicketFailure(byte seat,byte[] keys){return Offline70RoleProtocol.BatchPacketFG(1,16,new byte[]{0,0x11,0,0,0,seat},keys,false);}
 internal static byte[] ConsumeFailure(byte[] keys){return Offline70RoleProtocol.BatchPacketFG(1,16,new byte[]{0,1,0,0,0,0},keys,false);}
}
