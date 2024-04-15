using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls.Core.Toggler.Content;
using System.Security.Policy;
using Terraria.Graphics.Renderers;
using CalamityMod.Graphics.Renderers;
using Terraria.Graphics;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Content.Calamity.Projectiles;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class PlagueReaperEnchantment : BaseEnchant
    {

        public override Color nameColor => new Color(118, 146, 147);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Lime;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<PlagueReaperEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<CalamityMod.Items.Armor.PlagueReaper.PlagueReaperMask>(1);
            recipe.AddIngredient<CalamityMod.Items.Armor.PlagueReaper.PlagueReaperVest>(1);
            recipe.AddIngredient<CalamityMod.Items.Armor.PlagueReaper.PlagueReaperStriders>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Rogue.EpidemicShredder>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Melee.AnarchyBlade>(1);
            recipe.AddIngredient<CalamityMod.Items.Weapons.Melee.SoulHarvester>(1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }
    public class PlagueReaperEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<CosmoHeader>();
        public override int ToggleItemType => ModContent.ItemType<PlagueReaperEnchantment>();
        public override void PostUpdateEquips(Player player)
        {
            Main.NewText(player.CalamityDLC().PlagueCharge);
            for (int i = 0; i < Main.npc.Length; i++)
            {
                NPC npc = Main.npc[i];
                if (npc != null &&  npc.active && npc.HasBuff<Plague>() && npc.Distance(player.Center) < 100)
                {
                    player.CalamityDLC().PlagueCharge++;
                }
            }
            player.CalamityDLC().PlagueCharge++;
            if (player.CalamityDLC().PlagueCharge >= 200)
            {
                Projectile.NewProjectile(player.GetSource_EffectItem<PlagueReaperEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<PlagueCloud>(), 100, 0, player.whoAmI);
                player.CalamityDLC().PlagueCharge = 0;
            }
        }
    }
}
