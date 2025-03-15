using CalamityMod.NPCs.CalClone;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using FargowiltasSouls.Assets.ExtraTextures;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    public class BrothersDashTrail : ModProjectile, IPixelatedPrimitiveRenderer
    {
        public PixelationPrimitiveLayer LayerToRenderTo => PixelationPrimitiveLayer.BeforeNPCs;
        public override string Texture => FargoSoulsUtil.EmptyTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.scale = 1f;
            Projectile.timeLeft = 180;

            Projectile.hide = true;
        }
        public ref float NPCID => ref Projectile.ai[0];
        public ref float Duration => ref Projectile.ai[1];
        public ref float Timer => ref Projectile.ai[2];
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (npc.TypeAlive<Cataclysm>() || npc.TypeAlive<Catastrophe>())
            {
                var dlc = npc.GetDLCBehavior<CalamitasBrothersEternity>();
                if (dlc.State != (int)CalamitasBrothersEternity.States.Dash && dlc.State != (int)CalamitasBrothersEternity.States.SpinDashes)
                {
                    Projectile.Kill();
                    return;
                }
                Projectile.Center = npc.Center + Offset(npc);
                Projectile.velocity = npc.velocity;
            }
            else
            {
                Projectile.Kill();
                return;
            }
            Timer++;
            if (Timer > Duration - 40)
            {
                Projectile.Opacity -= (1f / 20);
            }
            if (Timer > Duration - 20)
            {
                Projectile.Kill();
                return;
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if (Projectile.hide)
                behindNPCs.Add(index);
        }
        public static Vector2 Offset(NPC npc) => (npc.rotation + MathHelper.PiOver2).ToRotationVector2() * -20;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * 40 * 3.6f;
            return MathHelper.SmoothStep(baseWidth, baseWidth * 0.8f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            float threshold = 0.08f;
            float opacity = 1f;
            if (completionRatio < threshold)
                opacity *= MathF.Pow(completionRatio / threshold, 2);
            return Color.Lerp(Color.Red, Color.Transparent, completionRatio) * 0.4f * opacity * Projectile.Opacity;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ManagedShader shader = ShaderManager.GetShader("FargowiltasCrossmod.FireTrail");
            FargoSoulsUtil.SetTexture1(FargosTextureRegistry.MagmaStreak.Value);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, _ => Projectile.Size * 0.5f, Pixelate: true, Shader: shader), 44);
        }
    }
}
