
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class FungusBall : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/PuffWarrior";
        public override void SetStaticDefaults()
        {
            
        }
        public override void SetDefaults()
        {
            
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.height = 22;
            Projectile.width = 22;
            Projectile.timeLeft = 500;
            Main.projFrames[Type] = 10;
            Projectile.Opacity = 1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            
        }
        
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // If the projectile hits the left or right side of the tile, reverse the X velocity
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            // If the projectile hits the top or bottom side of the tile, reverse the Y velocity
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 284, 32, 32), lightColor * Projectile.Opacity, Projectile.rotation, new Vector2(16, 16), Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.MushroomSpray, Alpha: 120, Scale: 2).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 8; i++)
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, new Vector2(-5, 0).RotatedBy(i * MathHelper.ToRadians(25.76f)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), Projectile.damage, 0);
            }
        }
        public override void AI()
        {
            Projectile.rotation += MathHelper.ToRadians(10);
            if (Projectile.velocity.Y < 10)
            {
                Projectile.velocity.Y += 0.3f;
            }
        }
    }
}
