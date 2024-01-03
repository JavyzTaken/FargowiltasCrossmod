using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Tools.ClimateChange;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Fargowiltas.Items.Summons.Abom;
using Fargowiltas.Items.Tiles;
using Fargowiltas.Utilities;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories;
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
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    //for putting mod stuff into souls recipes or vice versa
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalRecipeChanges : ModSystem
    {
        public override void AddRecipes()
        {
            #region QoLRecipes
            #region Custom Recipes
            Recipe.Create(ItemType<OnyxExcavatorKey>())
                .AddIngredient<Onyxplate>(25)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient<DubiousPlating>(20)
                .AddIngredient<MysteriousCircuitry>(20)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ItemType<RoverDrive>())
                .AddIngredient<WulfrumBattery>()
                .AddIngredient<EnergyCore>(2)
                .AddIngredient<WulfrumMetalScrap>(15)
                .AddIngredient<DubiousPlating>(5)
                .AddIngredient<MysteriousCircuitry>(5)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();
            //trinket of chi
            Recipe tocrecipe = Recipe.Create(ItemType<TrinketofChi>());
            tocrecipe.AddIngredient(ItemID.ClayBlock, 50);
            tocrecipe.AddIngredient(ItemID.Chain, 2);
            tocrecipe.AddIngredient(ItemID.RedHusk);
            tocrecipe.DisableDecraft();
            tocrecipe.AddTile(TileID.Furnaces);
            tocrecipe.Register();
            //gladiator's locket
            Recipe glrecipe = Recipe.Create(ItemType<GladiatorsLocket>());
            glrecipe.AddIngredient(ItemID.Marble, 50);
            glrecipe.AddIngredient(ItemID.LifeCrystal, 2);
            glrecipe.AddRecipeGroup("FargowiltasCrossmod:AnyGoldWatch", 1);
            glrecipe.DisableDecraft();
            glrecipe.AddTile(TileID.DemonAltar);
            glrecipe.Register();
            //granite core recipe
            Recipe ugcrecipe = Recipe.Create(ItemType<UnstableGraniteCore>());
            ugcrecipe.AddIngredient(ItemID.Granite, 50);
            ugcrecipe.AddIngredient<EnergyCore>(2);
            ugcrecipe.AddIngredient<AmidiasSpark>();
            ugcrecipe.DisableDecraft();
            ugcrecipe.AddTile(TileID.DemonAltar);
            ugcrecipe.Register();
            //symbiote recipe
            Recipe fgrecipe = Recipe.Create(ItemType<FungalSymbiote>());
            fgrecipe.AddIngredient(ItemID.GlowingMushroom, 50);
            fgrecipe.AddIngredient(ItemID.JungleSpores, 2);
            fgrecipe.AddIngredient(ItemID.TealMushroom, 1);
            fgrecipe.DisableDecraft();
            fgrecipe.AddTile(TileID.LivingLoom);
            fgrecipe.Register();

            //tundra leash recipe
            Recipe tlrecipe = Recipe.Create(ItemType<TundraLeash>());
            tlrecipe.AddRecipeGroup("FargowiltasCrossmod:AnySilverOre", 50);
            tlrecipe.AddIngredient(ItemID.Leather, 2);
            tlrecipe.AddIngredient(ItemID.Bunny);
            tlrecipe.DisableDecraft();
            tlrecipe.AddTile(TileID.CookingPots);
            tlrecipe.Register();
            //luxor recipe
            Recipe lgrecipe = Recipe.Create(ItemType<LuxorsGift>());
            lgrecipe.AddIngredient(ItemID.FossilOre, 50);
            lgrecipe.AddIngredient(ItemID.Ruby, 2);
            lgrecipe.AddIngredient<ScuttlersJewel>();
            lgrecipe.DisableDecraft();
            lgrecipe.AddTile(TileID.Anvils);
            lgrecipe.Register();

            //effigies recipes
            Recipe coref = Recipe.Create(ItemType<CorruptionEffigy>());
            coref.AddIngredient(ItemID.EbonstoneBlock, 75);
            coref.AddIngredient(ItemID.RottenChunk, 5);
            coref.AddIngredient(ItemID.AngelStatue);
            coref.DisableDecraft();
            coref.AddTile(TileID.DemonAltar);
            coref.Register();

            Recipe crief = Recipe.Create(ItemType<CrimsonEffigy>());
            crief.AddIngredient(ItemID.CrimstoneBlock, 75);
            crief.AddIngredient(ItemID.Vertebrae, 5);
            crief.AddIngredient(ItemID.AngelStatue);
            crief.DisableDecraft();
            crief.AddTile(TileID.DemonAltar);
            crief.Register();

            Recipe.Create(ItemType<EvilSmasher>())
                .AddIngredient(ItemID.SoulofNight, 12)
                .AddRecipeGroup("FargowiltasCrossmod:AnyEvilBar", 15)
                .AddTile(TileID.Anvils)
                .DisableDecraft()
                .Register();

            Recipe.Create(ItemType<Murasama>())
                .AddIngredient(ItemID.Muramasa)
                .AddIngredient(ItemType<MartianMemoryStick>()) //memories
                .AddIngredient(ItemID.BrokenHeroSword) //broken
                .AddIngredient(ItemID.PaintingTheTruthIsUpThere) //the truth goes unspoken
                .AddIngredient(ItemType<ForgottenApexWand>()) //i've even forgotten my name
                .AddIngredient(ItemID.PaintingTheSeason) //i don't know the season or what is the reason
                .AddIngredient(ItemID.BladeofGrass) //i'm standing here holding my blade
                .AddIngredient(ItemType<EyeofDesolation>()) //a desolate
                .AddIngredient(ItemID.PlacePainting) //place, without any trace
                .AddIngredient(ItemID.PaintingColdSnap) //it's only the cold
                .AddIngredient(ItemType<WindBlade>()) //wind I feel
                .AddIngredient(ItemType<SpitefulCandle>()) //it's me that i spite
                .AddIngredient(ItemID.DD2ElderCrystalStand) //as i stand up and fight
                .AddIngredient(ItemType<BloodOrb>()) //the only thing i know for real, there will be blood
                .DisableDecraft()
                .AddTile(TileID.BloodMoonMonolith)
                .Register();


            #endregion
            #region Conversion Recipes
            AddConvertRecipe(ItemType<RottenMatter>(), ItemType<BloodSample>());
            AddConvertRecipe(ItemType<FilthyGlove>(), ItemType<BloodstainedGlove>());
            AddConvertRecipe(ItemType<AntiTumorOintment>(), ItemType<AntiCystOintment>());
            AddConvertRecipe(ItemType<Teratoma>(), ItemType<BloodyWormFood>());
            AddConvertRecipe(ItemType<VileFeeder>(), ItemType<ScabRipper>());
            AddConvertRecipe(ItemType<PerfectDark>(), ItemType<VeinBurster>());
            AddConvertRecipe(ItemType<Shadethrower>(), ItemType<Eviscerator>());
            AddConvertRecipe(ItemType<ShaderainStaff>(), ItemType<BloodBath>());
            AddConvertRecipe(ItemType<DankStaff>(), ItemType<FleshOfInfidelity>());
            AddConvertRecipe(ItemType<RotBall>(), ItemType<ToothBall>());
            AddConvertRecipe(ItemType<CursedDagger>(), ItemType<IchorSpear>());
            void AddConvertRecipe(int itemID, int otherItemID)
            {
                RecipeHelper.CreateSimpleRecipe(itemID, otherItemID, TileID.DemonAltar, disableDecraft: true);
                RecipeHelper.CreateSimpleRecipe(otherItemID, itemID, TileID.DemonAltar, disableDecraft: true);
            }
            #endregion
            #region Bag Recipes
            CreateCalBagRecipes(ItemID.KingSlimeTrophy, new[] { ItemType<CrownJewel>() }); //slime
            CreateCalBagRecipes(ItemType<DesertScourgeBag>(), new[]
            {
                ItemType<SaharaSlicers>(),
                ItemType<Barinade>(),
                ItemType<SandstreamScepter>(),
                ItemType<BrittleStarStaff>(),
                ItemType<ScourgeoftheDesert>(),
                ItemType<SandCloak>()
            });
            CreateCalBagRecipes(ItemID.EyeOfCthulhuBossBag, new[] { ItemType<DeathstareRod>() }); //eoc bag
            CreateCalBagRecipes(ItemID.EyeofCthulhuTrophy, new[] { ItemType<TeardropCleaver>() }); //eoc trophy
            CreateCalBagRecipes(ItemType<CrabulonBag>(), new[]
            {
                ItemType<MycelialClaws>(),
                ItemType<Fungicide>(),
                ItemType<HyphaeRod>(),
                ItemType<PuffShroom>(),
                ItemType<InfestedClawmerang>(),
                ItemType<Mycoroot>()
            });
            CreateCalBagRecipes(ItemType<HiveMindTrophy>(), new[] { ItemType<RottingEyeball>() });
            CreateCalBagRecipes(ItemType<PerforatorTrophy>(), new[] { ItemType<BloodyVein>() });
            CreateCalBagRecipes(ItemID.QueenBeeTrophy, new[] { ItemType<TheBee>() }); //bee
            CreateCalBagRecipes(ItemType<SlimeGodBag>(), new[]
            {

                ItemType<OverloadedBlaster>(),
                ItemType<AbyssalTome>(),
                ItemType<EldritchTome>(),
                ItemType<CrimslimeStaff>(),
                ItemType<CorroslimeStaff>()
            });
            CreateCalBagRecipes(ItemID.WallOfFleshBossBag, new[]
            {
                ItemType<Meowthrower>(),
                ItemType<BlackHawkRemote>(),
                ItemType<BlastBarrel>(),
                ItemType<RogueEmblem>()
            }); //wof bag
            CreateCalBagRecipes(ItemID.WallofFleshTrophy, new[] { ItemType<Carnage>() }); //wof trophy
            CreateCalBagRecipes(ItemType<CryogenBag>(), new[]
            {
                ItemType<Avalanche>(),
                ItemType<HoarfrostBow>(),
                ItemType<SnowstormStaff>(),
                ItemType<Icebreaker>(),
                ItemType<CryoStone>(),
                ItemType<FrostFlare>()
            });
            CreateCalBagRecipes(ItemID.SpazmatismTrophy, new[] { ItemType<Arbalest>() }); //twins 1
            CreateCalBagRecipes(ItemID.RetinazerTrophy, new[] { ItemType<Arbalest>() }); //twins 2
            CreateCalBagRecipes(ItemType<AquaticScourgeBag>(), new[]
            {
                ItemType<SubmarineShocker>(),
                ItemType<Barinautical>(),
                ItemType<Downpour>(),
                ItemType<DeepseaStaff>(),
                ItemType<ScourgeoftheSeas>(),
                ItemType<CorrosiveSpine>(),
                ItemType<AeroStone>()
            });
            CreateCalBagRecipes(ItemType<AquaticScourgeTrophy>(), new[]
            {
                ItemType<DeepDiver>()
            });
            CreateCalBagRecipes(ItemType<BrimstoneWaifuBag>(), new[]
            {
                ItemType<Brimlance>(),
                ItemType<SeethingDischarge>(),
                ItemType<DormantBrimseeker>(),
                ItemType<RoseStone>()
            });
            CreateCalBagRecipes(ItemType<CalamitasCloneBag>(), new[]
            {
                ItemType<Oblivion>(),
                ItemType<Animosity>(),
                ItemType<HavocsBreath>(),
                ItemType<LashesofChaos>(),
                ItemType<EntropysVigil>(),
                ItemType<CrushsawCrasher>(),
                ItemType<ChaosStone>()
            });
            CreateCalBagRecipes(ItemType<CalamitasCloneTrophy>(), new[] { ItemType<Regenator>() });
            CreateCalBagRecipes(ItemID.PlanteraBossBag, new[] { ItemType<BloomStone>() }); //plant bag
            CreateCalBagRecipes(ItemID.PlanteraTrophy, new[] { ItemType<BlossomFlux>() }); //plant trophy
            CreateCalBagRecipes(ItemType<LeviathanBag>(), new[]
            {
                ItemType<Greentide>(),
                ItemType<Leviatitan>(),
                ItemType<AnahitasArpeggio>(),
                ItemType<Atlantis>(),
                ItemType<GastricBelcherStaff>(),
                ItemType<BrackishFlask>(),
                ItemType<LeviathanTeeth>(),
                ItemType<PearlofEnthrallment>()
            });
            CreateCalBagRecipes(ItemType<LeviathanTrophy>(), new[] { ItemType<TheCommunity>() });
            CreateCalBagRecipes(ItemType<AnahitaTrophy>(), new[] { ItemType<TheCommunity>() });
            CreateCalBagRecipes(ItemType<AstrumAureusBag>(), new[]
            {

                ItemType<Nebulash>(),
                ItemType<AuroraBlazer>(),
                ItemType<AlulaAustralis>(),
                ItemType<BorealisBomber>(),
                ItemType<AuroradicalThrow>()
            });
            CreateCalBagRecipes(ItemType<AstrumAureusTrophy>(), new[] { ItemType<HeavenfallenStardisk>() }); 
            CreateCalBagRecipes(ItemID.GolemTrophy, new[] { ItemType<AegisBlade>() }); //golem
            CreateCalBagRecipes(ItemType<PlaguebringerGoliathBag>(), new[]
            {
                ItemType<Virulence>(),
                ItemType<DiseasedPike>(),
                ItemType<Pandemic>(),
                ItemType<TheHive>(),
                ItemType<BlightSpewer>(),
                ItemType<Malevolence>(),
                ItemType<PestilentDefiler>(),
                ItemType<PlagueStaff>(),
                ItemType<FuelCellBundle>(),
                ItemType<InfectedRemote>(),
                ItemType<TheSyringe>(),
            });
            CreateCalBagRecipes(ItemType<PlaguebringerGoliathTrophy>(), new[]
            {
                ItemType<PlagueCaller>(),
            });
            CreateCalBagRecipes(ItemID.FishronBossBag, new[] { ItemType<DukesDecapitator>() }); //fishron bag
            CreateCalBagRecipes(ItemID.DukeFishronTrophy, new[] { ItemType<BrinyBaron>() }); //fishron trophy
            CreateCalBagRecipes(ItemType<RavagerBag>(), new[]
            {
                ItemType<UltimusCleaver>(),
                ItemType<RealmRavager>(),
                ItemType<Hematemesis>(),
                ItemType<SpikecragStaff>(),
                ItemType<CraniumSmasher>(),
                ItemType<BloodPact>(),
                ItemType<FleshTotem>()
            });
            CreateCalBagRecipes(ItemType<RavagerTrophy>(), new[]
            {
                ItemType<CorpusAvertor>()
            });
            CreateCalBagRecipes(ItemType<AstrumDeusBag>(), new[]
            {
                ItemType<TheMicrowave>(),
                ItemType<StarSputter>(),
                ItemType<StarShower>(),
                ItemType<StarspawnHelixStaff>(),
                ItemType<RegulusRiot>(),
                ItemType<ChromaticOrb>()
            });
            CreateCalBagRecipes(ItemID.MoonLordBossBag, new[] { ItemType<UtensilPoker>() }); //moon lord
            CreateCalBagRecipes(ItemType<ProfanedGuardiansRelic>(), new[]
            {
                ItemType<RelicOfDeliverance>(),
                ItemType<RelicOfResilience>(),
                ItemType<RelicOfConvergence>(),
            });
            CreateCalBagRecipes(ItemType<ProfanedGuardianTrophy>(), new[] { ItemType<WarbanneroftheSun>() });
            CreateCalBagRecipes(ItemType<DragonfollyBag>(), new[]
            {
                ItemType<GildedProboscis>(),
                ItemType<GoldenEagle>(),
                ItemType<RougeSlash>(),
                ItemType<FollyFeed>()
            });
            CreateCalBagRecipes(ItemType<ProvidenceBag>(), new[]
            {
                ItemType<HolyCollider>(),
                ItemType<SolarFlare>(),
                ItemType<BlissfulBombardier>(),
                ItemType<TelluricGlare>(),
                ItemType<PurgeGuzzler>(),
                ItemType<DazzlingStabberStaff>(),
                ItemType<MoltenAmputator>(),
            });
            CreateCalBagRecipes(ItemType<StormWeaverBag>(), new[]
            {
                ItemType<TheStorm>(),
                ItemType<StormDragoon>(),
                ItemType<LittleLight>()
            });
            CreateCalBagRecipes(ItemType<CeaselessVoidBag>(), new[]
            {
                ItemType<MirrorBlade>(),
                ItemType<VoidConcentrationStaff>(),
            });
            CreateCalBagRecipes(ItemType<SignusBag>(), new[]
            {
                ItemType<Cosmilamp>(),
                ItemType<CosmicKunai>(),
            });
            CreateCalBagRecipes(ItemType<PolterghastBag>(), new[]
            {
                ItemType<TerrorBlade>(),
                ItemType<BansheeHook>(),
                ItemType<DaemonsFlame>(),
                ItemType<FatesReveal>(),
                ItemType<GhastlyVisage>(),
                ItemType<EtherealSubjugator>(),
                ItemType<GhoulishGouger>(),
            });
            CreateCalBagRecipes(ItemType<OldDukeBag>(), new[]
            {
                ItemType<InsidiousImpaler>(),
                ItemType<FetidEmesis>(),
                ItemType<SepticSkewer>(),
                ItemType<VitriolicViper>(),
                ItemType<CadaverousCarrion>(),
                ItemType<ToxicantTwister>(),
                ItemType<MutatedTruffle>()
            });
            CreateCalBagRecipes(ItemType<DevourerofGodsBag>(), new[]
            {
                ItemType<Excelsus>(),
                ItemType<TheObliterator>(),
                ItemType<Deathwind>(),
                ItemType<DeathhailStaff>(),
                ItemType<StaffoftheMechworm>(),
                ItemType<Eradicator>(),
            });
            CreateCalBagRecipes(ItemType<DevourerofGodsTrophy>(), new[]
            {
                ItemType<Norfleet>()
            });
            CreateCalBagRecipes(ItemType<YharonBag>(), new[]
            {
                ItemType<TheBurningSky>(),
                ItemType<DragonRage>(),
                ItemType<DragonsBreath>(),
                ItemType<ChickenCannon>(),
                ItemType<PhoenixFlameBarrage>(),
                ItemType<YharonsKindleStaff>(),
                ItemType<Wrathwing>(),
                ItemType<FinalDawn>(),
            });
            CreateCalBagRecipes(ItemType<YharonTrophy>(), new[]
            {
                ItemType<ForgottenDragonEgg>(),
                ItemType<McNuggets>()
            });
            CreateCalBagRecipes(ItemType<DraedonBag>(), new[]
            {
                ItemType<SpineOfThanatos>(),
                ItemType<PhotonRipper>(),
                ItemType<TheJailor>(),
                ItemType<SurgeDriver>(),
                ItemType<AresExoskeleton>(),
                ItemType<AtlasMunitionsBeacon>(),
                ItemType<TheAtomSplitter>(),
                ItemType<RefractionRotor>(),
            });
            CreateCalBagRecipes(ItemType<CalamitasCoffer>(), new[]
            {
                ItemType<Violence>(),
                ItemType<Condemnation>(),
                ItemType<Heresy>(),
                ItemType<Vehemence>(),
                ItemType<Vigilance>(),
                ItemType<Perdition>(),
                ItemType<Sacrifice>(),

            });
            CreateCalBagRecipes(ItemID.MartianSaucerTrophy, new[]
            {
                ItemType<NullificationRifle>()
            });
            CreateCalBagRecipes(ItemType<MaulerTrophy>(), new[]
            {
                ItemType<SulphuricAcidCannon>()
            });
            void CreateCalBagRecipes(int input, int[] outputs)
            {
                for (int i = 0; i < outputs.Length; i++)
                {
                    Recipe.Create(outputs[i]).AddIngredient(input).AddTile(TileID.Solidifier).DisableDecraft().Register();
                }
            }
            #endregion
            #region Banner Recipes

            //additions to vanilla drops
            AddBannerToItemRecipe(ItemType<StormlionBanner>(), ItemID.ThunderSpear);
            AddBannerToItemRecipe(ItemType<StormlionBanner>(), ItemID.ThunderStaff);
            AddBannerToItemRecipe(ItemType<BoxJellyfishBanner>(), ItemID.JellyfishNecklace);
            AddBannerToItemRecipe(ItemType<GhostBellBanner>(), ItemID.JellyfishNecklace);
            AddBannerToItemRecipe(ItemType<CannonballJellyfishBanner>(), ItemID.JellyfishNecklace);
            AddBannerToItemRecipe(ItemType<MorayEelBanner>(), ItemID.Flipper);
            AddBannerToItemRecipe(ItemID.SharkBanner, ItemID.SharkToothNecklace);
            AddBannerToItemRecipe(ItemType<ToxicatfishBanner>(), ItemID.DivingHelmet);
            AddBannerToItemRecipe(ItemType<TrasherBanner>(), ItemID.DivingHelmet);
            AddBannerToItemRecipe(ItemType<RotdogBanner>(), ItemID.AdhesiveBandage);

            //prehardmode
            AddBannerToItemRecipe(ItemType<CuttlefishBanner>(),
                ItemType<InkBomb>());
            AddBannerToItemRecipe(ItemID.HarpyBanner, ItemType<CocosFeather>(), 5, 1);
            AddBannerToItemRecipe(ItemID.TombCrawlerBanner, ItemType<BurntSienna>());
            AddBannerToItemRecipe(ItemID.DemonBanner, ItemType<BladecrestOathsword>());
            AddBannerToItemRecipe(ItemID.GoblinSorcererBanner, ItemType<PlasmaRod>());
            AddBannerToItemRecipe(ItemType<BoxJellyfishBanner>(),
                ItemType<AbyssShocker>(), 1, 1, ItemID.Bone);
            AddBannerToItemRecipe(ItemType<NuclearToadBanner>(),
                ItemType<CausticCroakerStaff>());

            //hardmode
            AddBannerToItemRecipe(ItemType<ShockstormShuttleBanner>(),
                ItemType<OracleHeadphones>());
            AddBannerToItemRecipe(ItemID.MossHornetBanner, ItemType<Needler>());
            AddBannerToItemRecipe(ItemType<FlakCrabBanner>(),
                ItemType<FlakToxicannon>());
            AddBannerToItemRecipe(ItemID.GiantCursedSkullBanner, ItemType<Keelhaul>(), 1, 1,
                ItemType<LeviathanAmbergris>());
            AddBannerToItemRecipe(ItemType<AcidEelBanner>(),
                ItemType<SlitheringEels>(), 1, 1,
                ItemType<CorrodedFossil>());
            AddBannerToItemRecipe(ItemType<OrthoceraBanner>(),
                ItemType<OrthoceraShell>());
            AddBannerToItemRecipe(ItemType<SkyfinBanner>(),
                ItemType<SkyfinBombers>(), 1, 1,
                ItemType<CorrodedFossil>());
            AddBannerToItemRecipe(ItemID.NecromancerBanner, ItemType<WrathoftheAncients>());
            AddBannerToItemRecipe(ItemID.DeadlySphereBanner, ItemType<DefectiveSphere>());
            AddBannerToItemRecipe(ItemID.ClingerBanner, ItemType<CursedDagger>());
            AddBannerToItemRecipe(ItemID.IchorStickerBanner, ItemType<IchorSpear>());

            //post-ml
            AddBannerToItemRecipe(ItemType<ImpiousImmolatorBanner>(),
                ItemType<BlasphemousDonut>());
            AddBannerToItemRecipe(ItemType<ScryllarBanner>(),
                ItemType<GuidelightofOblivion>(), 1, 1,
                ItemType<DivineGeode>());
            AddBannerToItemRecipe(ItemType<ImpiousImmolatorBanner>(),
                ItemType<SanctifiedSpark>());

            void AddBannerToItemRecipe(int banner, int result, int bannerAmount = 1, int resultAmount = 1, int item2type = -1, int item2amount = 1, int tile = 220)
            {
                Recipe bannerRecipe1 = Recipe.Create(result, resultAmount);
                bannerRecipe1.AddIngredient(banner, bannerAmount);
                if (item2type > -1)
                {
                    bannerRecipe1.AddIngredient(item2type, item2amount);
                }
                bannerRecipe1.DisableDecraft();
                bannerRecipe1.AddTile(tile);
                bannerRecipe1.Register();
            }
            #endregion
            #region Crate Recipes
            CreateCrateRecipes(new int[]
            {
                ItemType<BallOFugu>(),
                ItemType<Archerfish>(),
                ItemType<BlackAnurian>(),
                ItemType<HerringStaff>(),
                ItemType<Lionfish>(),
            }, ItemType<SulphurousCrate>(), 5);

            CreateCrateRecipes(new int[]
            {
                ItemType<StrangeOrb>(),
                ItemType<TorrentialTear>(),
                ItemType<DepthCharm>(),
                ItemType<IronBoots>(),
                ItemType<AnechoicPlating>(),
            }, ItemType<SulphurousCrate>(), 3);

            CreateCrateRecipes(new int[]
            {
                ItemType<SkyfinBombers>(),
                ItemType<NuclearFuelRod>(),
                ItemType<SulphurousGrabber>(),
                ItemType<FlakToxicannon>(),
                ItemType<SpentFuelContainer>(),
                ItemType<SlitheringEels>(),
                ItemType<BelchingSaxophone>(),
            }, ItemType<SulphurousCrate>(), 5, extraItem: ItemType<CorrodedFossil>());

            CreateCrateRecipes(new int[]
            {
                ItemType<StellarKnife>(),
                ItemType<AstralachneaStaff>(),
                ItemType<TitanArm>(),
                ItemType<HivePod>(),
                ItemType<AstralScythe>(),
                ItemType<StellarCannon>(),
                ItemType<StarbusterCore>(),
            }, ItemType<AstralCrate>(), 5);

            CreateCrateRecipes(new int[]
            {
                ItemType<Poseidon>(),
                ItemType<ClamorRifle>(),
                ItemType<ShellfishStaff>(),
                ItemType<ClamCrusher>(),
            }, ItemType<SunkenCrate>(), 5, extraItem: ItemType<MolluskHusk>());

            void CreateCrateRecipes(int[] results, int crate, int crateAmount, int hardmodeCrate = -1, int extraItem = -1)
            {
                foreach (int result in results)
                {
                    if (crate != -1)
                    {
                        var recipe = Recipe.Create(result);
                        recipe.AddIngredient(crate, crateAmount);
                        if (extraItem != -1)
                        {
                            recipe.AddIngredient(extraItem);
                        }
                        recipe.AddTile(TileID.WorkBenches);
                        recipe.DisableDecraft();
                        recipe.Register();
                    }
                }
                
            }
            #endregion
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
                #region SummonRecipeNerfs
                if (recipe.HasResult<DesertMedallion>())
                {
                    recipe.ChangeIngredientStack(ItemType<StormlionMandible>(), 1);
                    recipe.ChangeIngredientStack(ItemID.SandBlock, 10);
                    recipe.ChangeIngredientStack(ItemID.AntlionMandible, 2);
                }
                if (recipe.HasResult<DecapoditaSprout>())
                {
                    recipe.ChangeIngredientStack(ItemID.GlowingMushroom, 25);
                }
                if (recipe.HasResult<OverloadedSludge>())
                {
                    recipe.ChangeIngredientStack(ItemType<BlightedGel>(), 15);
                }
                if (recipe.HasResult<CryoKey>())
                {
                    recipe.ChangeIngredientStack(ItemType<EssenceofEleum>(), 4);
                    recipe.ChangeIngredientStack(ItemID.SoulofNight, 2);
                    recipe.ChangeIngredientStack(ItemID.SoulofLight, 2);
                }
                if (recipe.HasResult<Seafood>())
                {
                    recipe.ChangeIngredientStack(ItemID.Starfish, 5);
                    recipe.ChangeIngredientStack(ItemID.SharkFin, 1);
                }
                if (recipe.HasResult<AstralChunk>())
                {
                    recipe.ChangeIngredientStack(ItemType<Stardust>(), 15);
                    recipe.ChangeIngredientStack(ItemID.FallenStar, 7);
                }
                if (recipe.HasResult<DeathWhistle>())
                {
                    recipe.ChangeIngredientStack(ItemID.LunarTabletFragment, 6);
                }
                if (recipe.HasResult<ProfanedShard>())
                {
                    recipe.ChangeIngredientStack(ItemType<UnholyEssence>(), 15);
                }
                if (recipe.HasResult<RuneofKos>())
                {
                    recipe.ChangeIngredientStack(ItemType<UnholyEssence>(), 20);
                }
                if (recipe.HasResult<NecroplasmicBeacon>())
                {
                    recipe.ChangeIngredientStack(ItemType<Polterplasm>(), 15);
                }
                if (recipe.HasResult<CosmicWorm>())
                {
                    recipe.ChangeIngredientStack(ItemType<Polterplasm>(), 20);
                    recipe.ChangeIngredientStack(ItemID.LunarBar, 20);
                }
                if (recipe.HasResult<YharonEgg>())
                {
                    recipe.ChangeIngredientStack(ItemType<LifeAlloy>(), 5);
                }

                #endregion SummonRecipeNerfs
                #region Compatibility
                if (recipe.HasResult<AngelTreads>() && recipe.HasIngredient(ItemID.TerrasparkBoots))
                {
                    if (recipe.RemoveIngredient(ItemID.TerrasparkBoots))
                    {
                        recipe.AddIngredient<ZephyrBoots>();
                    }
                }
                if (recipe.HasResult<AeolusBoots>() && recipe.HasIngredient<ZephyrBoots>())
                {
                    if (recipe.RemoveIngredient(ItemType<ZephyrBoots>()))
                        recipe.AddIngredient<AngelTreads>();
                    if (recipe.RemoveIngredient(ItemID.SoulofMight) && recipe.RemoveIngredient(ItemID.SoulofSight) && recipe.RemoveIngredient(ItemID.SoulofFright))
                        recipe.AddIngredient<LivingShard>(10);
                }
                if (recipe.HasResult<TracersCelestial>() && recipe.HasIngredient<AngelTreads>())
                {
                    if (recipe.RemoveIngredient(ItemType<AngelTreads>()))
                        recipe.AddIngredient<AeolusBoots>();
                }

                if (recipe.HasResult<SupersonicSoul>() && recipe.HasIngredient<AeolusBoots>())
                {
                    if (recipe.RemoveIngredient(ItemType<AeolusBoots>()))
                        recipe.AddIngredient<TracersSeraph>();
                    if (recipe.RemoveIngredient(ItemID.BundleofBalloons))
                        recipe.AddIngredient<MOAB>();
                    if (recipe.RemoveIngredient(ItemID.MasterNinjaGear))
                        recipe.AddIngredient<StatisVoidSash>();
                    if (recipe.RemoveIngredient(ItemID.EoCShield))
                        recipe.AddIngredient<ShieldoftheHighRuler>();
                    recipe.AddIngredient<TundraLeash>()
                    .AddIngredient<FollyFeed>()
                    .AddIngredient<WulfrumAcrobaticsPack>()
                    .AddIngredient<AuricBar>(10);
                }
                if (recipe.HasResult<FlightMasterySoul>() && !recipe.HasIngredient<SkylineWings>())
                {
                    recipe.AddIngredient<SkylineWings>()
                        .AddIngredient<HadarianWings>()
                        .AddIngredient<TarragonWings>()
                        .AddIngredient<SilvaWings>()
                        .AddIngredient<AuricBar>(10);

                }
                if (recipe.HasResult<ColossusSoul>() && recipe.HasIngredient(ItemID.WormScarf))
                {
                    if (recipe.RemoveIngredient(ItemID.WormScarf))
                        recipe.AddIngredient<BloodyWormScarf>();
                    if (recipe.RemoveIngredient(ItemID.BrainOfConfusion))
                        recipe.AddIngredient<TheAmalgam>();
                    if (recipe.RemoveIngredient(ItemID.AnkhShield))
                        recipe.AddIngredient<AsgardianAegis>();
                    if (recipe.RemoveIngredient(ItemID.CharmofMyths) && recipe.RemoveIngredient(ItemID.StarVeil) && recipe.RemoveIngredient(ItemID.FrozenShield))
                        recipe.AddIngredient<RampartofDeities>();
                    recipe.AddIngredient<AbomEnergy>(10);
                }
                if (recipe.HasResult<BerserkerSoul>() && recipe.HasIngredient(ItemID.StingerNecklace))
                {
                    if (recipe.RemoveIngredient(ItemID.StingerNecklace))
                        recipe.AddIngredient<ReaperToothNecklace>();
                    if (recipe.RemoveIngredient(ItemID.FireGauntlet))
                        recipe.AddIngredient<ElementalGauntlet>();
                    recipe.AddIngredient<BadgeofBravery>()
                            .AddIngredient<CelestialClaymore>()
                            .AddIngredient<Greentide>()
                            .AddIngredient<PulseDragon>()
                            .AddIngredient<DevilsDevastation>()
                            .AddIngredient<AbomEnergy>(10);
                }
                if (recipe.HasResult<ArchWizardsSoul>() && recipe.HasIngredient(ItemID.MagnetSphere))
                {
                    if (recipe.RemoveIngredient(ItemID.ArcaneFlower) && recipe.RemoveIngredient(ItemID.ManaCloak) && recipe.RemoveIngredient(ItemID.MagnetFlower) && recipe.RemoveIngredient(ItemID.CelestialEmblem))
                        recipe.AddIngredient<EtherealTalisman>();
                    if (recipe.RemoveIngredient(ItemID.MagnetSphere))
                        recipe.AddIngredient<VoltaicClimax>();
                    if (recipe.RemoveIngredient(ItemID.SparkleGuitar))
                        recipe.AddIngredient<FaceMelter>();
                    if (recipe.RemoveIngredient(ItemID.RazorbladeTyphoon))
                        recipe.AddIngredient<Atlantis>();
                    if (recipe.RemoveIngredient(ItemID.LaserMachinegun))
                        recipe.AddIngredient<AlphaRay>();
                    if (recipe.RemoveIngredient(ItemID.LastPrism))
                        recipe.AddIngredient<DarkSpark>();
                    recipe.AddIngredient<VitriolicViper>()
                             .AddIngredient<AbomEnergy>(10);
                }
                if (recipe.HasResult(ItemType<SnipersSoul>()) && recipe.HasIngredient(ItemID.MoltenQuiver))
                {
                    if (recipe.RemoveIngredient(ItemID.MoltenQuiver) && recipe.RemoveIngredient(ItemID.StalkersQuiver))
                    {
                        recipe.AddIngredient<ElementalQuiver>();
                        recipe.AddIngredient<QuiverofNihility>();
                    }
                    if (recipe.RemoveIngredient(ItemID.Megashark))
                        recipe.AddIngredient<Seadragon>();
                    if (recipe.RemoveIngredient(ItemID.PulseBow))
                        recipe.AddIngredient<Ultima>();
                    if (recipe.RemoveIngredient(ItemID.PiranhaGun))
                        recipe.AddIngredient<Starmageddon>();
                    if (recipe.RemoveIngredient(ItemID.SniperRifle))
                        recipe.AddIngredient<AntiMaterielRifle>();
                    if (recipe.RemoveIngredient(ItemID.Tsunami))
                        recipe.AddIngredient<Alluvion>();
                    if (recipe.RemoveIngredient(ItemID.Xenopopper))
                        recipe.AddIngredient<Vortexpopper>();
                    recipe.AddIngredient<HalleysInferno>();
                    recipe.AddIngredient<StormDragoon>();
                    recipe.AddIngredient<PridefulHuntersPlanarRipper>();
                    recipe.AddIngredient<DynamoStemCells>();
                    recipe.AddIngredient<AbomEnergy>(10);
                }
                if (recipe.HasResult(ItemType<ConjuristsSoul>()) && recipe.HasIngredient(ItemID.PygmyNecklace))
                {
                    if (recipe.RemoveIngredient(ItemID.PygmyNecklace))
                        recipe.AddIngredient<Nucleogenesis>();

                    recipe.RemoveIngredient(ItemID.Smolstar);

                    if (recipe.RemoveIngredient(ItemID.StaffoftheFrostHydra))
                        recipe.AddIngredient<EndoHydraStaff>();
                    if (recipe.RemoveIngredient(ItemID.RavenStaff))
                        recipe.AddIngredient<CorvidHarbringerStaff>();
                    if (recipe.RemoveIngredient(ItemID.XenoStaff))
                        recipe.AddIngredient<MidnightSunBeacon>();
                    if (recipe.RemoveIngredient(ItemID.EmpressBlade))
                        recipe.AddIngredient<ElementalAxe>();
                    recipe.AddIngredient<ResurrectionButterfly>();
                    recipe.AddIngredient<GlacialEmbrace>();
                    recipe.AddIngredient<GuidelightofOblivion>();
                    recipe.AddIngredient<AbomEnergy>(10);
                }
                if (recipe.HasResult<VagabondsSoul>() && !recipe.HasIngredient<AbomEnergy>())
                    recipe.AddIngredient<AbomEnergy>(10);
                if (recipe.HasResult(ItemType<UniverseSoul>()) && !recipe.HasIngredient<VagabondsSoul>())
                {
                    recipe.AddIngredient<VagabondsSoul>();
                }
                if (recipe.HasResult<SupremeBaitTackleBoxFishingStation>() && recipe.HasIngredient(ItemID.LavaproofTackleBag))
                {
                    if (recipe.RemoveIngredient(ItemID.LavaproofTackleBag))
                    {
                        recipe.AddIngredient<AlluringBait>().AddIngredient<EnchantedPearl>();
                    }
                }
                if (recipe.HasResult(ItemType<TrawlerSoul>()) && recipe.HasIngredient(ItemID.ArcticDivingGear))
                {
                    if (recipe.RemoveIngredient(ItemID.ArcticDivingGear))
                    {
                        recipe.AddIngredient<AbyssalDivingSuit>();
                    }
                    recipe.AddIngredient<SupremeBaitTackleBoxFishingStation>()
                        .AddIngredient<DragoonDrizzlefish>()
                        .AddIngredient<PolarisParrotfish>()
                        .AddIngredient<SparklingEmpress>()
                        .AddIngredient<TheDevourerofCods>()
                        .AddIngredient<AbomEnergy>(10);
                }
                if (recipe.HasResult(ItemType<WorldShaperSoul>()) && !recipe.HasIngredient<BlossomPickaxe>())
                {
                    recipe.AddIngredient<BlossomPickaxe>()
                        .AddIngredient<ArchaicPowder>()
                        .AddIngredient<SpelunkersAmulet>()
                        .AddIngredient<OnyxExcavatorKey>()
                        .AddIngredient<AbomEnergy>(10);
                }

                if (recipe.HasResult<EternitySoul>() && !recipe.HasIngredient<BrandoftheBrimstoneWitch>())
                {
                    recipe.AddIngredient<BrandoftheBrimstoneWitch>();
                }
                if (recipe.HasIngredient(ItemID.BloodMoonStarter) && recipe.HasResult(ItemType<BloodOrb>())) // bloody tear -> blood orb disabled because abom
                {
                    recipe.DisableRecipe();
                }

                #endregion

                #region Balance and Progression Locks
                if (DLCCalamityConfig.Instance.BalanceRework)
                {
                    if (recipe.HasIngredient<EternalEnergy>() && recipe.createItem != null && CalamityContentLists.RockItems.Contains(recipe.createItem.type) && !recipe.HasIngredient<Rock>())
                    {
                        recipe.AddIngredient<Rock>();
                    }
                    if (recipe.HasResult(ItemID.AnkhShield) && !recipe.HasIngredient(ItemID.SoulofNight))
                    {
                        recipe.AddIngredient(ItemID.SoulofNight, 3);
                    }
                    if (recipe.HasResult(ItemType<SigilOfChampions>()) && !recipe.HasIngredient<DivineGeode>())
                    {
                        recipe.AddIngredient<DivineGeode>(5);
                    }
                    if (recipe.createItem.ModItem is BaseForce && !recipe.HasIngredient<DivineGeode>())
                    {
                        recipe.AddIngredient<DivineGeode>(4);
                    }
                    if (recipe.HasResult<AbomsCurse>() && !recipe.HasIngredient<AuricBar>())
                    {
                        recipe.AddIngredient<AuricBar>(2);
                    }
                    List<int> Tier2Souls = new List<int>
                    {
                        ItemType<TerrariaSoul>(),
                        ItemType<UniverseSoul>(),
                        ItemType<DimensionSoul>(),
                        ItemType<MasochistSoul>()
                    };

                    if (Tier2Souls.Contains(recipe.createItem.type) && !recipe.HasIngredient(ItemType<ShadowspecBar>()))
                    {
                        recipe.AddIngredient<ShadowspecBar>(5);
                        if (recipe.RemoveTile(TileType<CrucibleCosmosSheet>()))
                        {
                            recipe.AddTile<DraedonsForge>();
                        }
                    }
                }
                #endregion
            }
        }
        public override void AddRecipeGroups()
        {
            #region RecipeGroups
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyGoldWatch", 
                new(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Gold Watch"}",
                ItemID.GoldWatch,
                ItemID.PlatinumWatch
                ));

            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnySilverOre", 
                new(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Silver Ore"}",
                ItemID.SilverOre,
                ItemID.TungstenOre
                ));

            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyGildedDagger", 
                new(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Gilded Dagger"}",
               ItemType<GildedDagger>(),
               ItemType<GleamingDagger>()
               ));

            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyEvilBar", 
                new(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Evil Bar"}",
                ItemID.DemoniteBar,
                ItemID.CrimtaneBar
                ));
            #endregion
        }
    }
}