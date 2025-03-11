
using CalamityMod;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Core.Systems;
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
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DarkIceCrystal : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Melee/DarkIceZero";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;

            Projectile.light = 1f;
            Projectile.coldDamage = true;
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
            float x = Projectile.localAI[0] / 100;
            float opacity = (float)-Math.Pow((2 * x - 1), 2) + 1;
            //Main.NewText(opacity);
            if (Projectile.localAI[0] >= 100)
            {
                opacity = 0;
                for (int i = 0; i < 7; i++)
                {
                    DLCUtils.DrawBackglow(t, Color.LightBlue * (1- (i / Projectile.oldPos.Length)), Projectile.oldPos[i] + Projectile.Size/2, t.Size() / 2, Projectile.rotation, Projectile.scale, SpriteEffects.None, 12, 2);
                    Main.EntitySpriteDraw(t.Value, Projectile.oldPos[i] + Projectile.Size/2 - Main.screenPosition, null, lightColor * (1 - ((float)i / Projectile.oldPos.Length)), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
                }
                DLCUtils.DrawBackglow(t, Color.LightBlue, Projectile.Center, t.Size() / 2, Projectile.rotation, Projectile.scale, SpriteEffects.None, 12, 2);
                Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None);
               
            }
            if (Projectile.ai[2] < 0 || Projectile.localAI[0] >= 100)
                return false;
            //DLCUtils.DrawBackglow(line, Color.Cyan * opacity, Projectile.Center, new Vector2(0, line.Height() / 2), new Vector2(5, 2), Projectile.rotation - MathHelper.PiOver2, SpriteEffects.None, 12, 2);
            if (Projectile.localAI[0] >= 50)
                opacity = 1f;
            Main.EntitySpriteDraw(line.Value, Projectile.Center - Main.screenPosition, null, Color.LightBlue * opacity, Projectile.rotation - MathHelper.PiOver2, new Vector2(0, line.Height() / 2), new Vector2(5, 2), SpriteEffects.None);
            
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
            //Main.NewText(Projectile.localAI[0]);
            if (Projectile.localAI[0] < 100)
            {
                Projectile.Center = owner.Center;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(targetvel), 0.1f);

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            }
            if (Projectile.localAI[0] == 100)
            {
                SoundEngine.PlaySound(SoundID.Item109, Projectile.Center);
                int dir = Projectile.ai[2] < 0 ? -1 : 1;

                float vel = WorldSavingSystem.MasochistModeReal ? 35 : 32;
                Projectile.velocity *= vel * dir;
            }
            if (Projectile.localAI[0] > 100) //leave a lingering trail
            {
                if (DLCUtils.HostCheck && Projectile.localAI[0] % 2 == 0)
                {
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center + Main.rand.NextVector2Circular(10, 10), Vector2.Zero, ModContent.ProjectileType<IceCloud>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
            }
            Projectile.localAI[0]++;

            base.AI();
        }
    }
}
