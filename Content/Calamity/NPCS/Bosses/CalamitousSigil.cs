using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Items.Materials;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofAnnihilation;
using FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDesolation;
using FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation;
using FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation;
using FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses
{
    [JITWhenModsEnabled("CalamityMod")]
    [ExtendsFromMod("CalamityMod")]
    public class CalamitousSigil : ModItem
    {
        public override string Texture => "CalamityMod/Items/Accessories/OccultSkullCrown";
        public override void SetStaticDefaults()
        {
            
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 13;
            Terraria.GameContent.Creative.CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults()
        {
            Item.width = 82;
            Item.height = 62;
            Item.rare = ModContent.RarityType<CalamityMod.Rarities.CalamityRed>();
            Item.maxStack = 1;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(1);
        }
        public override bool CanUseItem(Player player)
        {
            if (NPC.AnyNPCs(Type: ModContent.NPCType<ChampionofExploration.ChampionofExploration>()))
            {
                return false;
            }else if (NPC.AnyNPCs(ModContent.NPCType<ChampionofDevastation.ChampionofDevastation>()))
            {
                return false;
            }else if (NPC.AnyNPCs(ModContent.NPCType<DesolationHead>()))
            {
                return false;
            }else if (NPC.AnyNPCs(ModContent.NPCType<ChampionofExaltation.ChampionofExaltation>()))
            {
                return false;
            }else if (NPC.AnyNPCs(ModContent.NPCType<ChampionofAnnihilation.ChampionofAnnihilation>()))
            {
                return false;
            }
            return true;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            bool abyssZone4 = ModLoader.GetMod("CalamityMod").Call("GetInZone", player, "layer4") is true;
            bool crags = ModLoader.GetMod("CalamityMod").Call("GetInZone", player, "crags") is true;
            bool sunken = ModLoader.GetMod("CalamityMod").Call("GetInZone", player, "sunkensea") is true;
            if (player.ZoneSkyHeight)
            {
                if (player.altFunctionUse == 2)
                {
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.Items.CalamitousSigil.ExplorationFound"), new Color(175, 0, 0));
                }
                else
                {
                    int type = ModContent.NPCType<ChampionofExploration.ChampionofExploration>();
                    FargowiltasSouls.FargoSoulsUtil.SpawnBossNetcoded(player, type);
                    
                }
            }else if(player.ZoneSnow && player.Center.Y < Main.worldSurface * 16)
            {
                if (player.altFunctionUse == 2)
                {
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.Items.CalamitousSigil.DevastationFound"), new Color(175, 0, 0));
                }
                else
                {
                    int type = ModContent.NPCType<ChampionofDevastation.ChampionofDevastation>();
                    FargowiltasSouls.FargoSoulsUtil.SpawnBossNetcoded(player, type);
                }
            }
            else if (abyssZone4)
            {
                if (player.altFunctionUse == 2)
                {
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.Items.CalamitousSigil.DesolationFound"), new Color(175, 0, 0));
                }
                else
                {
                    int type = ModContent.NPCType<DesolationHead>();
                    FargowiltasSouls.FargoSoulsUtil.SpawnBossNetcoded(player, type);
                }
            }
            else if (crags)
            {
                if (player.altFunctionUse == 2)
                {
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.Items.CalamitousSigil.ExaltationFound"), new Color(175, 0, 0));
                }
                else
                {
                    int type = ModContent.NPCType<ChampionofExaltation.ChampionofExaltation>();
                    FargowiltasSouls.FargoSoulsUtil.SpawnBossNetcoded(player, type);
                }
            }
            else if (sunken)
            {
                if (player.altFunctionUse == 2)
                {
                    Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.Items.CalamitousSigil.AnnihilationFound"), new Color(175, 0, 0));
                }
                else
                {
                    int type = ModContent.NPCType<ChampionofAnnihilation.ChampionofAnnihilation>();
                    FargowiltasSouls.FargoSoulsUtil.SpawnBossNetcoded(player, type);
                }
            }
            else
            {
                if (player.altFunctionUse == 2)
                Main.NewText(Language.GetTextValue("Mods.FargowiltasCrossmod.Items.CalamitousSigil.NothingFound"), new Color(175, 0, 0));
            }
            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<CryonicBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AerialiteBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DepthCells>(), 5);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 5);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 5);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
            
        }
    }
}
