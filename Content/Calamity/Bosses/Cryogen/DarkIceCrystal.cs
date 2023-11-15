using CalamityMod;
using FargowiltasCrossmod.Core.Utils;
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

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    public class DarkIceCrystal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/DarkIceZero";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 300;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> line = TextureAssets.Extra[178];
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            float x = Projectile.ai[2] / 100;
            float opacity = (float)-Math.Pow((2 * x - 1), 2) + 1;
            //Main.NewText(opacity);
            if (Projectile.ai[2] >= 100)
            {
                opacity = 0;
                for (int i = 0; i < 7; i++)
                {
                    DLCUtils.DrawBackglow(t, Color.Blue * (1- (i / Projectile.oldPos.Length)), Projectile.oldPos[i] + Projectile.Size/2, t.Size() / 2, Projectile.rotation, Projectile.scale, SpriteEffects.None, 12, 2);
                    Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size/2 - Main.screenPosition, null, lightColor * (1 - ((float)i / Projectile.oldPos.Length)), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
                }
                DLCUtils.DrawBackglow(t, Color.Blue, Projectile.Center, t.Size() / 2, Projectile.rotation, Projectile.scale, SpriteEffects.None, 12, 2);
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
               
            }
            //DLCUtils.DrawBackglow(line, Color.Cyan * opacity, Projectile.Center, new Vector2(0, line.Height() / 2), new Vector2(5, 2), Projectile.rotation - MathHelper.PiOver2, SpriteEffects.None, 12, 2);
            Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition, null, Color.Cyan * opacity, Projectile.rotation - MathHelper.PiOver2, new Vector2(0, line.Height() / 2), new Vector2(5, 2), SpriteEffects.None);
            
            return false;
        }
        public override void AI()
        {
            Player target = Main.player[(int)Projectile.ai[1]];
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            if (target == null || owner == null || !owner.active || !target.active || owner.type != ModContent.NPCType<PermafrostBoss>())
            {
                Projectile.Kill();
                return;
            }
            
            Vector2 targetvel = CalamityUtils.CalculatePredictiveAimToTarget(Projectile.Center, target, 35);
            //Main.NewText(Projectile.ai[2]);
            if (Projectile.ai[2] < 100)
            {
                Projectile.Center = owner.Center;
                Projectile.rotation = targetvel.ToRotation() + MathHelper.PiOver2;
            }
            if (Projectile.ai[2] == 100)
            {
                SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                Projectile.velocity = targetvel.SafeNormalize(Vector2.Zero) * 35;
            }
            Projectile.ai[2]++;

            base.AI();
        }
    }
}
