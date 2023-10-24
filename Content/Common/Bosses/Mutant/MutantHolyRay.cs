using System;
using System.IO;
using System.Runtime.CompilerServices;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs.Providence;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Player;
using static Terraria.Utils;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MutantHolyRay : ModProjectile
    {
        [CompilerGenerated]
        private static class _003C_003EO
        {
            public static TileActionAttempt _003C0_003E__CastLight;

            public static TileActionAttempt _003C1_003E__CutTiles;
        }

        public override string Texture => "CalamityMod/Projectiles/Boss/ProvidenceHolyRay";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            CooldownSlot = 1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            /*
            if (CalamityGlobalNPC.holyBoss != -1)
            {
                if (((Entity)Main.npc[CalamityGlobalNPC.holyBoss]).active)
                {
                    Projectile.maxPenetrate = (int)Main.npc[CalamityGlobalNPC.holyBoss].localAI[1];
                }
            }
            else
            {
                
            }
            */
            Projectile.maxPenetrate = 0;
            bool scissorLasers = true;//CalamityWorld.revenge || Projectile.maxPenetrate != 0;
            Vector2? vector78 = null;
            if (Utils.HasNaNs(Projectile.velocity) || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            if (((Entity)Main.npc[(int)Projectile.ai[1]]).active && Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<MutantBoss>())
            {
                Vector2 laserOffset = (-Vector2.UnitY * 0f);
                Vector2 fireFrom = new Vector2(((Entity)Main.npc[(int)Projectile.ai[1]]).Center.X, ((Entity)Main.npc[(int)Projectile.ai[1]]).Center.Y) + laserOffset;
                Projectile.position = fireFrom - new Vector2((float)Projectile.width, (float)Projectile.height) / 2f;
            }
            else
            {
                Projectile.Kill();
            }
            if (Utils.HasNaNs(Projectile.velocity) || Projectile.velocity == Vector2.Zero)
            {
                Projectile.velocity = -Vector2.UnitY;
            }
            float num801 = 1f;//((Main.npc[(int)Projectile.ai[1]].type == ModContent.NPCType<ProfanedGuardianCommander>()) ? 0.66f : 1f);
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] >= (scissorLasers ? 100f : 180f))
            {
                Projectile.Kill();
                return;
            }
            Projectile.scale = (float)Math.Sin((double)(Projectile.localAI[0] * (float)Math.PI / (scissorLasers ? 100f : 180f))) * 10f * num801;
            if (Projectile.scale > num801)
            {
                Projectile.scale = num801;
            }
            float num802 = Utils.ToRotation(Projectile.velocity);
            num802 += Projectile.ai[0];
            Projectile.rotation = num802 - (float)Math.PI / 2f;
            Projectile.velocity = Utils.ToRotationVector2(num802);
            float num803 = 3f;
            float num804 = Projectile.width;
            Vector2 samplingPoint = Projectile.Center;
            if (vector78.HasValue)
            {
                samplingPoint = vector78.Value;
            }
            float[] array3 = new float[(int)num803];
            Collision.LaserScan(samplingPoint, Projectile.velocity, num804 * Projectile.scale, 2400f, array3);
            float num805 = 0f;
            for (int num806 = 0; num806 < array3.Length; num806++)
            {
                num805 += array3[num806];
            }
            num805 /= num803;
            if (!Collision.CanHitLine(((Entity)Main.npc[(int)Projectile.ai[1]]).Center, 1, 1, ((Entity)Main.player[Main.npc[(int)Projectile.ai[1]].target]).Center, 1, 1))
            {
                num805 = 2400f;
            }
            int dustType = ProvUtils.GetDustID(Projectile.maxPenetrate);
            float amount = 0.5f;
            Projectile.localAI[1] = MathHelper.Lerp(Projectile.localAI[1], num805, amount);
            Vector2 vector79 = Projectile.Center + Projectile.velocity * (Projectile.localAI[1] - 14f);
            Vector2 vector80 = default(Vector2);
            for (int num807 = 0; num807 < 2; num807++)
            {
                float num808 = Utils.ToRotation(Projectile.velocity) + (Utils.NextBool(Main.rand) ? (-1f) : 1f) * ((float)Math.PI / 2f);
                float num809 = (float)Main.rand.NextDouble() * 2f + 2f;
                vector80 = new((float)Math.Cos((double)num808) * num809, (float)Math.Sin((double)num808) * num809);
                int num810 = Dust.NewDust(vector79, 0, 0, dustType, vector80.X, vector80.Y, 0, default(Color), 1f);
                Main.dust[num810].noGravity = true;
                Main.dust[num810].scale = 1.7f;
            }
            if (Utils.NextBool(Main.rand, 5))
            {
                Vector2 value29 = Utils.RotatedBy(Projectile.velocity, 1.5707963705062866, default(Vector2)) * ((float)Main.rand.NextDouble() - 0.5f) * (float)Projectile.width;
                int num811 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, dustType, 0f, 0f, 100, default(Color), 1.5f);
                Dust obj = Main.dust[num811];
                obj.velocity *= 0.5f;
                Main.dust[num811].velocity.Y = 0f - Math.Abs(Main.dust[num811].velocity.Y);
            }
            DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
            Vector2 center = Projectile.Center;
            Vector2 val = Projectile.Center + Projectile.velocity * Projectile.localAI[1];
            float num812 = (float)Projectile.width * Projectile.scale;
            object obj2 = _003C_003EO._003C0_003E__CastLight;
            if (obj2 == null)
            {
                TileActionAttempt val2 = DelegateMethods.CastLight;
                obj2 = (object)val2;
                _003C_003EO._003C0_003E__CastLight = val2;
            }
            Utils.PlotTileLine(center, val, num812, (TileActionAttempt)obj2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }
            bool num225 = Projectile.maxPenetrate == 0;
            Texture2D texture2D19 = (num225 ? ModContent.Request<Texture2D>(((ModProjectile)this).Texture, (AssetRequestMode)1).Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/ProvidenceHolyRayNight", (AssetRequestMode)1).Value);
            Texture2D texture2D20 = (num225 ? ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayMid", (AssetRequestMode)1).Value : ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayMidNight", (AssetRequestMode)1).Value);
            Texture2D texture2D21 = (num225 ? ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayEnd", (AssetRequestMode)1).Value : ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/ProvidenceHolyRayEndNight", (AssetRequestMode)1).Value);
            float num223 = Projectile.localAI[1];
            Color color44 = ProvUtils.GetProjectileColor(Projectile.maxPenetrate, 0) * 0.9f;
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(texture2D19, vector, (Rectangle?)null, color44, Projectile.rotation, Utils.Size(texture2D19) / 2f, Projectile.scale, (SpriteEffects)0, 0f);
            num223 -= (float)(texture2D19.Height / 2 + texture2D21.Height) * Projectile.scale;
            Vector2 value20 = Projectile.Center;
            value20 += Projectile.velocity * Projectile.scale * (float)texture2D19.Height / 2f;
            if (num223 > 0f)
            {
                float num224 = 0f;
                Rectangle rectangle7 = new(0, 36 * (Projectile.timeLeft / 3 % 4), texture2D20.Width, 36);
                while (num224 + 1f < num223)
                {
                    if (num223 - num224 < (float)rectangle7.Height)
                    {
                        rectangle7.Height = (int)(num223 - num224);
                    }
                    Main.spriteBatch.Draw(texture2D20, value20 - Main.screenPosition, (Rectangle?)rectangle7, color44, Projectile.rotation, new Vector2((float)(rectangle7.Width / 2), 0f), Projectile.scale, (SpriteEffects)0, 0f);
                    num224 += (float)rectangle7.Height * Projectile.scale;
                    value20 += Projectile.velocity * (float)rectangle7.Height * Projectile.scale;
                    rectangle7.Y += 36;
                    if (rectangle7.Y + rectangle7.Height > texture2D20.Height)
                    {
                        rectangle7.Y = 0;
                    }
                }
            }
            Vector2 vector2 = value20 - Main.screenPosition;
            Main.spriteBatch.Draw(texture2D21, vector2, (Rectangle?)null, color44, Projectile.rotation, Utils.Top(Utils.Frame(texture2D21, 1, 1, 0, 0, 0, 0)), Projectile.scale, (SpriteEffects)0, 0f);
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = (TileCuttingContext)2;
            Vector2 unit = Projectile.velocity;
            Vector2 center = Projectile.Center;
            Vector2 val = Projectile.Center + unit * Projectile.localAI[1];
            float num = (float)Projectile.width * Projectile.scale;
            object obj = _003C_003EO._003C1_003E__CutTiles;
            if (obj == null)
            {
                TileActionAttempt val2 = DelegateMethods.CutTiles;
                obj = (object)val2;
                _003C_003EO._003C1_003E__CutTiles = val2;
            }
            Utils.PlotTileLine(center, val, num, (TileActionAttempt)obj);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(Utils.TopLeft(targetHitbox), Utils.Size(targetHitbox), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[1], 22f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }

        public override void ModifyHitPlayer(Player target, ref HurtModifiers modifiers)
        {
            if (Projectile.maxPenetrate >= 1)
            {
                ref StatModifier sourceDamage = ref modifiers.SourceDamage;
                sourceDamage *= 0f;
            }
        }

        public override void OnHitPlayer(Player target, HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 60 * 5);
            if ((info.Damage > 0 || Projectile.maxPenetrate >= 1) && !target.creativeGodMode)
            {
                ProvUtils.ApplyHitEffects(target, Projectile.maxPenetrate, 400, 20);
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return Projectile.scale >= 0.5f;
        }
    }
}
