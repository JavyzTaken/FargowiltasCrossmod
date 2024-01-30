using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FargowiltasCrossmod.Core.Calamity.Systems;
using Terraria.ModLoader;
using Terraria;

namespace FargowiltasCrossmod.Core.Thorium
{
    public class FallenPaladinPacket : BaseDLCPacket
    {
        public override void Write(ModPacket packet, params object[] context)
        {
            packet.Write(Main.myPlayer);
        }

        public override void Read(BinaryReader reader)
        {
            // TODO: Test if this effect still works.
            int healer = reader.ReadInt32();
            if (healer != Main.myPlayer)
            {
                Player healerPlayer = Main.player[healer];
                for (int i = 0; i < Main.LocalPlayer.buffType.Length; i++)
                {
                    if (Content.Thorium.Items.Accessories.Enchantments.FallenPaladinEnchant.WhiteList.Contains(Main.LocalPlayer.buffType[i]))
                    {
                        healerPlayer.AddBuff(Main.LocalPlayer.buffType[i], Main.LocalPlayer.buffTime[i], false);
                    }
                }
                Main.LocalPlayer.AddBuff(ModContent.BuffType<Content.Thorium.Buffs.FallenPaladinBuff>(), 2);
            }
        }
    }
}
