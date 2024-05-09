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

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class SulphurEnchantment : BaseEnchant
    {

        public override Color nameColor => new Color(181, 139, 161);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Green;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SulphurEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient<CalamityMod.Items.Armor.Sulphurous.SulphurousHelmet>();
            recipe.AddIngredient<CalamityMod.Items.Armor.Sulphurous.SulphurousBreastplate>();
            recipe.AddIngredient<CalamityMod.Items.Armor.Sulphurous.SulphurousLeggings>();
            recipe.AddIngredient<CalamityMod.Items.Weapons.Rogue.ContaminatedBile>();
            recipe.AddIngredient<CalamityMod.Items.Weapons.Summon.CausticCroakerStaff>();
            recipe.AddIngredient<CalamityMod.Items.Accessories.RustyMedallion>();
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    public class SulphurEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<SulphurEnchantment>();
        
        public override void PostUpdateEquips(Player player)
        {
            player.GetJumpState<SulphurJump>().Enable();
        }
    }
    public class SulphurJump : ExtraJump
    {
        public override void UpdateHorizontalSpeeds(Player player)
        {
            player.runAcceleration *= 1.5f;
            player.maxRunSpeed *= 1.25f;
        }
        public override Position GetDefaultPosition()
        {
            return BeforeBottleJumps;
        }

        public override float GetDurationMultiplier(Player player)
        {
            return 1.15f;
        }
        //copy pasted dust code from cal
        public override void OnStarted(Player player, ref bool playSound)
        {
            int offset = player.height;
            if (player.gravDir == -1f)
                offset = 0;

            for (int i = 0; i < 25; ++i)
            {
                Dust dust = Dust.NewDustPerfect(new Vector2(player.Center.X, player.Center.Y + offset), Main.rand.NextBool(3) ? 75 : 161, new Vector2(-player.velocity.X, 6).RotatedByRandom(MathHelper.ToRadians(50f)) * Main.rand.NextFloat(0.1f, 0.8f), 100, default, Main.rand.NextFloat(1.7f, 2.2f));
                dust.noGravity = true;
                if (dust.type == 161)
                {
                    dust.scale = 1.5f;
                    dust.velocity = new Vector2(Main.rand.NextFloat(-4, 4) + -player.velocity.X * 0.3f, Main.rand.NextFloat(2, 4));
                    dust.noGravity = false;
                    dust.alpha = 190;
                }
            }

            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_EffectItem<SulphurEffect>(), player.Center, Vector2.Zero, ModContent.ProjectileType<SulphurBubble>(), 50, 1, player.whoAmI);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncProjectile, number: proj.whoAmI);
            }
        }
        public override void ShowVisuals(Player player)
        {
            base.ShowVisuals(player);
        }
    }
}
