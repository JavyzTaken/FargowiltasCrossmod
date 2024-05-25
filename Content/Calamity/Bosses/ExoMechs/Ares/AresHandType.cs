using CalamityMod.Particles;
using Luminance.Assets;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Ares
{
    /// <summary>
    /// Represents a type of hand that Ares may use.
    /// </summary>
    /// <param name="NameLocalizationKey">The localization key for the hand's display name.</param>
    /// <param name="TexturePath">The path to the hand's texture.</param>
    /// <param name="GlowmaskPath">The path to the hand's glowmask texture.</param>
    /// <param name="TotalHorizontalFrames">The amount of horizontal frames on the texture's sheet.</param>
    /// <param name="TotalVerticalFrames">The amount of vertical frames on the texture's sheet.</param>
    /// <param name="EnergyTelegraphColor">The color of energy particles generated prior to attacking via the <see cref="AresCannonChargeParticleSet"/>.</param>
    /// <param name="ExtraDrawAction">An optional secondary action that should be taken when drawing the hand.</param>
    /// <param name="CustomGoreNames">The path for custom gore types.</param>
    public record AresHandType(string NameLocalizationKey, string TexturePath, string GlowmaskPath, int TotalHorizontalFrames, int TotalVerticalFrames, Color EnergyTelegraphColor, Action<NPC, Vector2>? ExtraDrawAction, params string[] CustomGoreNames)
    {
        /// <summary>
        /// The representation of Ares' plasma cannon.
        /// </summary>
        public static readonly AresHandType PlasmaCannon = new("Mods.CalamityMod.NPCs.AresPlasmaFlamethrower.DisplayName", "CalamityMod/NPCs/ExoMechs/Ares/AresPlasmaFlamethrower", "CalamityMod/NPCs/ExoMechs/Ares/AresPlasmaFlamethrowerGlow", 6, 8, Color.GreenYellow,
            null, "CalamityMod/AresPlasmaFlamethrower1", "CalamityMod/AresPlasmaFlamethrower2");

        /// <summary>
        /// The representation of Ares' tesla cannon.
        /// </summary>
        public static readonly AresHandType TeslaCannon = new("Mods.CalamityMod.NPCs.AresTeslaCannon.DisplayName", "CalamityMod/NPCs/ExoMechs/Ares/AresTeslaCannon", "CalamityMod/NPCs/ExoMechs/Ares/AresTeslaCannonGlow", 6, 8, Color.Aqua,
            null, "CalamityMod/AresTeslaCannon1", "CalamityMod/AresTeslaCannon2");

        /// <summary>
        /// The representation of Ares' laser cannon.
        /// </summary>
        public static readonly AresHandType LaserCannon = new("Mods.CalamityMod.NPCs.AresLaserCannon.DisplayName", "CalamityMod/NPCs/ExoMechs/Ares/AresLaserCannon", "CalamityMod/NPCs/ExoMechs/Ares/AresLaserCannonGlow", 6, 8, Color.OrangeRed,
            null, "CalamityMod/AresLaserCannon1", "CalamityMod/AresLaserCannon2");

        /// <summary>
        /// The representation of Ares' gauss nuke.
        /// </summary>
        public static readonly AresHandType GaussNuke = new("Mods.CalamityMod.NPCs.AresGaussNuke.DisplayName", "CalamityMod/NPCs/ExoMechs/Ares/AresGaussNuke", "CalamityMod/NPCs/ExoMechs/Ares/AresGaussNukeGlow", 9, 12, Color.Yellow,
            null, "CalamityMod/AresGaussNuke1", "CalamityMod/AresGaussNuke2", "CalamityMod/AresGaussNuke3");

        /// <summary>
        /// The representation of Ares' energy katana.
        /// </summary>
        public static readonly AresHandType EnergyKatana = new("Mods.FargowiltasCrossmod.NPCs.AresHand.EnergyKatanaDisplayName", "FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Ares/AresEnergyKatana", "FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Ares/AresEnergyKatanaGlow", 1, 1, Color.Red,
            DrawEnergyKatana);

        /// <summary>
        /// Writes the state of this arm type to a given <see cref="BinaryWriter"/>, for the purposes of being sent across the network.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(EnergyTelegraphColor.R);
            writer.Write(EnergyTelegraphColor.G);
            writer.Write(EnergyTelegraphColor.B);
            writer.Write(EnergyTelegraphColor.A);

            writer.Write(NameLocalizationKey);
            writer.Write(TexturePath);
            writer.Write(GlowmaskPath);
            writer.Write(TotalHorizontalFrames);
            writer.Write(TotalVerticalFrames);
        }

        /// <summary>
        /// Constructs an Ares arm type from data in a <see cref="BinaryReader"/>, for the purposes of being received across the network.
        /// </summary>
        /// <param name="reader"></param>
        public static AresHandType ReadFrom(BinaryReader reader)
        {
            byte energyR = reader.ReadByte();
            byte energyG = reader.ReadByte();
            byte energyB = reader.ReadByte();
            byte energyA = reader.ReadByte();

            // TODO -- Make this more robust. Currently doesn't include the draw delegate or gores.
            return new(reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadInt32(), reader.ReadInt32(), new(energyR, energyG, energyB, energyA), null);
        }

        // TODO -- This should probably be in AresHand.cs?
        /// <summary>
        /// Draws the katana on top of the actual energy katana.
        /// </summary>
        /// <param name="npc">The katana's NPC instance.</param>
        /// <param name="drawPosition">The draw position of the katana.</param>
        public static void DrawEnergyKatana(NPC npc, Vector2 drawPosition)
        {
            if (!npc.As<AresHand>().KatanaInUse)
                return;

            float squishInterpolant = Utils.Remap(npc.position.Distance(npc.oldPosition), 30f, 50f, 0f, 0.6f);

            int bladeFrameNumber = (int)((Main.GlobalTimeWrappedHourly * 16f + npc.whoAmI * 7.13f) % 9f);
            float bladeRotation = npc.rotation + npc.spriteDirection * MathHelper.PiOver2;
            Texture2D bladeTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/PhaseslayerBlade").Value;
            Rectangle bladeFrame = bladeTexture.Frame(3, 7, bladeFrameNumber / 7, bladeFrameNumber % 7);
            Vector2 bladeOrigin = bladeFrame.Size() * new Vector2(0.5f, 1f);
            Vector2 bladeDrawPosition = drawPosition - npc.rotation.ToRotationVector2() * npc.scale * npc.spriteDirection * -24f;
            Vector2 bladeScale = new Vector2(1f - squishInterpolant, 1f) * npc.scale;
            Vector2 bloomScale = new Vector2(1f, 1f + squishInterpolant * 2f) * npc.scale;
            Color bloomColor = Color.Lerp(Color.Crimson, Color.Wheat, squishInterpolant * 0.7f);
            SpriteEffects bladeDirection = npc.spriteDirection.ToSpriteDirection();

            Texture2D bloom = MiscTexturesRegistry.BloomCircleSmall.Value;
            Main.EntitySpriteDraw(bloom, bladeDrawPosition, null, npc.GetAlpha(bloomColor) with { A = 0 } * 0.6f, npc.rotation, bloom.Size() * new Vector2(0.2f, 0.5f), bloomScale * new Vector2(2.6f, 1.56f), bladeDirection, 0);
            Main.EntitySpriteDraw(bloom, bladeDrawPosition, null, npc.GetAlpha(bloomColor) with { A = 0 } * 0.7f, npc.rotation, bloom.Size() * new Vector2(0.2f, 0.5f), bloomScale * new Vector2(2.6f, 1.1f), bladeDirection, 0);
            Main.EntitySpriteDraw(bloom, bladeDrawPosition, null, npc.GetAlpha(Color.Red) with { A = 0 } * 0.7f, 0f, bloom.Size() * 0.5f, npc.scale, bladeDirection, 0);

            Main.EntitySpriteDraw(bladeTexture, bladeDrawPosition, bladeFrame, npc.GetAlpha(Color.White), bladeRotation, bladeOrigin, bladeScale, bladeDirection, 0);
        }
    }
}
