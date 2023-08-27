using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation
{
    [JITWhenModsEnabled("CalamityMod")]
    [ExtendsFromMod("CalamityMod")]
    public class PlaguebringerSmalliath : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/PlaguebringerSummon";
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Main.projFrames[Type] = 6;
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.ai[1] = Main.rand.Next(0, 3);
                Projectile.netUpdate = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(Projectile.Center, 0, 0, DustID.TerraBlade);
            }
            Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModLoader.GetMod("CalamityMod").Find<ModGore>("PlagueCharger").Type);
            for (int i = 2; i <= 4; i++)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModLoader.GetMod("CalamityMod").Find<ModGore>("PlagueCharger" + i).Type);
            }
        }

        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            if (target == null || target.dead || !target.active)
            {
                Projectile.Kill();
            }
            if (target.Center.X > Projectile.Center.X && Projectile.ai[1] != 3)
            {
                Projectile.spriteDirection = -1;
            }
            else if (Projectile.ai[1] != 3)
            {
                Projectile.spriteDirection = 1;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 10)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.ai[1] != 3 && Projectile.frame >= 3)
            {
                Projectile.frame = 0;
            }
            else if (Projectile.ai[1] == 3 && Projectile.frame >= 6)
            {
                Projectile.frame = 3;
            }
            if (Projectile.ai[1] == 0)
            {
                GoAboveStingers();
            }
            else if (Projectile.ai[1] == 1)
            {
                RandomAboveKillables();
            }
            else if (Projectile.ai[1] == 2)
            {
                PrepareDash();
            }
            else if (Projectile.ai[1] == 3)
            {
                Dash();
            }
        }
        public void GoAboveStingers()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center + new Vector2(0, -375) - Projectile.Center).SafeNormalize(Vector2.Zero) * 15, 0.05f);
            if (Projectile.timeLeft < 200 && Projectile.timeLeft % 30 == 0)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.PlagueStingerGoliath>(), 50, 0);
                SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);
            }
        }
        public void RandomAboveKillables()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center + new Vector2(0, -375) - Projectile.Center).SafeNormalize(Vector2.Zero) * 15, 0.05f);
            if (Projectile.timeLeft < 200 && Projectile.timeLeft % 40 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.rand.NextBool())
                    {
                        NPC missile = NPC.NewNPCDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(10, 10), ModContent.NPCType<CalamityMod.NPCs.PlaguebringerGoliath.PlagueHomingMissile>(), ai2: 90);
                        missile.velocity = (target.Center - missile.Center).SafeNormalize(Vector2.Zero) * 10;

                    }
                    else
                    {
                        NPC.NewNPCDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(10 * Projectile.spriteDirection * -1, 20), ModContent.NPCType<CalamityMod.NPCs.PlaguebringerGoliath.PlagueMine>());
                    }
                    Projectile.netUpdate = true;
                }
                SoundEngine.PlaySound(SoundID.Item42, Projectile.Center);
            }
        }
        public void PrepareDash()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            if (Projectile.localAI[0] < 1)
            {
                Projectile.localAI[0] += 0.03f;
            }
            Vector2 pos = target.Center;
            if (Projectile.Center.X > target.Center.X)
            {
                pos.X += 600;
            }
            else
            {
                pos.X -= 600;
            }
            Projectile.velocity = Vector2.Lerp(Vector2.Zero, (pos - Projectile.Center).SafeNormalize(Vector2.Zero) * 15, Projectile.localAI[0]);
            if (Projectile.Distance(pos) <= 15)
            {
                Projectile.ai[1] = 3;
                Projectile.localAI[0] = 0;
            }
        }
        public void Dash()
        {
            Player target = Main.player[(int)Projectile.ai[0]];
            if (Projectile.localAI[0] == 0)
            {
                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 15;
                Projectile.frame = 3;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PlagueSounds/PBGDash"), Projectile.Center);
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 60)
            {
                Projectile.ai[1] = 2;
                Projectile.localAI[0] = 0;
            }
        }
    }
}
