using System;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using Humanizer;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PerforatorLegHitbox : ModProjectile
    {
        // Projectile.Center is tip of spike
        // Thus hitbox extends backwards from it
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public ref float ParentID => ref Projectile.ai[0];
        public ref float LegIndex => ref Projectile.ai[1];
        public ref float Duration => ref Projectile.ai[2];

        public ref float Timer => ref Projectile.localAI[0];

        public static int Length = 80;
        public static int Width = 22;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.FargoSouls().CanSplit = false;

            Projectile.scale = 1f;
            Projectile.Opacity = 0f;

            Projectile.FargoSouls().GrazeCheck =
            Projectile =>
            {
                float num6 = 0f;
                if (CanDamage() != false && Collision.CheckAABBvLineCollision(Main.LocalPlayer.Hitbox.TopLeft(), Main.LocalPlayer.Hitbox.Size(), Projectile.Center,
                    Projectile.Center - Projectile.rotation.ToRotationVector2() * Length, Width * Projectile.scale + Main.LocalPlayer.FargoSouls().GrazeRadius * 2f + Player.defaultHeight, ref num6))
                {
                    return true;
                }
                return false;
            };
        }
        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.Center = LumUtils.FindGround(Projectile.Center.ToTileCoordinates(), Vector2.UnitY).ToWorldCoordinates() + Vector2.UnitY * 4;
        }
        public override void AI()
        {
            if (++Timer > Duration)
            {
                Projectile.Kill();
                return;
            }
            NPC parent = Main.npc[(int)ParentID];
            if (!parent.TypeAlive<PerforatorHive>())
            {
                Projectile.Kill();
                return;
            }
            PerfsEternity parentAI = parent.GetDLCBehavior<PerfsEternity>();
            var leg = parentAI.Legs[(int)LegIndex];
            Projectile.Center = leg.GetEndPoint();
            Projectile.rotation = leg.Leg[leg.Leg.JointCount - 1].Rotation;

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center - Projectile.rotation.ToRotationVector2() * Length, Width * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 60 * 3);
        }
    }
}
