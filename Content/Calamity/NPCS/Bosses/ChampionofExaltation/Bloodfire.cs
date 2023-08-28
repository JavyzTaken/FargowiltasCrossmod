using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation
{
    
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class Bloodfire : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Main.projFrames[Type] = 7;
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 35;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.hide = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Asset<Texture2D> texture = TextureAssets.Projectile[Type];
            Main.EntitySpriteDraw(texture.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 98, 98, 98), new Color(200, 20, 20), Projectile.rotation, new Vector2(98, 98) / 2, 2, SpriteEffects.None, 0);
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override void AI()
        {
            if (Projectile.timeLeft % 5 == 0)
            {
                Projectile.frame++;

            }
            Projectile.rotation += MathHelper.ToRadians(5);
        }

    }
}
