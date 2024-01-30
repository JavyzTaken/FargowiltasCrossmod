using System.IO;
using CalamityMod.NPCs.Polterghast;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MutantPolter : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/Polterghast/Polterghast";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = Main.npcFrameCount[ModContent.NPCType<Polterghast>()];
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.height = Projectile.width = 90;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }
        public override bool CanHitPlayer(Player target) => Projectile.alpha < 50;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }
        public const int StartDistance = 1000;
        public override void AI()
        {
            Vector2 centerPos = Projectile.ai[0] * Vector2.UnitX + Projectile.ai[1] * Vector2.UnitY;
            float rotDir = Projectile.ai[2];

            Vector2 ToCenter = centerPos - Projectile.Center;
            float distance = ToCenter.Length();

            Vector2 velCenter = ToCenter * 0.015f;
            float distanceFraction = (distance / StartDistance);

            float circumference = distance * MathHelper.TwoPi;
            float maxRotationSpeed = circumference / 120;
            float rotationSpeed = maxRotationSpeed * distanceFraction;
            Vector2 velRotation = Vector2.Normalize(ToCenter).RotatedBy(MathHelper.PiOver2 * rotDir) * rotationSpeed;

            Projectile.velocity = velCenter + velRotation;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            #region Visuals
            if (distance > Projectile.width / 3)
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 7;
                }
                else
                {
                    Projectile.alpha = 0;
                }
            }
            else
            {
                if (Projectile.alpha < 255)
                {
                    Projectile.alpha += 15;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            if (Projectile.frame < 8)
            {
                Projectile.frame = 8;
            }
            if (++Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame > 11)
                {
                    Projectile.frame = 8;
                }
            }
            #endregion
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            base.OnHitPlayer(target, info);
        }
    }
}
