using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items
{
    public class CalamityAddonGlobalProjectile : GlobalProjectile
    {
        public List<int> HitBubble = new List<int>();
        public override bool InstancePerEntity => true;
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            return base.OnTileCollide(projectile, oldVelocity);
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(projectile, target, hit, damageDone);
        }

    }
}
