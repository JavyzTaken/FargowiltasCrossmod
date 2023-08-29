using FargowiltasSouls.Content.Items.Accessories.Masomode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Mounts;
using Terraria.GameContent.ItemDropRules;
using Fargowiltas.Items.Tiles;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Tools;
using Microsoft.CodeAnalysis.Operations;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Summons;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    //for putting mod stuff into souls recipes or vice versa
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalRecipeChanges : ModSystem
    {
        public override void PostAddRecipes()
        {
            //!!! WARNING !!!
            //Make sure condition to go into a recipe change is false if the change already happened !!!
            //else it will cause an infinite loop and game will not load and your computer will be set ablaze
            //Check if the recipe contains one of the ingredients you're adding or removing.


            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                #region Compatibility
                if (recipe.HasResult(ModContent.ItemType<AeolusBoots>()) && recipe.HasIngredient(ItemID.TerrasparkBoots))
                {
                    if (recipe.RemoveIngredient(ItemID.TerrasparkBoots))
                        recipe.AddIngredient(ModContent.ItemType<AngelTreads>());
                    if (recipe.RemoveIngredient(ItemID.SoulofMight) && recipe.RemoveIngredient(ItemID.SoulofSight) && recipe.RemoveIngredient(ItemID.SoulofFright))
                        recipe.AddIngredient(ModContent.ItemType<LivingShard>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<TracersCelestial>()) && recipe.HasIngredient<AngelTreads>())
                {
                    if (recipe.RemoveIngredient(ModContent.ItemType<AngelTreads>()))
                        recipe.AddIngredient(ModContent.ItemType<AeolusBoots>());
                }

                if (recipe.HasResult(ModContent.ItemType<SupersonicSoul>()) && recipe.HasIngredient(ModContent.ItemType<AeolusBoots>()))
                {
                    if (recipe.RemoveIngredient(ModContent.ItemType<AeolusBoots>()))
                        recipe.AddIngredient(ModContent.ItemType<TracersElysian>());
                    if (recipe.RemoveIngredient(ItemID.BundleofBalloons))
                        recipe.AddIngredient(ModContent.ItemType<MOAB>());
                    if (recipe.RemoveIngredient(ItemID.MasterNinjaGear))
                        recipe.AddIngredient(ModContent.ItemType<StatisVoidSash>());
                    if (recipe.RemoveIngredient(ItemID.EoCShield))
                        recipe.AddIngredient(ModContent.ItemType<ShieldoftheHighRuler>());
                    recipe.AddIngredient(ModContent.ItemType<TundraLeash>())
                    .AddIngredient(ModContent.ItemType<FollyFeed>())
                    .AddIngredient(ModContent.ItemType<WulfrumAcrobaticsPack>())
                    .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<FlightMasterySoul>()) && !recipe.HasIngredient(ModContent.ItemType<SkylineWings>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<SkylineWings>())
                        .AddIngredient(ModContent.ItemType<HadarianWings>())
                        .AddIngredient(ModContent.ItemType<TarragonWings>())
                        .AddIngredient(ModContent.ItemType<SilvaWings>());

                }
                if (recipe.HasResult(ModContent.ItemType<ColossusSoul>()) && recipe.HasIngredient(ItemID.WormScarf))
                {
                    if (recipe.RemoveIngredient(ItemID.WormScarf))
                        recipe.AddIngredient(ModContent.ItemType<BloodyWormScarf>());
                    if (recipe.RemoveIngredient(ItemID.BrainOfConfusion))
                        recipe.AddIngredient(ModContent.ItemType<TheAmalgam>());
                    if (recipe.RemoveIngredient(ItemID.AnkhShield))
                        recipe.AddIngredient(ModContent.ItemType<AsgardianAegis>());
                    if (recipe.RemoveIngredient(ItemID.CharmofMyths) && recipe.RemoveIngredient(ItemID.StarVeil) && recipe.RemoveIngredient(ItemID.FrozenShield))
                        recipe.AddIngredient(ModContent.ItemType<RampartofDeities>());
                    recipe.AddIngredient(ModContent.ItemType<TheCamper>())
                    .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<BerserkerSoul>()) && recipe.HasIngredient(ItemID.SharkToothNecklace))
                {
                    if (recipe.RemoveIngredient(ItemID.SharkToothNecklace))
                        recipe.AddIngredient(ModContent.ItemType<ReaperToothNecklace>());
                    if (recipe.RemoveIngredient(ItemID.FireGauntlet))
                        recipe.AddIngredient(ModContent.ItemType<ElementalGauntlet>());
                    recipe.AddIngredient(ModContent.ItemType<BadgeofBravery>())
                            .AddIngredient(ModContent.ItemType<CelestialClaymore>())
                            .AddIngredient(ModContent.ItemType<ScourgeoftheCosmos>())
                            .AddIngredient(ModContent.ItemType<Greentide>())
                            .AddIngredient(ModContent.ItemType<PulseDragon>())
                            .AddIngredient(ModContent.ItemType<DevilsDevastation>())
                            .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<ArchWizardsSoul>()) && recipe.HasIngredient(ItemID.MagnetSphere))
                {
                    if (recipe.RemoveIngredient(ItemID.ArcaneFlower) && recipe.RemoveIngredient(ItemID.ManaCloak) && recipe.RemoveIngredient(ItemID.MagnetFlower) && recipe.RemoveIngredient(ItemID.CelestialEmblem))
                        recipe.AddIngredient(ModContent.ItemType<EtherealTalisman>());
                    if (recipe.RemoveIngredient(ItemID.MagnetSphere))
                        recipe.AddIngredient(ModContent.ItemType<VoltaicClimax>());
                    if (recipe.RemoveIngredient(ItemID.SparkleGuitar))
                        recipe.AddIngredient(ModContent.ItemType<FaceMelter>());
                    if (recipe.RemoveIngredient(ItemID.RazorbladeTyphoon))
                        recipe.AddIngredient(ModContent.ItemType<Atlantis>());
                    if (recipe.RemoveIngredient(ItemID.LaserMachinegun))
                        recipe.AddIngredient(ModContent.ItemType<AlphaRay>());
                    if (recipe.RemoveIngredient(ItemID.LastPrism))
                        recipe.AddIngredient(ModContent.ItemType<DarkSpark>());
                    recipe.AddIngredient(ModContent.ItemType<VitriolicViper>())
                             .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<SnipersSoul>()) && recipe.HasIngredient(ItemID.MoltenQuiver))
                {
                    if (recipe.RemoveIngredient(ItemID.MoltenQuiver) && recipe.RemoveIngredient(ItemID.StalkersQuiver))
                    {
                        recipe.AddIngredient(ModContent.ItemType<ElementalQuiver>());
                        recipe.AddIngredient(ModContent.ItemType<QuiverofNihility>());
                    }
                    if (recipe.RemoveIngredient(ItemID.Megashark))
                        recipe.AddIngredient(ModContent.ItemType<Seadragon>());
                    if (recipe.RemoveIngredient(ItemID.PulseBow))
                        recipe.AddIngredient(ModContent.ItemType<Ultima>());
                    if (recipe.RemoveIngredient(ItemID.PiranhaGun))
                        recipe.AddIngredient(ModContent.ItemType<Starmageddon>());
                    if (recipe.RemoveIngredient(ItemID.SniperRifle))
                        recipe.AddIngredient(ModContent.ItemType<AntiMaterielRifle>());
                    if (recipe.RemoveIngredient(ItemID.Tsunami))
                        recipe.AddIngredient(ModContent.ItemType<Alluvion>());
                    if (recipe.RemoveIngredient(ItemID.Xenopopper))
                        recipe.AddIngredient(ModContent.ItemType<Vortexpopper>());
                    recipe.AddIngredient(ModContent.ItemType<HalleysInferno>());
                    recipe.AddIngredient(ModContent.ItemType<StormDragoon>());
                    recipe.AddIngredient(ModContent.ItemType<PridefulHuntersPlanarRipper>());
                    recipe.AddIngredient(ModContent.ItemType<DaawnlightSpiritOrigin>());
                    recipe.AddIngredient(ModContent.ItemType<DynamoStemCells>());
                    recipe.AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<ConjuristsSoul>()) && recipe.HasIngredient(ItemID.PygmyNecklace))
                {
                    if (recipe.RemoveIngredient(ItemID.PygmyNecklace))
                        recipe.AddIngredient(ModContent.ItemType<Nucleogenesis>());
                    if (recipe.RemoveIngredient(ItemID.Smolstar))
                        recipe.AddIngredient(ModContent.ItemType<PlantationStaff>());
                    if (recipe.RemoveIngredient(ItemID.StaffoftheFrostHydra))
                        recipe.AddIngredient(ModContent.ItemType<EndoHydraStaff>());
                    if (recipe.RemoveIngredient(ItemID.RavenStaff))
                        recipe.AddIngredient(ModContent.ItemType<CorvidHarbringerStaff>());
                    if (recipe.RemoveIngredient(ItemID.XenoStaff))
                        recipe.AddIngredient(ModContent.ItemType<MidnightSunBeacon>());
                    if (recipe.RemoveIngredient(ItemID.EmpressBlade))
                        recipe.AddIngredient(ModContent.ItemType<ElementalAxe>());
                    recipe.AddIngredient(ModContent.ItemType<ResurrectionButterfly>());
                    recipe.AddIngredient(ModContent.ItemType<GlacialEmbrace>());
                    recipe.AddIngredient(ModContent.ItemType<GuidelightofOblivion>());
                    recipe.AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<TrawlerSoul>()) && recipe.HasIngredient(ItemID.ArcticDivingGear))
                {
                    if (recipe.RemoveIngredient(ItemID.ArcticDivingGear))
                    {
                        recipe.AddIngredient(ModContent.ItemType<AbyssalDivingSuit>());
                    }
                    recipe.AddIngredient(ModContent.ItemType<AlluringBait>())
                        .AddIngredient(ModContent.ItemType<EnchantedPearl>())
                        .AddIngredient(ModContent.ItemType<DragoonDrizzlefish>())
                        .AddIngredient(ModContent.ItemType<PolarisParrotfish>())
                        .AddIngredient(ModContent.ItemType<SparklingEmpress>())
                        .AddIngredient(ModContent.ItemType<TheDevourerofCods>())
                        .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<WorldShaperSoul>()) && !recipe.HasIngredient(ModContent.ItemType<BlossomPickaxe>()))
                {
                    recipe.AddIngredient(ModContent.ItemType<BlossomPickaxe>())
                        .AddIngredient(ModContent.ItemType<ArchaicPowder>())
                        .AddIngredient(ModContent.ItemType<SpelunkersAmulet>())
                        .AddIngredient(ModContent.ItemType<OnyxExcavatorKey>());
                }
                #endregion

                #region Balance and Progression Locks
                if (CalamityConfig.Instance.ProgressionChanges)
                {
                    if (recipe.createItem.ModItem is BaseForce && !recipe.HasIngredient(ModContent.ItemType<DivineGeode>()))
                    {
                        recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 4);
                    }
                    if (recipe.HasResult(ModContent.ItemType<AbomsCurse>()) && !recipe.HasIngredient(ModContent.ItemType<AuricBar>()))
                    {
                        recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 2);
                    }
                    List<int> Tier2Souls = new List<int>() 
                    {
                        ModContent.ItemType<TerrariaSoul>(), 
                        ModContent.ItemType<UniverseSoul>(),
                        ModContent.ItemType<DimensionSoul>(),
                        ModContent.ItemType<MasochistSoul>()
                    };
                    if (Tier2Souls.Contains(recipe.createItem.type) && !recipe.HasIngredient(ModContent.ItemType<ShadowspecBar>()))
                    {
                        recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
                        if (recipe.RemoveTile(ModContent.TileType<CrucibleCosmosSheet>()))
                        {
                            recipe.AddTile(ModContent.TileType<DraedonsForge>());
                        }
                    }
                }
                #endregion
            }
        }
    }
}