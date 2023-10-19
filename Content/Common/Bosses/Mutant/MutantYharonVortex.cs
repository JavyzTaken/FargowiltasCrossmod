using CalamityMod;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MutantYharonVortex : YharonBulletHellVortex
    {
        public const int ThrowTime = 120;
        public override void AI()
        {
            NPC mutant = Main.npc[(int)(Projectile.ai[1])];
            if (mutant.active && mutant.type == ModContent.NPCType<MutantBoss>())
            {
                if (TimeCountdown > 0f)
                {
                    
                    if (Projectile.scale < 1f)
                    {
                        Projectile.scale = MathHelper.Clamp(Projectile.scale + 0.05f, 0f, 1f);
                    }
                    
                    Projectile.velocity = (mutant.Center - Projectile.Center);
                }
                else
                {
                    if (TimeCountdown == 0f)
                    {
                        if (mutant.target.WithinBounds(Main.maxPlayers))
                        {
                            Player player = Main.player[mutant.target];
                            if (player.active && !player.dead)
                            {
                                Projectile.velocity = Projectile.DirectionTo(player.Center) * 10f;
                            }
                        }
                    }
                    if (TimeCountdown <= -(ThrowTime - 20))
                    {
                        Projectile.scale = MathHelper.Clamp(Projectile.scale - 0.05f, 0f, 1f);
                    }
                    if (TimeCountdown < -ThrowTime)
                    {
                        Projectile.Kill();
                    }
                }
                TimeCountdown -= 1f;
            }
            else
            {
                Projectile.Kill();
            }
        }
    }
}
