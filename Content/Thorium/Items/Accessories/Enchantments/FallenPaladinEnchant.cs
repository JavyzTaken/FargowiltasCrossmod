using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Chat;

// TODO: this needs testing in mp with more than 1 person, but it should work (tm)

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class FallenPaladinEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Beige;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.FallenPaladinEnch = true;
            DLCPlayer.FallenPaladinEnchItem = Item;
        }

        public static readonly List<int> WhiteList = new()
        {
            20,
            24,
            39,
            44,
            70,
            153,
            144,
            22,
            23,
            31,
            32,
            33,
            35,
            36,
            46,
            47,
            69,
            80,
            156,
            148,
            197,
            149,
            30
        };
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void FallenPaladinEffect()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            if (Player.dead || !Player.active) return;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player other = Main.player[i];
                if (i == Player.whoAmI || !other.active || other.dead) continue; 

                for (int buff = 0; buff < Main.player[i].buffType.Length; buff++)
                {
                    int buffType = other.buffType[buff];
                    int buffTime = other.buffTime[buff];

                    if (buffType <= 0 || buffTime <= 0) continue;
                    ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral($"{buffType}, {buffTime}"), Color.White);

                    if (Items.Accessories.Enchantments.FallenPaladinEnchant.WhiteList.Contains(buffType))
                    {
                        if (Player.HasBuff(buffType))
                        {
                            // increase the time for stacking buffs
                            Player.buffTime[Player.FindBuffIndex(buffType)] += buffTime;
                        }
                        else
                        {
                            Player.AddBuff(buffType, buffTime);
                        }

                        other.AddBuff(ModContent.BuffType<Buffs.FallenPaladinBuff>(), 60);
                    }
                }
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium.Buffs
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class FallenPaladinBuff : ModBuff
    {
        public override string Texture => "FargowiltasSouls/Content/Buffs/PlaceholderBuff";
        public override void Update(Player player, ref int buffIndex)
        {
            foreach (int buffType in Items.Accessories.Enchantments.FallenPaladinEnchant.WhiteList)
            {
                player.buffImmune[buffType] = true;
            }
        }
    }
}