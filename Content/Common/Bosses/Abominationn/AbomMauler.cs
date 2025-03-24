using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.NPCs.AcidRain;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.MutantBoss;
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

namespace FargowiltasCrossmod.Content.Common.Bosses.Abominationn
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class AbomMauler : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/AcidRain/Mauler";
        public ref float Timer => ref Projectile.localAI[2];
        public ref float Variant => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90 ;
            Projectile.height = 90;
            Projectile.aiStyle = -1;
            AIType = -1;

            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.Opacity = 1f;
            Projectile.light = 1f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.scale = 0;
            if (Main.rand.NextBool(5, 10))
            {
                Variant = 1;

                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 40;
                Projectile.Center = Projectile.position;

                Projectile.netUpdate = true;
            }
        }
        public override void AI()
        {
            if (Projectile.scale < 1)
                Projectile.scale += 0.05f;
            if (Projectile.velocity.Length() < 35)
                Projectile.velocity *= 1.05f;

            bool velRot = true;

            if (Main.rand.NextBool(4))
            {
                Vector2 dustVel = -Projectile.velocity.RotatedByRandom(MathHelper.PiOver2 * 0.3f) * Main.rand.NextFloat(0.7f, 1.4f);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid, dustVel.X, dustVel.Y);
            }

            if (Projectile.ai[0] == 1)
            {
                Timer++;
                int redirStart = 65;
                int redirEnd = 100;
                if (Timer >= redirStart && Timer < redirEnd)
                {
                    int playerID = (int)Projectile.ai[2];
                    if (!playerID.IsWithinBounds(Main.maxPlayers))
                        return;
                    Player player = Main.player[playerID];
                    if (!player.Alive())
                        return;
                    Projectile.velocity *= 0.8f;
                    float prog = (Timer - redirStart) / redirEnd;
                    Projectile.rotation = MathHelper.Lerp(Projectile.rotation, (Projectile.DirectionTo(player.Center).ToRotation()) % MathHelper.TwoPi, prog);
                    velRot = false;
                    if (Timer == redirEnd - 10)
                    {
                        SoundEngine.PlaySound(CalamityMod.NPCs.AcidRain.Mauler.RoarSound with { Pitch = 0.2f }, player.Center);
                    }
                    if (Timer == redirEnd - 1)
                    {
                        Projectile.velocity = Projectile.DirectionTo(player.Center);
                        Projectile.velocity *= 3f;
                    }
                }
                else if (Timer >= redirEnd)
                {
                   
                }
            }
            if (velRot)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            Projectile.spriteDirection = Projectile.direction = -Projectile.rotation.ToRotationVector2().X.NonZeroSign();

            if (Variant == 0)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 4)
                {
                    Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
                    Projectile.frameCounter = 0;
                }
            }
            else if (Variant == 1)
            {
                if (++Projectile.frameCounter > 5)
                    Projectile.frame++;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Variant == 0)
            {
                Texture2D texture = TextureAssets.Projectile[Type].Value;
                Texture2D glowmask = Mauler.GlowTexture.Value;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                int frameHeight = texture.Height / Main.projFrames[Type];
                Rectangle frame = new(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
                Vector2 origin = frame.Size() * 0.5f;
                SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;


                Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0f);
                Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, direction, 0f);
            }
            else if (Variant == 1)
            {
                int frameCount = 8;

                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SulphurousSea/Trasher").Value;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + Vector2.UnitY * Projectile.gfxOffY;
                int frameHeight = texture.Height / frameCount;
                Rectangle frame = new(0, frameHeight * Projectile.frame, texture.Width, frameHeight);
                Vector2 origin = frame.Size() * 0.5f;
                SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;


                Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0f);

            }
            return false;
        }
        /*
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 420);
        }
        */
    }
}
