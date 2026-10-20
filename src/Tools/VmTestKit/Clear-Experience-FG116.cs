using System;using System.IO;
// Ordinary solo clear_reward arithmetic: current CN70 PVF, supplied TW70 ELF.
// Each native float store is retained before the next multiply/truncation.
class Offline70ClearExperienceFG116 {
 internal class Reward {internal ushort Dungeon,Basis;internal byte Difficulty,Player,Rank;internal int Kills,RankIndex;internal uint Reference,Pool,Base,RankBonus,BaseAward,RankAward,Award;internal float BeforeDifficulty,AfterDifficulty,AfterPenalty;}
 internal static readonly ushort[] RankGrades=new ushort[]{99,97,95,81,76,71,66,61};
 internal static readonly float[] RankRates=new float[]{0.05f,0.1f,0.15f,0.2f,0.3f};
 internal static ushort Basis(ushort dungeon){switch(dungeon){case 1:return 1;case 2:return 2;case 3:return 4;case 4:return 6;case 5:return 8;case 6:return 10;case 7:return 13;case 8:return 15;case 9:return 11;case 11:return 19;case 12:return 21;case 13:return 23;case 14:return 25;case 15:return 28;case 17:return 27;case 1000:return 17;case 21:return 30;case 22:return 31;case 23:return 32;case 24:return 33;case 25:return 35;case 26:return 37;case 27:return 39; case 31:return 40; case 32:return 41; case 33:return 55; case 34:return 43; case 35:return 42; case 36:return 54; case 37:return 44; case 1502:return 64; case 40:return 45; case 41:return 46; case 42:return 46; case 43:return 47; case 44:return 49; case 45:return 57; case 141:return 48; case 1500:return 58; case 1501:return 61; case 50:return 51; case 51:return 53; case 52:return 66; case 53:return 67; case 1506:return 71; case 80:return 58; case 81:return 59; case 82:return 60; case 83:return 63; case 84:return 64; case 85:return 65; case 88:return 61; case 89:return 62; case 90:return 38; case 91:return 36; case 60:return 55; case 61:return 56; case 100:return 60; case 110:return 49; case 111:return 50; case 112:return 50; case 86:return 68; case 87:return 69; case 92:return 70; case 93:return 71; case 140:return 52; case 1504:return 68; case 3503:return 5; case 500:return 23; case 503:return 23; case 504:return 23; case 506:return 23; case 514:return 23; default:throw new InvalidDataException("Unproven clear dungeon basis");}}
 internal static int RankIndex(byte rank){for(int i=0;i<5;i++)if(rank>=RankGrades[i])return 4-i;return -1;}
 // Marshal float bits to force the original f32 store even with CLR x87 locals.
 static float Store(double value){return BitConverter.ToSingle(BitConverter.GetBytes((float)value),0);}
 internal static Reward Calculate(ushort dungeon,byte difficulty,byte player,int killed,byte rank){
  if(difficulty>3||player<1||player>70||killed<1||killed>4096)throw new InvalidDataException("Clear XP verified inputs");
  Reward r=new Reward{Dungeon=dungeon,Basis=Basis(dungeon),Difficulty=difficulty,Player=player,Kills=killed,Rank=rank,RankIndex=RankIndex(rank)};
  r.Reference=Offline70MonsterExperienceFG115.BaseReward(r.Basis);
  r.BeforeDifficulty=Store((double)Store(r.Reference)*killed/2.0*1f);
  r.AfterDifficulty=Store((double)r.BeforeDifficulty*Offline70MonsterExperienceFG115.Difficulty[difficulty]);
  r.Pool=checked((uint)((double)r.AfterDifficulty*Offline70MonsterExperienceFG115.DungeonWeight(dungeon)));
  r.AfterPenalty=Store((double)Store(r.Pool)*Offline70MonsterExperienceFG115.LevelPenalty(player,r.Basis));
  r.Base=checked((uint)r.AfterPenalty); // MemberPenaltyRate1 and member count1.
  r.RankBonus=r.RankIndex<0?0:checked((uint)((double)r.Base*RankRates[r.RankIndex]));
  r.BaseAward=Offline70TestRatesFG114.Experience(r.Base);r.RankAward=Offline70TestRatesFG114.Experience(r.RankBonus);r.Award=checked(r.BaseAward+r.RankAward);return r;
 }
 // Current CN35 is clear-base, rank-bonus, party-bonus, avatar-bonus ... .
 // It contains earned XP, not the account's cumulative XP or prior balance.
 internal static byte[] Body(uint actualBase,uint actualRank){byte[] b=new byte[91];BitConverter.GetBytes(actualBase).CopyTo(b,0);BitConverter.GetBytes(actualRank).CopyTo(b,4);b[62]=1;BitConverter.GetBytes(Offline70DungeonFlowFG.DiagnosticClearGold).CopyTo(b,67);BitConverter.GetBytes(Offline70PaidCardFG.Cost).CopyTo(b,74);return b;}
}
