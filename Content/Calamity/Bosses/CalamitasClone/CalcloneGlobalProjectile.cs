using CalamityMod.Dusts;
using CalamityMod.NPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CalamityMod.NPCs.CalClone;
using Luminance.Common.Utilities;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CalcloneGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        bool Applies = false;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (CalamityGlobalNPC.calamitas >= 0 && Main.npc[CalamityGlobalNPC.calamitas] is NPC n && n.type == ModContent.NPCType<CalamityMod.NPCs.CalClone.CalamitasClone>() && n.TryGetDLCBehavior(out CalamitasCloneEternity emode) && emode != null && 
                source is EntitySource_Parent parent && parent.Entity is NPC npc)
            {
                if (npc.type == ModContent.NPCType<CalamityMod.NPCs.CalClone.CalamitasClone>())
                {
                    Applies = true;
                }
            }
        }
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (Applies)
            {
                Asset<Texture2D> t = TextureAssets.Projectile[projectile.type];
                Vector2 frameSize = new(t.Value.Width, t.Value.Height / Main.projFrames[projectile.type]);

                DrawBlackBorder(projectile, t, projectile.Center, frameSize / 2, projectile.rotation, projectile.scale, offsetMult: 5, sourceRectangle: new Rectangle(0, (int)frameSize.Y * projectile.frame, (int)frameSize.X, (int)frameSize.Y));
            }
            return base.PreDraw(projectile, ref lightColor);
        }
        public void DrawBlackBorder(Projectile proj, Asset<Texture2D> texture, Vector2 position, Vector2 origin, float rotation = 0, float scale = 1, SpriteEffects spriteEffects = SpriteEffects.None, int iterations = 12, float offsetMult = 1, Rectangle? sourceRectangle = null)
        {
            for (int j = 0; j < iterations; j++)
            {
                Vector2 afterimageOffset = (MathHelper.TwoPi * j / iterations).ToRotationVector2() * offsetMult;
                Color glowColor = new(40, 24, 48);
                glowColor *= 0.7f * proj.Opacity;

                Main.EntitySpriteDraw(texture.Value, position + afterimageOffset - Main.screenPosition, sourceRectangle, glowColor, rotation, origin, scale, spriteEffects);
            }
        }
    }
}
