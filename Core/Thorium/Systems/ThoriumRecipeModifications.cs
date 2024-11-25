using FargowiltasSouls.Content.Items.Accessories.Masomode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using FargowiltasSouls.Content.Items.Materials;
using ThoriumMod.Items.Terrarium;
using FargowiltasSouls.Content.Items.Accessories.Souls;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using ThoriumMod.Items.BossThePrimordials;
using ThoriumMod.Items.BasicAccessories;
using Terraria.GameContent.ItemDropRules;
using Fargowiltas.Items.Tiles;
using ThoriumMod.Items.Donate;
using ThoriumMod.Items.NPCItems;
using ThoriumMod.Items.BossBuriedChampion;
using ThoriumMod.Items.Titan;
using ThoriumMod.Items.BossThePrimordials.Aqua;
using ThoriumMod.Items.BossLich;
using ThoriumMod.Items.Depths;
using ThoriumMod.Items.BossThePrimordials.Slag;
using FargowiltasSouls.Content.Items.Accessories.Essences;
using ThoriumMod.Items.BossMini;
using ThoriumMod.Items.Dragon;
using ThoriumMod.Items.MagicItems;
using ThoriumMod.Items.SummonItems;
using ThoriumMod.Items.Misc;
using ThoriumMod.Items.HealerItems;
using ThoriumMod.Items.TransformItems;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Souls;
using FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments;

