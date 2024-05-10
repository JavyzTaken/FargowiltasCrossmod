using CalamityMod.NPCs.AcidRain;
using FargowiltasSouls.Content.Bosses.Champions.Cosmos;
using FargowiltasSouls.Content.Bosses.Champions.Earth;
using FargowiltasSouls.Content.Bosses.Champions.Life;
using FargowiltasSouls.Content.Bosses.Champions.Nature;
using FargowiltasSouls.Content.Bosses.Champions.Shadow;
using FargowiltasSouls.Content.Bosses.Champions.Spirit;
using FargowiltasSouls.Content.Bosses.Champions.Terra;
using FargowiltasSouls.Content.Bosses.Champions.Timber;
using FargowiltasSouls.Content.Bosses.Champions.Will;
using FargowiltasSouls.Content.Items.Weapons.SwarmDrops;
using FargowiltasSouls.Content.Patreon.DemonKing;
using FargowiltasSouls.Content.Patreon.Duck;
using FargowiltasSouls.Content.Patreon.GreatestKraken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Common
{
    public static class ContentLists
    {
        #region Items
        public static List<int> ChampionTierFargoWeapons = new List<int>
        {
            ModContent.ItemType<EaterLauncher>(),
            ModContent.ItemType<FleshCannon>(),
            ModContent.ItemType<HellZone>(),
            ModContent.ItemType<MechanicalLeashOfCthulhu>(),
            ModContent.ItemType<SlimeSlingingSlasher>(),
            ModContent.ItemType<TheBigSting>(),
            ModContent.ItemType<ScientificRailgun>(),
            ModContent.ItemType<VortexMagnetRitual>()
        };
        public static List<int> AbomTierFargoWeapons = new List<int>
        {
            ModContent.ItemType<DragonBreath2>(),
            ModContent.ItemType<DestroyerGun2>(),
            ModContent.ItemType<GolemTome2>(),
            ModContent.ItemType<GeminiGlaives>(),
            ModContent.ItemType<Blender>(),
            ModContent.ItemType<RefractorBlaster2>(),
            ModContent.ItemType<NukeFishron>(),
            ModContent.ItemType<StaffOfUnleashedOcean>(),
        };
        #endregion
        #region NPCs
        public static List<int> Champions = new List<int>
        {
            ModContent.NPCType<CosmosChampion>(),
            ModContent.NPCType<EarthChampion>(),
            ModContent.NPCType<LifeChampion>(),
            ModContent.NPCType<NatureChampion>(),
            ModContent.NPCType<ShadowChampion>(),
            ModContent.NPCType<SpiritChampion>(),
            ModContent.NPCType<TerraChampion>(),
            ModContent.NPCType<TimberChampion>(),
            ModContent.NPCType<WillChampion>()
        };

        public static List<int> SolarEclipseEnemies = new List<int>
        {
            NPCID.Eyezor,
            NPCID.Frankenstein,
            NPCID.SwampThing,
            NPCID.Vampire,
            NPCID.CreatureFromTheDeep,
            NPCID.Fritz,
            NPCID.ThePossessed,
            NPCID.Reaper,
            NPCID.Mothron,
            NPCID.MothronEgg,
            NPCID.MothronSpawn,
            NPCID.Butcher,
            NPCID.DeadlySphere,
            NPCID.DrManFly,
            NPCID.Nailhead,
            NPCID.Psycho
        };
        public static List<int> PumpkinMoonEnemies = new List<int>
        {
            NPCID.Scarecrow1, NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5,
            NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10,
            NPCID.Splinterling,
            NPCID.Hellhound,
            NPCID.Poltergeist,
            NPCID.HeadlessHorseman,
            NPCID.MourningWood,
            NPCID.Pumpking
        };
        public static List<int> FrostMoonEnemies = new List<int>
        {
            NPCID.PresentMimic,
            NPCID.Flocko,
            NPCID.GingerbreadMan,
            NPCID.ZombieElf,NPCID.ZombieElfBeard,NPCID.ZombieElfGirl,
            NPCID.ElfArcher,
            NPCID.Nutcracker,
            NPCID.Yeti,
            NPCID.ElfCopter,
            NPCID.Krampus,
            NPCID.Everscream,
            NPCID.SantaNK1,
            NPCID.IceQueen
        };
        public static List<int> SandstormEnemies = new List<int>
        {
            NPCID.SandElemental,
            NPCID.DuneSplicerHead,
            NPCID.Tumbleweed,
            NPCID.WalkingAntlion,
            NPCID.GiantWalkingAntlion,
            NPCID.FlyingAntlion,
            NPCID.GiantFlyingAntlion,
            NPCID.SandShark, NPCID.SandsharkCorrupt, NPCID.SandsharkCrimson, NPCID.SandsharkHallow
        };
        #endregion
        #region Projectiles
        #endregion
        
    }
}
