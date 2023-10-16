using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
/*
namespace FargowiltasCrossmod.Content.Calamity.Bosses.Cryogen
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class PermafrostHeldWeapon : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Magic/IcicleTrident";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.timeLeft = 100;
            Projectile.hostile = false;
            Projectile.friendly = false;
            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[Type];
            Vector2 origin = new Vector2(0, t.Height());
            float maxscale = 1;
            if (Projectile.ai[0] == 2)
            {
                t = TextureAssets.Projectile[ModContent.ProjectileType<ArcticPaw>()];
                origin = t.Size() / 2;
                maxscale = 2;
            }
            if (Projectile.localAI[0] < maxscale)
            {
                Projectile.localAI[0] += 0.03f;
            }
            float scale = MathHelper.Lerp(0, Projectile.scale, Projectile.localAI[0]);
            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, origin, scale, SpriteEffects.None);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override void AI()
        {
            NPC owner = Main.npc[(int)Projectile.ai[2]];

            if (owner == null || !owner.active || owner.type != ModContent.NPCType<PermafrostBoss>())
            {
                Projectile.Kill();
                return;
            }
            float angle = Projectile.ai[1];
            float scale = 1;
            if (Projectile.ai[0] == 0)
            {
                angle += MathHelper.PiOver4;
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, angle, 0.05f);
                if (Projectile.localAI[0] < scale)
                {
                    Vector2 thing = new Vector2(0, -50 * Projectile.scale).RotatedBy(Projectile.rotation);
                    if (Projectile.ai[0] == 0) thing = thing.RotatedBy(MathHelper.PiOver4);
                    for (int i = 0; i < 5; i++)
                        Dust.NewDustDirect(Projectile.Center + thing * Main.rand.NextFloat(0f, 1f), 0, 0, DustID.SnowflakeIce).noGravity = true;
                }
                if (Projectile.timeLeft == 1)
                {
                    Vector2 thing = new Vector2(0, -50 * Projectile.scale).RotatedBy(Projectile.rotation);
                    if (Projectile.ai[0] == 0) thing = thing.RotatedBy(MathHelper.PiOver4);
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDustDirect(Projectile.Center + thing * Main.rand.NextFloat(0f, 1f), 0, 0, DustID.SnowflakeIce).noGravity = true;
                    }
                }
            }
            if (Projectile.ai[0] == 2)
            {
                Projectile.Opacity = 0.7f;
                Projectile.rotation = owner.velocity.ToRotation() + MathHelper.PiOver2;
                scale = 2;
                if (Projectile.localAI[0] < scale)
                {
                    Projectile.Resize((int)(Projectile.localAI[0] * 30), (int)(Projectile.localAI[0] * 30));
                    for (int i = 0; i < 5; i++)
                        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                }
                if (Projectile.timeLeft == 1)
                {
                    
                    for (int i = 0; i < 50; i++)
                    {
                        Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce).noGravity = true;
                    }
                }
            }
            if (Projectile.localAI[0] == 0.06f)
            {
                if (Projectile.ai[0] == 2)
                {
                    Projectile.timeLeft =160;
                }
            }
            Projectile.Center = owner.Center;
            
            
            base.AI();
        }
    }
}
*/