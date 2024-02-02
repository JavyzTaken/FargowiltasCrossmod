using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using FargowiltasSouls;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasCrossmod.Content.Thorium.Projectiles;
using Terraria.DataStructures;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WarlockEnchant : BaseSynergyEnchant<SacredEffect>
    {
        public override Color nameColor => Color.DarkGray;
        internal override int SynergyEnch => ModContent.ItemType<SacredEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<WarlockEffect>(Item);
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class WarlockEffect : SynergyEffect<SacredEffect>
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.AlfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<WarlockEnchant>();
        public override bool MinionEffect => true;

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            bool synergy = SynergyActive(player);
            if (synergy) return;

            if (hitInfo.Crit)
            {
                int projType = ModContent.ProjectileType<DLCShadowWisp>();

                if (player.ownedProjectileCounts[projType] < 24)
                {
                    Projectile.NewProjectile(GetSource_EffectItem(player), target.Center, Vector2.Zero, projType, 16, 1, player.whoAmI, 0, 0, 0);
                }

                if (player.ownedProjectileCounts[projType] == 24)
                {
                    Vector2 vec = Vector2.UnitX;
                    for (int i = 0; i < 32; i++)
                    {
                        vec = vec.RotatedBy(MathF.PI / 16f);
                        Dust.NewDustPerfect(player.Center + vec * 16 - Vector2.UnitY * 128f, DustID.ShadowbeamStaff, vec * 8f);
                    }
                }
            }
        }

        public override void PostUpdateEquips(Player player)
        {
            bool synergy = SynergyActive(player);
            if (synergy) return;

            var DLCPlayer = player.ThoriumDLC();
            if (DLCPlayer.warlockReleaseTimer > 0)
            {
                DLCPlayer.warlockReleaseTimer--;
                int releaseInterval = 4;

                if (DLCPlayer.warlockReleaseTimer % releaseInterval == 0)
                {
                    Vector2 spawnPos = player.Center - Vector2.UnitY * 128f + Main.rand.NextVector2Circular(32, 32);
                    Vector2 spawnVel = (player.Center - Vector2.UnitY * 128f).DirectionTo(Main.MouseWorld) * 16f;
                    int type = 0;
                    int damage = 16;

                    Projectile.NewProjectile(GetSource_EffectItem(player), spawnPos, spawnVel, ModContent.ProjectileType<DLCShadowWispPro>(), damage, 2f, player.whoAmI, 0f, 0f, type);
                }
            }
        }

        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            bool synergy = SynergyActive(player);
            if (synergy) return;

            Texture2D OrbTexture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_18").Value;
            int wispCount = player.ownedProjectileCounts[ModContent.ProjectileType<DLCShadowWisp>()];
            Color color = Color.Lerp(Color.Transparent, Color.White, MathHelper.SmoothStep(0, 0.2f, wispCount / 24f));
            var DLCPlayer = player.ThoriumDLC();
            if (DLCPlayer.warlockReleaseTimer > 0)
            {
                color = Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(1f, 0.8f, DLCPlayer.warlockReleaseTimer / 120f));
            }
            Main.EntitySpriteDraw(OrbTexture, player.Center - Main.screenPosition - Vector2.UnitY * 128f, OrbTexture.Bounds, color, 0f, OrbTexture.Size() * 0.5f, 2f, SpriteEffects.None);
        }
    }
}