using System;using System.IO;
// Generated from the CN70 client's Script.pvf (town/*.twn, area .map [town movable area] and [NPC]).
// Door rows are the client's own portal rectangles: x y w h targetTown targetArea.
// DungeonPortal is the client's dungeon entrance rectangle (rows with target -1/-1).
class Offline70TownContentFG75 {
 // Town 6 = 根特 (Gent, town/gent.twn): areas 0 Gent_Market / 1 Gent_Workshop /
 // 2 Gent2Ant1 and 3 Gent2Ant2 (both `[dungeon gate]` 12 = worldmap.lst #12 Antberr.wdm)
 // and 4 Gate_Gent, the `[gate] 474 234` Seria room.  Re-derived by
 // local-run/pvf/r2_gen_gent_lufthafen.py (town-evidence section).
 internal static bool TownSupported(int town){return town>=1&&town<=6||town==9||town==12;}
 internal static int SeriaArea(int town){if(!TownSupported(town))throw new InvalidDataException("Unsupported town region");
  // Area the role wakes up in (the shared Seria room map; the NPC trio 2/32/22 marks it in
  // every town). Town 5 (StormPass, 万年雪山): area 1 = StormPass/Gate_Storm.map; area 0 is
  // the town square whose board does not contain the login point (474,234).
  return town==1?1:town==2?5:town==3?2:town==5?1:town==6?4:town==9?2:town==12?3:0;}
 internal static bool Exists(int town,int area){switch(town*16+area){
  case 16:return true;
  case 17:return true;
  case 18:return true;
  case 19:return true;
  case 32:return true;
  case 33:return true;
  case 34:return true;
  case 35:return true;
  case 36:return true;
  case 37:return true;
  case 38:return true;
  case 39:return true;
  case 40:return true;
  case 41:return true;
  case 48:return true;
  case 49:return true;
  case 50:return true;
  case 51:return true;
  case 52:return true;
  case 53:return true;
  case 54:return true;
  case 55:return true;
  case 56:return true;
  case 57:return true;
  case 58:return true;
  case 64:return true;
  case 65:return true;
  case 66:return true;
  case 67:return true;
  case 68:return true;
  case 80:return true;
  case 81:return true;
  case 82:return true;
  case 83:return true;
  case 96:return true;
  case 97:return true;
  case 98:return true;
  case 99:return true;
  case 100:return true;
  // Town 9 = 鲁夫特悬空海港 (town/lufthafen.twn), areas 0..3 = map/lufthafen/*.map; the rows were
  // already carried by Portals/Npc/DungeonPortal, only the region set was closed. Re-derived by
  // local-run/pvf/r2_gen_gent_lufthafen.py (town-evidence section).
  case 144:return true;
  case 145:return true;
  case 146:return true;
  case 147:return true;
  // Town 12 = 虚祖首都素喃 (town/shonan.twn), areas 0..3 = map/shonan/*.map; area2 is the
  // 武斗大会 `[dungeon gate]` 16 area and stays closed (PvP tournament, no maze rooms in the .dgn),
  // the town itself is walkable. Rows already carried by Portals/Npc; re-derived by
  // local-run/pvf/gen_town_content.py town-evidence (export/town/shonan.twn.txt).
  case 192:return true;
  case 193:return true;
  case 194:return true;
  case 195:return true;
 default:return false;}}
 internal static bool Npc(int town,int area,int npc){switch(town*16+area){
  case 16:return npc==1||npc==8;
  case 17:return npc==2||npc==22||npc==32;
  case 32:return npc==6||npc==13||npc==17||npc==20||npc==75||npc==84;
  case 33:return npc==15||npc==18||npc==27||npc==45||npc==92||npc==93||npc==94||npc==95||npc==97;
  case 34:return npc==3||npc==4||npc==7||npc==36||npc==80;
  case 35:return npc==9||npc==64;
  case 36:return npc==5;
  case 37:return npc==2||npc==22||npc==32;
  case 38:return npc==123;
  case 48:return npc==10||npc==11||npc==15||npc==21||npc==29||npc==30;
  case 49:return npc==12||npc==28||npc==38;
  case 50:return npc==2||npc==22||npc==32;
  case 52:return npc==19;
  case 54:return npc==60;
  case 55:return npc==72;
  case 64:return npc==23||npc==24||npc==25||npc==26||npc==44;
  case 65:return npc==2||npc==22||npc==32;
  case 68:return npc==62;
  case 80:return npc==39||npc==40||npc==41||npc==42;
  case 81:return npc==2||npc==22||npc==32;
  case 96:return npc==56||npc==57;
  case 97:return npc==15||npc==58||npc==59;
  case 100:return npc==2||npc==22||npc==32;
  case 112:return npc==98;
  case 116:return npc==96;
  case 128:return npc==99;
  case 129:return npc==99;
  case 131:return npc==99;
  case 145:return npc==65;
  case 146:return npc==2||npc==22||npc==32;
  case 147:return npc==66;
  case 160:return npc==22||npc==32||npc==69;
  case 161:return npc==70||npc==71;
  case 176:return npc==98;
  case 179:return npc==96;
  case 192:return npc==125||npc==126;
  case 193:return npc==122||npc==124;
  case 195:return npc==2||npc==22||npc==32;
 default:return false;}}
 internal static int[][] Portals(int town,int area){switch(town*16+area){
  case 16:return new int[][]{new int[]{-5,192,29,141,2,1},new int[]{730,147,110,22,1,1},new int[]{1540,189,28,144,1,2},new int[]{1049,341,88,19,1,3}};
  case 17:return new int[][]{new int[]{330,324,289,24,1,0}};
  case 18:return new int[][]{new int[]{-2,176,21,143,1,0}};
  case 19:return new int[][]{new int[]{158,151,70,20,1,0},new int[]{873,188,17,137,4,0}};
  case 32:return new int[][]{new int[]{1,230,19,195,2,6},new int[]{1172,157,107,21,2,2},new int[]{1396,456,100,40,2,1},new int[]{3210,169,96,14,2,4},new int[]{3555,223,21,220,3,0},new int[]{1552,170,79,24,2,5}};
  case 33:return new int[][]{new int[]{1855,163,106,18,2,0},new int[]{3777,219,20,117,1,0},new int[]{4,224,21,107,2,7}};
  case 34:return new int[][]{new int[]{1133,155,87,19,2,3},new int[]{1289,350,95,19,2,0},new int[]{5,217,20,121,2,8}};
  case 35:return new int[][]{new int[]{462,342,91,20,2,2},new int[]{2,211,20,132,2,9}};
  case 36:return new int[][]{new int[]{74,170,21,77,2,0}};
  case 37:return new int[][]{new int[]{330,324,289,24,2,0}};
  case 38:return new int[][]{new int[]{873,122,22,187,2,0}};
  case 39:return new int[][]{new int[]{873,122,22,187,2,1}};
  case 40:return new int[][]{new int[]{865,225,23,112,2,2},new int[]{13,225,23,103,5,0}};
  case 41:return new int[][]{new int[]{865,210,23,139,2,3}};
  case 48:return new int[][]{new int[]{-2,206,18,250,2,0},new int[]{287,164,99,16,3,1},new int[]{723,168,99,15,3,2},new int[]{2893,219,18,237,3,3},new int[]{2500,476,94,18,3,5}};
  case 49:return new int[][]{new int[]{333,351,100,15,3,0},new int[]{1765,221,21,110,3,4},new int[]{1173,161,91,21,3,6},new int[]{13,223,30,107,3,7},new int[]{288,161,84,23,3,8}};
  case 50:return new int[][]{new int[]{330,324,289,24,3,0}};
  case 51:return new int[][]{new int[]{4,114,19,188,3,0}};
  case 52:return new int[][]{new int[]{2,221,21,129,3,1}};
  case 53:return new int[][]{new int[]{4,114,19,188,3,0}};
  case 54:return new int[][]{new int[]{2,221,21,129,3,1}};
  case 55:return new int[][]{new int[]{871,230,23,127,3,1}};
  case 56:return new int[][]{new int[]{430,348,91,20,3,1},new int[]{3,207,20,142,3,9},new int[]{862,207,23,142,3,10}};
  case 57:return new int[][]{new int[]{867,196,23,172,3,8}};
  case 58:return new int[][]{new int[]{4,198,23,174,3,8}};
  case 64:return new int[][]{new int[]{9,183,22,258,1,3},new int[]{497,120,85,21,4,1},new int[]{2430,184,18,258,4,2},new int[]{1004,453,76,25,4,3},new int[]{321,442,81,29,4,4}};
  case 65:return new int[][]{new int[]{330,324,289,24,4,0}};
  case 66:return new int[][]{new int[]{9,173,23,126,4,0}};
  case 67:return new int[][]{new int[]{285,131,60,20,4,0}};
  case 68:return new int[][]{new int[]{924,128,83,29,4,0}};
  case 80:return new int[][]{new int[]{2190,695,25,217,2,8},new int[]{31,353,19,92,5,2},new int[]{31,801,20,112,5,3},new int[]{1225,439,95,19,5,1}};
  case 81:return new int[][]{new int[]{330,324,289,24,5,0}};
  case 82:return new int[][]{new int[]{851,355,20,124,5,0}};
  case 83:return new int[][]{new int[]{851,355,20,124,5,0}};
  case 96:return new int[][]{new int[]{833,140,106,18,6,1},new int[]{1537,209,28,122,6,2}};
  case 97:return new int[][]{new int[]{2,232,28,75,9,0},new int[]{784,338,96,23,6,0},new int[]{1980,210,30,121,6,3},new int[]{1112,145,97,21,6,4}};
  case 98:return new int[][]{new int[]{3,161,17,59,6,0}};
  case 99:return new int[][]{new int[]{3,161,17,59,6,1}};
  case 100:return new int[][]{new int[]{330,324,289,24,6,1}};
  case 112:return new int[][]{new int[]{863,166,23,158,7,1}};
  case 113:return new int[][]{new int[]{7,167,19,297,7,0},new int[]{1090,129,22,308,7,2}};
  case 114:return new int[][]{new int[]{2,217,19,253,7,1},new int[]{1099,139,17,286,7,3}};
  case 115:return new int[][]{new int[]{2,154,21,400,7,2},new int[]{1087,158,20,303,7,5}};
  case 116:return new int[][]{new int[]{6,157,19,160,7,5}};
  case 117:return new int[][]{new int[]{9,160,19,411,7,3},new int[]{1092,145,20,331,7,4}};
  case 128:return new int[][]{};
  case 129:return new int[][]{};
  case 130:return new int[][]{new int[]{859,187,32,75,8,3},new int[]{803,171,49,40,8,3},new int[]{849,201,30,40,8,3},new int[]{835,187,30,40,8,3}};
  case 131:return new int[][]{new int[]{12,226,30,40,8,2},new int[]{65,175,30,40,8,2},new int[]{47,187,38,45,8,2},new int[]{31,204,30,40,8,2}};
  case 132:return new int[][]{new int[]{330,324,289,24,2,2}};
  case 144:return new int[][]{new int[]{1,226,29,103,9,3},new int[]{441,150,106,25,9,2},new int[]{1537,209,28,122,6,1}};
  case 145:return new int[][]{new int[]{3,141,20,178,9,3}};
  case 146:return new int[][]{new int[]{330,324,289,24,9,0}};
  case 147:return new int[][]{new int[]{172,162,198,18,9,1},new int[]{865,202,28,122,9,0}};
  case 160:return new int[][]{new int[]{330,324,289,24,10,1}};
  case 161:return new int[][]{new int[]{74,172,19,77,10,0}};
  case 176:return new int[][]{new int[]{863,166,23,158,11,1}};
  case 177:return new int[][]{new int[]{7,167,19,297,11,0},new int[]{1090,129,22,308,11,2}};
  case 178:return new int[][]{new int[]{2,154,21,400,11,1},new int[]{1087,158,20,303,11,3}};
  case 179:return new int[][]{new int[]{6,157,19,160,11,2}};
  case 192:return new int[][]{new int[]{1257,119,222,31,12,2},new int[]{767,348,96,23,12,1},new int[]{2016,142,97,21,12,3}};
  case 193:return new int[][]{new int[]{574,127,103,21,12,0}};
  case 194:return new int[][]{new int[]{7,207,22,163,12,0}};
  case 195:return new int[][]{new int[]{330,324,289,24,12,0}};
 default:throw new InvalidDataException("Unsupported town area");}}
 internal static int[] DungeonPortal(int town,int area){switch(town*16+area){
  case 18:return new int[]{863,174,27,145};
  case 36:return new int[]{1235,138,50,100};
  case 38:return new int[]{1,150,23,150};
  case 39:return new int[]{1,150,23,150};
  case 41:return new int[]{13,210,23,140};
  case 51:return new int[]{871,120,23,155};
  case 52:return new int[]{869,261,23,88};
  case 53:return new int[]{871,120,23,155};
  case 54:return new int[]{869,269,23,88};
  case 55:return new int[]{7,229,23,128};
  case 57:return new int[]{1,200,23,174};
  case 58:return new int[]{866,194,23,178};
  case 66:return new int[]{1000,199,22,78};
  case 67:return new int[]{1000,199,22,78};
  case 68:return new int[]{165,210,35,46};
  case 82:return new int[]{14,354,19,125};
  case 83:return new int[]{14,354,19,125};
  case 98:return new int[]{989,155,20,72};
  case 99:return new int[]{989,155,20,72};
  case 145:return new int[]{853,138,30,199};
  case 194:return new int[]{992,295,37,62};
 default:return null;}}
 internal static bool Linked(int town,int area,int targetTown,int targetArea){foreach(int[] p in Portals(town,area))if(p[4]==targetTown&&p[5]==targetArea)return true;return false;}
 internal static bool DungeonGate(int town,int area){return town==1&&area==2||town==2&&area==6||town==2&&area==9||town==2&&(area==4||area==7)||town==3&&(area==3||area==4||area==5||area==6)||town==3&&(area==9||area==10)||town==4&&(area==2||area==3)||town==5&&(area==2||area==3)||town==6&&area==2||town==6&&area==3||town==9&&area==1;}
}
