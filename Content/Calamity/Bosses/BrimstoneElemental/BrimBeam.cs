using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.Graphics;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BrimBeam : ModProjectile
    {
        public override string Texture => "CalamityMod/ExtraTextures/Lasers/BrimstoneRayMid";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.width = Projectile.height = 10;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.FargoSouls().DeletionImmuneRank = 2;
            Projectile.penetrate = -1;
            Projectile.scale = 1.3f;
            Projectile.timeLeft = 200;
        }
        public int length = 40;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.Center, Projectile.Center + new Vector2(0, 30 * length - 22).RotatedBy(Projectile.rotation) * Projectile.scale);
        }
        public override void OnSpawn(IEntitySource source)
        {
            
            base.OnSpawn(source);
        }
        public override void OnKill(int timeLeft)
        {
        }
        VertexStrip TrailDrawer = new VertexStrip();
        public Color TrailColorFunction(float completionRatio)
        {
            float opacity = 1;
            return Color.Lerp(Color.Red, Color.Black, 0);
        }

        public float TrailWidthFunction(float completionRatio) => 800;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft == 200) return false;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1000;
            Asset<Texture2D> begin = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneRay");
            Asset<Texture2D> mid = TextureAssets.Projectile[Type];
            Asset<Texture2D> end = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/BrimstoneRayEnd");

            Color color = Color.PaleVioletRed with { A = 200 };
            Vector2 scale = new Vector2(Projectile.localAI[0], Projectile.scale);
            //afterimage
            for (int j = Projectile.oldPos.Length-1; j >= 0; j--)
            {
                if (Projectile.oldRot[j] != 0)
                {
                    Main.EntitySpriteDraw(begin.Value, Projectile.Center - Main.screenPosition, null, color * (1 - (float)j / Projectile.oldRot.Length) * 0.5f, Projectile.oldRot[j], begin.Size() / 2, scale, SpriteEffects.None);
                    for (int i = 0; i < length; i++)
                    {
                        Vector2 sectionOffset = new Vector2(0, begin.Height() / 2 + mid.Height() * i + mid.Height() / 2) * Projectile.scale;
                        Vector2 endOffset = new Vector2(0, begin.Height() / 2 + mid.Height() * (i + 1) + end.Height() / 2) * Projectile.scale;
                        Main.EntitySpriteDraw(mid.Value, Projectile.Center - Main.screenPosition + sectionOffset.RotatedBy(Projectile.oldRot[j]), null, color * (1 - (float)j / Projectile.oldRot.Length) * 0.5f, Projectile.oldRot[j], mid.Size() / 2, scale, SpriteEffects.None);
                        if (i == length - 1)
                        {
                            Main.EntitySpriteDraw(end.Value, Projectile.Center - Main.screenPosition + endOffset.RotatedBy(Projectile.oldRot[j]), null, color * (1 - (float)j / Projectile.oldRot.Length) * 0.5f, Projectile.oldRot[j], end.Size() / 2, scale, SpriteEffects.None);
                        }
                    }
                }
            }

            Main.EntitySpriteDraw(begin.Value, Projectile.Center - Main.screenPosition, null, color, Projectile.rotation, begin.Size() / 2, scale, SpriteEffects.None);
            for (int i = 0; i < length; i++)
            {
                Vector2 sectionOffset = new Vector2(0, begin.Height() / 2 + mid.Height() * i + mid.Height() / 2) * Projectile.scale;
                Vector2 endOffset = new Vector2(0, begin.Height() / 2 + mid.Height() * (i + 1) + end.Height() / 2) * Projectile.scale;
                Main.EntitySpriteDraw(mid.Value, Projectile.Center - Main.screenPosition + sectionOffset.RotatedBy(Projectile.rotation), null, color, Projectile.rotation, mid.Size() / 2, scale, SpriteEffects.None);
                if (i == length - 1)
                {
                    Main.EntitySpriteDraw(end.Value, Projectile.Center - Main.screenPosition + endOffset.RotatedBy(Projectile.rotation), null, color, Projectile.rotation, end.Size() / 2, scale, SpriteEffects.None);
                }
            }
            
            
            return false;
        }
        public override void AI()
        {
            if (Projectile.timeLeft == 200)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
                Projectile.rotation = Projectile.ai[2];
                Projectile.ai[2] = 0;
            }
            if (Projectile.localAI[0] < Projectile.scale && Projectile.timeLeft > 20) 
            {
                Projectile.localAI[0] += 0.05f * Projectile.scale;
            }else if (Projectile.timeLeft <= 20)
            {
                Projectile.localAI[0] = MathHelper.Lerp(Projectile.scale, 0, 1 - (Projectile.timeLeft / 20f));
            }
            if (Projectile.ai[1] == 1)
            {
                Projectile.rotation += MathHelper.ToRadians(0.7f);
            }
            else if (Projectile.ai[1] == -1)
            {
                Projectile.rotation += MathHelper.ToRadians(-0.7f);
            }
            Projectile.ai[2]++;
            if (Projectile.ai[2] % 60 == 0 && DLCUtils.HostCheck)
            {
                for (int i = 0; i < 7; i++)
                {
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, 200 * i).RotatedBy(Projectile.rotation), new Vector2(0, 4).RotatedBy(Projectile.rotation + MathHelper.PiOver2 + MathHelper.PiOver4), ModContent.ProjectileType<BrimstoneBarrage>(), Projectile.damage, 0);
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, 200 * i).RotatedBy(Projectile.rotation), new Vector2(0, 4).RotatedBy(Projectile.rotation - MathHelper.PiOver2 - MathHelper.PiOver4), ModContent.ProjectileType<BrimstoneBarrage>(), Projectile.damage, 0);
                }
            }

            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = owner.Center + new Vector2(20 * owner.spriteDirection, -60);
            
            //Projectile.rotation = MathHelper.PiOver2;
            base.AI();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        
    }
}