namespace FargowiltasCrossmod.Core.Thorium.Systems
{
    //for putting mod stuff into souls recipes or vice versa
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class ThoriumRecipesModifications : ModSystem
    {
        //for when 
        public override void PostAddRecipes()
        {
            //!!! WARNING !!!
            //Make sure condition to go into a recipe change is false if the change already happened !!!
            //else it will cause an infinite loop and game will not load and your computer will be set ablaze
            

            bool FMSEdited = false;
            bool SSSEdited = false;
            bool ColossusEdited = false;
            bool BerserkerEdited = false;
            bool WizardEdited = false;
            bool ConjuristEdited = false;
            bool SniperEdited = false;
            bool TrawlerEdited = false;
            bool ShaperEdited = false;
            bool UniverseEdited = false;

            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                if (recipe.HasResult(ModContent.ItemType<SupersonicSoul>()) && !SSSEdited)
                {
                    SSSEdited = true;
                    if (recipe.RemoveIngredient(ModContent.ItemType<AeolusBoots>()))
                        recipe.AddIngredient(ModContent.ItemType<TerrariumParticleSprinters>());
                    recipe.AddIngredient(ModContent.ItemType<SurvivalistBoots>())
                            .AddIngredient(ModContent.ItemType<AirWalkers>())
                            .AddIngredient(ModContent.ItemType<WeightedWinglets>())
                            .AddIngredient(ModContent.ItemType<SpectralFang>())
                            .AddIngredient(ModContent.ItemType<MoltenCollar>())
                            
                            .AddIngredient(ModContent.ItemType<TheOmegaCore>());
                }
                if (recipe.HasResult(ModContent.ItemType<FlightMasterySoul>()) && !FMSEdited)
                {
                    FMSEdited = true;
                    recipe.AddIngredient(ModContent.ItemType<ChampionWing>())
                            .AddIngredient(ModContent.ItemType<TitanWings>())
                            .AddIngredient(ModContent.ItemType<TerrariumWings>())
                            .AddIngredient(ModContent.ItemType<OceanEssence>(), 5);
                }
                if (recipe.HasResult(ModContent.ItemType<ColossusSoul>()) && !ColossusEdited)
                {
                    ColossusEdited = true;
                    if (recipe.RemoveIngredient(ItemID.BeeCloak))
                        recipe.AddIngredient(ModContent.ItemType<SweetVengeance>());
                    if (recipe.RemoveIngredient(ItemID.ObsidianHorseshoe))
                        recipe.AddIngredient(ModContent.ItemType<ObsidianScale>());
                    recipe.AddIngredient(ModContent.ItemType<TerrariumDefender>())
                            .AddIngredient(ModContent.ItemType<Phylactery>())
                            .AddIngredient(ModContent.ItemType<HeartOfStone>())
                            .AddIngredient(ModContent.ItemType<SpinyShell>())
                            .AddIngredient(ModContent.ItemType<InfernoEssence>(), 5);
                }
                if (recipe.HasResult(ModContent.ItemType<BerserkerSoul>()) && !BerserkerEdited)
                {
                    BerserkerEdited = true;
                    if (recipe.RemoveIngredient(ItemID.SharkToothNecklace))
                        recipe.AddIngredient(ModContent.ItemType<DragonTalonNecklace>());
                    recipe.AddIngredient(ModContent.ItemType<RapierBadge>())
                    .AddIngredient(ModContent.ItemType<TheJuggernaut>())
                    .AddIngredient(ModContent.ItemType<TheWhirlpool>())
                    .AddIngredient(ModContent.ItemType<TerrariumSaber>())
                    .AddIngredient(ModContent.ItemType<TerrariansLastKnife>());
                }
                if (recipe.HasResult(ModContent.ItemType<ArchWizardsSoul>()) && !WizardEdited)
                {
                    WizardEdited = true;
                    recipe.AddIngredient(ModContent.ItemType<MurkyCatalyst>())
                        .AddIngredient(ModContent.ItemType<ThoriumMod.Items.Donate.NuclearFury>())
                        .AddIngredient(ModContent.ItemType<LightningStaff>())
                        .AddIngredient(ModContent.ItemType<WitherStaff>())
                        .AddIngredient(ModContent.ItemType<NorthernLight>());
                }
                if (recipe.HasResult(ModContent.ItemType<SnipersSoul>()) && !SniperEdited)
                {
                    SniperEdited = true;
                    if (recipe.RemoveIngredient(ItemID.PulseBow))
                        recipe.AddIngredient(ModContent.ItemType<ShadowFlareBow>());
                    if (recipe.RemoveIngredient(ItemID.SniperRifle))
                        recipe.AddIngredient(ModContent.ItemType<DMR>());
                    recipe.AddIngredient(ModContent.ItemType<BeetleBlaster>());
                    recipe.AddIngredient(ModContent.ItemType<EmperorsWill>());
                    recipe.AddIngredient(ModContent.ItemType<QuasarsFlare>());
                    recipe.AddIngredient(ModContent.ItemType<ConcussiveWarhead>());

                }
                if (recipe.HasResult(ModContent.ItemType<ConjuristsSoul>()) && !ConjuristEdited)
                {
                    ConjuristEdited = true;
                    if (recipe.RemoveIngredient(ItemID.PygmyNecklace))
                        recipe.AddIngredient(ModContent.ItemType<NecroticSkull>());
                    recipe.AddIngredient(ModContent.ItemType<CrystalScorpion>());
                    recipe.AddIngredient(ModContent.ItemType<YumasPendant>());
                    recipe.AddIngredient(ModContent.ItemType<TerrariumEnigmaStaff>());
                    recipe.AddIngredient(ModContent.ItemType<EmberStaff>());
                    recipe.AddIngredient(ModContent.ItemType<StellarRod>());
                    recipe.AddIngredient(ModContent.ItemType<RudeWand>());

                }
                if (recipe.HasResult(ModContent.ItemType<TrawlerSoul>()) && !TrawlerEdited)
                {
                    TrawlerEdited = true;

                    if (recipe.RemoveIngredient(ItemID.GoldenFishingRod))
                    {
                        recipe.AddIngredient(ModContent.ItemType<TerrariumWhaleCatcher>());
                    }
                    recipe.AddIngredient(ModContent.ItemType<HightechSonarDevice>())
                        .AddIngredient(ModContent.ItemType<RottenCod>())
                        .AddIngredient(ModContent.ItemType<SpittingFish>())
                        .AddIngredient(ModContent.ItemType<GoldenScale>());

                }
                if (recipe.HasResult(ModContent.ItemType<WorldShaperSoul>()) && !ShaperEdited)
                {
                    ShaperEdited = true;
                    recipe.AddIngredient(ModContent.ItemType<TerrariumCanyonSplitter>());
                    recipe.RemoveIngredient(ModContent.ItemType<MinerEnchant>());
                    recipe.AddIngredient<GeodeEnchant>();
                }
                if (recipe.HasResult(ModContent.ItemType<UniverseSoul>()) && !UniverseEdited)
                {
                    UniverseEdited = true;
                    recipe.AddIngredient(ModContent.ItemType<OlympiansSoul>());
                    recipe.AddIngredient(ModContent.ItemType<ArchangelSoul>());

                }
                if (recipe.HasResult(ModContent.ItemType<TerrariumParticleSprinters>()) && recipe.HasIngredient(ItemID.TerrasparkBoots))
                {
                    if (recipe.RemoveIngredient(ItemID.TerrasparkBoots))
                        recipe.AddIngredient(ModContent.ItemType<AeolusBoots>());
                }

                if (recipe.HasResult<ThoriumMod.Items.BossThePrimordials.DoomSayersCoin>())
                {
                    recipe.AddIngredient<AbomEnergy>(10);
                }
            }
        }
    }
}
