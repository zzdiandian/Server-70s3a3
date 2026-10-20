// Generated from original CN70 PVF: first swordman promotion SP skills.
class Offline70SkillRuleFG {internal byte Id,Maximum,Required,Step;internal ushort Cost;internal bool Active;internal byte[] Prerequisites,GrowCaps;}
partial class Offline70SkillSlots {
 internal static readonly Offline70SkillRuleFG[] Rules=new Offline70SkillRuleFG[]{
 new Offline70SkillRuleFG{Id=1,Cost=20,Maximum=5,Required=5,Step=5,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{5,5,5,5,5}}, // skill/swordman/guard.skl
 new Offline70SkillRuleFG{Id=2,Cost=25,Maximum=30,Required=20,Step=2,Active=true,Prerequisites=new byte[]{1,1},GrowCaps=new byte[]{0,0,0,0,30}}, // skill/swordman/reflectguard.skl
 new Offline70SkillRuleFG{Id=3,Cost=20,Maximum=10,Required=25,Step=3,Active=true,Prerequisites=new byte[]{1,5},GrowCaps=new byte[]{0,10,0,0,0}}, // skill/swordman/autoguard.skl
 new Offline70SkillRuleFG{Id=4,Cost=20,Maximum=1,Required=20,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/lightswordmastery.skl
 new Offline70SkillRuleFG{Id=5,Cost=15,Maximum=30,Required=1,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/hardattack.skl
 new Offline70SkillRuleFG{Id=6,Cost=15,Maximum=20,Required=25,Step=3,Active=false,Prerequisites=new byte[]{5,5,25,5},GrowCaps=new byte[]{0,0,20,0,0}}, // skill/swordman/hardattackcharge.skl
 new Offline70SkillRuleFG{Id=7,Cost=20,Maximum=20,Required=30,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,20,0,0,0}}, // skill/swordman/hitback.skl
 new Offline70SkillRuleFG{Id=8,Cost=15,Maximum=30,Required=10,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/tripleslash.skl
 new Offline70SkillRuleFG{Id=9,Cost=40,Maximum=30,Required=35,Step=2,Active=true,Prerequisites=new byte[]{8,5,67,1},GrowCaps=new byte[]{0,30,0,0,0}}, // skill/swordman/momentaryslash.skl
 new Offline70SkillRuleFG{Id=10,Cost=30,Maximum=1,Required=18,Step=0,Active=false,Prerequisites=new byte[]{20,5},GrowCaps=new byte[]{0,0,0,0,1}}, // skill/swordman/cancelwave.skl
 new Offline70SkillRuleFG{Id=11,Cost=15,Maximum=30,Required=5,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/dashattackmultihit.skl
 new Offline70SkillRuleFG{Id=12,Cost=20,Maximum=20,Required=15,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{20,1,20,20,20}}, // skill/swordman/shortswordmastery.skl
 new Offline70SkillRuleFG{Id=13,Cost=20,Maximum=20,Required=15,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{20,1,20,20,20}}, // skill/swordman/blademastery.skl
 new Offline70SkillRuleFG{Id=14,Cost=20,Maximum=20,Required=15,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{20,1,20,20,20}}, // skill/swordman/heavyswordmastery.skl
 new Offline70SkillRuleFG{Id=15,Cost=20,Maximum=20,Required=15,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{20,1,20,20,20}}, // skill/swordman/bluntmastery.skl
 new Offline70SkillRuleFG{Id=16,Cost=15,Maximum=30,Required=10,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/ashenfork.skl
 new Offline70SkillRuleFG{Id=17,Cost=50,Maximum=1,Required=15,Step=1,Active=true,Prerequisites=new byte[]{16,1},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/jumpattackmulti.skl
 new Offline70SkillRuleFG{Id=18,Cost=30,Maximum=20,Required=30,Step=3,Active=true,Prerequisites=new byte[]{25,5,77,5},GrowCaps=new byte[]{0,0,20,0,0}}, // skill/swordman/ghoststep.skl
 new Offline70SkillRuleFG{Id=19,Cost=25,Maximum=20,Required=25,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,20,0}}, // skill/swordman/pinchpowerup.skl
 new Offline70SkillRuleFG{Id=20,Cost=15,Maximum=30,Required=5,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/normalwave.skl
 new Offline70SkillRuleFG{Id=21,Cost=25,Maximum=30,Required=30,Step=2,Active=true,Prerequisites=new byte[]{20,5},GrowCaps=new byte[]{0,0,0,0,30}}, // skill/swordman/icewave.skl
 new Offline70SkillRuleFG{Id=22,Cost=40,Maximum=30,Required=35,Step=2,Active=true,Prerequisites=new byte[]{21,3},GrowCaps=new byte[]{0,0,0,0,30}}, // skill/swordman/firewave.skl
 new Offline70SkillRuleFG{Id=23,Cost=30,Maximum=20,Required=35,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,20,0}}, // skill/swordman/sacrifice.skl
 new Offline70SkillRuleFG{Id=24,Cost=40,Maximum=30,Required=30,Step=2,Active=true,Prerequisites=new byte[]{31,3},GrowCaps=new byte[]{0,0,0,30,0}}, // skill/swordman/bloodblast.skl
 new Offline70SkillRuleFG{Id=25,Cost=20,Maximum=20,Required=5,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,20,10,10}}, // skill/swordman/khazan.skl
 new Offline70SkillRuleFG{Id=26,Cost=50,Maximum=1,Required=20,Step=0,Active=false,Prerequisites=new byte[]{8,5},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/canceltripleslash.skl
 new Offline70SkillRuleFG{Id=27,Cost=20,Maximum=20,Required=20,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,20,0,0,0}}, // skill/swordman/weaponmasteryup.skl
 new Offline70SkillRuleFG{Id=28,Cost=10,Maximum=1,Required=2,Step=0,Active=false,Prerequisites=new byte[]{5,1},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/cancelhardattack.skl
 new Offline70SkillRuleFG{Id=30,Cost=30,Maximum=10,Required=20,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,10,0,0}}, // skill/swordman/darktoleranceup.skl
 new Offline70SkillRuleFG{Id=31,Cost=25,Maximum=30,Required=25,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,30,0}}, // skill/swordman/grabblastblood.skl
 new Offline70SkillRuleFG{Id=32,Cost=20,Maximum=20,Required=25,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,20}}, // skill/swordman/releasewave.skl
 new Offline70SkillRuleFG{Id=34,Cost=20,Maximum=20,Required=20,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,20,0}}, // skill/swordman/diehard.skl
 new Offline70SkillRuleFG{Id=36,Cost=40,Maximum=30,Required=30,Step=2,Active=true,Prerequisites=new byte[]{25,5},GrowCaps=new byte[]{0,0,30,0,0}}, // skill/swordman/saya.skl
 new Offline70SkillRuleFG{Id=38,Cost=25,Maximum=20,Required=25,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,20,0,0,0}}, // skill/swordman/overdrive.skl
 new Offline70SkillRuleFG{Id=39,Cost=20,Maximum=10,Required=20,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,10,0,0,0}}, // skill/swordman/durabilityup.skl
 new Offline70SkillRuleFG{Id=40,Cost=25,Maximum=20,Required=25,Step=3,Active=true,Prerequisites=new byte[]{64,5},GrowCaps=new byte[]{0,0,0,20,0}}, // skill/swordman/reckless.skl
 new Offline70SkillRuleFG{Id=41,Cost=25,Maximum=20,Required=20,Step=3,Active=true,Prerequisites=new byte[]{25,5},GrowCaps=new byte[]{0,0,20,0,0}}, // skill/swordman/bremen.skl
 new Offline70SkillRuleFG{Id=44,Cost=40,Maximum=30,Required=35,Step=2,Active=true,Prerequisites=new byte[]{30,5},GrowCaps=new byte[]{0,0,30,0,0}}, // skill/swordman/tombstonerain.skl
 new Offline70SkillRuleFG{Id=45,Cost=50,Maximum=1,Required=15,Step=0,Active=false,Prerequisites=new byte[]{1,3},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/cancelguard.skl
 new Offline70SkillRuleFG{Id=46,Cost=20,Maximum=10,Required=1,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/upperslash.skl
 new Offline70SkillRuleFG{Id=47,Cost=20,Maximum=20,Required=17,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,20}}, // skill/swordman/wavemark.skl
 new Offline70SkillRuleFG{Id=48,Cost=50,Maximum=1,Required=1,Step=0,Active=false,Prerequisites=new byte[]{46,1},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/cancelupperslash.skl
 new Offline70SkillRuleFG{Id=49,Cost=50,Maximum=1,Required=20,Step=1,Active=true,Prerequisites=new byte[]{169,1},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/backstepcutter.skl
 new Offline70SkillRuleFG{Id=50,Cost=25,Maximum=30,Required=20,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,30}}, // skill/swordman/grandwave.skl
 new Offline70SkillRuleFG{Id=51,Cost=50,Maximum=1,Required=20,Step=1,Active=false,Prerequisites=new byte[]{50,1},GrowCaps=new byte[]{0,0,0,0,1}}, // skill/swordman/grandwavecharge.skl
 new Offline70SkillRuleFG{Id=52,Cost=30,Maximum=30,Required=30,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,30}}, // skill/swordman/dotarea.skl
 new Offline70SkillRuleFG{Id=53,Cost=10,Maximum=20,Required=25,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,20}}, // skill/swordman/mindflash.skl
 new Offline70SkillRuleFG{Id=54,Cost=50,Maximum=1,Required=30,Step=0,Active=false,Prerequisites=new byte[]{31,3},GrowCaps=new byte[]{0,0,0,1,0}}, // skill/swordman/cancelgrabblastblood.skl
 new Offline70SkillRuleFG{Id=57,Cost=50,Maximum=30,Required=40,Step=2,Active=true,Prerequisites=new byte[]{52,5},GrowCaps=new byte[]{0,0,0,0,30}}, // skill/swordman/shockwavearea.skl
 new Offline70SkillRuleFG{Id=58,Cost=15,Maximum=30,Required=10,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/vaneslash.skl
 new Offline70SkillRuleFG{Id=59,Cost=50,Maximum=1,Required=20,Step=0,Active=false,Prerequisites=new byte[]{58,5},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/cancelvaneslash.skl
 new Offline70SkillRuleFG{Id=60,Cost=50,Maximum=20,Required=40,Step=2,Active=true,Prerequisites=new byte[]{18,4,80,1},GrowCaps=new byte[]{0,0,20,0,0}}, // skill/swordman/ghoststepslash.skl
 new Offline70SkillRuleFG{Id=62,Cost=40,Maximum=30,Required=35,Step=2,Active=true,Prerequisites=new byte[]{52,3,53,4},GrowCaps=new byte[]{0,0,0,0,30}}, // skill/swordman/standalonewave.skl
 new Offline70SkillRuleFG{Id=63,Cost=20,Maximum=1,Required=20,Step=1,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,1,0}}, // skill/swordman/bloodyvigorous.skl
 new Offline70SkillRuleFG{Id=64,Cost=15,Maximum=30,Required=10,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/gorecross.skl
 new Offline70SkillRuleFG{Id=65,Cost=20,Maximum=30,Required=15,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/hopsmash.skl
 new Offline70SkillRuleFG{Id=66,Cost=50,Maximum=1,Required=25,Step=0,Active=false,Prerequisites=new byte[]{65,5},GrowCaps=new byte[]{0,0,0,1,0}}, // skill/swordman/cancelhopsmash.skl
 new Offline70SkillRuleFG{Id=67,Cost=50,Maximum=1,Required=20,Step=1,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/weaponcombo.skl
 new Offline70SkillRuleFG{Id=68,Cost=30,Maximum=30,Required=30,Step=2,Active=true,Prerequisites=new byte[]{46,5},GrowCaps=new byte[]{0,30,0,0,0}}, // skill/swordman/chargecrash.skl
 new Offline70SkillRuleFG{Id=69,Cost=50,Maximum=1,Required=40,Step=0,Active=false,Prerequisites=new byte[]{68,5},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/cancelchargecrash.skl
 new Offline70SkillRuleFG{Id=70,Cost=50,Maximum=1,Required=40,Step=0,Active=false,Prerequisites=new byte[]{24,5},GrowCaps=new byte[]{0,0,0,1,0}}, // skill/swordman/cancelbloodblast.skl
 new Offline70SkillRuleFG{Id=71,Cost=50,Maximum=1,Required=25,Step=0,Active=false,Prerequisites=new byte[]{2,3},GrowCaps=new byte[]{0,0,0,0,1}}, // skill/swordman/cancelreflectguard.skl
 new Offline70SkillRuleFG{Id=72,Cost=50,Maximum=30,Required=40,Step=2,Active=true,Prerequisites=new byte[]{8,5,68,3},GrowCaps=new byte[]{0,30,0,0,0}}, // skill/swordman/rapidmoveslash.skl
 new Offline70SkillRuleFG{Id=73,Cost=60,Maximum=30,Required=45,Step=2,Active=true,Prerequisites=new byte[]{38,5,9,5},GrowCaps=new byte[]{0,30,0,0,0}}, // skill/swordman/illusionslash.skl
 new Offline70SkillRuleFG{Id=74,Cost=60,Maximum=30,Required=45,Step=2,Active=true,Prerequisites=new byte[]{47,5,51,1},GrowCaps=new byte[]{0,0,0,0,30}}, // skill/swordman/wavespinarea.skl
 new Offline70SkillRuleFG{Id=75,Cost=40,Maximum=30,Required=35,Step=2,Active=true,Prerequisites=new byte[]{41,5,36,3},GrowCaps=new byte[]{0,0,30,0,0}}, // skill/swordman/epidemicrasa.skl
 new Offline70SkillRuleFG{Id=76,Cost=30,Maximum=20,Required=17,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,20,0}}, // skill/swordman/frenzy.skl
 new Offline70SkillRuleFG{Id=77,Cost=20,Maximum=30,Required=15,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{30,30,30,30,30}}, // skill/swordman/moonlightslash.skl
 new Offline70SkillRuleFG{Id=78,Cost=50,Maximum=1,Required=20,Step=0,Active=false,Prerequisites=new byte[]{64,5},GrowCaps=new byte[]{0,0,0,1,0}}, // skill/swordman/cancelgorecross.skl
 new Offline70SkillRuleFG{Id=79,Cost=50,Maximum=30,Required=35,Step=2,Active=true,Prerequisites=new byte[]{24,3,76,1},GrowCaps=new byte[]{0,0,0,30,0}}, // skill/swordman/bloodyrave.skl
 new Offline70SkillRuleFG{Id=80,Cost=100,Maximum=1,Required=20,Step=1,Active=false,Prerequisites=new byte[]{77,3},GrowCaps=new byte[]{0,0,1,0,0}}, // skill/swordman/moonlightslashfull.skl
 new Offline70SkillRuleFG{Id=81,Cost=60,Maximum=30,Required=45,Step=2,Active=true,Prerequisites=new byte[]{65,5,76,4},GrowCaps=new byte[]{0,0,0,30,0}}, // skill/swordman/outragebreak.skl
 new Offline70SkillRuleFG{Id=82,Cost=60,Maximum=30,Required=45,Step=2,Active=true,Prerequisites=new byte[]{6,5,30,5},GrowCaps=new byte[]{0,0,30,0,0}}, // skill/swordman/kalla.skl
 new Offline70SkillRuleFG{Id=83,Cost=50,Maximum=1,Required=25,Step=0,Active=false,Prerequisites=new byte[]{77,5},GrowCaps=new byte[]{0,0,1,0,0}}, // skill/swordman/cancelmoonlightslash.skl
 new Offline70SkillRuleFG{Id=84,Cost=20,Maximum=10,Required=20,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,10,0,0}}, // skill/swordman/keiga.skl
 new Offline70SkillRuleFG{Id=103,Cost=50,Maximum=30,Required=40,Step=2,Active=true,Prerequisites=new byte[]{76,4},GrowCaps=new byte[]{0,0,0,30,0}}, // skill/swordman/bloodsword.skl
 new Offline70SkillRuleFG{Id=104,Cost=50,Maximum=1,Required=40,Step=0,Active=false,Prerequisites=new byte[]{103,5},GrowCaps=new byte[]{0,0,0,1,0}}, // skill/swordman/cancelbloodsword.skl
 new Offline70SkillRuleFG{Id=105,Cost=50,Maximum=1,Required=25,Step=2,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/flowmind.skl
 new Offline70SkillRuleFG{Id=106,Cost=50,Maximum=1,Required=25,Step=0,Active=false,Prerequisites=new byte[]{105,1},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/cancelflowmind.skl
 new Offline70SkillRuleFG{Id=107,Cost=25,Maximum=30,Required=25,Step=2,Active=true,Prerequisites=new byte[]{105,1,11,5},GrowCaps=new byte[]{0,30,0,0,0}}, // skill/swordman/flowmindone.skl
 new Offline70SkillRuleFG{Id=108,Cost=25,Maximum=30,Required=25,Step=2,Active=true,Prerequisites=new byte[]{105,1,8,5},GrowCaps=new byte[]{0,30,0,0,0}}, // skill/swordman/flowmindtwo.skl
 new Offline70SkillRuleFG{Id=109,Cost=30,Maximum=30,Required=30,Step=2,Active=true,Prerequisites=new byte[]{105,1,17,1},GrowCaps=new byte[]{0,30,0,0,0}}, // skill/swordman/flowmindthree.skl
 new Offline70SkillRuleFG{Id=110,Cost=30,Maximum=30,Required=30,Step=3,Active=true,Prerequisites=new byte[]{105,1},GrowCaps=new byte[]{0,30,0,0,0}}, // skill/swordman/flowmindpowerup.skl
 new Offline70SkillRuleFG{Id=111,Cost=30,Maximum=30,Required=30,Step=3,Active=true,Prerequisites=new byte[]{18,1},GrowCaps=new byte[]{0,0,30,0,0}}, // skill/swordman/ghostsidewind.skl
 new Offline70SkillRuleFG{Id=112,Cost=30,Maximum=30,Required=35,Step=3,Active=true,Prerequisites=new byte[]{111,2},GrowCaps=new byte[]{0,0,30,0,0}}, // skill/swordman/triplestab.skl
 new Offline70SkillRuleFG{Id=113,Cost=50,Maximum=1,Required=40,Step=0,Active=false,Prerequisites=new byte[]{111,5},GrowCaps=new byte[]{0,0,1,0,0}}, // skill/swordman/cancelghostsidewind.skl
 new Offline70SkillRuleFG{Id=114,Cost=50,Maximum=1,Required=40,Step=0,Active=false,Prerequisites=new byte[]{112,5},GrowCaps=new byte[]{0,0,1,0,0}}, // skill/swordman/canceltriplestab.skl
 new Offline70SkillRuleFG{Id=169,Cost=30,Maximum=1,Required=5,Step=0,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/backstep.skl
 new Offline70SkillRuleFG{Id=170,Cost=70,Maximum=1,Required=10,Step=0,Active=false,Prerequisites=new byte[]{169,1},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/cancelbackstep.skl
 new Offline70SkillRuleFG{Id=171,Cost=50,Maximum=1,Required=20,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/cancelthrowitem.skl
 new Offline70SkillRuleFG{Id=175,Cost=20,Maximum=10,Required=10,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/jumppowerup.skl
 new Offline70SkillRuleFG{Id=176,Cost=25,Maximum=10,Required=15,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/magicalattackup.skl
 new Offline70SkillRuleFG{Id=177,Cost=25,Maximum=10,Required=10,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/throwmastery.skl
 new Offline70SkillRuleFG{Id=178,Cost=20,Maximum=10,Required=20,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/physicalbackattackcritical.skl
 new Offline70SkillRuleFG{Id=180,Cost=25,Maximum=10,Required=15,Step=3,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/superarmoroncast.skl
 new Offline70SkillRuleFG{Id=181,Cost=10,Maximum=4,Required=1,Step=10,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{2,4,4,2,2}}, // skill/swordman/compoundcraft.skl
 new Offline70SkillRuleFG{Id=182,Cost=10,Maximum=4,Required=1,Step=10,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{2,4,4,2,2}}, // skill/swordman/compoundweaving.skl
 new Offline70SkillRuleFG{Id=184,Cost=10,Maximum=2,Required=1,Step=10,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{2,2,2,2,2}}, // skill/swordman/compoundchemistry.skl
 new Offline70SkillRuleFG{Id=186,Cost=20,Maximum=10,Required=20,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/physicalcriticalhitup.skl
 new Offline70SkillRuleFG{Id=188,Cost=20,Maximum=10,Required=20,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/magicalcriticalhitup.skl
 new Offline70SkillRuleFG{Id=189,Cost=20,Maximum=10,Required=20,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/magicalbackattackcritical.skl
 new Offline70SkillRuleFG{Id=190,Cost=10,Maximum=1,Required=1,Step=0,Active=true,Prerequisites=new byte[]{},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/quickstanding.skl
 new Offline70SkillRuleFG{Id=29,Cost=0,Maximum=1,Required=18,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,1,0,0}}, // skill/swordman/darkfriendship.skl
 new Offline70SkillRuleFG{Id=33,Cost=0,Maximum=1,Required=1,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/equiplightsword.skl
 new Offline70SkillRuleFG{Id=35,Cost=0,Maximum=1,Required=1,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,1,0,0}}, // skill/swordman/ghostpowerup.skl
 new Offline70SkillRuleFG{Id=37,Cost=0,Maximum=1,Required=1,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/fastequip.skl
 new Offline70SkillRuleFG{Id=55,Cost=0,Maximum=1,Required=18,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,1}}, // skill/swordman/senseviolenttemper.skl
 new Offline70SkillRuleFG{Id=56,Cost=0,Maximum=1,Required=18,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,1,0}}, // skill/swordman/armormasteryheavybk.skl
 new Offline70SkillRuleFG{Id=61,Cost=0,Maximum=1,Required=1,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,1}}, // skill/swordman/armormasteryplatewave.skl
 new Offline70SkillRuleFG{Id=93,Cost=0,Maximum=1,Required=18,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,1,0,0}}, // skill/swordman/armormasteryclothsb.skl
 new Offline70SkillRuleFG{Id=94,Cost=0,Maximum=1,Required=1,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/armormasterylightwm.skl
 new Offline70SkillRuleFG{Id=172,Cost=0,Maximum=1,Required=1,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{1,1,1,1,1}}, // skill/swordman/armormasterylight.skl
 new Offline70SkillRuleFG{Id=187,Cost=0,Maximum=1,Required=1,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,1,0,0,0}}, // skill/swordman/armormasterycloth.skl
 new Offline70SkillRuleFG{Id=197,Cost=0,Maximum=1,Required=20,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,1,1,1,1}}, // skill/swordman/conversion.skl
 // Expert job skills, PVF measured 2026-10-17 (all four: [purchase cost] 0, [required level] 20,
 // [type] [passive], [maximum level] 20, [growtype maximum level] 10 10 10 10 10 10, [skill class] 4):
 // Maximum/GrowCaps follow this file's existing convention (first five grow caps, Maximum = their max,
 // cf. 181 compoundcraft "2 4 4 2 2 2" -> Maximum=4 GrowCaps={2,4,4,2,2}). Cost=0 keeps them out of
 // the SP learn path; they are granted only by the [expert job] reward (Reward-Types-FG183).
 new Offline70SkillRuleFG{Id=191,Cost=0,Maximum=10,Required=20,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/enchant.skl (enchanter)
 new Offline70SkillRuleFG{Id=192,Cost=0,Maximum=10,Required=20,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/dollmaking.skl (doll controller)
 new Offline70SkillRuleFG{Id=193,Cost=0,Maximum=10,Required=20,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/alchemy.skl (alchemist)
 new Offline70SkillRuleFG{Id=194,Cost=0,Maximum=10,Required=20,Step=0,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{10,10,10,10,10}}, // skill/swordman/opendisjointshop.skl (disjointer)
 // First-awakening passives, character/swordman/swordman.chr [growtype grow+1][awakening 1]
 // [awakening skill] = (id,1); PVF measured 2026-10-17 over the running HTTP API
 // (local-run/pvf/awakening_pvf_truth.py -> local-run/driver/awakening-pvf-truth.tsv).
 // All four: [purchase cost] 30, [required level] 48, [required level range] 3 (= Step, the same
 // tag 12 shortswordmastery reads 3 from), [type] [passive], no prerequisite tag, and
 // [growtype maximum level] 0 0 0 0 0 0 -> GrowCaps {0,0,0,0,0}, so JobMaximum(grow)=0 keeps them
 // out of the op31 SP path (Learn rejects JobMaximum==0) exactly as the script says. Maximum
 // follows this file's convention (max of the five grow caps, cf. 191 above), i.e. 0: the script's
 // absolute [maximum level] column (50/50/50/20 - 92 stuckuponcounter really is 20, the earlier
 // awakening report said 50 for all four) only matters for the post-awakening ceiling carried by
 // [second growtype maximum level] (20/20/20/10 at the owner growtype), which is the QP skill path
 // this build does not implement. BaseLevelFG139 supplies the granted level as baseline, so
 // Validate admits level 1 (Math.Max(baseline,Maximum)) and the ledger charges (1-1)*30 = 0 SP.
 new Offline70SkillRuleFG{Id=91,Cost=30,Maximum=0,Required=48,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,0}}, // skill/swordman/defensedownoncritical.skl (swordman grow 1)
 new Offline70SkillRuleFG{Id=89,Cost=30,Maximum=0,Required=48,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,0}}, // skill/swordman/darkaura.skl (swordman grow 2)
 new Offline70SkillRuleFG{Id=90,Cost=30,Maximum=0,Required=48,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,0}}, // skill/swordman/bleedingmastery.skl (swordman grow 3)
 new Offline70SkillRuleFG{Id=92,Cost=30,Maximum=0,Required=48,Step=3,Active=false,Prerequisites=new byte[]{},GrowCaps=new byte[]{0,0,0,0,0}}, // skill/swordman/stuckuponcounter.skl (swordman grow 4)
 };
 internal static Offline70SkillRuleFG Rule(byte id){foreach(Offline70SkillRuleFG r in Rules)if(r.Id==id)return r;return null;}
 internal static byte JobMaximum(Offline70SkillRuleFG r,byte grow){if(r==null||grow>4)return 0;return r.GrowCaps[grow];}
}
