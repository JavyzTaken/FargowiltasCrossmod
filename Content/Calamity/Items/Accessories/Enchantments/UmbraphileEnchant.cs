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
using FargowiltasSouls.Content.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls.Content.Items;
using System.Data.Common;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [LegacyName("UmbraphileEnchantment")]
    public class UmbraphileEnchant : BaseEnchant
    {
        public override List<AccessoryEffect> ActiveSkillTooltips =>
            [AccessoryEffectLoader.GetEffect<UmbraphileEffect>()];
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
            return true;
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
            return false;
            return true;
        }
        public override Header ToggleHeader => null; // Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<UmbraphileEnchant>();
        public override bool ActiveSkill => true;
        public override void PostUpdateEquips(Player player)
        {
            CalDLCAddonPlayer cd = player.GetModPlayer<CalDLCAddonPlayer>();
            int damage = player.ForceEffect<UmbraphileEffect>() ? 100 : 200;
            

            if (cd.BatTime == 1)
            {
                for (int i = 0; i < player.width; i += 3)
                {
                    for (int j = 0; j < player.height; j += 3)
                    {
                        Dust d = Dust.NewDustDirect(player.position + new Vector2(i, j), 1, 1, DustID.Smoke, Scale: 1.2f);
                        d.velocity *= 0.5f;
                    }
                }
            }
            if (cd.BatTime > 0 && cd.BatStartupTimer == 0)
            {
                player.moveSpeed += 0.3f;
                player.Calamity().infiniteFlight = true;
                CooldownBarManager.Activate("UmbraphileCooldown", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/UmbraphileEnchant").Value, new Color(200, 50, 50),
                () => cd.BatTime / 180f);
            }
            else if (cd.BatCooldown > 0)
            {
                CooldownBarManager.Activate("UmbraphileCooldown", ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Items/Accessories/Enchantments/UmbraphileEnchant").Value, new Color(200, 50, 50),
                () => 1 - cd.BatCooldown / (60 * 60f));
            }

            if (cd.BatStartupTimer > 0)
            {
                if (cd.BatStartupTimer % 5 == 0)
                {
                    Projectile proj = Projectile.NewProjectileDirect(player.GetSource_EffectItem<UmbraphileEffect>(), player.Center, new Vector2(Main.rand.NextFloat(1, 10), 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<BatMode>(), FargoSoulsUtil.HighestDamageTypeScaling(player, damage), 0, player.whoAmI);
                }
            }
            if (cd.BatStartupTimer == 1)
            {
                
                for (int i = 0; i < 20; i++)
                {
                    Projectile proj = Projectile.NewProjectileDirect(player.GetSource_EffectItem<UmbraphileEffect>(), player.Center, new Vector2(Main.rand.NextFloat(1, 10), 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<BatMode>(), FargoSoulsUtil.HighestDamageTypeScaling(player, damage), 0, player.whoAmI);
                }
                for (int i = 0; i < player.width; i += 3)
                {
                    for (int j = 0; j < player.height; j += 3)
                    {
                        Dust d = Dust.NewDustDirect(player.position + new Vector2(i, j), 1, 1, DustID.Smoke, Scale: 1.2f);
                        d.velocity *= 0.5f;
                    }
                }
                SoundEngine.PlaySound(SoundID.NPCDeath4 with { Volume = 0.8f }, player.Center);
            }
            base.PostUpdate(player);
        }
        public override void ActiveSkillJustPressed(Player player, bool stunned)
        {
            
            CalDLCAddonPlayer cd = player.GetModPlayer<CalDLCAddonPlayer>();
            if (cd.BatCooldown == 0)
            {
                cd.BatCooldown = 60 * 60;
                
                cd.BatStartupTimer = 30;
                cd.BatTime = 180 + cd.BatStartupTimer;
            }
        }
        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            CalDLCAddonPlayer cd = player.GetModPlayer<CalDLCAddonPlayer>();
            if (cd.BatStartupTimer > 0)
            {
                float l = 1 - cd.BatStartupTimer / 30f;
                r = MathHelper.Lerp(r, 0, l);
                g = MathHelper.Lerp(g, 0, l);
                b = MathHelper.Lerp(b, 0, l);
                l -= 0.4f;
                if (l < 0) l = 0;
                a = MathHelper.Lerp(a, 0, l);
            }
            if (cd.BatTime > 0 && cd.BatStartupTimer == 0)
            {
                r = 0;
                g = 0;
                b = 0;
                a = 0;
            }
        }
    }
}
