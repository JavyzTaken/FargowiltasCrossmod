using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;

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
    /// <param name="SwapSound">An optional sound type to play when this hand is swapped to by Ares.</param>
    /// <param name="CustomGoreNames">The path for custom gore types.</param>
    public record AresHandType(string NameLocalizationKey, string TexturePath, string GlowmaskPath, int TotalHorizontalFrames, int TotalVerticalFrames, Color EnergyTelegraphColor, Action<NPC, Vector2>? ExtraDrawAction, SoundStyle? SwapSound,
        params string[] CustomGoreNames)
    {
        /// <summary>
        /// The set of all Ares hand variants.
        /// </summary>
        private static readonly List<AresHandType> hands = new(8);

        /// <summary>
        /// The representation of Ares' plasma cannon.
        /// </summary>
        public static readonly AresHandType PlasmaCannon = New("Mods.CalamityMod.NPCs.AresPlasmaFlamethrower.DisplayName", "CalamityMod/NPCs/ExoMechs/Ares/AresPlasmaFlamethrower", "CalamityMod/NPCs/ExoMechs/Ares/AresPlasmaFlamethrowerGlow", 6, 8, Color.GreenYellow,
            null, AresHand.PlasmaCannonSwapSound, "CalamityMod/AresPlasmaFlamethrower1", "CalamityMod/AresPlasmaFlamethrower2");

        /// <summary>
        /// The representation of Ares' tesla cannon.
        /// </summary>
        public static readonly AresHandType TeslaCannon = New("Mods.CalamityMod.NPCs.AresTeslaCannon.DisplayName", "CalamityMod/NPCs/ExoMechs/Ares/AresTeslaCannon", "CalamityMod/NPCs/ExoMechs/Ares/AresTeslaCannonGlow", 6, 8, Color.Aqua,
            null, AresHand.TeslaCannonSwapSound, "CalamityMod/AresTeslaCannon1", "CalamityMod/AresTeslaCannon2");

        /// <summary>
        /// The representation of Ares' laser cannon.
        /// </summary>
        public static readonly AresHandType LaserCannon = New("Mods.CalamityMod.NPCs.AresLaserCannon.DisplayName", "CalamityMod/NPCs/ExoMechs/Ares/AresLaserCannon", "CalamityMod/NPCs/ExoMechs/Ares/AresLaserCannonGlow", 6, 8, Color.OrangeRed,
            null, AresHand.LaserCannonSwapSound, "CalamityMod/AresLaserCannon1", "CalamityMod/AresLaserCannon2");

        /// <summary>
        /// The representation of Ares' gauss nuke.
        /// </summary>
        public static readonly AresHandType GaussNuke = New("Mods.CalamityMod.NPCs.AresGaussNuke.DisplayName", "CalamityMod/NPCs/ExoMechs/Ares/AresGaussNuke", "CalamityMod/NPCs/ExoMechs/Ares/AresGaussNukeGlow", 9, 12, Color.Yellow,
            null, AresHand.GaussNukeSwapSound, "CalamityMod/AresGaussNuke1", "CalamityMod/AresGaussNuke2", "CalamityMod/AresGaussNuke3");

        /// <summary>
        /// The representation of Ares' pulse cannon.
        /// </summary>
        public static readonly AresHandType PulseCannon = New("Mods.FargowiltasCrossmod.NPCs.AresHand.PulseCannonDisplayName", "FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Ares/AresPulseCannon",
            "FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Ares/AresPulseCannonGlow", 4, 12, Color.Purple,
            null, AresHand.PulseCannonSwapSound);

        /// <summary>
        /// The representation of Ares' energy katana.
        /// </summary>
        public static readonly AresHandType EnergyKatana = New("Mods.FargowiltasCrossmod.NPCs.AresHand.EnergyKatanaDisplayName", "FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Ares/AresEnergyKatana",
            "FargowiltasCrossmod/Content/Calamity/Bosses/ExoMechs/Ares/AresEnergyKatanaGlow", 1, 1, Color.Red,
            AresHand.DrawEnergyKatana, null);

        private static AresHandType New(string nameLocalizationKey, string texturePath, string glowmaskPath, int totalHorizontalFrames, int totalVerticalFrames, Color energyTelegraphColor, Action<NPC, Vector2>? extraDrawAction, SoundStyle? swapSound, params string[] customGoreNames)
        {
            AresHandType hand = new(nameLocalizationKey, texturePath, glowmaskPath, totalHorizontalFrames, totalVerticalFrames, energyTelegraphColor, extraDrawAction, swapSound, customGoreNames);
            hands.Add(hand);

            return hand;
        }

        /// <summary>
        /// Writes the state of this arm type to a given <see cref="BinaryWriter"/>, for the purposes of being sent across the network.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(hands.IndexOf(this));
        }

        /// <summary>
        /// Constructs an Ares arm type from data in a <see cref="BinaryReader"/>, for the purposes of being received across the network.
        /// </summary>
        /// <param name="reader"></param>
        public static AresHandType ReadFrom(BinaryReader reader)
        {
            int index = reader.ReadInt32();

            if (index >= 0 && index < hands.Count)
                return hands[index];

            return PlasmaCannon;
        }
    }
}
