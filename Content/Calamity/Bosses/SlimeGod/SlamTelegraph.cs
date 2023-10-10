﻿using CalamityMod.NPCs.TownNPCs;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Primitives;
using FargowiltasSouls.Common.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Bosses.Lifelight;
using System.IO;
using Terraria.DataStructures;
using Terraria.ID;
using CalamityMod.NPCs.SlimeGod;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlamTelegraph : ModProjectile
    {
        public PrimDrawer TelegraphDrawer
        {
            get;
            private set;
        }
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
            writer.Write7BitEncodedInt(npc);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc = reader.Read7BitEncodedInt();
        }
        int npc;
        public override void OnSpawn(IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC parentNpc && (parentNpc.type == ModContent.NPCType<CrimulanPaladin>() || parentNpc.type == ModContent.NPCType<EbonianPaladin>()))
            {
                npc = parentNpc.whoAmI;
                Projectile.rotation = Vector2.UnitY.ToRotation();
                Crimson = parentNpc.type == ModContent.NPCType<CrimulanPaladin>();
            }
        }

        public override void AI()
        {
            NPC parent = FargoSoulsUtil.NPCExists(npc);
            if (parent != null)
            {
                Projectile.Center = parent.Center;// + Vector2.UnitY * parent.height / 2;
                Projectile.rotation = Vector2.UnitY.ToRotation();
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
            float modifier = 2 * Math.Abs(progress - 0.5f);
            return Color.Lerp(Color.Transparent, mainColor, opacity * modifier);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Shader shader = ShaderManager.GetShaderIfExists("Vertex_ArcTelegraph");

            FargoSoulsUtil.SetTexture1(ModContent.Request<Texture2D>("Terraria/Images/Extra_193").Value);
            if (Crimson)
            {
                shader.SetMainColor(Color.Lerp(Color.Crimson, Color.OrangeRed, 0.7f));
            }
            else
            {
                shader.SetMainColor(Color.Lerp(Color.Lavender, Color.Purple, 0.7f));
            }
            
            shader.Apply();

            VertexStrip vertexStrip = new();
            List<Vector2> positions = new();
            List<float> rotations = new();
            for (float i = 0; i < 1; i += 0.005f)
            {
                positions.Add(Projectile.rotation.ToRotationVector2() * Length + Projectile.Center + Vector2.UnitX * Width * (-0.5f + i));
                rotations.Add(Projectile.rotation + MathHelper.PiOver2);
            }
            vertexStrip.PrepareStrip(positions.ToArray(), rotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
            vertexStrip.DrawTrail();
            Main.spriteBatch.ExitShaderRegion();
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