using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ShroomGas : ModProjectile
    {
        public override string Texture => "FargowiltasCrossmod/Content/Calamity/Bosses/Crabulon/ShroomGas1";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {

            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.height = 22;
            Projectile.width = 22;
            Projectile.timeLeft = 200;
            Projectile.tileCollide = false;
            Projectile.Opacity = 1;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> gas = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/Crabulon/ShroomGas1");
            if (Projectile.ai[0] == 1)
            {
                gas = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/Crabulon/ShroomGas2");
            }
            else if (Projectile.ai[0] == 2)
            {
                gas = ModContent.Request<Texture2D>("FargowiltasCrossmod/Content/Calamity/Bosses/Crabulon/ShroomGas3");
            }
            Main.EntitySpriteDraw(gas.Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation, gas.Size() / 2, Projectile.scale, SpriteEffects.None);
            return false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.ai[0] = Main.rand.Next(0, 3);
            Projectile.ai[1] = Main.rand.NextFloat() * (Main.rand.NextBool() ? 1 : -1);
        }
        public override void AI()
        {
            if (Projectile.timeLeft <= 30)
            {
                Projectile.Opacity -= 0.0334f;
            }
            if (Projectile.velocity.Length() > 1)
            {
                Projectile.velocity /= 1.05f;
            }
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.1f, 0.4f));
            Projectile.rotation += MathHelper.ToRadians(Projectile.ai[1]);
            if (Main.rand.NextBool(10))
                Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GlowingMushroom, Alpha: 100).noGravity = true;
        }
    }
}
