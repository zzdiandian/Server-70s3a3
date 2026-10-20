using System;using System.IO;

// CN7804 identity and reset-to-zero/no-amplification text are current data.
// Automatic use / one charge only on actual protection were confirmed by the
// user. This is a bounded research policy, not a recovered CN service method.
class Offline70ReinforcementProtectionFG180 {
    internal const byte Notice=216;
    internal static Offline70EconomyEntry Find(Offline70EconomyState s) {
        Offline70EconomyEntry first=null;
        foreach(Offline70EconomyEntry e in s.Inventory)if(e.List==0&&e.ItemId==7804&&(first==null||e.Slot<first.Slot))first=e;
        return first;
    }
    internal static void Consume(Offline70EconomyState s,CouponResultFG141 result) {
        Offline70EconomyEntry e=Find(s);if(e==null||e.Quantity==0)throw new InvalidDataException("Protection disappeared inside transaction");
        result.NoticeFG180=Notice;result.ProtectionSlotFG180=e.Slot;result.ProtectionRemainingFG180=--e.Quantity;
        if(e.Quantity==0)s.Inventory.Remove(e);
    }
    // Receipt envelope: original failed83 prefix00 D8, version180, then the
    // routing and committed counts needed for deterministic replay. Only00 D8
    // is sent as83; this private26B envelope is never sent to the client.
    internal static byte[] Encode(CouponRequestFG141 r,CouponResultFG141 o) {
        using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)) {
            w.Write((byte)0);w.Write(Notice);w.Write((byte)180);w.Write(r.Type);w.Write(r.Target);w.Write(r.Item);
            w.Write(r.Ticket);w.Write(o.Remaining);w.Write(r.Additional);w.Write(o.Old);w.Write(o.New);
            w.Write(o.ProtectionSlotFG180);w.Write(o.ProtectionRemainingFG180);return m.ToArray();
        }
    }
    static bool MaterialSlot(ushort s){return s>=3&&s<9||s>=105&&s<137;}
    internal static void Validate(Offline70EconomyReceipt receipt,byte[] b) {
        if(receipt.Kind!="npc-reinforce"||b.Length!=26||b[0]!=0||b[1]!=Notice||b[2]!=180||b[3]!=0||b[18]<8||b[18]>30||b[19]!=0||BitConverter.ToUInt16(b,16)!=65535||BitConverter.ToUInt32(b,12)>999||BitConverter.ToUInt32(b,22)>999)throw new InvalidDataException("Protected reinforcement receipt profile");
        ushort target=BitConverter.ToUInt16(b,4),material=BitConverter.ToUInt16(b,10),protection=BitConverter.ToUInt16(b,20);
        if(target<3||target>40||!MaterialSlot(material)||!MaterialSlot(protection)||material==target||protection==target||material==protection||Offline70Economy.Definition(BitConverter.ToUInt32(b,6)).Kind!=4)throw new InvalidDataException("Protected reinforcement receipt routing");
    }
    internal static CouponResultFG141 Restore(byte[] b,CouponRequestFG141 r) {
        if(b[3]!=r.Type||BitConverter.ToUInt16(b,4)!=r.Target||BitConverter.ToUInt32(b,6)!=r.Item||BitConverter.ToUInt16(b,10)!=r.Ticket||BitConverter.ToUInt16(b,16)!=r.Additional)throw new InvalidDataException("Protected reinforcement saved request");
        return new CouponResultFG141{MechanismFG176=0,NoticeFG180=Notice,Old=b[18],New=0,Result=2,Remaining=BitConverter.ToUInt32(b,12),ProtectionSlotFG180=BitConverter.ToUInt16(b,20),ProtectionRemainingFG180=BitConverter.ToUInt32(b,22)};
    }
}
