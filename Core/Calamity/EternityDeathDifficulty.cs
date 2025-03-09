using CalamityMod.Systems;
using CalamityMod.World;
using Fargowiltas.Projectiles;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasSouls.Content.NPCs;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static CalamityMod.Systems.DifficultyModeSystem;

namespace FargowiltasCrossmod.Core.Calamity
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EternityDeathDifficulty : DifficultyMode
    {
        public override bool Enabled
        {
            get => CalDLCWorldSavingSystem.EternityDeath;
            set
            {
                CalDLCWorldSavingSystem.EternityRev = value;
                CalDLCWorldSavingSystem.EternityDeath = value;
                if (value)
                {
                    CalamityWorld.revenge = true;
                    CalamityWorld.death = true;

                    int deviType = ModContent.NPCType<UnconsciousDeviantt>();
                    if (!WorldSavingSystem.SpawnedDevi && !NPC.AnyNPCs(deviType))
                    {
                        WorldSavingSystem.SpawnedDevi = true;

                        Vector2 spawnPos = (Main.zenithWorld || Main.remixWorld) ? Main.LocalPlayer.Center : Main.LocalPlayer.Center - 1000 * Vector2.UnitY;
                        Projectile.NewProjectile(Main.LocalPlayer.GetSource_Misc(""), spawnPos, Vector2.Zero, ModContent.ProjectileType<SpawnProj>(), 0, 0, Main.myPlayer, deviType);

                        FargoSoulsUtil.PrintLocalization("Announcement.HasAwoken", new Color(175, 75, 255), Language.GetTextValue("Mods.Fargowiltas.NPCs.Deviantt.DisplayName"));
                    }
                }
                bool emode = value;
                if (ModCompatibility.InfernumMode.Loaded)
                    if (ModCompatibility.InfernumMode.InfernumDifficulty && CalDLCConfig.Instance.InfernumDisablesEternity)
                        emode = false;

                if (Main.expertMode)
                {
                    WorldSavingSystem.EternityMode = emode;
                    WorldSavingSystem.ShouldBeEternityMode = emode;
                }
                if (Main.netMode != NetmodeID.SinglePlayer)
                    PacketManager.SendPacket<EternityDeathPacket>();
            }
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                _texture ??= ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/EternityDeathIcon");

                return _texture;
            }
        }

        //TODO: add conditions to this description, for priority and Maso line
        public override LocalizedText ExpandedDescription => Language.GetText("Mods.FargowiltasCrossmod.EternityDeathDifficulty.ExpandedDescription");

        public EternityDeathDifficulty()
        {
            DifficultyScale = 2f;
            Name = Language.GetText("Mods.FargowiltasCrossmod.EternityDeathDifficulty.Name");
            ShortDescription = Language.GetText("Mods.FargowiltasCrossmod.EternityDeathDifficulty.ShortDescription");

            ActivationTextKey = "Mods.FargowiltasCrossmod.EternityDeathDifficulty.Activation";
            DeactivationTextKey = "Mods.FargowiltasCrossmod.EternityDeathDifficulty.Deactivation";

            ActivationSound = SoundID.Roar with { Pitch = -0.5f };
            ChatTextColor = Color.DarkRed;

            //MostAlternateDifficulties = 1;
            //Difficulties = new DifficultyMode[] { new NoDifficulty(), new RevengeanceDifficulty(), new DeathDifficulty(), this };
            //Difficulties = Difficulties.OrderBy(d => d.DifficultyScale).ToArray();
            //Difficulties.Add(this);

            //DifficultyTiers = new List<DifficultyMode[]>();
            //float currentTier = -1;
            //int tierIndex = -1;

            //for (int i = 0; i < Difficulties.Count; i++)
            //{
            //    // If at a new tier, create a new list of difficulties at that tier.
            //    if (currentTier != Difficulties[i].DifficultyScale)
            //    {
            //        DifficultyTiers.Add(new DifficultyMode[] { Difficulties[i] });
            //        currentTier = Difficulties[i].DifficultyScale;
            //        tierIndex++;
            //    }

            //    // If the tier already exists, just add it to the list of other difficulties at that tier.
            //    else
            //    {
            //        DifficultyTiers[tierIndex] = DifficultyTiers[tierIndex].Append(Difficulties[i]).ToArray();
            //        MostAlternateDifficulties = Math.Max(DifficultyTiers[tierIndex].Length, MostAlternateDifficulties);
            //    }
            //}
        }

        public override int FavoredDifficultyAtTier(int tier)
        {
            DifficultyMode[] tierList = DifficultyTiers[tier];

            for (int i = 0; i < tierList.Length; i++)
            {
                if (tierList[i].Name.Value == "Death")
                    return i;
            }

            return 0;
        }
    }
}