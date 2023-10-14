/*
using CalamityMod.NPCs.SlimeGod;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class CrimsonPaladinSpike : ModProjectile
    {
        public const int TotalTime = 180;
        const int TelegraphTime = 45;
        const int ExtensionTime = 50;
        public float Length => 54 * Projectile.scale;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.hostile = true;
            Projectile.timeLeft = TotalTime;
            Projectile.scale = 1.5f;
            Projectile.hide = true;
        }
        public ref float Direction => ref Projectile.ai[0];
        public ref float BaseDistance => ref Projectile.ai[1];


        public override bool? CanDamage() => Projectile.timeLeft < TotalTime - TelegraphTime - (ExtensionTime / 2);
        public override void AI()
        {
            NPC slime = Projectile.GetSourceNPC();
            if (slime == null || !slime.active || slime.type != ModContent.NPCType<CrimulanPaladin>())
            {
                Projectile.Kill();
                return;
            }
            float extension = 0.2f;

            if (Projectile.timeLeft < TotalTime - TelegraphTime)
            {
                float extensionTimeMax = TotalTime - TelegraphTime;
                float extensionTimeMin = TotalTime - TelegraphTime - ExtensionTime;
                float modifier = (Projectile.timeLeft - extensionTimeMin) / extensionTimeMax;
                modifier = MathHelper.Clamp(modifier, 0, 1);
                extension = MathHelper.SmoothStep(0.7f, 0.2f, modifier);
            }
            
            float extensionOffset = (1 - extension) * Length;
            Projectile.Center = slime.Center + Direction.ToRotationVector2() * (BaseDistance - extensionOffset);
            Projectile.rotation = Direction;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 origin = texture.Size() / 2;
            SpriteEffects effects = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 drawOffset = Direction.ToRotationVector2() * Length / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center + drawOffset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects, 0);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Direction.ToRotationVector2() * Length * Projectile.scale, Projectile.width, ref num6))
            {
                return true;
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);

        }
    }
}
*/