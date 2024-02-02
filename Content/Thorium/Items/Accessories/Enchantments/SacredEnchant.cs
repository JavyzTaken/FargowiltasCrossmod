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
    public class SacredEnchant : BaseSynergyEnchant<WarlockEffect>
    {
        public override Color nameColor => Color.Orange;
        internal override int SynergyEnch => ModContent.ItemType<WarlockEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SacredEffect>(Item);
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SacredEffect : SynergyEffect<WarlockEffect>
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.AlfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<SacredEnchant>();
        public override bool MinionEffect => true;

        public override void OnHitNPCEither(Player player, NPC target, NPC.HitInfo hitInfo, DamageClass damageClass, int baseDamage, Projectile projectile, Item item)
        {
            if (hitInfo.Crit)
            {
                int projType = ModContent.ProjectileType<DLCShadowWisp>();
                bool synergy = SynergyActive(player);

                if (player.ownedProjectileCounts[projType] < 24)
                {
                    int shadowWispType = synergy ? (Main.rand.NextBool(4) ? 1 : 0) : 1; 

                    int damage = (shadowWispType == 0) ? 16 : 0; 
                    float kb = 0f; 

                    Projectile.NewProjectile(GetSource_EffectItem(player), target.Center, Vector2.Zero, projType, damage, kb, player.whoAmI, 0, 0, shadowWispType);
                }

                if (player.ownedProjectileCounts[projType] == 24)
                {
                    Vector2 vec = Vector2.UnitX;
                    for (int i = 0; i < 32; i++)
                    {
                        vec = vec.RotatedBy(MathF.PI / 16f);
                        int dustType = synergy ? (i % 2 == 1 ? DustID.ShadowbeamStaff : DustID.CursedTorch) : DustID.CursedTorch;
                        Dust.NewDustPerfect(player.Center + vec * 24 - Vector2.UnitY * 128f, dustType, vec * 4f).noGravity = true;
                    }
                }
            }
        }

        public override void PostUpdateEquips(Player player)
        {
            bool synergy = SynergyActive(player);

            var DLCPlayer = player.ThoriumDLC();
            if (DLCPlayer.warlockReleaseTimer > 0)
            {
                DLCPlayer.warlockReleaseTimer--;
                int releaseInterval = synergy ? 3 : 6;

                if (DLCPlayer.warlockReleaseTimer % releaseInterval == 0)
                {
                    Vector2 spawnPos = player.Center - Vector2.UnitY * 128f + Main.rand.NextVector2Circular(32, 32);
                    Vector2 spawnVel = (player.Center - Vector2.UnitY * 128f).DirectionTo(Main.MouseWorld) * 16f;
                    int type = synergy ? (Main.rand.NextBool(3) ? 1 : 0) : 1;
                    int damage = type == 0 ? 16 : 0;

                    Projectile.NewProjectile(GetSource_EffectItem(player), spawnPos, spawnVel, ModContent.ProjectileType<DLCShadowWispPro>(), damage, 2f, player.whoAmI, 0f, 0f, type);
                }
            }
        }

        public override void DrawEffects(Player player, PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            bool synergy = SynergyActive(player);
            Texture2D OrbTexture = ModContent.Request<Texture2D>("Terraria/Images/Projectile_18").Value;
            int wispCount = player.ownedProjectileCounts[ModContent.ProjectileType<DLCShadowWisp>()];

            Color baseColor = synergy ? Color.Lerp(Color.Green, Color.White, 0.5f) : Color.Green;
            Color color = Color.Lerp(Color.Transparent, baseColor, MathHelper.SmoothStep(0, 0.2f, wispCount / 24f));

            var DLCPlayer = player.ThoriumDLC();
            if (DLCPlayer.warlockReleaseTimer > 0)
            {
                color = Color.Lerp(baseColor, Color.Transparent, MathHelper.SmoothStep(1f, 0.8f, DLCPlayer.warlockReleaseTimer / 120f));
            }

            Main.EntitySpriteDraw(OrbTexture, player.Center - Main.screenPosition - Vector2.UnitY * 128f, OrbTexture.Bounds, color, 0f, OrbTexture.Size() * 0.5f, 2f, SpriteEffects.None);
            //drawInfo.DrawDataCache.Add();
        }
    }
}