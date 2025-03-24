using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
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
using FargowiltasSouls.Content.Patreon.DevAesthetic;
using FargowiltasSouls.Content.Patreon.Duck;
using FargowiltasSouls.Content.Patreon.GreatestKraken;
using FargowiltasSouls.Content.Patreon.Sasha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasCrossmod.Core.Common
{
    public class DLCSets : ModSystem
    {
        /// <summary>
        /// Get boolean value, false if set is null. Necessary to alleviate SetDefaultsBeforeLookupsAreBuilt error.
        /// </summary>
        public static bool GetValue(bool[] set, int index) => set != null && set[index]; 
        public class Items
        {
            public static bool[] ChampionTierFargoWeapon;
            public static bool[] AbomTierFargoWeapon;
        }
        public class NPCs
        {
            public static bool[] Champion;
            public static bool[] SolarEclipseEnemy;
            public static bool[] PumpkinMoonEnemy;
            public static bool[] FrostMoonEnemy;
            public static bool[] SandstormEnemy;
        }
        public class Projectiles
        {

        }
        public override void PostSetupContent()
        {
            #region Items
            SetFactory itemFactory = ItemID.Sets.Factory;
            Items.ChampionTierFargoWeapon = itemFactory.CreateBoolSet(false,
                ItemType<EaterLauncher>(),
                ItemType<FleshCannon>(),
                ItemType<HellZone>(),
                ItemType<MechanicalLeashOfCthulhu>(),
                ItemType<SlimeSlingingSlasher>(),
                ItemType<TheBigSting>(),
                ItemType<BigBrainBuster>(),
                ItemType<ScientificRailgun>(),
                ItemType<VortexMagnetRitual>(),
                ItemType<MissDrakovisFishingPole>(),
                ItemType<DeviousAestheticus>()
            );
            Items.AbomTierFargoWeapon = itemFactory.CreateBoolSet(false,
                ItemType<DragonBreath2>(),
                ItemType<DestroyerGun2>(),
                ItemType<GolemTome2>(),
                ItemType<GeminiGlaives>(),
                ItemType<Blender>(),
                ItemType<RefractorBlaster2>(),
                ItemType<NukeFishron>(),
                ItemType<StaffOfUnleashedOcean>(),
                ItemType<TheDestroyer>(),
                ItemType<UmbraRegalia>()
            );
            #endregion

            #region NPCs
            SetFactory npcFactory = NPCID.Sets.Factory;
            NPCs.Champion = npcFactory.CreateBoolSet(false,
                NPCType<CosmosChampion>(),
                NPCType<EarthChampion>(),
                NPCType<LifeChampion>(),
                NPCType<NatureChampion>(),
                NPCType<ShadowChampion>(),
                NPCType<SpiritChampion>(),
                NPCType<TerraChampion>(),
                NPCType<TimberChampion>(),
                NPCType<WillChampion>()
            );
            NPCs.SolarEclipseEnemy = npcFactory.CreateBoolSet(false,
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
            );
            NPCs.PumpkinMoonEnemy = npcFactory.CreateBoolSet(false,
                NPCID.Scarecrow1, NPCID.Scarecrow2, NPCID.Scarecrow3, NPCID.Scarecrow4, NPCID.Scarecrow5,
                NPCID.Scarecrow6, NPCID.Scarecrow7, NPCID.Scarecrow8, NPCID.Scarecrow9, NPCID.Scarecrow10,
                NPCID.Splinterling,
                NPCID.Hellhound,
                NPCID.Poltergeist,
                NPCID.HeadlessHorseman,
                NPCID.MourningWood,
                NPCID.Pumpking
            );
            NPCs.FrostMoonEnemy = npcFactory.CreateBoolSet(false,
                NPCID.PresentMimic,
                NPCID.Flocko,
                NPCID.GingerbreadMan,
                NPCID.ZombieElf, NPCID.ZombieElfBeard, NPCID.ZombieElfGirl,
                NPCID.ElfArcher,
                NPCID.Nutcracker,
                NPCID.Yeti,
                NPCID.ElfCopter,
                NPCID.Krampus,
                NPCID.Everscream,
                NPCID.SantaNK1,
                NPCID.IceQueen
            );
            NPCs.SandstormEnemy = npcFactory.CreateBoolSet(false);
            foreach (int i in DLCLists.SandstormEnemies)
                NPCs.SandstormEnemy[i] = true;
            #endregion
        }
    }
}
