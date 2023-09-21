using System.Collections.Generic;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Tools;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Fargowiltas.Items.Tiles;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Souls;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls.Content.Items.Accessories.Forces;
using FargowiltasSouls.Content.Items.Accessories.Masomode;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Materials;
using FargowiltasSouls.Content.Items.Summons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Balance
{
    //for putting mod stuff into souls recipes or vice versa
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalRecipeChanges : ModSystem
    {
        public override void AddRecipes()
        {
            #region QoLRecipes
            Recipe.Create(ModContent.ItemType<OnyxExcavatorKey>())
                .AddIngredient(ModContent.ItemType<Onyxplate>(), 25)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient(ModContent.ItemType<DubiousPlating>(), 20)
                .AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 20)
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ModContent.ItemType<RoverDrive>())
                .AddIngredient(ModContent.ItemType<WulfrumBattery>())
                .AddIngredient(ModContent.ItemType<EnergyCore>(), 2)
                .AddIngredient(ModContent.ItemType<WulfrumMetalScrap>(), 15)
                .AddIngredient(ModContent.ItemType<DubiousPlating>(), 5)
                .AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 5)
                .AddTile(TileID.Anvils)
                .Register();
            //trinket of chi
            Recipe tocrecipe = Recipe.Create(ModContent.ItemType<TrinketofChi>());
            tocrecipe.AddIngredient(ItemID.ClayBlock, 20);
            tocrecipe.AddIngredient(ItemID.Chain, 2);
            tocrecipe.AddIngredient(ItemID.RedHusk);
            tocrecipe.AddTile(TileID.Furnaces);
            tocrecipe.Register();
            //gladiator's locket
            Recipe glrecipe = Recipe.Create(ModContent.ItemType<GladiatorsLocket>());
            glrecipe.AddIngredient(ItemID.Marble, 20);
            glrecipe.AddIngredient(ItemID.LifeCrystal, 2);
            glrecipe.AddIngredient(ItemID.GoldWatch);
            glrecipe.AddTile(TileID.DemonAltar);
            glrecipe.Register();
            Recipe glrecipe2 = Recipe.Create(ModContent.ItemType<GladiatorsLocket>());
            glrecipe2.AddIngredient(ItemID.Marble, 20);
            glrecipe2.AddIngredient(ItemID.LifeCrystal, 2);
            glrecipe2.AddIngredient(ItemID.PlatinumWatch);
            glrecipe2.AddTile(TileID.DemonAltar);
            glrecipe2.Register();
            //granite core recipe
            Recipe ugcrecipe = Recipe.Create(ModContent.ItemType<UnstableGraniteCore>());
            ugcrecipe.AddIngredient(ItemID.Granite, 20);
            ugcrecipe.AddIngredient(ModContent.ItemType<EnergyCore>(), 2);
            ugcrecipe.AddIngredient(ModContent.ItemType<AmidiasSpark>());
            ugcrecipe.AddTile(TileID.DemonAltar);
            ugcrecipe.Register();
            //symbiote recipe
            Recipe fgrecipe = Recipe.Create(ModContent.ItemType<FungalSymbiote>());
            fgrecipe.AddIngredient(ItemID.GlowingMushroom, 20);
            fgrecipe.AddIngredient(ItemID.Acorn, 2);
            fgrecipe.AddIngredient(ItemID.ClayPot);
            fgrecipe.AddTile(TileID.LivingLoom);
            fgrecipe.Register();

            //tundra leash recipe
            Recipe tlrecipe = Recipe.Create(ModContent.ItemType<TundraLeash>());
            tlrecipe.AddRecipeGroup("AnySilverBar", 20);
            tlrecipe.AddIngredient(ItemID.Leather, 2);
            tlrecipe.AddIngredient(ItemID.Bunny);
            tlrecipe.AddTile(TileID.CookingPots);
            tlrecipe.Register();
            //luxor recipe
            Recipe lgrecipe = Recipe.Create(ModContent.ItemType<LuxorsGift>());
            lgrecipe.AddIngredient(ItemID.FossilOre, 20);
            lgrecipe.AddIngredient(ItemID.Ruby, 2);
            lgrecipe.AddIngredient(ModContent.ItemType<ScuttlersJewel>());
            lgrecipe.AddTile(TileID.Anvils);
            lgrecipe.Register();

            //effigies recipes
            Recipe coref = Recipe.Create(ModContent.ItemType<CorruptionEffigy>());
            coref.AddIngredient(ItemID.EbonstoneBlock, 25);
            coref.AddIngredient(ItemID.RottenChunk, 5);
            coref.AddIngredient(ItemID.AngelStatue);
            coref.AddTile(TileID.DemonAltar);
            coref.Register();

            Recipe crief = Recipe.Create(ModContent.ItemType<CrimsonEffigy>());
            crief.AddIngredient(ItemID.CrimstoneBlock, 25);
            crief.AddIngredient(ItemID.Vertebrae, 5);
            crief.AddIngredient(ItemID.AngelStatue);
            crief.AddTile(TileID.DemonAltar);
            crief.Register();

            CreateCalBagRecipes(2489, new[] { ModContent.ItemType<CrownJewel>() }); //slime
            CreateCalBagRecipes(ModContent.ItemType<DesertScourgeBag>(), new[]
            {
                ModContent.ItemType<AquaticDischarge>(),
                ModContent.ItemType<Barinade>(),
                ModContent.ItemType<StormSpray>(),
                ModContent.ItemType<SeaboundStaff>(),
                ModContent.ItemType<ScourgeoftheDesert>(),
                ModContent.ItemType<SandCloak>()
            });
            CreateCalBagRecipes(3319, new[] { ModContent.ItemType<DeathstareRod>() }); //eoc bag
            CreateCalBagRecipes(1360, new[] { ModContent.ItemType<TeardropCleaver>() }); //eoc trophy
            CreateCalBagRecipes(ModContent.ItemType<CrabulonBag>(), new[]
            {
                ModContent.ItemType<MycelialClaws>(),
                ModContent.ItemType<Fungicide>(),
                ModContent.ItemType<HyphaeRod>(),
                ModContent.ItemType<PuffShroom>(),
                ModContent.ItemType<InfestedClawmerang>(),
                ModContent.ItemType<Mycoroot>()
            });
            CreateCalBagRecipes(ModContent.ItemType<HiveMindTrophy>(), new[] { ModContent.ItemType<RottingEyeball>() });
            CreateCalBagRecipes(ModContent.ItemType<PerforatorTrophy>(), new[] { ModContent.ItemType<BloodyVein>() });
            CreateCalBagRecipes(1364, new[] { ModContent.ItemType<TheBee>() }); //bee
            CreateCalBagRecipes(ModContent.ItemType<SlimeGodBag>(), new[]
            {

                ModContent.ItemType<OverloadedBlaster>(),
                ModContent.ItemType<AbyssalTome>(),
                ModContent.ItemType<EldritchTome>(),
                ModContent.ItemType<CrimslimeStaff>(),
                ModContent.ItemType<CorroslimeStaff>()
            });
            CreateCalBagRecipes(3324, new[]
            {
                ModContent.ItemType<Meowthrower>(),
                ModContent.ItemType<BlackHawkRemote>(),
                ModContent.ItemType<BlastBarrel>(),
                ModContent.ItemType<RogueEmblem>()
            }); //wof bag
            CreateCalBagRecipes(1365, new[] { ModContent.ItemType<Carnage>() }); //wof trophy
            CreateCalBagRecipes(ModContent.ItemType<CryogenBag>(), new[]
            {
                ModContent.ItemType<Avalanche>(),
                ModContent.ItemType<HoarfrostBow>(),
                ModContent.ItemType<SnowstormStaff>(),
                ModContent.ItemType<Icebreaker>(),
                ModContent.ItemType<CryoStone>(),
                ModContent.ItemType<FrostFlare>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CryogenTrophy>(), new[] { ModContent.ItemType<GlacialEmbrace>() });
            CreateCalBagRecipes(1368, new[] { ModContent.ItemType<Arbalest>() }); //twins 1
            CreateCalBagRecipes(1369, new[] { ModContent.ItemType<Arbalest>() }); //twins 2
            CreateCalBagRecipes(ModContent.ItemType<AquaticScourgeBag>(), new[]
            {
                ModContent.ItemType<SubmarineShocker>(),
                ModContent.ItemType<Barinautical>(),
                ModContent.ItemType<Downpour>(),
                ModContent.ItemType<DeepseaStaff>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<CorrosiveSpine>()
            });
            CreateCalBagRecipes(ModContent.ItemType<AquaticScourgeTrophy>(), new[]
            {
                ModContent.ItemType<SeasSearing>(),
                ModContent.ItemType<DeepDiver>()
            });
            CreateCalBagRecipes(ModContent.ItemType<BrimstoneWaifuBag>(), new[]
            {
                ModContent.ItemType<Brimlance>(),
                ModContent.ItemType<SeethingDischarge>(),
                ModContent.ItemType<DormantBrimseeker>(),
                ModContent.ItemType<Gehenna>(),
                ModContent.ItemType<Abaddon>(),
                ModContent.ItemType<RoseStone>()
            });
            CreateCalBagRecipes(ModContent.ItemType<BrimstoneElementalTrophy>(), new[]
            {
                ModContent.ItemType<Hellborn>(),
                ModContent.ItemType<FlameLickedShell>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamitasCloneBag>(), new[]
            {
                ModContent.ItemType<Oblivion>(),
                ModContent.ItemType<Animosity>(),
                ModContent.ItemType<HavocsBreath>(),
                ModContent.ItemType<LashesofChaos>(),
                ModContent.ItemType<EntropysVigil>(),
                ModContent.ItemType<CrushsawCrasher>(),
                ModContent.ItemType<ChaosStone>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamitasCloneTrophy>(), new[] { ModContent.ItemType<Regenator>() });
            CreateCalBagRecipes(3328, new[] { ModContent.ItemType<BloomStone>() }); //plant bag
            CreateCalBagRecipes(1370, new[] { ModContent.ItemType<BlossomFlux>() }); //plant trophy
            CreateCalBagRecipes(ModContent.ItemType<LeviathanBag>(), new[]
            {
                ModContent.ItemType<Greentide>(),
                ModContent.ItemType<Leviatitan>(),
                ModContent.ItemType<AnahitasArpeggio>(),
                ModContent.ItemType<Atlantis>(),
                ModContent.ItemType<GastricBelcherStaff>(),
                ModContent.ItemType<BrackishFlask>(),
                ModContent.ItemType<LeviathanTeeth>(),
                ModContent.ItemType<PearlofEnthrallment>()
            });
            CreateCalBagRecipes(ModContent.ItemType<LeviathanTrophy>(), new[] { ModContent.ItemType<TheCommunity>() });
            CreateCalBagRecipes(ModContent.ItemType<AnahitaTrophy>(), new[] { ModContent.ItemType<TheCommunity>() });
            CreateCalBagRecipes(ModContent.ItemType<AstrumAureusBag>(), new[]
            {

                ModContent.ItemType<Nebulash>(),
                ModContent.ItemType<AuroraBlazer>(),
                ModContent.ItemType<AlulaAustralis>(),
                ModContent.ItemType<BorealisBomber>(),
                ModContent.ItemType<AuroradicalThrow>()
            });
            CreateCalBagRecipes(ModContent.ItemType<AstrumAureusTrophy>(), new[] { ModContent.ItemType<LeonidProgenitor>() });
            CreateCalBagRecipes(1371, new[] { ModContent.ItemType<AegisBlade>() }); //golem
            CreateCalBagRecipes(ModContent.ItemType<PlaguebringerGoliathBag>(), new[]
            {
                ModContent.ItemType<Virulence>(),
                ModContent.ItemType<DiseasedPike>(),
                ModContent.ItemType<Pandemic>(),
                ModContent.ItemType<TheHive>(),
                ModContent.ItemType<BlightSpewer>(),
                ModContent.ItemType<Malevolence>(),
                ModContent.ItemType<PestilentDefiler>(),
                ModContent.ItemType<PlagueStaff>(),
                ModContent.ItemType<FuelCellBundle>(),
                ModContent.ItemType<InfectedRemote>(),
                ModContent.ItemType<TheSyringe>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<PlaguebringerGoliathTrophy>(), new[]
            {
                ModContent.ItemType<Malachite>(),
                ModContent.ItemType<PlagueCaller>(),
            });
            CreateCalBagRecipes(3330, new[] { ModContent.ItemType<DukesDecapitator>() }); //fishron bag
            CreateCalBagRecipes(2589, new[] { ModContent.ItemType<BrinyBaron>() }); //fishron trophy
            CreateCalBagRecipes(ModContent.ItemType<RavagerBag>(), new[]
            {
                ModContent.ItemType<UltimusCleaver>(),
                ModContent.ItemType<RealmRavager>(),
                ModContent.ItemType<Hematemesis>(),
                ModContent.ItemType<SpikecragStaff>(),
                ModContent.ItemType<CraniumSmasher>(),
                ModContent.ItemType<BloodPact>(),
                ModContent.ItemType<FleshTotem>()
            });
            CreateCalBagRecipes(ModContent.ItemType<RavagerTrophy>(), new[]
            {
                ModContent.ItemType<Vesuvius>(),
                ModContent.ItemType<CorpusAvertor>()
            });
            CreateCalBagRecipes(ModContent.ItemType<AstrumDeusBag>(), new[]
            {
                ModContent.ItemType<TheMicrowave>(),
                ModContent.ItemType<StarSputter>(),
                ModContent.ItemType<StarShower>(),
                ModContent.ItemType<StarspawnHelixStaff>(),
                ModContent.ItemType<RegulusRiot>(),
                ModContent.ItemType<ChromaticOrb>()
            });
            CreateCalBagRecipes(3332, new[] { ModContent.ItemType<UtensilPoker>() }); //moon lodr
            CreateCalBagRecipes(ModContent.ItemType<ProfanedGuardiansRelic>(), new[]
            {
                ModContent.ItemType<RelicOfDeliverance>(),
                ModContent.ItemType<RelicOfResilience>(),
                ModContent.ItemType<RelicOfConvergence>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<ProfanedGuardianTrophy>(), new[] { ModContent.ItemType<WarbanneroftheSun>() });
            CreateCalBagRecipes(ModContent.ItemType<DragonfollyBag>(), new[]
            {
                ModContent.ItemType<GildedProboscis>(),
                ModContent.ItemType<GoldenEagle>(),
                ModContent.ItemType<RougeSlash>(),
                ModContent.ItemType<FollyFeed>()
            });
            CreateCalBagRecipes(ModContent.ItemType<DragonfollyTrophy>(), new[] { ModContent.ItemType<Swordsplosion>() });
            CreateCalBagRecipes(ModContent.ItemType<ProvidenceBag>(), new[]
            {
                ModContent.ItemType<HolyCollider>(),
                ModContent.ItemType<SolarFlare>(),
                ModContent.ItemType<BlissfulBombardier>(),
                ModContent.ItemType<TelluricGlare>(),
                ModContent.ItemType<PurgeGuzzler>(),
                ModContent.ItemType<DazzlingStabberStaff>(),
                ModContent.ItemType<MoltenAmputator>(),
                ModContent.ItemType<ElysianWings>(),
                ModContent.ItemType<ElysianAegis>()
            });
            CreateCalBagRecipes(ModContent.ItemType<ProvidenceTrophy>(), new[] { ModContent.ItemType<PristineFury>() });
            CreateCalBagRecipes(ModContent.ItemType<StormWeaverBag>(), new[]
            {
                ModContent.ItemType<TheStorm>(),
                ModContent.ItemType<StormDragoon>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<WeaverTrophy>(), new[] { ModContent.ItemType<Thunderstorm>() });
            CreateCalBagRecipes(ModContent.ItemType<CeaselessVoidBag>(), new[]
            {
                ModContent.ItemType<MirrorBlade>(),
                ModContent.ItemType<VoidConcentrationStaff>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<SignusBag>(), new[]
            {
                ModContent.ItemType<Cosmilamp>(),
                ModContent.ItemType<CosmicKunai>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<PolterghastBag>(), new[]
            {
                ModContent.ItemType<TerrorBlade>(),
                ModContent.ItemType<BansheeHook>(),
                ModContent.ItemType<DaemonsFlame>(),
                ModContent.ItemType<FatesReveal>(),
                ModContent.ItemType<GhastlyVisage>(),
                ModContent.ItemType<EtherealSubjugator>(),
                ModContent.ItemType<GhoulishGouger>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<OldDukeBag>(), new[]
            {
                ModContent.ItemType<InsidiousImpaler>(),
                ModContent.ItemType<FetidEmesis>(),
                ModContent.ItemType<SepticSkewer>(),
                ModContent.ItemType<VitriolicViper>(),
                ModContent.ItemType<CadaverousCarrion>(),
                ModContent.ItemType<ToxicantTwister>(),
                ModContent.ItemType<OldDukeScales>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<OldDukeTrophy>(), new[] { ModContent.ItemType<TheOldReaper>() });
            CreateCalBagRecipes(ModContent.ItemType<DevourerofGodsBag>(), new[]
            {
                ModContent.ItemType<Excelsus>(),
                ModContent.ItemType<TheObliterator>(),
                ModContent.ItemType<Deathwind>(),
                ModContent.ItemType<DeathhailStaff>(),
                ModContent.ItemType<StaffoftheMechworm>(),
                ModContent.ItemType<Eradicator>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<DevourerofGodsTrophy>(), new[]
            {
                ModContent.ItemType<CosmicDischarge>(),
                ModContent.ItemType<Norfleet>()
            });
            CreateCalBagRecipes(ModContent.ItemType<YharonBag>(), new[]
            {
                ModContent.ItemType<TheBurningSky>(),
                ModContent.ItemType<DragonRage>(),
                ModContent.ItemType<DragonsBreath>(),
                ModContent.ItemType<ChickenCannon>(),
                ModContent.ItemType<PhoenixFlameBarrage>(),
                ModContent.ItemType<YharonsKindleStaff>(),
                ModContent.ItemType<Wrathwing>(),
                ModContent.ItemType<FinalDawn>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<YharonTrophy>(), new[]
            {
                ModContent.ItemType<YharimsCrystal>(),
                ModContent.ItemType<ForgottenDragonEgg>(),
                ModContent.ItemType<McNuggets>()
            });
            CreateCalBagRecipes(ModContent.ItemType<DraedonBag>(), new[]
            {
                ModContent.ItemType<SpineOfThanatos>(),
                ModContent.ItemType<PhotonRipper>(),
                ModContent.ItemType<TheJailor>(),
                ModContent.ItemType<SurgeDriver>(),
                ModContent.ItemType<AresExoskeleton>(),
                ModContent.ItemType<AtlasMunitionsBeacon>(),
                ModContent.ItemType<TheAtomSplitter>(),
                ModContent.ItemType<RefractionRotor>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamitasCoffer>(), new[]
            {
                ModContent.ItemType<Violence>(),
                ModContent.ItemType<Condemnation>(),
                ModContent.ItemType<Heresy>(),
                ModContent.ItemType<Vehemence>(),
                ModContent.ItemType<Vigilance>(),
                ModContent.ItemType<Perdition>(),
                ModContent.ItemType<Sacrifice>(),

            });

            void CreateCalBagRecipes(int input, int[] outputs)
            {
                for (int i = 0; i < outputs.Length; i++)
                {
                    Recipe.Create(outputs[i]).AddIngredient(input).AddTile(TileID.Solidifier).Register();
                }
            }

            AddBannerToItemRecipe(ItemID.TombCrawlerBanner, ModContent.ItemType<BurntSienna>());
            AddBannerToItemRecipe(ItemID.DemonBanner, ModContent.ItemType<BladecrestOathsword>());
            AddBannerToItemRecipe(ItemID.GoblinSorcererBanner, ModContent.ItemType<PlasmaRod>());
            AddBannerToItemRecipe(ModContent.ItemType<BoxJellyfishBanner>(),
                ModContent.ItemType<AbyssShocker>(), 1, 1, ItemID.Bone);
            AddBannerToItemRecipe(1682, ModContent.ItemType<StaffOfNecrosteocytes>());
            AddBannerToItemRecipe(ModContent.ItemType<NuclearToadBanner>(),
                ModContent.ItemType<CausticCroakerStaff>());
            //hardmode
            AddBannerToItemRecipe(ItemID.MossHornetBanner, ModContent.ItemType<Needler>());
            AddBannerToItemRecipe(ItemID.GiantCursedSkullBanner, ModContent.ItemType<Keelhaul>(), 1, 1,
                ModContent.ItemType<LeviathanAmbergris>());
            AddBannerToItemRecipe(ModContent.ItemType<AcidEelBanner>(),
                ModContent.ItemType<SlitheringEels>(), 1, 1,
                ModContent.ItemType<CorrodedFossil>());
            AddBannerToItemRecipe(ItemID.NecromancerBanner, ModContent.ItemType<WrathoftheAncients>());
            AddBannerToItemRecipe(ModContent.ItemType<OrthoceraBanner>(),
                ModContent.ItemType<OrthoceraShell>());
            AddBannerToItemRecipe(ItemID.DeadlySphereBanner, ModContent.ItemType<DefectiveSphere>());
            AddBannerToItemRecipe(ItemID.ClingerBanner, ModContent.ItemType<CursedDagger>());
            AddBannerToItemRecipe(ItemID.IchorStickerBanner, ModContent.ItemType<IchorSpear>());
            //post-ml
            AddBannerToItemRecipe(ModContent.ItemType<ScryllarBanner>(),
                ModContent.ItemType<GuidelightofOblivion>(), 1, 1,
                ModContent.ItemType<DivineGeode>());
            AddBannerToItemRecipe(ModContent.ItemType<ImpiousImmolatorBanner>(),
                ModContent.ItemType<SanctifiedSpark>());

            void AddBannerToItemRecipe(int banner, int result, int bannerAmount = 1, int resultAmount = 1, int item2type = -1, int item2amount = 1, int tile = 220)
            {
                Recipe bannerRecipe1 = Recipe.Create(result, resultAmount);
                bannerRecipe1.AddIngredient(banner, bannerAmount);
                if (item2type > -1)
                {
                    bannerRecipe1.AddIngredient(item2type, item2amount);
                }
                bannerRecipe1.AddTile(tile);
                bannerRecipe1.Register();
            }
            #endregion QoLRecipes
        }
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
                        recipe.AddIngredient(ModContent.ItemType<TracersSeraph>());
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
                        .AddIngredient(ModContent.ItemType<SilvaWings>())
                        .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);

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
                    recipe
                        .AddIngredient(ModContent.ItemType<Purity>())
                    .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                if (recipe.HasResult(ModContent.ItemType<BerserkerSoul>()) && recipe.HasIngredient(ItemID.StingerNecklace))
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
                if (recipe.HasResult<VagabondsSoul>() && !recipe.HasIngredient<AbomEnergy>())
                    recipe.AddIngredient<AbomEnergy>(10);
                if (recipe.HasResult(ModContent.ItemType<UniverseSoul>()) && !recipe.HasIngredient<VagabondsSoul>())
                {
                    recipe.AddIngredient(ModContent.ItemType<VagabondsSoul>());
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
                        .AddIngredient(ModContent.ItemType<OnyxExcavatorKey>())
                        .AddIngredient(ModContent.ItemType<AbomEnergy>(), 10);
                }
                
                #endregion

                #region Balance and Progression Locks
                if (CalamityConfig.Instance.BalanceRework)
                {
                    if (recipe.HasIngredient<EternalEnergy>() && !recipe.HasIngredient<Rock>())
                    {
                        recipe.AddIngredient<Rock>();
                    }
                    if (recipe.HasResult(ItemID.AnkhShield) && !recipe.HasIngredient(ItemID.SoulofNight))
                    {
                        recipe.AddIngredient(ItemID.SoulofNight, 3);
                    }
                    if (recipe.HasResult(ModContent.ItemType<SigilOfChampions>()) && !recipe.HasIngredient(ModContent.ItemType<DivineGeode>()))
                    {
                        recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 5);
                    }
                    if (recipe.createItem.ModItem is BaseForce && !recipe.HasIngredient(ModContent.ItemType<DivineGeode>()))
                    {
                        recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 4);
                    }
                    if (recipe.HasResult(ModContent.ItemType<AbomsCurse>()) && !recipe.HasIngredient(ModContent.ItemType<AuricBar>()))
                    {
                        recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 2);
                    }
                    List<int> Tier2Souls = new List<int>
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