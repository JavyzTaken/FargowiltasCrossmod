using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using ThoriumMod;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod("ThoriumMod")]
    public class Templar_Fire : ThoriumMod.Projectiles.Healer.HolyFirePro
    {
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            base.OnTileCollide(oldVelocity);

            if (Main.myPlayer == Projectile.owner)
            {
                int HealingOrbType = ModContent.ProjectileType<ThoriumMod.Projectiles.Healer.HealingOrbYellow>();
                for (int i = 0; i < Main.rand.Next(4, 6); i++)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_Death(),
                                 Projectile.Center,
                                 new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, 3)),
                                 HealingOrbType,
                                 0,
                                 0f,
                                 Projectile.owner);

                    Main.projectile[proj].timeLeft -= Main.rand.Next(0, 10) * 6;
                }
            }

            return true;
        }
    }
}
