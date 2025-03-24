using System;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.Cavern;
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
using static FargowiltasSouls.Content.Projectiles.EffectVisual;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PerforatorSpike : ModProjectile
    {
        // Projectile.Center is tip of spike
        // Thus hitbox extends backwards from it

        //public override string Texture => FargoSoulsUtil.EmptyTexture;

        public static int TelegraphTime => 60;
        public static int ExtensionTime => 13;
        public static int EndTime => 50;
        public static int FadeoutTime => 12;
        public ref float Timer => ref Projectile.ai[1];
        public const int TipLength = 30;
        public const int BodySegmentLength = 72;
        public const int TipSegmentLength = BodySegmentLength + TipLength;
        public const int BodyParts = 6;
        public const int BodyLength = BodySegmentLength * BodyParts;
        public const int Length = TipSegmentLength + BodyLength;
        public static float Width => 22;
        public bool Damaging => Timer > TelegraphTime;
        public int SpikeVariant;

        //public int[] Sprites = new int[BodyParts + 1];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60 * 10;
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
            if (Projectile.localAI[0] == 0)
            {
                SoundEngine.PlaySound(Main.rand.NextFromList(PerfsEternity.RockCrunch) with { Volume = 0.75f }, Projectile.Center);
                Projectile.netUpdate = true;
                Projectile.rotation = Projectile.velocity.ToRotation(); // initialize starting rotation from velocity set in NewProjectile

                Projectile.spriteDirection = Main.rand.NextBool() ? 1 : -1;
                Projectile.localAI[0] = 1;
                Projectile.frame = Main.rand.Next(Main.projFrames[Type]);
                SpikeVariant = Main.rand.Next(3);
                /*
                for (int i = 0; i < Sprites.Length; i++)
                {
                    Sprites[i] = Main.rand.Next(4);
                }
                */
            }
            int startupTime = 15;
            if (Timer < startupTime)
            {
                if (Projectile.Opacity < 1f)
                {
                    Projectile.Opacity += 1f / 8;
                }
                if (Timer >= 0)
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * ((float)TipSegmentLength / startupTime) * 0.5f;
            }
            else if (Timer >= TelegraphTime && Timer < TelegraphTime + ExtensionTime)
            {

                if (Timer == TelegraphTime)
                {
                    SoundEngine.PlaySound(Main.rand.NextFromList(PerfsEternity.SpikeSound) with { Volume = 0.75f }, Projectile.Center);
                }
                Projectile.light = 0;
                float totalExtension = Length - TipSegmentLength / 2.5f;
                float extensionPerFrame = totalExtension / ExtensionTime;

                // acceleration logic
                float progress = (Timer - TelegraphTime);
                float maxSpeed = extensionPerFrame * 2 / ExtensionTime;
                float deltaVel = progress * maxSpeed;

                Projectile.velocity = deltaVel * Projectile.rotation.ToRotationVector2();
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
            if (!Damaging || Projectile.Opacity < 1f)
                return false;
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
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            int x = Projectile.frame;
            int y = SpikeVariant; // tip = 0, 1, 2, unused = 3, middle = 4, bottom = 5
            int framesY = 6;

            SpriteEffects effects = ((Projectile.spriteDirection <= 0) ? SpriteEffects.FlipVertically : SpriteEffects.None);

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 tipCenter = Projectile.Center - dir * TipSegmentLength / 2;
            int tipWidth = TipSegmentLength;
            int ySize = tex.Height / framesY;

            Rectangle tipFrame = new(tipWidth * x, ySize * y, tipWidth, ySize);
            Rectangle[] bodyFrames = new Rectangle[BodyParts];
            Vector2[] bodyCenters = new Vector2[BodyParts];
            Color color = Projectile.GetAlpha(Lighting.GetColor(Projectile.Center.ToTileCoordinates()));
            Color[] colors = new Color[BodyParts];

            int bodyWidth = BodySegmentLength;
            bodyCenters[0] = Projectile.Center - dir * (tipWidth + (bodyWidth / 2));
            Vector2 bodyOffset = dir * bodyWidth;
            for (int i = 0; i < BodyParts; i++)
            {
                if (i > 0)
                    bodyCenters[i] = bodyCenters[i - 1] - bodyOffset;
                y = 4;
                if (i == BodyParts - 1) // end part
                    y = 5;

                bodyFrames[i] = new(tipWidth * x, ySize * y, bodyWidth, ySize);
                colors[i] = Projectile.GetAlpha(Lighting.GetColor(bodyCenters[i].ToTileCoordinates()));
            }
            if (Projectile.Opacity >= 0.99f)
            {
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f;
                    Main.EntitySpriteDraw(tex, tipCenter + afterimageOffset - Main.screenPosition, tipFrame, color * 0.5f, Projectile.rotation, tipFrame.Size() / 2, Projectile.scale, effects);
                    if (Damaging)
                    {
                        for (int i = 0; i < BodyParts; i++)
                            Main.EntitySpriteDraw(tex, bodyCenters[i] + afterimageOffset - Main.screenPosition, bodyFrames[i], colors[i] * 0.5f, Projectile.rotation, bodyFrames[i].Size() / 2, Projectile.scale, effects);
                    }
                }
                Main.spriteBatch.ResetToDefault();
            }

            Main.EntitySpriteDraw(tex, tipCenter - Main.screenPosition, tipFrame, color, Projectile.rotation, tipFrame.Size() / 2, Projectile.scale, effects);
            for (int i = 0; i < BodyParts; i++)
                Main.EntitySpriteDraw(tex, bodyCenters[i] - Main.screenPosition, bodyFrames[i], colors[i], Projectile.rotation, bodyFrames[i].Size() / 2, Projectile.scale, effects);
            
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 60 * 3);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCsAndTiles.Add(index);
        }
    }
}
