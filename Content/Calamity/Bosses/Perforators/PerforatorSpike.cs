using System;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
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
    public class PerforatorSpike : ModProjectile
    {
        // Projectile.Center is tip of spike
        // Thus hitbox extends backwards from it
        public override string Texture => FargoSoulsUtil.EmptyTexture;

        public static int TelegraphTime => 60;
        public static int ExtensionTime => 16;
        public static int EndTime => 60;
        public static int FadeoutTime => 12;
        public ref float Timer => ref Projectile.ai[1];
        public const int TipLength = 80;
        public const int BodyParts = 6;
        public const int BodyLength = 80 * BodyParts;
        public const int Length = TipLength + BodyLength;
        public static float Width => 22;
        public bool Damaging => Timer > TelegraphTime;

        public int[] Sprites = new int[BodyParts + 1];

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
            Projectile.timeLeft = TelegraphTime + ExtensionTime + EndTime;
            Projectile.tileCollide = false;
            Projectile.light = 0.75f;
            Projectile.ignoreWater = true;
            //Projectile.extraUpdates = 1;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.FargoSouls().CanSplit = false;

            Projectile.scale = 1f;
            Projectile.hide = true;
            Projectile.Opacity = 0f;

            Projectile.FargoSouls().GrazeCheck =
            Projectile =>
            {
                float num6 = 0f;
                if (CanDamage() != false && Collision.CheckAABBvLineCollision(Main.LocalPlayer.Hitbox.TopLeft(), Main.LocalPlayer.Hitbox.Size(), Projectile.Center,
                    Projectile.Center - Projectile.velocity * Length, Width * Projectile.scale + Main.LocalPlayer.FargoSouls().GrazeRadius * 2f + Player.defaultHeight, ref num6))
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
            if (Projectile.localAI[0] == 0)
            {
                Projectile.netUpdate = true;
                Projectile.rotation = Projectile.velocity.ToRotation(); // initialize starting rotation from velocity set in NewProjectile

                Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
                Projectile.localAI[0] = 1;
                for (int i = 0; i < Sprites.Length; i++)
                {
                    Sprites[i] = Main.rand.Next(4);
                }
            }
            int startupTime = 15;
            if (Timer < startupTime)
            {
                if (Projectile.Opacity < 1f)
                {
                    Projectile.Opacity += 1f / 8;
                }
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * ((float)TipLength / startupTime) * 0.5f;
            }
            else if (Timer >= TelegraphTime && Timer < TelegraphTime + ExtensionTime)
            {
                float totalExtension = Length - TipLength;
                float extensionPerFrame = totalExtension / ExtensionTime;
                Projectile.velocity = extensionPerFrame * Projectile.rotation.ToRotationVector2();
            }
            else
                Projectile.velocity *= 0;

            if (Timer >= TelegraphTime + ExtensionTime + EndTime - FadeoutTime)
            {
                Projectile.Opacity -= 1f / FadeoutTime;
                if (Projectile.Opacity < 0.05f)
                    Projectile.Kill();
            }
            Timer++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Damaging || Projectile.Opacity < 0.5f)
                return false;
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center - Projectile.velocity * Length, Width * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D tip = PerfsEternityNew.LegEndTextures[Sprites[0]].Value;
            Texture2D[] bodies = new Texture2D[BodyParts];
            for (int i = 0; i < BodyParts; i++)
                bodies[i] = PerfsEternityNew.LegTextures[Sprites[i + 1]].Value;

            SpriteEffects effects = ((Projectile.spriteDirection <= 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 tipCenter = Projectile.Center - dir * tip.Width / 2;

            Vector2[] bodyCenters = new Vector2[BodyParts];
            bodyCenters[0] = Projectile.Center - dir * (tip.Width + (bodies[0].Width / 2));
            Vector2 bodyOffset = dir * bodies[0].Width;
            for (int i = 1; i <  bodyCenters.Length; i++)
                bodyCenters[i] = bodyCenters[i - 1] - bodyOffset;

            Color[] colors = new Color[BodyParts];
            for (int i = 0; i < BodyParts; i++)
                colors[i] = Projectile.GetAlpha(Lighting.GetColor(bodyCenters[i].ToTileCoordinates()));

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f;
                Main.EntitySpriteDraw(tip, tipCenter + afterimageOffset - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tip.Size() / 2, Projectile.scale, effects);
                if (Damaging)
                {
                    for (int i = 0; i < BodyParts; i++)
                        Main.EntitySpriteDraw(bodies[i], bodyCenters[i] + afterimageOffset - Main.screenPosition, null, colors[i], Projectile.rotation, bodies[i].Size() / 2, Projectile.scale, effects);
                }
            }

            Main.spriteBatch.ResetToDefault();
            Main.EntitySpriteDraw(tip, tipCenter - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tip.Size() / 2, Projectile.scale, effects);
            for (int i = 0; i < BodyParts; i++)
                Main.EntitySpriteDraw(bodies[i], bodyCenters[i] - Main.screenPosition, null, colors[i], Projectile.rotation, bodies[i].Size() / 2, Projectile.scale, effects);
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCsAndTiles.Add(index);
        }
    }
}
