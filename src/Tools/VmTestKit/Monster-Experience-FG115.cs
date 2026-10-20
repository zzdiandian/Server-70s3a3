using System;
using System.IO;
// Supplied TW70 monster_reward_ref seed matches all 100 supplied86 values.
// Current CN70 PVF supplies rates and the existing room actor levels/kinds.
// This is the bounded one-player ordinary-dungeon route; no event/premium bonus.
class Offline70MonsterExperienceFG115 {
 internal class Reward {internal uint Reference,Pool,Base,Award;internal float Penalty;internal ushort Monster,MonsterLevel;internal byte PlayerLevel,Difficulty,Kind;internal bool Named;}
 internal static readonly uint[] Reference=new uint[]{0,30,40,50,60,70,80,90,100,110,120,130,140,150,160,170,185,201,218,235,253,271,290,310,330,351,372,394,417,440,464,488,513,539,565,592,619,647,676,705,735,765,796,828,860,893,926,960,995,1030,1066,1102,1139,1177,1215,1254,1293,1333,1374,1415,1457,1499,1542,1586,1630,1675,1720,1766,1813,1860,1908,1956,2005,2055,2105,2156,2207,2259,2312,2365,2419,2473,2528,2584,2640,2697,2754,2812,2871,2930,2990,3050,3111,3173,3235,3298,3361,3425,3490,3555,3575};
 internal static readonly float[] Difficulty=new float[]{0.93f,1.3f,1.67f,2.05f};
 internal static readonly float[] Kind=new float[]{1f,2f,2f,4f};
 internal static uint BaseReward(int level){if(level<1||level>=Reference.Length)throw new InvalidDataException("Monster XP reference level");return Reference[level];}
 internal static float DungeonWeight(ushort id){switch(id){case 1:return 1.2f;case 2:return 1.2f;case 3:return 1f;case 4:return 1.4f;case 5:return 0.8f;case 6:return 1f;case 7:return 1f;case 8:return 1f;case 9:return 1f;case 11:return 0.6f;case 12:return 0.8f;case 13:return 1.4f;case 14:return 0.9f;case 15:return 1f;case 17:return 0.9f;case 1000:return 1f;case 21:return 1.20000005f;case 22:return 0.899999976f;case 23:return 0.899999976f;case 24:return 0.800000012f;case 25:return 1.10000002f;case 26:return 1.20000005f;case 27:return 1f; case 31:return 1f; case 32:return 1f; case 33:return 1f; case 34:return 1.20000005f; case 35:return 1.5f; case 36:return 1f; case 37:return 1.20000005f; case 1502:return 1f; case 40:return 1.39999998f; case 41:return 1.60000002f; case 42:return 1.10000002f; case 43:return 1.29999995f; case 44:return 1.70000005f; case 45:return 1.29999995f; case 141:return 1.39999998f; case 1500:return 1f; case 1501:return 1f; case 50:return 0.800000012f; case 51:return 1f; case 52:return 1.29999995f; case 53:return 1.20000005f; case 1506:return 1f; case 80:return 1f; case 81:return 1f; case 82:return 1.20000005f; case 83:return 1.20000005f; case 84:return 1.20000005f; case 85:return 1f; case 88:return 0.800000012f; case 89:return 0.699999988f; case 90:return 1.20000005f; case 91:return 1.10000002f; case 60:return 1f; case 61:return 1.10000002f; case 100:return 1f; case 110:return 1.10000002f; case 111:return 1f; case 112:return 1f; case 86:return 1.29999995f; case 87:return 1.5f; case 92:return 1.5f; case 93:return 1.5f; case 140:return 1f; case 1504:return 1f; case 3503:return 1f; case 500:return 1f; case 503:return 1f; case 504:return 1f; case 506:return 1f; case 514:return 1f; default:throw new InvalidDataException("Unproven dungeon XP weight");}}
 internal static float LevelPenalty(int player,int monster){
  // The monster bound is the reference table's own domain, not the 70 player cap: PVF gives
  // dungeon 1506 level 71/74 actors and basis 71, so a 70 clamp would refuse every kill there.
  if(player<1||player>70||monster<1||monster>=Reference.Length)throw new InvalidDataException("Monster XP level bounds");
  int difference=monster-player;
  if(difference<-6)return .05f;if(difference==-6)return .2f;if(difference==-5)return .5f;if(difference==-4)return .75f;
  if(difference<=0)return 1f;if(difference<=3)return 1.12f;if(difference<=5)return 1f;
  if(difference==6)return .75f;if(difference==7)return .7f;if(difference==8)return .6f;if(difference==9)return .5f;return .05f;
 }
 internal static Reward Calculate(ushort dungeon,byte difficulty,ushort[] monster,byte playerLevel){
  if(monster==null||monster.Length!=3||monster[0]==0||monster[1]<1||monster[1]>=Reference.Length||monster[2]>127||difficulty>3)throw new InvalidDataException("Monster XP actor fields");
  Reward r=new Reward{Monster=monster[0],MonsterLevel=monster[1],PlayerLevel=playerLevel,Difficulty=difficulty,Kind=(byte)monster[2]};
  r.Named=r.Monster>=50000&&r.Monster<=51000||r.Monster>=56000&&r.Monster<=60000;
  r.Reference=BaseReward(r.MonsterLevel);float role=r.Kind<=3?Kind[r.Kind]:1f;if(r.Named)role*=3f;
  // getMonsterTotalExp truncates once after the entire pool; role does not
  // multiply an already truncated ordinary-monster amount.
  r.Pool=checked((uint)((double)r.Reference/2.0*1f*Difficulty[difficulty]*DungeonWeight(dungeon)*role));
  r.Penalty=LevelPenalty(playerLevel,r.MonsterLevel);
  r.Base=checked((uint)((double)r.Pool*r.Penalty));
  // Apply the user-requested test multiplier only to the computed base reward.
  r.Award=Offline70TestRatesFG114.Experience(r.Base);return r;
 }
}
