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
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Content.Projectiles.BossWeapons;

namespace FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class StatigelEnchantment : BaseEnchant
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Color nameColor => new(89, 170, 204);

        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<StatigelEffect>(Item);
            
        }
        
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("FargowiltasCrossmod:AnyStatisHelms");
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelArmor>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Armor.Statigel.StatigelGreaves>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Accessories.VitalJelly>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.OverloadedBlaster>());
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Rogue.GelDart>(), 300);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class StatigelEffect : AccessoryEffect
    {
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override Header ToggleHeader => Header.GetHeader<DevastationHeader>();
        public override int ToggleItemType => ModContent.ItemType<StatigelEnchantment>();
        
        public override void PostUpdateEquips(Player player)
        {

        }
        public static void StatigelProjEffect(Projectile projectile, NPC? target)
        {
            if (projectile.owner == Main.myPlayer && !projectile.minion && FargoSoulsUtil.OnSpawnEnchCanAffectProjectile(projectile, false) &&
                projectile.type != ProjectileID.WireKite && projectile.aiStyle != 190 && Main.LocalPlayer.heldProj != projectile.whoAmI && 
                ((projectile.type == ModContent.ProjectileType<PureGel>() && projectile.ai[1] == 0) || projectile.type != ModContent.ProjectileType<PureGel>())
                && (projectile.penetrate == 1 || target == null))
            {
                int t = -1;
                if (target != null)
                    t = DLCUtils.ClosestNPCExcludingOne(projectile.Center, 1000, target.whoAmI, false);
                else
                {
                    t = DLCUtils.ClosestNPCExcludingOne(projectile.Center, 1000, -1, false);
                }
                if (t < 0)
                {
                    Projectile proj = Projectile.NewProjectileDirect(projectile.GetSource_Death(), projectile.Center, projectile.velocity.RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<PureGel>(), projectile.damage / 2, projectile.knockBack, projectile.owner);

                    proj.ai[1] = 1;
                    if (projectile.type == ModContent.ProjectileType<PureGel>())
                        proj.ai[1] = projectile.ai[1] + 1;
                    if (target != null)
                        proj.ai[2] = target.whoAmI;
                    if (proj.velocity.Length() < 8) proj.velocity = proj.velocity.SafeNormalize(Vector2.Zero) * 8;
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendData(MessageID.SyncProjectile, number: proj.whoAmI);
                    }
                    return;
                }
                Projectile proje = Projectile.NewProjectileDirect(projectile.GetSource_Death(), projectile.Center, (Main.npc[t].Center - projectile.Center).SafeNormalize(Vector2.Zero) * projectile.velocity.Length(), ModContent.ProjectileType<PureGel>(), projectile.damage / 2, projectile.knockBack, projectile.owner);

                proje.ai[1] = 1;
                if (projectile.type == ModContent.ProjectileType<PureGel>())
                    proje.ai[1] = projectile.ai[1] + 1;
                if (target != null)
                    proje.ai[2] = target.whoAmI;
                if (proje.velocity.Length() < 8) proje.velocity = proje.velocity.SafeNormalize(Vector2.Zero) * 8;
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.SyncProjectile, number: proje.whoAmI);
                }
            }
        }
        public static void StatigelProjEffect2(Projectile projectile, Player player)
        {
            if (player.immuneTime <= 0 || player.ForceEffect<StatigelEffect>())
            {
                projectile.velocity = (player.Center - projectile.Center).SafeNormalize(Vector2.Zero) * projectile.velocity.Length();
                int target = -1;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC j = Main.npc[i];
                    if (j.active && !j.friendly && j.lifeMax > 5 && 
                        j.AngleFrom(projectile.Center) < projectile.rotation + MathHelper.ToRadians(20) &&
                        j.AngleFrom(projectile.Center) > projectile.rotation - MathHelper.ToRadians(20) &&
                        (target == -1 || j.Center.Distance(projectile.Center) < Main.npc[target].Center.Distance(projectile.Center)))
                    {
                        target = i;
                    }
                }
                if (target >= 0)
                {
                    projectile.velocity = (Main.npc[target].Center - projectile.Center).SafeNormalize(Vector2.Zero) * projectile.velocity.Length();
                }
                projectile.friendly = true;
                projectile.hostile = false;
            }
        }
    }
}
