using FargowiltasCrossmod.Content.Calamity.Items.Accessories.Enchantments;
using FargowiltasCrossmod.Content.Calamity.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles.BossWeapons;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Core.Calamity.Globals
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CalamityAddonGlobalProjectile : GlobalProjectile
    {
        //1 if a clone projectile, 2 if clone of a clone etc
        public int statigelClone = 0;
        public int statigelOriginNPC = -1;
        public int statigelHitTimer = 0;
        public override bool IsLoadingEnabled(Mod mod)
        {
            return FargowiltasCrossmod.EnchantLoadingEnabled;
        }
        public override bool InstancePerEntity => true;
        public override bool? CanHitNPC(Projectile projectile, NPC target)
        {
            if (statigelHitTimer > 0 && statigelOriginNPC == target.whoAmI)
            {
                statigelHitTimer--;
                return false;
            }
            return base.CanHitNPC(projectile, target);
        }
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (Main.LocalPlayer.HasEffect<StatigelEffect>())
            {
                StatigelEffect.StatigelProjEffect(projectile, null);
            }
            return base.OnTileCollide(projectile, oldVelocity);
        }
        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            
            
            return base.PreKill(projectile, timeLeft);
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.LocalPlayer.HasEffect<StatigelEffect>())
            {
                //StatigelEffect.StatigelProjEffect(projectile, target);
            }
            base.OnHitNPC(projectile, target, hit, damageDone);
        }
        public override bool PreAI(Projectile projectile)
        {
            if (Main.netMode != NetmodeID.Server &&  projectile.Hitbox.Intersects(Main.LocalPlayer.Hitbox))
            {
                StatigelEffect.StatigelProjEffect2(projectile, Main.LocalPlayer);
            }
            return base.PreAI(projectile);
        }

    }
}
