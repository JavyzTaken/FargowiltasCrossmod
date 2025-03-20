using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using FargowiltasCrossmod.Core.Common;
using Terraria.Audio;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls.Content.NPCs.EternityModeNPCs.VanillaEnemies.SolarEclipse;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimstoneTeleport : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/BrimstoneElemental/BrimstoneElemental";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.width = 150;
            Projectile.height = 180;
            Main.projFrames[Type] = 12;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200;
            Projectile.light = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[ProjectileID.RainbowCrystalExplosion];
            Main.instance.LoadProjectile(ProjectileID.RainbowCrystalExplosion);
            Color color = Color.PaleVioletRed with { A = 0 } * 0.7f;
            //color.A = 0;

            float lerper = ((float)Math.Sin(100 * Projectile.localAI[0]) + 10 * Projectile.localAI[0]) * 0.065f;
            float spinLerper = 1 - (float)Math.Pow(1 - Projectile.localAI[0], 3);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition,
                null,
                color, Projectile.rotation, t.Size()/2, Vector2.Lerp(new Vector2(2, 0), new Vector2(2, 4), lerper),
                SpriteEffects.None);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition,
                null,
                color, MathHelper.Lerp(0, MathHelper.TwoPi * 2, spinLerper), t.Size() / 2, Vector2.Lerp(new Vector2(2, 0), new Vector2(2, 4), lerper),
                SpriteEffects.None);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 150; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.LifeDrain, Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7), Scale: 2).noGravity = true;
            }
            if (DLCUtils.HostCheck)
            {
                NPC owner = Main.npc[(int)Projectile.ai[0]];
                if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>())
                {
                    return;
                }
                owner.Center = Projectile.Center;
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, owner.whoAmI);

                if (WorldSavingSystem.MasochistModeReal && owner.GetLifePercent() > 0.33f)
                {
                    float projs = 17;
                    for (int i = 0; i < projs; i++)
                    {
                        float rot = MathF.Tau * i / projs;
                        Vector2 vel = rot.ToRotationVector2() * 3;
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<BrimstoneBarrage>(), FargoSoulsUtil.ScaledProjectileDamage(owner.defDamage), 0);
                    }
                }
            }
        }
        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[1]];
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>() || target == null || !target.active)
            {
                Projectile.Kill();
                return;
            }
            if (Projectile.localAI[0] < 1)
            {
                Projectile.localAI[0] += 0.005f;
                for (int i = 0; i < 10; i++)
                Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.LifeDrain, Main.rand.NextFloat(-5, 5) + Projectile.velocity.X, Main.rand.NextFloat(-5, 5) + Projectile.velocity.Y).noGravity = true;
            }

            //if (Projectile.Center.X > target.Center.X)
            //{
            //    Projectile.spriteDirection = 1;

            //}
            //else
            //{
            //    Projectile.spriteDirection = -1;
            //}
            

            //Projectile.frameCounter++;
            //if (Projectile.frameCounter > 12)
            //{
            //    Projectile.frame++;
            //    Projectile.frameCounter = 0;
            //}
            //if (Projectile.frame > 3)
            //{
            //    Projectile.frame = 0;
            //}
            Vector2 targetPos = target.Center + new Vector2(350 * Projectile.ai[2], 0);
            if (Projectile.ai[2] == 0) targetPos = target.Center + new Vector2(0, -300);
            if (Projectile.ai[2] == 2) targetPos = target.Center;
            Vector2 baseVel = (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, baseVel * Projectile.Distance(targetPos) / 50f, 0.03f);
        }
    }
}
