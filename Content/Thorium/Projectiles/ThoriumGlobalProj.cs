using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod("ThoriumMod")]
    public class ThoriumGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Player player = Main.player[Main.myPlayer];
                if (player.GetModPlayer<CrossplayerThorium>().GildedBinoculars)
                {
                    Lighting.AddLight(projectile.Center, new Vector3(0.6f, 0.6f, 0.6f));
                }
            }
            base.PostDraw(projectile, lightColor);
        }

        public override bool PreAI(Projectile projectile)
        {
            // Myna debuff projectile-dodging effect for players
            if (Main.player[projectile.owner].HasBuff<Buffs.MynaDB>())
            {
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.active) continue;
                    if (!Main.player[projectile.owner].CanNPCBeHitByPlayerOrPlayerProjectile(npc, projectile)) continue;
                    if (projectile.Center.Distance(npc.Center) > 192f) continue;
                    Vector2 a = npc.Center - projectile.Center;
                    float angle = projectile.velocity.ToRotation() - a.ToRotation();
                    if (MathF.Abs(angle) < MathF.PI / 6)
                    {
                        projectile.velocity = projectile.velocity.RotatedBy(MathF.PI / 60f * MathF.Sign(angle));
                    }
                }
            }

            if (projectile.hostile)
            {
                var player = Main.player[Main.myPlayer];
                if (player.GetModPlayer<CrossplayerThorium>().MynaAccessory && projectile.Center.Distance(player.Center) <= 192f)
                {
                    Vector2 a = player.Center - projectile.Center;
                    float angle = projectile.velocity.ToRotation() - a.ToRotation();
                    if (MathF.Abs(angle) < MathF.PI / 8)
                    {
                        projectile.velocity = projectile.velocity.RotatedBy(MathF.PI / 240f * MathF.Sign(angle));
                    }
                }
            }
            return base.PreAI(projectile);
        }
    }
}
