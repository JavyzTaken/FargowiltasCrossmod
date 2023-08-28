using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation
{
    
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class PlantHook : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.PlanterasHook;
        public override void SetStaticDefaults()
        {


        }
        public override void SetDefaults()
        {
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.alpha = 0;
            Projectile.width = 56;
            Projectile.height = 50;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Main.projFrames[Type] = 4;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = 2;
        }
        public override void Kill(int timeLeft)
        {

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player target = Main.player[Main.npc[(int)Projectile.ai[0]].target];
            if (Projectile.Center.Y > target.Center.Y)
            {
                Projectile.frame = 0;
                Projectile.velocity *= 0;
            }
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, owner.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            Vector2 pos = new Vector2(Projectile.position.X + Projectile.width / 2, Projectile.position.Y + Projectile.height / 2);
            float num6 = owner.Center.X - pos.X;
            float num7 = owner.Center.Y - pos.Y;
            float rotation = (float)Math.Atan2((double)num7, (double)num6) - 1.57f;
            bool flag3 = true;
            while (flag3)
            {
                int num8 = 16;
                int num9 = 32;
                float num10 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                if (num10 < num9)
                {
                    num8 = (int)num10 - num9 + num8;
                    flag3 = false;
                }
                num10 = num8 / num10;
                num6 *= num10;
                num7 *= num10;
                pos.X += num6;
                pos.Y += num7;
                num6 = owner.Center.X - pos.X + owner.netOffset.X;
                num7 = owner.Center.Y - pos.Y + owner.netOffset.Y;
                Color color = Lighting.GetColor((int)pos.X / 16, (int)(pos.Y / 16f));
                Main.spriteBatch.Draw(TextureAssets.Chain26.Value, new Vector2(pos.X - Main.screenPosition.X, pos.Y - Main.screenPosition.Y), new Rectangle?(new Rectangle(0, 0, TextureAssets.Chain26.Width(), num8)), color, rotation, new Vector2(TextureAssets.Chain26.Width() * 0.5f, TextureAssets.Chain26.Height() * 0.5f), 1f, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void AI()
        {

            Player target = Main.player[Main.npc[(int)Projectile.ai[0]].target];
            NPC owner = Main.npc[(int)Projectile.ai[0]];
            Projectile.rotation = Projectile.AngleFrom(owner.Center) + MathHelper.PiOver2;

            if (Projectile.Center.Y > target.Center.Y && WorldGen.SolidTile(Projectile.Center.ToTileCoordinates()))
            {
                Projectile.frame = 0;
                Projectile.velocity *= 0;
            }
        }
    }
}
