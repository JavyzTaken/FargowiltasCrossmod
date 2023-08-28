using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using CalamityMod.Items.Placeables.Furniture;


namespace FargowiltasCrossmod.Content.Calamity
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalamityRecipeSystem : ModSystem
    {
        public override void AddRecipes()
        {
            //trinket of chi
            Recipe tocrecipe = Recipe.Create(ModContent.ItemType<CalamityMod.Items.Accessories.TrinketofChi>());
            tocrecipe.AddIngredient(ItemID.ClayBlock, 20);
            tocrecipe.AddIngredient(ItemID.Chain, 2);
            tocrecipe.AddIngredient(ItemID.RedHusk, 1);
            tocrecipe.AddTile(TileID.Furnaces);
            tocrecipe.Register();
            //gladiator's locket
            Recipe glrecipe = Recipe.Create(ModContent.ItemType<CalamityMod.Items.Accessories.GladiatorsLocket>());
            glrecipe.AddIngredient(ItemID.Marble, 20);
            glrecipe.AddIngredient(ItemID.LifeCrystal, 2);
            glrecipe.AddRecipeGroup("FargowiltasCrossmod:AnyGoldWatch", 1);
            glrecipe.AddTile(TileID.DemonAltar);
            glrecipe.Register();
            //granite core recipe
            Recipe ugcrecipe = Recipe.Create(ModContent.ItemType<CalamityMod.Items.Accessories.UnstableGraniteCore>());
            ugcrecipe.AddIngredient(ItemID.Granite, 20);
            ugcrecipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Materials.EnergyCore>(), 2);
            ugcrecipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.AmidiasSpark>(), 1);
            ugcrecipe.AddTile(TileID.DemonAltar);
            ugcrecipe.Register();
            //symbiote recipe
            Recipe fgrecipe = Recipe.Create(ModContent.ItemType<CalamityMod.Items.Accessories.FungalSymbiote>());
            fgrecipe.AddIngredient(ItemID.GlowingMushroom, 20);
            fgrecipe.AddIngredient(ItemID.Acorn, 2);
            fgrecipe.AddIngredient(ItemID.ClayPot, 1);
            fgrecipe.AddTile(TileID.LivingLoom);
            fgrecipe.Register();

            //tundra leash recipe
            Recipe tlrecipe = Recipe.Create(ModContent.ItemType<CalamityMod.Items.Mounts.TundraLeash>());
            tlrecipe.AddRecipeGroup("AnySilverBar", 20);
            tlrecipe.AddIngredient(ItemID.Leather, 2);
            tlrecipe.AddIngredient(ItemID.Bunny, 1);
            tlrecipe.AddTile(TileID.CookingPots);
            tlrecipe.Register();
            //luxor recipe
            Recipe lgrecipe = Recipe.Create(ModContent.ItemType<CalamityMod.Items.Accessories.LuxorsGift>());
            lgrecipe.AddIngredient(ItemID.FossilOre, 20);
            lgrecipe.AddIngredient(ItemID.Ruby, 2);
            lgrecipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.ScuttlersJewel>(), 1);
            lgrecipe.AddTile(TileID.Anvils);
            lgrecipe.Register();

            //effigies recipes
            Recipe coref = Recipe.Create(ModContent.ItemType<CorruptionEffigy>());
            coref.AddIngredient(ItemID.EbonstoneBlock, 25);
            coref.AddIngredient(ItemID.RottenChunk, 5);
            coref.AddIngredient(ItemID.AngelStatue, 1);
            coref.AddTile(TileID.DemonAltar);
            coref.Register();

            Recipe crief = Recipe.Create(ModContent.ItemType<CrimsonEffigy>());
            crief.AddIngredient(ItemID.CrimstoneBlock, 25);
            crief.AddIngredient(ItemID.Vertebrae, 5);
            crief.AddIngredient(ItemID.AngelStatue, 1);
            crief.AddTile(TileID.DemonAltar);
            crief.Register();

            CreateCalBagRecipes(2489, new int[1] { ModContent.ItemType<CalamityMod.Items.Accessories.CrownJewel>() }); //slime
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.DesertScourgeBag>(), new int[6]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.AquaticDischarge>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Barinade>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.StormSpray>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.SeaboundStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.ScourgeoftheDesert>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.SandCloak>()
            });
            CreateCalBagRecipes(3319, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Summon.DeathstareRod>() }); //eoc bag
            CreateCalBagRecipes(1360, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Melee.TeardropCleaver>() }); //eoc trophy
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.CrabulonBag>(), new int[6]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.MycelialClaws>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Fungicide>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.HyphaeRod>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.PuffShroom>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.InfestedClawmerang>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.Mycoroot>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.HiveMindTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Pets.RottingEyeball>() });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.PerforatorTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Pets.BloodyVein>() });
            CreateCalBagRecipes(1364, new int[1] { ModContent.ItemType<CalamityMod.Items.Accessories.TheBee>() }); //bee
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.SlimeGodBag>(), new int[5]
            {

                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.OverloadedBlaster>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.AbyssalTome>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.EldritchTome>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.CrimslimeStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.CorroslimeStaff>()
            });
            CreateCalBagRecipes(3324, new int[4]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Meowthrower>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.BlackHawkRemote>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.BlastBarrel>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.RogueEmblem>()
            }); //wof bag
            CreateCalBagRecipes(1365, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Carnage>() }); //wof trophy
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.CryogenBag>(), new int[6]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Avalanche>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.HoarfrostBow>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.SnowstormStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.Icebreaker>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.CryoStone>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.FrostFlare>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.CryogenTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Summon.GlacialEmbrace>() });
            CreateCalBagRecipes(1368, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Arbalest>() }); //twins 1
            CreateCalBagRecipes(1369, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Arbalest>() }); //twins 2
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.AquaticScourgeBag>(), new int[6]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.SubmarineShocker>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Barinautical>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.Downpour>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.DeepseaStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.ScourgeoftheSeas>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.CorrosiveSpine>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.AquaticScourgeTrophy>(), new int[2]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.SeasSearing>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.DeepDiver>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.BrimstoneWaifuBag>(), new int[6]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Brimlance>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.SeethingDischarge>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.DormantBrimseeker>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.Gehenna>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.Abaddon>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.RoseStone>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.BrimstoneElementalTrophy>(), new int[2]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Hellborn>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.FlameLickedShell>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.CalamitasCloneBag>(), new int[7]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Oblivion>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Animosity>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.HavocsBreath>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.LashesofChaos>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.EntropysVigil>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.CrushsawCrasher>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.ChaosStone>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.CalamitasCloneTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Accessories.Regenator>() });
            CreateCalBagRecipes(3328, new int[1] { ModContent.ItemType<CalamityMod.Items.Accessories.BloomStone>() }); //plant bag
            CreateCalBagRecipes(1370, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.BlossomFlux>() }); //plant trophy
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.LeviathanBag>(), new int[8]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Greentide>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Leviatitan>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.AnahitasArpeggio>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.Atlantis>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.GastricBelcherStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.BrackishFlask>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.LeviathanTeeth>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.PearlofEnthrallment>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.LeviathanTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Accessories.TheCommunity>() });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.AnahitaTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Accessories.TheCommunity>() });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.AstrumAureusBag>(), new int[5]
            {

                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Nebulash>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.AuroraBlazer>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.AlulaAustralis>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.BorealisBomber>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.AuroradicalThrow>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.AstrumAureusTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.LeonidProgenitor>() });
            CreateCalBagRecipes(1371, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Melee.AegisBlade>() }); //golem
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.PlaguebringerGoliathBag>(), new int[11]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Virulence>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.DiseasedPike>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Pandemic>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.TheHive>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.BlightSpewer>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Malevolence>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.PestilentDefiler>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.PlagueStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.FuelCellBundle>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.InfectedRemote>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.TheSyringe>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.PlaguebringerGoliathTrophy>(), new int[2]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.Malachite>(),
                ModContent.ItemType<CalamityMod.Items.Pets.PlagueCaller>(),
            });
            CreateCalBagRecipes(3330, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.DukesDecapitator>() }); //fishron bag
            CreateCalBagRecipes(2589, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Melee.BrinyBaron>() }); //fishron trophy
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.RavagerBag>(), new int[7]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.UltimusCleaver>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.RealmRavager>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.Hematemesis>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.SpikecragStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.CraniumSmasher>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.BloodPact>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.FleshTotem>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.RavagerTrophy>(), new int[2]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.Vesuvius>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.CorpusAvertor>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.AstrumDeusBag>(), new int[6]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.TheMicrowave>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.StarSputter>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.StarShower>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.GodspawnHelixStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.RegulusRiot>(),
                ModContent.ItemType<CalamityMod.Items.Pets.ChromaticOrb>()
            });
            CreateCalBagRecipes(3332, new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.UtensilPoker>() }); //moon lodr
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.BossRelics.ProfanedGuardiansRelic>(), new int[3]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Typeless.RelicOfDeliverance>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Typeless.RelicOfResilience>(),
                ModContent.ItemType<CalamityMod.Items.RelicOfConvergence>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.ProfanedGuardianTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Accessories.WarbanneroftheSun>() });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.DragonfollyBag>(), new int[4]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.GildedProboscis>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.GoldenEagle>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.RougeSlash>(),
                ModContent.ItemType<CalamityMod.Items.Mounts.FollyFeed>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.DragonfollyTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Swordsplosion>() });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.ProvidenceBag>(), new int[9]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.HolyCollider>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.SolarFlare>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.BlissfulBombardier>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.TelluricGlare>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.PurgeGuzzler>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.DazzlingStabberStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.MoltenAmputator>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.Wings.ElysianWings>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.ElysianAegis>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.ProvidenceTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.PristineFury>() });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.StormWeaverBag>(), new int[2]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.TheStorm>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.StormDragoon>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.WeaverTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Magic.Thunderstorm>() });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.CeaselessVoidBag>(), new int[2]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.MirrorBlade>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.VoidConcentrationStaff>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.SignusBag>(), new int[2]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.Cosmilamp>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.CosmicKunai>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.PolterghastBag>(), new int[7]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.TerrorBlade>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.BansheeHook>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.DaemonsFlame>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.FatesReveal>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.GhastlyVisage>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.EtherealSubjugator>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.GhoulishGouger>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.OldDukeBag>(), new int[7]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.InsidiousImpaler>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.FetidEmesis>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.SepticSkewer>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.VitriolicViper>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.CadaverousCarrion>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.ToxicantTwister>(),
                ModContent.ItemType<CalamityMod.Items.Accessories.OldDukeScales>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.OldDukeTrophy>(), new int[1] { ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.TheOldReaper>() });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.DevourerofGodsBag>(), new int[6]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Excelsus>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.TheObliterator>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Deathwind>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.DeathhailStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.StaffoftheMechworm>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.Eradicator>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.DevourerofGodsTrophy>(), new int[2]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.CosmicDischarge>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Norfleet>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.YharonBag>(), new int[8]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.TheBurningSky>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.DragonRage>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.DragonsBreath>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.ChickenCannon>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.PhoenixFlameBarrage>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.YharonsKindleStaff>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.Wrathwing>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.FinalDawn>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.Trophies.YharonTrophy>(), new int[3]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.YharimsCrystal>(),
                ModContent.ItemType<CalamityMod.Items.Pets.ForgottenDragonEgg>(),
                ModContent.ItemType<CalamityMod.Items.Pets.McNuggets>()
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.DraedonBag>(), new int[8]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.SpineOfThanatos>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.PhotonRipper>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.TheJailor>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.SurgeDriver>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.AresExoskeleton>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.AtlasMunitionsBeacon>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.TheAtomSplitter>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.RefractionRotor>(),
            });
            CreateCalBagRecipes(ModContent.ItemType<CalamityMod.Items.TreasureBags.CalamitasCoffer>(), new int[7]
            {
                ModContent.ItemType<CalamityMod.Items.Weapons.Melee.Violence>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Condemnation>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.Heresy>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.Vehemence>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.Vigilance>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.Perdition>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.Sacrifice>(),

            });

            void CreateCalBagRecipes(int input, int[] outputs)
            {
                for (int i = 0; i < outputs.Length; i++)
                {
                    Recipe.Create(outputs[i]).AddIngredient(input).AddTile(TileID.Solidifier).Register();
                }
            }

            AddBannerToItemRecipe(ItemID.TombCrawlerBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Melee.BurntSienna>());
            AddBannerToItemRecipe(ItemID.DemonBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Melee.BladecrestOathsword>());
            AddBannerToItemRecipe(ItemID.GoblinSorcererBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Magic.PlasmaRod>());
            AddBannerToItemRecipe(ModContent.ItemType<CalamityMod.Items.Placeables.Banners.BoxJellyfishBanner>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.AbyssShocker>(), 1, 1, ItemID.Bone);
            AddBannerToItemRecipe(1682, ModContent.ItemType<CalamityMod.Items.Weapons.Summon.StaffOfNecrosteocytes>());
            AddBannerToItemRecipe(ModContent.ItemType<CalamityMod.Items.Placeables.Banners.NuclearToadBanner>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.CausticCroakerStaff>());
            //hardmode
            AddBannerToItemRecipe(ItemID.MossHornetBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.Needler>());
            AddBannerToItemRecipe(ItemID.GiantCursedSkullBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Magic.Keelhaul>(), 1, 1,
                ModContent.ItemType<CalamityMod.Items.Accessories.LeviathanAmbergris>());
            AddBannerToItemRecipe(ModContent.ItemType<CalamityMod.Items.Placeables.Banners.AcidEelBanner>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Magic.SlitheringEels>(), 1, 1,
                ModContent.ItemType<CalamityMod.Items.Materials.CorrodedFossil>());
            AddBannerToItemRecipe(ItemID.NecromancerBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Magic.WrathoftheAncients>());
            AddBannerToItemRecipe(ModContent.ItemType<CalamityMod.Items.Placeables.Banners.OrthoceraBanner>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.OrthoceraShell>());
            AddBannerToItemRecipe(ItemID.DeadlySphereBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.DefectiveSphere>());
            AddBannerToItemRecipe(ItemID.ClingerBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.CursedDagger>());
            AddBannerToItemRecipe(ItemID.IchorStickerBanner, ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.IchorSpear>());
            //post-ml
            AddBannerToItemRecipe(ModContent.ItemType<CalamityMod.Items.Placeables.Banners.ScryllarBanner>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.GuidelightofOblivion>(), 1, 1,
                ModContent.ItemType<CalamityMod.Items.Materials.DivineGeode>());
            AddBannerToItemRecipe(ModContent.ItemType<CalamityMod.Items.Placeables.Banners.ImpiousImmolatorBanner>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Summon.SanctifiedSpark>());

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
        }
        public override void AddRecipeGroups()
        {
            RecipeGroup T3WatchGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Gold Watch"}",
                ItemID.GoldWatch,
                ItemID.PlatinumWatch);
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyGoldWatch", T3WatchGroup);

            //reaver head group
            RecipeGroup ReaverHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Reaver Headpiece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Reaver.ReaverHeadExplore>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Reaver.ReaverHeadMobility>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Reaver.ReaverHeadMobility>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyReaverHelms", ReaverHelmsGroup);
            //daedalus head group
            RecipeGroup DeadalusHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Daedalus Headpiece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusHeadMelee>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusHeadRanged>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusHeadMagic>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusHeadSummon>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Daedalus.DaedalusHeadRogue>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyDaedalusHelms", DeadalusHelmsGroup);
            //bloodflare head group
            RecipeGroup BloodflareHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Bloodflare Headpiece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Bloodflare.BloodflareHeadMelee>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Bloodflare.BloodflareHeadRanged>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Bloodflare.BloodflareHeadMagic>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Bloodflare.BloodflareHeadSummon>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Bloodflare.BloodflareHeadRogue>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyBloodflareHelms", BloodflareHelmsGroup);
            //victide head group
            RecipeGroup VictideHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Victide Headpiece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Victide.VictideHeadMelee>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Victide.VictideHeadRanged>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Victide.VictideHeadMagic>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Victide.VictideHeadSummon>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Victide.VictideHeadRogue>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyVictideHelms", VictideHelmsGroup);
            //aerospec head group
            RecipeGroup AerospecHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Aerospec Headpiece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Aerospec.AerospecHelm>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Aerospec.AerospecHood>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Aerospec.AerospecHat>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Aerospec.AerospecHelmet>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Aerospec.AerospecHeadgear>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyAerospecHelms", AerospecHelmsGroup);
            RecipeGroup StatigelHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Statigel Headpiece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelHeadMelee>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelHeadMagic>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelHeadRanged>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelHeadRogue>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelHeadSummon>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyStatisHelms", AerospecHelmsGroup);
            //aerospec head group
            RecipeGroup HydrothermHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Hydrothermic Headpiece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicHeadMelee>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicHeadRanged>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicHeadMagic>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicHeadSummon>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Hydrothermic.HydrothermicHeadRogue>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyHydrothermHelms", HydrothermHelmsGroup);
            //statigel head group
            RecipeGroup SlayerHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"God Slayer Headpiece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.GodSlayer.GodSlayerHeadMelee>(),
                ModContent.ItemType<CalamityMod.Items.Armor.GodSlayer.GodSlayerHeadRanged>(),
                ModContent.ItemType<CalamityMod.Items.Armor.GodSlayer.GodSlayerHeadRogue>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnySlayerHelms", SlayerHelmsGroup);
            RecipeGroup TarragonHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Tarragon Head Piece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Tarragon.TarragonHeadMagic>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Tarragon.TarragonHeadRanged>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Tarragon.TarragonHeadSummon>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Tarragon.TarragonHeadRogue>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Tarragon.TarragonHeadMelee>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyTarragonHelms", TarragonHelmsGroup);
            RecipeGroup SilvaHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Silva Head Piece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Silva.SilvaHeadMagic>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Silva.SilvaHeadSummon>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnySilvaHelms", SilvaHelmsGroup);
            RecipeGroup AuricHelmsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Auric Head Piece"}",
                ModContent.ItemType<CalamityMod.Items.Armor.Auric.AuricTeslaRoyalHelm>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Auric.AuricTeslaPlumedHelm>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Auric.AuricTeslaSpaceHelmet>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Auric.AuricTeslaWireHemmedVisage>(),
                ModContent.ItemType<CalamityMod.Items.Armor.Auric.AuricTeslaHoodedFacemask>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyAuricHelms", AuricHelmsGroup);
            RecipeGroup RailgunsGroup = new RecipeGroup(() => $"{Language.GetTextValue("LegacyMisc.37")} {"Railgun"}",
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.AdamantiteParticleAccelerator>(),
                ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.TitaniumRailgun>());
            RecipeGroup.RegisterGroup("FargowiltasCrossmod:AnyRailguns", RailgunsGroup);
        }
    }
}
