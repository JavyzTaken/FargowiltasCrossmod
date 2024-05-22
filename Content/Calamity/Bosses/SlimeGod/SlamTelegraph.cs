using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.NPCs.SlimeGod;
using FargowiltasCrossmod.Content.Common.Bosses.Mutant;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlamTelegraph : ModProjectile
    {
        public ref float Timer => ref Projectile.ai[0];

        public ref float Width => ref Projectile.ai[1];
        public float Duration = 45;
        public const int maxLength = 600;
        public float Length = 0;
        bool Crimson;

        // Can be anything.
        public override string Texture => "Terraria/Images/Extra_" + ExtrasID.MartianProbeScanWave;

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;

        public override void SetDefaults()
        {
            Projectile.timeLeft = (int)Duration;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.hide = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(owner);
            writer.Write(npc);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            owner = reader.Read7BitEncodedInt();
            npc = reader.ReadBoolean();
        }
        int owner;
        bool npc = true;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC parentNpc && (parentNpc.type == ModContent.NPCType<CrimulanPaladin>() || parentNpc.type == ModContent.NPCType<EbonianPaladin>()))
            {
                owner = parentNpc.whoAmI;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Crimson = parentNpc.type == ModContent.NPCType<CrimulanPaladin>();
                npc = true;
            }
            else if (source is EntitySource_Parent parent2 && parent2.Entity is Projectile parentProj && parentProj.type == ModContent.ProjectileType<MutantSlimeGod>())
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                owner = parentProj.whoAmI;
                Crimson = parentProj.ai[0] == 1;
                npc = false;
            }
            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            if (npc)
            {
                NPC parent = FargoSoulsUtil.NPCExists(owner);
                if (parent != null)
                {
                    Projectile.Center = parent.Center;// + Vector2.UnitY * parent.height / 2;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            else
            {
                Projectile parent = FargoSoulsUtil.ProjectileExists(owner);
                if (parent != null)
                {
                    Projectile.Center = parent.Center;// + Vector2.UnitY * parent.height / 2;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            Length = maxLength * Math.Min((float)Math.Pow(Timer / 60f, 0.3f), 1f);
            Timer++;
        }

        public override bool ShouldUpdatePosition() => false;

        public float WidthFunction(float progress) => Length;


        public Color ColorFunction(float progress)
        {
            float opacity = Math.Min(Timer / 30f, Math.Min(Projectile.timeLeft / 15f, 1));
            opacity *= 0.4f;
            Color mainColor = Crimson ? Color.Crimson : Color.Lavender;
            float modifier = 1 - progress;//2 * Math.Abs(progress - 0.5f);
            return Color.Lerp(Color.Transparent, mainColor, opacity * modifier);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            ManagedShader shader = ShaderManager.GetShader("FargowiltasSouls.Vertex_ArcTelegraph");

            FargoSoulsUtil.SetTexture1(ModContent.Request<Texture2D>("Terraria/Images/Extra_193").Value);
            if (Crimson)
            {
                shader.TrySetParameter("mainColor", Color.Lerp(Color.Crimson, Color.OrangeRed, 0.7f));
            }
            else
            {
                shader.TrySetParameter("mainColor", Color.Lerp(Color.Lavender, Color.Purple, 0.7f));
            }

            //shader.Apply();

            //VertexStrip vertexStrip = new();
            List<Vector2> positions = [];
            //List<float> rotations = new();
            for (float i = 0; i < 1; i += 0.005f)
            {
                positions.Add(Projectile.rotation.ToRotationVector2() * Length + Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * Width * (-0.5f + i));
                //rotations.Add(Projectile.rotation + MathHelper.PiOver2);
            }
            PrimitiveRenderer.RenderTrail(positions, new(WidthFunction, ColorFunction, Shader: shader), 30);
            //vertexStrip.PrepareStrip(positions.ToArray(), rotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
            //vertexStrip.DrawTrail();
            //Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCsAndTiles.Add(index);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }
    }
}
