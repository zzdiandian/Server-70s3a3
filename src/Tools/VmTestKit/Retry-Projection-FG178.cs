using System;
using System.IO;
using System.Collections.Generic;

// Research only. Command14 uses absolute inventory records. Command83's
// destructive result uses ADD quantities and must not be replayed.
class Offline70ReinforcementRetryFG178 {
    internal sealed class Context {
        readonly ushort[] slots;readonly ushort target,material;
        readonly uint item,remaining;readonly byte result,grade;
        readonly Dictionary<ushort,uint> beforeIds=new Dictionary<ushort,uint>(),afterIds=new Dictionary<ushort,uint>();
        internal Context(Offline70EconomyState before,Offline70EconomyState after,CouponRequestFG141 request,CouponResultFG141 outcome) {
            if(request==null||outcome==null||outcome.MechanismFG176!=0)throw new InvalidDataException("Ordinary retry context");
            slots=Slots(before,after);target=request.Target;material=request.Ticket;item=request.Item;remaining=outcome.Remaining;result=outcome.Result;grade=outcome.New;
            foreach(Offline70EconomyEntry e in before.Inventory)if(e.List==0)beforeIds.Add(e.Slot,e.ItemId);
            foreach(Offline70EconomyEntry e in after.Inventory)if(e.List==0)afterIds.Add(e.Slot,e.ItemId);
        }
        internal bool CanReplay83(Offline70EconomyState current) {
            if(result==3)return false;
            Offline70EconomyEntry t=Offline70Economy.Find(current,0,target),m=Offline70Economy.Find(current,0,material);
            // Replaying an old83 after another operation can touch a reused
            // material slot or a target which has since disappeared. Only
            // replay when its absolute target/material fields remain current.
            return t!=null&&t.ItemId==item&&t.Upgrade==grade&&(remaining==0?m==null:m!=null&&m.ItemId==3171&&m.Quantity==remaining);
        }
        internal byte[] Body(Offline70EconomyState current){return Offline70ReinforcementRetryFG178.Body(current,slots,beforeIds,afterIds);}
        internal byte[] Reply(Offline70EconomyState current,byte[] keys){return Offline70EconomyProtocol.UpdatePacket(Body(current),keys);}
    }
    internal static ushort[] Slots(Offline70EconomyState before,Offline70EconomyState after) {
        Offline70Economy.Validate(before);Offline70Economy.Validate(after);
        if(before.AccountId!=after.AccountId||before.RoleId!=after.RoleId)throw new InvalidDataException("Reinforcement retry identity");
        SortedDictionary<ushort,bool> slots=new SortedDictionary<ushort,bool>();
        foreach(Offline70EconomyEntry e in before.Inventory)if(e.List==0)slots[e.Slot]=true;
        foreach(Offline70EconomyEntry e in after.Inventory)if(e.List==0)slots[e.Slot]=true;
        ushort[] result=new ushort[slots.Count];slots.Keys.CopyTo(result,0);return result;
    }
    internal static byte[] Body(Offline70EconomyState current,ushort[] knownSlots) {
        return Body(current,knownSlots,null,null);
    }
    static byte[] Body(Offline70EconomyState current,ushort[] knownSlots,Dictionary<ushort,uint> beforeIds,Dictionary<ushort,uint> afterIds) {
        Offline70Economy.Validate(current);
        if(knownSlots==null||knownSlots.Length>134)throw new InvalidDataException("Reinforcement retry slot count");
        List<ushort> removed=new List<ushort>();ushort previous=0;
        foreach(ushort slot in knownSlots) {
            if(!(slot>=3&&slot<41||slot>=57&&slot<89||slot>=105&&slot<137||slot>=153&&slot<185)||slot<=previous)throw new InvalidDataException("Reinforcement retry slot order");
            previous=slot;Offline70EconomyEntry currentEntry=Offline70Economy.Find(current,0,slot);uint before,after;
            if(currentEntry==null||(beforeIds!=null&&beforeIds.TryGetValue(slot,out before)&&before!=currentEntry.ItemId)||(afterIds!=null&&afterIds.TryGetValue(slot,out after)&&after!=currentEntry.ItemId))removed.Add(slot);
        }
        byte[] live=Offline70EconomyProtocol.InventoryBody(current,false,0);
        using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)) {
            w.Write((byte)0);w.Write(checked((ushort)(BitConverter.ToUInt16(live,1)+removed.Count)));
            // Empty slots and changed item IDs need an explicit removal first.
            // Native14's in-place gear setter does not change the item ID.
            foreach(ushort slot in removed){w.Write(slot);w.Write(UInt32.MaxValue);w.Write(new byte[25]);}
            w.Write(live,3,live.Length-3);return m.ToArray();
        }
    }
    internal static byte[] Reply(Offline70EconomyState current,ushort[] knownSlots,byte[] keys) {
        return Offline70EconomyProtocol.UpdatePacket(Body(current,knownSlots),keys);
    }
}
