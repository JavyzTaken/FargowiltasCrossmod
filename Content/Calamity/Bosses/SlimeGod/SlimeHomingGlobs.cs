using System;
using System.IO;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    internal class SlimeHomingGlobs
    {
        public static void AI(Projectile projectile, int var, ref int timer)
        {
            NPC slime = projectile.GetSourceNPC();
            if (slime != null && slime.active && (slime.type == ModContent.NPCType<CrimulanPaladin>() || slime.type == ModContent.NPCType<EbonianPaladin>()))
            {
                if (++timer > 90)
                {
                    const float MaxSpeed = 30;
                    float inertia = 15f;
                    float CloseEnough = slime.width / 2;

                    Vector2 toTarget = slime.Center - projectile.Center;
                    float distance = toTarget.Length();
                    if (projectile.velocity == Vector2.Zero)
                    {
                        projectile.velocity.X = -0.15f;
                        projectile.velocity.Y = -0.05f;
                    }
                    if (distance > CloseEnough)
                    {
                        toTarget.Normalize();
                        toTarget *= MaxSpeed;
                        projectile.velocity = (projectile.velocity * (inertia - 1f) + toTarget) / inertia;
                    }
                    else
                    {
                        if (projectile.timeLeft > 30)
                        {
                            projectile.timeLeft = 30;
                        }
                        projectile.scale = projectile.timeLeft / 30f;
                        projectile.velocity = toTarget * (1 - (projectile.timeLeft / 60f)); //60 is intentional
                    }
                }
                else
                {
                    projectile.velocity *= 0.96f;
                }
            }
            else
            {
                if (projectile.timeLeft > 30)
                {
                    projectile.timeLeft = 30;
                }
                projectile.scale = projectile.timeLeft / 30f;
            }

            if (projectile.timeLeft < 30)
            {
                projectile.Opacity = MathHelper.Lerp(0f, 0.8f, (float)projectile.timeLeft / 60f);
            }

            if (Main.rand.NextBool())
            {
                Color crimson = var == 0 ? Color.Crimson : Color.Lavender;
                crimson.A = 150;
                int num2 = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, DustID.TintableDust, 0f, 0f, projectile.alpha, crimson);
                Main.dust[num2].noGravity = true;
            }

            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.05f;
        }
        public static bool PreDraw(Projectile projectile, Color glowColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[projectile.type];

            //draw glow
            for (int j = 0; j < 12; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                Color drawGlowColor = glowColor with { A = 0 } * 0.7f;


                Main.EntitySpriteDraw(t.Value, projectile.Center + afterimageOffset - Main.screenPosition, null, drawGlowColor, projectile.rotation, t.Size() / 2, projectile.scale, SpriteEffects.None);
            }
            return true;
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeHomingCrimuleanGlob : UnstableCrimulanGlob
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/UnstableCrimulanGlob";
        int Timer = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(Timer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Timer = reader.Read7BitEncodedInt();
        }
        public override void AI()
        {
            SlimeHomingGlobs.AI(Projectile, 0, ref Timer);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return SlimeHomingGlobs.PreDraw(Projectile, Color.Crimson);
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeHomingEbonianGlob : UnstableEbonianGlob
    {
        public override string Texture => "CalamityMod/Projectiles/Boss/UnstableEbonianGlob";
        int Timer = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(Timer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Timer = reader.Read7BitEncodedInt();
        }
        public override void AI()
        {
            SlimeHomingGlobs.AI(Projectile, 1, ref Timer);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return SlimeHomingGlobs.PreDraw(Projectile, Color.Lavender);
        }
    }
}
