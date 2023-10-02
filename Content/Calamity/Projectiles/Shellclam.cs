using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.ID;


namespace FargowiltasCrossmod.Content.Calamity.Projectiles
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class Shellclam : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/Shellfish";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 2;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 120;
            Projectile.DamageType = DamageClass.Generic;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(offset);
        }
        public override bool? CanDamage()
        {
            return Projectile.ai[0] == 0;
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            offset = reader.ReadVector2();
        }
        public Vector2 offset;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            offset = Projectile.Center - target.Center + new Vector2(0, -10);
            Projectile.ai[0] = 1;
            Projectile.ai[1] = target.whoAmI;
            Projectile.timeLeft = 600;
            target.buffImmune[ModContent.BuffType<ShellfishClaps>()] = target.Calamity().DR >= 0.99f;
            target.AddBuff(ModContent.BuffType<ShellfishClaps>(), 600000, false);
            Projectile.netUpdate = true;
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Water);
            }
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 10)
                {
                    Projectile.frame = Projectile.frame == 0 ? 1 : 0;
                    Projectile.frameCounter = 0;
                }
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                Projectile.spriteDirection = -Projectile.direction;
                if (Projectile.direction == 1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
                if (Projectile.velocity.Y < 10) Projectile.velocity.Y += 0.04f;
            }
            if (Projectile.ai[0] == 1)
            {
                Projectile.frame = 0;
                Projectile.Center = Main.npc[(int)Projectile.ai[1]].Center + offset;
                if (!Main.npc[(int)Projectile.ai[1]].active)
                {
                    Projectile.Kill();
                }
            }
        }
    }
}
