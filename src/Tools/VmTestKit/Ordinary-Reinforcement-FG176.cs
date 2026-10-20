using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Security.Cryptography;

public class OrdinaryReinforcementTargetFG176 {
    public uint Id,GoldCost;
    public byte Grade,MinimumLevel,Rarity;
    public ushort EquipType,Durability;
}
public class OrdinaryReinforcementStepFG176 {
    public byte Grade,Penalty,Drop;
    public uint FailureThreshold,Material,Quantity;
}
public class OrdinaryReinforcementCatalogFG176 {
    public string PvfSha256,UpgradeDocumentSha256;
    public List<OrdinaryReinforcementTargetFG176> Targets=new List<OrdinaryReinforcementTargetFG176>();
    public List<OrdinaryReinforcementStepFG176> Steps=new List<OrdinaryReinforcementStepFG176>();
}

// Isolated research only. The build script explicitly supplies the shared-file
// patches; none of this file is included by the stable build.
class Offline70OrdinaryReinforcementFG176 {
    static readonly Dictionary<uint,OrdinaryReinforcementTargetFG176> Targets=new Dictionary<uint,OrdinaryReinforcementTargetFG176>();
    static readonly Dictionary<byte,OrdinaryReinforcementStepFG176> Steps=new Dictionary<byte,OrdinaryReinforcementStepFG176>();
    static readonly RandomNumberGenerator Generator=RandomNumberGenerator.Create();
    internal static void Load(string path) {
        if(new FileInfo(path).Length>1048576)throw new InvalidDataException("Ordinary reinforcement catalog bounds");
        OrdinaryReinforcementCatalogFG176 c;
        using(FileStream f=File.OpenRead(path))using(XmlTextReader r=new XmlTextReader(f)) {
            r.ProhibitDtd=true;r.XmlResolver=null;
            c=(OrdinaryReinforcementCatalogFG176)new XmlSerializer(typeof(OrdinaryReinforcementCatalogFG176)).Deserialize(r);
        }
        if(c.PvfSha256!="61ccc511c0b3804741387344dc6862c3d6731aad3d4334142a35e095141aae0d"||c.UpgradeDocumentSha256!="7102e11ce78934ef246ec24d9ab6af731547a8d1a28113990fdbe62b56a04f71"||c.Targets.Count==0||c.Steps.Count!=31)throw new InvalidDataException("Ordinary reinforcement profile");
        Dictionary<uint,OrdinaryReinforcementTargetFG176> targets=new Dictionary<uint,OrdinaryReinforcementTargetFG176>();
        Dictionary<byte,OrdinaryReinforcementStepFG176> steps=new Dictionary<byte,OrdinaryReinforcementStepFG176>();
        foreach(OrdinaryReinforcementTargetFG176 t in c.Targets) {
            Offline70EconomyItemDefinition d=Offline70Economy.Definition(t.Id);
            if(d.Kind!=4||d.MinimumLevel!=t.MinimumLevel||d.EquipSlot!=t.EquipType||d.MaxDurability!=t.Durability||t.Rarity>5||t.Grade>=100||t.GoldCost>Offline70Economy.WalletLimit||t.EquipType<10||t.EquipType>21||t.EquipType==11)throw new InvalidDataException("Ordinary reinforcement target profile");
            targets.Add(t.Id,t);
        }
        foreach(OrdinaryReinforcementStepFG176 s in c.Steps) {
            if(s.Grade>30||s.Material!=3171||s.Quantity<1||s.Quantity>1000||s.FailureThreshold>100000||s.Penalty<1||s.Penalty>3||s.Penalty==1&&s.Drop!=0||s.Penalty==2&&s.Drop==0)throw new InvalidDataException("Ordinary reinforcement step profile");
            steps.Add(s.Grade,s);
        }
        Targets.Clear();Steps.Clear();foreach(KeyValuePair<uint,OrdinaryReinforcementTargetFG176> t in targets)Targets.Add(t.Key,t.Value);foreach(KeyValuePair<byte,OrdinaryReinforcementStepFG176> s in steps)Steps.Add(s.Key,s.Value);
    }
    // The 1031 reference MT bounded draw is inclusive [0,100000]. Preserve
    // that denominator instead of silently using a percent/100 shortcut.
    // RNG engine is a host RNG; bit-identical MT sequences are not claimed.
    internal static int Roll() {
        const uint span=100001;
        uint limit=UInt32.MaxValue-(UInt32.MaxValue%span);
        lock(Generator) {
            byte[] b=new byte[4];uint n;
            do{Generator.GetBytes(b);n=BitConverter.ToUInt32(b,0);}while(n>=limit);
            return (int)(n%span);
        }
    }
    static byte[] Outcome(CouponRequestFG141 r,CouponResultFG141 o) {
        if(o.NoticeFG180==Offline70ReinforcementProtectionFG180.Notice)return Offline70ReinforcementProtectionFG180.Encode(r,o);
        using(MemoryStream m=new MemoryStream())using(BinaryWriter w=new BinaryWriter(m)) {
            w.Write((byte)1);w.Write(r.Type);w.Write(r.Ticket);w.Write(o.Remaining);w.Write(r.Additional);
            w.Write((byte)0);w.Write(o.Old);w.Write(o.Result);w.Write(o.New);w.Write(r.Target);if(o.TailFG177!=null)w.Write(o.TailFG177);return m.ToArray();
        }
    }
    internal static void ValidateReceipt(Offline70EconomyReceipt receipt) {
        if(receipt.ReinforcementOutcomeFG176==null)return;
        byte[] b;
        try{b=Convert.FromBase64String(receipt.ReinforcementOutcomeFG176);}catch(FormatException){throw new InvalidDataException("Ordinary reinforcement receipt encoding");}
        if(b.Length>0&&b[0]==0){Offline70ReinforcementProtectionFG180.Validate(receipt,b);return;}
        if(receipt.Kind!="npc-reinforce"||b.Length<16||b.Length>397||b[0]!=1||b[1]!=0||b[10]!=0||b[11]>30||b[13]>31||b[12]>3||BitConverter.ToUInt16(b,8)!=65535||BitConverter.ToUInt16(b,14)<3||BitConverter.ToUInt16(b,14)>40)throw new InvalidDataException("Ordinary reinforcement receipt outcome");
        if(b[12]==3){if(b.Length<17||b[13]!=0||b[16]<1||b[16]>38||b.Length!=17+10*b[16])throw new InvalidDataException("Break-return receipt tail");}else if(b.Length!=16)throw new InvalidDataException("Unexpected reinforcement receipt tail");
    }
    static CouponResultFG141 Restore(Offline70EconomyState s,string receipt,CouponRequestFG141 r) {
        foreach(Offline70EconomyReceipt saved in s.Receipts)if(saved.Id==receipt) {
            ValidateReceipt(saved);
            if(saved.ReinforcementOutcomeFG176==null)throw new InvalidDataException("Ordinary reinforcement missing saved outcome");
            byte[] b=Convert.FromBase64String(saved.ReinforcementOutcomeFG176);
            if(b[0]==0)return Offline70ReinforcementProtectionFG180.Restore(b,r);
            if(b[1]!=r.Type||BitConverter.ToUInt16(b,2)!=r.Ticket||BitConverter.ToUInt16(b,8)!=r.Additional||BitConverter.ToUInt16(b,14)!=r.Target)throw new InvalidDataException("Ordinary reinforcement saved routing");
            byte[] tail=null;if(b.Length>16){tail=new byte[b.Length-16];Buffer.BlockCopy(b,16,tail,0,tail.Length);}
            return new CouponResultFG141{MechanismFG176=0,Old=b[11],New=b[13],Result=b[12],Remaining=BitConverter.ToUInt32(b,4),TailFG177=tail};
        }
        throw new InvalidDataException("Ordinary reinforcement committed receipt missing");
    }
    internal static Offline70EconomyState Use(string path,string receipt,CouponRequestFG141 r,int roll,out CouponResultFG141 result,out bool duplicate) {
        return Use(path,receipt,r,roll,Offline70DisjointFG92.Draw(10000),Offline70DisjointFG92.Draw(10000),out result,out duplicate);
    }
    internal static Offline70EconomyState Use(string path,string receipt,CouponRequestFG141 r,int roll,int choice,int chance,out CouponResultFG141 result,out bool duplicate) {
        if(r==null||r.Type!=0||r.Additional!=65535||r.Target<3||r.Target>40||r.Target==r.Ticket||roll<0||roll>100000||choice<0||choice>=10000||chance<0||chance>=10000)throw new CouponEligibilityErrorFG141(0x13,"Ordinary reinforcement request routing");
        string payload=r.Type+":"+r.Target+":"+r.Item+":"+r.Ticket+":"+r.Additional;
        CouponResultFG141 outcome=new CouponResultFG141{MechanismFG176=0};
        // Receipt outcome is saved in the SAME transaction as gold, material
        // and grade. Retry returns that outcome even after subsequent upgrades.
        Offline70EconomyState after=Offline70Economy.ApplyWithReceiptFG176(path,receipt,"npc-reinforce",payload,delegate(Offline70EconomyState s,Offline70EconomyReceipt saved) {
            Offline70EconomyEntry target=Offline70Economy.Find(s,0,r.Target),material=Offline70Economy.Find(s,0,r.Ticket);
            OrdinaryReinforcementTargetFG176 t;OrdinaryReinforcementStepFG176 step;
            if(target==null||target.ItemId!=r.Item)throw new CouponEligibilityErrorFG141(4,"Ordinary reinforcement target ownership");
            if(!Targets.TryGetValue(target.ItemId,out t)||target.Ability!=0||target.ExpiresUnix!=0||target.Sealed)throw new CouponEligibilityErrorFG141(0x13,"Ordinary reinforcement target unsupported");
            if(!Steps.TryGetValue(target.Upgrade,out step))throw new CouponEligibilityErrorFG141(0x13,"Reinforcement beyond serialized grade range");
            bool destroys=step.Penalty==3||t.Rarity<2&&target.Upgrade>=8;
            if(target.Durability!=t.Durability)throw new CouponEligibilityErrorFG141(7,"Ordinary reinforcement requires full durability");
            if(material==null||material.ItemId!=step.Material||material.Quantity<step.Quantity)throw new CouponEligibilityErrorFG141(0x16,"Ordinary reinforcement material shortage");
            if(s.Gold<t.GoldCost)throw new CouponEligibilityErrorFG141(10,"Ordinary reinforcement gold shortage");
            if(destroys)Offline70BreakReturnsFG177.Preflight(s,t);
            outcome.Old=target.Upgrade;
            bool fail=roll<step.FailureThreshold;
            outcome.Result=fail?(destroys?(byte)3:step.Penalty):(byte)0;
            outcome.New=fail?(byte)Math.Max(0,target.Upgrade-step.Drop):(byte)(target.Upgrade+1);
            s.Gold-=t.GoldCost;material.Quantity-=step.Quantity;
            outcome.Remaining=material.Quantity;if(material.Quantity==0)s.Inventory.Remove(material);
            if(fail&&destroys&&Offline70ReinforcementProtectionFG180.Find(s)!=null){outcome.New=0;outcome.Result=2;target.Upgrade=0;Offline70ReinforcementProtectionFG180.Consume(s,outcome);}
            else if(fail&&destroys){outcome.New=0;outcome.TailFG177=Offline70BreakReturnsFG177.Destroy(s,target,t,choice,chance);}else target.Upgrade=outcome.New;
            saved.ReinforcementOutcomeFG176=Convert.ToBase64String(Outcome(r,outcome));
        },out duplicate);
        result=Restore(after,receipt,r);return after;
    }
}
