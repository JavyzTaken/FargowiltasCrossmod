using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.BrimstoneElemental
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BEGlobalProjectile : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation)
        {
            return (entity.type == ModContent.ProjectileType<BrimstoneBarrage>() || entity.type == ModContent.ProjectileType<BrimstoneHellfireball>() || entity.type == ModContent.ProjectileType<BrimstoneFireblast>());
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            Asset<Texture2D> t = TextureAssets.Projectile[projectile.type];
            if (NPC.AnyNPCs(ModContent.NPCType<CalamityMod.NPCs.BrimstoneElemental.BrimstoneElemental>())) {
                Vector2 frameSize = Vector2.Zero;
                if (projectile.type == ModContent.ProjectileType<BrimstoneBarrage>()) {
                    frameSize = new Vector2(18, 44);
                }
                if (projectile.type == ModContent.ProjectileType<BrimstoneFireblast>())
                {
                    frameSize = new Vector2(36, 50);
                }
                DLCUtils.DrawBackglow(t, new Color(100, 100, 100), projectile.Center, frameSize / 2, projectile.rotation, projectile.scale, offsetMult: 2, sourceRectangle: new Rectangle(0, (int)frameSize.Y * projectile.frame, (int)frameSize.X, (int)frameSize.Y));
            }
            return base.PreDraw(projectile, ref lightColor);
        }
    }
}
