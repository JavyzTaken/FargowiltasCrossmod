using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using ReLogic.Content;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class WulfrumScrap : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Materials/WulfrumMetalScrap";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 18;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 100;
            Projectile.timeLeft = 500;
            Projectile.scale = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void OnKill(int timeLeft)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Item[ModContent.ItemType<CalamityMod.Items.Materials.WulfrumMetalScrap>()];

            Main.EntitySpriteDraw(t.Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, t.Width(), t.Height()), lightColor * (1 - 255 / (float)(Projectile.alpha + 1)), Projectile.rotation, t.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override void AI()
        {
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Zero, 0.03f);
            Projectile.rotation += Projectile.velocity.Length() / 10;
            if (Projectile.ai[0] == 180)
            {
                Projectile.alpha = 0;
                Projectile.hostile = true;
                for (int i = 0; i < 10; i++)
                {
                    Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GemEmerald).noGravity = true;
                }
            }
            Projectile.ai[0]++;

        }
    }
}
