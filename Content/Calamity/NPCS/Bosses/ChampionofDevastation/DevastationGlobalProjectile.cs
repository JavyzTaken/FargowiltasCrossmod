using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Projectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation
{
    [JITWhenModsEnabled("CalamityMod")]
    [ExtendsFromMod("CalamityMod")]
    public class DevastationGlobalProjectile : GlobalProjectile
    {
        public int counter;
        public override bool InstancePerEntity => true;
        public override bool PreAI(Projectile projectile)
        {

            if (projectile.type == ModContent.ProjectileType<GlowLine>() && projectile.ai[0] == 20)
            {
                projectile.hide = true;
                GlowLine glowline = (GlowLine)projectile.ModProjectile;
                glowline.color = Color.Magenta;
                int maxTime = 50;
                int alphaModifier = 3;
                projectile.rotation = projectile.ai[1];
                if (++counter > maxTime)
                {
                    projectile.Kill();
                    return false;
                }
                if (alphaModifier >= 0)
                {
                    projectile.alpha = 255 - (int)(255 * Math.Sin(Math.PI / maxTime * counter) * alphaModifier);
                    if (projectile.alpha < 50)
                        projectile.alpha = 50;
                }
                return false;
            }
            return base.PreAI(projectile);
        }
        public override void DrawBehind(Projectile projectile, int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (projectile.type == ModContent.ProjectileType<GlowLine>() && projectile.ai[0] == 20)
            {
                behindNPCsAndTiles.Add(index);

            }
            base.DrawBehind(projectile, index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
    }
}
