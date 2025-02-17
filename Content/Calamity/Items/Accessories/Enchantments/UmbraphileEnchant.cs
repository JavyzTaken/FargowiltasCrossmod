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
using CalamityMod.Projectiles.Turret;
using CalamityMod.Particles;
using Terraria.Audio;
using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Forces;
using FargowiltasSouls;
using FargowiltasCrossmod.Core.Calamity.ModPlayers;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("UmbraphileEnchantment")]
    public class UmbraphileEnchant : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Color nameColor => new Color(117, 69, 87);

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
            player.AddEffect<UmbraphileEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Umbraphile.UmbraphileHood>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Umbraphile.UmbraphileRegalia>(), 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Umbraphile.UmbraphileBoots>(), 1);
            recipe.AddIngredient(ItemID.VampireKnives, 1);
            recipe.AddIngredient(ItemID.SanguineStaff, 1);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.VampiricTalisman>(), 1);
            recipe.AddTile(TileID.CrystalBall);
            recipe.Register();
        }
    }

    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class UmbraphileEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Header ToggleHeader => null; // Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<UmbraphileEnchant>();
        public override bool ActiveSkill => true;
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            
            CalDLCAddonPlayer cd = player.GetModPlayer<CalDLCAddonPlayer>();
            if (cd.BatCooldown == 0)
            {
                cd.BatCooldown = 240;
                cd.BatTime = 240;
                Projectile proj = Projectile.NewProjectileDirect(player.GetSource_EffectItem<UmbraphileEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<BatMode>(), 100, 0, player.whoAmI);
                for (int i = 0; i < proj.width; i += 3)
                {
                    for (int j = 0; j < proj.height; j += 3)
                    {
                        Dust d = Dust.NewDustDirect(proj.position + new Vector2(i, j), 1, 1, DustID.Smoke, Scale:1.2f);
                        d.velocity *= 0.5f;
                    }
                }
                SoundEngine.PlaySound(SoundID.NPCDeath4 with { Volume = 0.8f}, player.Center);
            }
        }
        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            CalDLCAddonPlayer cd = player.GetModPlayer<CalDLCAddonPlayer>();
            if (cd.BatTime > 0)
            {
                r = 0;
                g = 0;
                b = 0;
                a = 0;
            }
        }
    }
}
