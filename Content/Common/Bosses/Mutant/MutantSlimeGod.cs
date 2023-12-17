using CalamityMod;
using CalamityMod.NPCs.SlimeGod;
using FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Content.Buffs.Boss;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MutantSlimeGod : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/SlimeGod/EbonianPaladin";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.npcFrameCount[ModContent.NPCType<EbonianPaladin>()];
        }
        private bool Crimson => Projectile.ai[0] == 1;
        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 92;
            Projectile.Opacity = 0.8f;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
        }
        public const int SlamTime = 90;
        public override void AI()
        {
            if (Projectile.ai[2] < SlamTime / 2)
            {
                Projectile.scale = MathHelper.Lerp(0.4f, 1.1f, Projectile.ai[2] / (SlamTime / 2));
            }
            if (Projectile.ai[2] == SlamTime / 2 + 10)
            {
                Projectile.scale = 1.1f;
                SoundEngine.PlaySound(SlimeGodCoreEternity.ShotSound);
                if (DLCUtils.HostCheck)
                {
                    int p = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Vector2.UnitY * Projectile.height / 2, Vector2.UnitY, ModContent.ProjectileType<SlamTelegraph>(), 0, 0, Main.myPlayer, ai1: 150 * 1.2f);
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        (Main.projectile[p].ModProjectile as SlamTelegraph).Duration = SlamTime - Projectile.ai[2];
                    }
                }
            }
            float gravity = 0.5f;
            if (Projectile.ai[2] > SlamTime / 2)
            {
                Projectile.velocity.X *= 0.85f;
                if (Projectile.ai[2] > SlamTime / 2 + SlamTime / 4)
                {
                    gravity *= 2.5f;
                }

            }

            Projectile.velocity += gravity * Vector2.UnitY;

            if (++Projectile.ai[2] > SlamTime * 3)
            {
                Projectile.Kill();
            }

        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (WorldSavingSystem.EternityMode)
                target.AddBuff(ModContent.BuffType<MutantFangBuff>(), 180);
            base.OnHitPlayer(target, info);
        }
        public override void OnKill(int timeLeft)
        {

            Color dustColor = Crimson ? Color.Crimson : Color.Lavender;
            dustColor.A = 150;
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.height, Projectile.height, DustID.TintableDust, Projectile.velocity.X, Projectile.velocity.Y, Projectile.alpha, dustColor, 2f);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            string texName = Crimson ? "CrimulanPaladin" : "EbonianPaladin";

            Texture2D texture = ModContent.Request<Texture2D>($"CalamityMod/NPCs/SlimeGod/{texName}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            //draw projectile 
            int num156 = texture.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;

            Color color26 = lightColor;
            color26 = Projectile.GetAlpha(color26);

            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Color color27 = color26 * 0.75f;
                color27 *= (float)(ProjectileID.Sets.TrailCacheLength[Projectile.type] - i) / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Rectangle?(rectangle), color27, num165, origin2, Projectile.scale, effects, 0);
            }


            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);

            return false;
        }
    }
}
