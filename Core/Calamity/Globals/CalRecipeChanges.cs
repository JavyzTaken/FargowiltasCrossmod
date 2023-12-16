using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
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
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
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

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    //for putting mod stuff into souls recipes or vice versa
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalRecipeChanges : ModSystem
    {
        public override void AddRecipes()
        {
            #region QoLRecipes
            Recipe.Create(ModContent.ItemType<OnyxExcavatorKey>())
                .AddIngredient<Onyxplate>(25)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient<DubiousPlating>(20)
                .AddIngredient<MysteriousCircuitry>(20)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();
            Recipe.Create(ModContent.ItemType<RoverDrive>())
                .AddIngredient<WulfrumBattery>()
                .AddIngredient<EnergyCore>(2)
                .AddIngredient<WulfrumMetalScrap>(15)
                .AddIngredient<DubiousPlating>(5)
                .AddIngredient<MysteriousCircuitry>(5)
                .DisableDecraft()
                .AddTile(TileID.Anvils)
                .Register();
            //trinket of chi
            Recipe tocrecipe = Recipe.Create(ModContent.ItemType<TrinketofChi>());
            tocrecipe.AddIngredient(ItemID.ClayBlock, 50);
            tocrecipe.AddIngredient(ItemID.Chain, 2);
            tocrecipe.AddIngredient(ItemID.RedHusk);
            tocrecipe.DisableDecraft();
            tocrecipe.AddTile(TileID.Furnaces);
            tocrecipe.Register();
            //gladiator's locket
            Recipe glrecipe = Recipe.Create(ModContent.ItemType<GladiatorsLocket>());
            glrecipe.AddIngredient(ItemID.Marble, 50);
            glrecipe.AddIngredient(ItemID.LifeCrystal, 2);
            glrecipe.AddRecipeGroup("FargowiltasCrossmod:AnyGoldWatch", 1);
            glrecipe.DisableDecraft();
            glrecipe.AddTile(TileID.DemonAltar);
            glrecipe.Register();
            //granite core recipe
            Recipe ugcrecipe = Recipe.Create(ModContent.ItemType<UnstableGraniteCore>());
            ugcrecipe.AddIngredient(ItemID.Granite, 50);
            ugcrecipe.AddIngredient<EnergyCore>(2);
            ugcrecipe.AddIngredient<AmidiasSpark>();
            ugcrecipe.DisableDecraft();
            ugcrecipe.AddTile(TileID.DemonAltar);
            ugcrecipe.Register();
            //symbiote recipe
            Recipe fgrecipe = Recipe.Create(ModContent.ItemType<FungalSymbiote>());
            fgrecipe.AddIngredient(ItemID.GlowingMushroom, 50);
            fgrecipe.AddIngredient(ItemID.JungleSpores, 2);
            fgrecipe.AddIngredient(ItemID.TealMushroom, 1);
            fgrecipe.DisableDecraft();
            fgrecipe.AddTile(TileID.LivingLoom);
            fgrecipe.Register();

            //tundra leash recipe
            Recipe tlrecipe = Recipe.Create(ModContent.ItemType<TundraLeash>());
            tlrecipe.AddRecipeGroup("FargowiltasCrossmod:AnySilverOre", 50);
            tlrecipe.AddIngredient(ItemID.Leather, 2);
            tlrecipe.AddIngredient(ItemID.Bunny);
            tlrecipe.DisableDecraft();
            tlrecipe.AddTile(TileID.CookingPots);
            tlrecipe.Register();
            //luxor recipe
            Recipe lgrecipe = Recipe.Create(ModContent.ItemType<LuxorsGift>());
            lgrecipe.AddIngredient(ItemID.FossilOre, 50);
            lgrecipe.AddIngredient(ItemID.Ruby, 2);
            lgrecipe.AddIngredient<ScuttlersJewel>();
            lgrecipe.DisableDecraft();
            lgrecipe.AddTile(TileID.Anvils);
            lgrecipe.Register();

            //effigies recipes
            Recipe coref = Recipe.Create(ModContent.ItemType<CorruptionEffigy>());
            coref.AddIngredient(ItemID.EbonstoneBlock, 75);
            coref.AddIngredient(ItemID.RottenChunk, 5);
            coref.AddIngredient(ItemID.AngelStatue);
            coref.DisableDecraft();
            coref.AddTile(TileID.DemonAltar);
            coref.Register();

            Recipe crief = Recipe.Create(ModContent.ItemType<CrimsonEffigy>());
            crief.AddIngredient(ItemID.CrimstoneBlock, 75);
            crief.AddIngredient(ItemID.Vertebrae, 5);
            crief.AddIngredient(ItemID.AngelStatue);
            crief.DisableDecraft();
            crief.AddTile(TileID.DemonAltar);
            crief.Register();

            CreateCalBagRecipes(ItemID.KingSlimeTrophy, new[] { ModContent.ItemType<CrownJewel>() }); //slime
            CreateCalBagRecipes(ModContent.ItemType<DesertScourgeBag>(), new[]
            {
                ModContent.ItemType<SaharaSlicers>(),
                ModContent.ItemType<Barinade>(),
                ModContent.ItemType<SandstreamScepter>(),
                ModContent.ItemType<BrittleStarStaff>(),
                ModContent.ItemType<ScourgeoftheDesert>(),
                ModContent.ItemType<SandCloak>()
            });
            CreateCalBagRecipes(ItemID.EyeOfCthulhuBossBag, new[] { ModContent.ItemType<DeathstareRod>() }); //eoc bag
            CreateCalBagRecipes(ItemID.EyeofCthulhuTrophy, new[] { ModContent.ItemType<TeardropCleaver>() }); //eoc trophy
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
            CreateCalBagRecipes(ItemID.QueenBeeTrophy, new[] { ModContent.ItemType<TheBee>() }); //bee
            CreateCalBagRecipes(ModContent.ItemType<SlimeGodBag>(), new[]
            {

                ModContent.ItemType<OverloadedBlaster>(),
                ModContent.ItemType<AbyssalTome>(),
                ModContent.ItemType<EldritchTome>(),
                ModContent.ItemType<CrimslimeStaff>(),
                ModContent.ItemType<CorroslimeStaff>()
            });
            CreateCalBagRecipes(ItemID.WallOfFleshBossBag, new[]
            {
                ModContent.ItemType<Meowthrower>(),
                ModContent.ItemType<BlackHawkRemote>(),
                ModContent.ItemType<BlastBarrel>(),
                ModContent.ItemType<RogueEmblem>()
            }); //wof bag
            CreateCalBagRecipes(ItemID.WallofFleshTrophy, new[] { ModContent.ItemType<Carnage>() }); //wof trophy
            CreateCalBagRecipes(ModContent.ItemType<CryogenBag>(), new[]
            {
                ModContent.ItemType<Avalanche>(),
                ModContent.ItemType<HoarfrostBow>(),
                ModContent.ItemType<SnowstormStaff>(),
                ModContent.ItemType<Icebreaker>(),
                ModContent.ItemType<CryoStone>(),
                ModContent.ItemType<FrostFlare>()
            });
            CreateCalBagRecipes(ItemID.SpazmatismTrophy, new[] { ModContent.ItemType<Arbalest>() }); //twins 1
            CreateCalBagRecipes(ItemID.RetinazerTrophy, new[] { ModContent.ItemType<Arbalest>() }); //twins 2
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
                ModContent.ItemType<DeepDiver>()
            });
            CreateCalBagRecipes(ModContent.ItemType<BrimstoneWaifuBag>(), new[]
            {
                ModContent.ItemType<Brimlance>(),
                ModContent.ItemType<SeethingDischarge>(),
                ModContent.ItemType<DormantBrimseeker>(),
                ModContent.ItemType<Abaddon>(),
                ModContent.ItemType<RoseStone>()
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
            CreateCalBagRecipes(ItemID.PlanteraBossBag, new[] { ModContent.ItemType<BloomStone>() }); //plant bag
            CreateCalBagRecipes(ItemID.PlanteraTrophy, new[] { ModContent.ItemType<BlossomFlux>() }); //plant trophy
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
            CreateCalBagRecipes(ItemID.GolemTrophy, new[] { ModContent.ItemType<AegisBlade>() }); //golem
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
                ModContent.ItemType<PlagueCaller>(),
            });
            CreateCalBagRecipes(ItemID.FishronBossBag, new[] { ModContent.ItemType<DukesDecapitator>() }); //fishron bag
            CreateCalBagRecipes(ItemID.DukeFishronTrophy, new[] { ModContent.ItemType<BrinyBaron>() }); //fishron trophy
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
            CreateCalBagRecipes(ItemID.MoonLordBossBag, new[] { ModContent.ItemType<UtensilPoker>() }); //moon lord
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
            CreateCalBagRecipes(ModContent.ItemType<StormWeaverBag>(), new[]
            {
                ModContent.ItemType<TheStorm>(),
                ModContent.ItemType<StormDragoon>(),
            });
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
                    Recipe.Create(outputs[i]).AddIngredient(input).AddTile(TileID.Solidifier).DisableDecraft().Register();
                }
            }

            //additions to vanilla drops
            AddBannerToItemRecipe(ModContent.ItemType<StormlionBanner>(), ItemID.ThunderSpear);
            AddBannerToItemRecipe(ModContent.ItemType<StormlionBanner>(), ItemID.ThunderStaff);
            AddBannerToItemRecipe(ModContent.ItemType<BoxJellyfishBanner>(), ItemID.JellyfishNecklace);
            AddBannerToItemRecipe(ModContent.ItemType<GhostBellBanner>(), ItemID.JellyfishNecklace);
            AddBannerToItemRecipe(ModContent.ItemType<CannonballJellyfishBanner>(), ItemID.JellyfishNecklace);
            AddBannerToItemRecipe(ModContent.ItemType<MorayEelBanner>(), ItemID.Flipper);
            AddBannerToItemRecipe(ItemID.SharkBanner, ItemID.SharkToothNecklace);
            AddBannerToItemRecipe(ModContent.ItemType<ToxicatfishBanner>(), ItemID.DivingHelmet);
            AddBannerToItemRecipe(ModContent.ItemType<TrasherBanner>(), ItemID.DivingHelmet);
            AddBannerToItemRecipe(ModContent.ItemType<RotdogBanner>(), ItemID.AdhesiveBandage);

            //prehardmode
            AddBannerToItemRecipe(ModContent.ItemType<CuttlefishBanner>(),
                ModContent.ItemType<InkBomb>());
            AddBannerToItemRecipe(ItemID.HarpyBanner, ModContent.ItemType<CocosFeather>(), 5, 1);
            AddBannerToItemRecipe(ItemID.TombCrawlerBanner, ModContent.ItemType<BurntSienna>());
            AddBannerToItemRecipe(ItemID.DemonBanner, ModContent.ItemType<BladecrestOathsword>());
            AddBannerToItemRecipe(ItemID.GoblinSorcererBanner, ModContent.ItemType<PlasmaRod>());
            AddBannerToItemRecipe(ModContent.ItemType<BoxJellyfishBanner>(),
                ModContent.ItemType<AbyssShocker>(), 1, 1, ItemID.Bone);
            AddBannerToItemRecipe(ModContent.ItemType<NuclearToadBanner>(),
                ModContent.ItemType<CausticCroakerStaff>());

            //hardmode
            AddBannerToItemRecipe(ModContent.ItemType<ShockstormShuttleBanner>(),
                ModContent.ItemType<OracleHeadphones>());
            AddBannerToItemRecipe(ItemID.MossHornetBanner, ModContent.ItemType<Needler>());
            AddBannerToItemRecipe(ModContent.ItemType<FlakCrabBanner>(),
                ModContent.ItemType<FlakToxicannon>());
            AddBannerToItemRecipe(ItemID.GiantCursedSkullBanner, ModContent.ItemType<Keelhaul>(), 1, 1,
                ModContent.ItemType<LeviathanAmbergris>());
            AddBannerToItemRecipe(ModContent.ItemType<AcidEelBanner>(),
                ModContent.ItemType<SlitheringEels>(), 1, 1,
                ModContent.ItemType<CorrodedFossil>());
            AddBannerToItemRecipe(ModContent.ItemType<OrthoceraBanner>(),
                ModContent.ItemType<OrthoceraShell>());
            AddBannerToItemRecipe(ModContent.ItemType<SkyfinBanner>(),
                ModContent.ItemType<SkyfinBombers>(), 1, 1,
                ModContent.ItemType<CorrodedFossil>());
            AddBannerToItemRecipe(ItemID.NecromancerBanner, ModContent.ItemType<WrathoftheAncients>());
            AddBannerToItemRecipe(ItemID.DeadlySphereBanner, ModContent.ItemType<DefectiveSphere>());
            AddBannerToItemRecipe(ItemID.ClingerBanner, ModContent.ItemType<CursedDagger>());
            AddBannerToItemRecipe(ItemID.IchorStickerBanner, ModContent.ItemType<IchorSpear>());

            //post-ml
            AddBannerToItemRecipe(ModContent.ItemType<ImpiousImmolatorBanner>(),
                ModContent.ItemType<BlasphemousDonut>());
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
                bannerRecipe1.DisableDecraft();
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
                #region SummonRecipeNerfs
                if (recipe.HasResult<DesertMedallion>())
                {
                    recipe.ChangeIngredientStack(ModContent.ItemType<StormlionMandible>(), 1);
                    recipe.ChangeIngredientStack(ItemID.SandBlock, 10);
                    recipe.ChangeIngredientStack(ItemID.AntlionMandible, 2);
                }
                if (recipe.HasResult<DecapoditaSprout>())
                {
                    recipe.ChangeIngredientStack(ItemID.GlowingMushroom, 25);
                }
                if (recipe.HasResult<OverloadedSludge>())
                {
                    recipe.ChangeIngredientStack(ModContent.ItemType<BlightedGel>(), 15);
                }
                if (recipe.HasResult<CryoKey>())
                {
                    recipe.ChangeIngredientStack(ModContent.ItemType<EssenceofEleum>(), 4);
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
                    recipe.ChangeIngredientStack(ModContent.ItemType<Stardust>(), 15);
                    recipe.ChangeIngredientStack(ItemID.FallenStar, 7);
                }
                if (recipe.HasResult<DeathWhistle>())
                {
                    recipe.ChangeIngredientStack(ItemID.LunarTabletFragment, 6);
                }
                if (recipe.HasResult<ProfanedShard>())
                {
                    recipe.ChangeIngredientStack(ModContent.ItemType<UnholyEssence>(), 15);
                }
                if (recipe.HasResult<RuneofKos>())
                {
                    recipe.ChangeIngredientStack(ModContent.ItemType<UnholyEssence>(), 20);
                }
                if (recipe.HasResult<NecroplasmicBeacon>())
                {
                    recipe.ChangeIngredientStack(ModContent.ItemType<Polterplasm>(), 15);
                }
                if (recipe.HasResult<CosmicWorm>())
                {
                    recipe.ChangeIngredientStack(ModContent.ItemType<Polterplasm>(), 20);
                    recipe.ChangeIngredientStack(ItemID.LunarBar, 20);
                }
                if (recipe.HasResult<YharonEgg>())
                {
                    recipe.ChangeIngredientStack(ModContent.ItemType<LifeAlloy>(), 5);
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
                    if (recipe.RemoveIngredient(ModContent.ItemType<ZephyrBoots>()))
                        recipe.AddIngredient<AngelTreads>();
                    if (recipe.RemoveIngredient(ItemID.SoulofMight) && recipe.RemoveIngredient(ItemID.SoulofSight) && recipe.RemoveIngredient(ItemID.SoulofFright))
                        recipe.AddIngredient<LivingShard>(10);
                }
                if (recipe.HasResult<TracersCelestial>() && recipe.HasIngredient<AngelTreads>())
                {
                    if (recipe.RemoveIngredient(ModContent.ItemType<AngelTreads>()))
                        recipe.AddIngredient<AeolusBoots>();
                }

                if (recipe.HasResult<SupersonicSoul>() && recipe.HasIngredient<AeolusBoots>())
                {
                    if (recipe.RemoveIngredient(ModContent.ItemType<AeolusBoots>()))
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
                            .AddIngredient<ScourgeoftheCosmos>()
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
                if (recipe.HasResult(ModContent.ItemType<SnipersSoul>()) && recipe.HasIngredient(ItemID.MoltenQuiver))
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
                if (recipe.HasResult(ModContent.ItemType<ConjuristsSoul>()) && recipe.HasIngredient(ItemID.PygmyNecklace))
                {
                    if (recipe.RemoveIngredient(ItemID.PygmyNecklace))
                        recipe.AddIngredient<Nucleogenesis>();
                    if (recipe.RemoveIngredient(ItemID.Smolstar))
                        recipe.AddIngredient<PlantationStaff>();
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
                if (recipe.HasResult(ModContent.ItemType<UniverseSoul>()) && !recipe.HasIngredient<VagabondsSoul>())
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
                if (recipe.HasResult(ModContent.ItemType<TrawlerSoul>()) && recipe.HasIngredient(ItemID.ArcticDivingGear))
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
                if (recipe.HasResult(ModContent.ItemType<WorldShaperSoul>()) && !recipe.HasIngredient<BlossomPickaxe>())
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
                if (recipe.HasIngredient(ItemID.BloodMoonStarter) && recipe.HasResult(ModContent.ItemType<BloodOrb>())) // bloody tear -> blood orb disabled because abom
                {
                    recipe.DisableRecipe();
                }

                #endregion

                #region Balance and Progression Locks
                if (DLCCalamityConfig.Instance.BalanceRework)
                {
                    if (recipe.HasIngredient<EternalEnergy>() && recipe.createItem != null && CalItemBalance.RockItems.Contains(recipe.createItem.type) && !recipe.HasIngredient<Rock>())
                    {
                        recipe.AddIngredient<Rock>();
                    }
                    if (recipe.HasResult(ItemID.AnkhShield) && !recipe.HasIngredient(ItemID.SoulofNight))
                    {
                        recipe.AddIngredient(ItemID.SoulofNight, 3);
                    }
                    if (recipe.HasResult(ModContent.ItemType<SigilOfChampions>()) && !recipe.HasIngredient<DivineGeode>())
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
                        ModContent.ItemType<TerrariaSoul>(),
                        ModContent.ItemType<UniverseSoul>(),
                        ModContent.ItemType<DimensionSoul>(),
                        ModContent.ItemType<MasochistSoul>()
                    };

                    if (Tier2Souls.Contains(recipe.createItem.type) && !recipe.HasIngredient(ModContent.ItemType<ShadowspecBar>()))
                    {
                        recipe.AddIngredient<ShadowspecBar>(5);
                        if (recipe.RemoveTile(ModContent.TileType<CrucibleCosmosSheet>()))
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
            RecipeGroup T3WatchGroup = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Gold Watch"}",
                ItemID.GoldWatch,
                ItemID.PlatinumWatch);
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyGoldWatch", T3WatchGroup);

            RecipeGroup T3OreGroup = new(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Silver Ore"}",
                ItemID.SilverOre,
                ItemID.TungstenOre);
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnySilverOre", T3OreGroup);
            #endregion
        }
    }
}