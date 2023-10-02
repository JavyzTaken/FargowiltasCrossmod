using CalamityMod.Systems;
using CalamityMod.World;
using FargowiltasCrossmod.Core.Systems;
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
    public class EternityRevDifficulty : DifficultyMode
    {
        public override bool Enabled
        {
            get => DLCWorldSavingSystem.EternityRev;
            set
            {
                DLCWorldSavingSystem.EternityRev = value;
                if (value)
                {
                    CalamityWorld.revenge = true;
                    CalamityWorld.death = false;
                }
                FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode = value;
                FargowiltasSouls.Core.Systems.WorldSavingSystem.ShouldBeEternityMode = value;
                if (Main.netMode != NetmodeID.SinglePlayer)
                    PacketManager.SendPacket<EternityRevPacket>();
            }
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                _texture ??= ModContent.Request<Texture2D>("FargowiltasCrossmod/Assets/EternityRevIcon");

                return _texture;
            }
        }

        //TODO: add conditions to this description, for priority and Maso line
        public override LocalizedText ExpandedDescription => Language.GetText("Mods.FargowiltasCrossmod.EternityRevDifficulty.ExpandedDescription");

        public EternityRevDifficulty()
        {
            DifficultyScale = 1f;
            Name = Language.GetText("Mods.FargowiltasCrossmod.EternityRevDifficulty.Name");
            ShortDescription = Language.GetText("Mods.FargowiltasCrossmod.EternityRevDifficulty.ShortDescription");

            ActivationTextKey = "Mods.FargowiltasCrossmod.EternityRevDifficulty.Activation";
            DeactivationTextKey = "Mods.FargowiltasCrossmod.EternityRevDifficulty.Deactivation";

            ActivationSound = SoundID.Roar with { Pitch = -0.5f};
            ChatTextColor = Color.Cyan;
            
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