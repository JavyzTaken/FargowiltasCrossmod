using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.Chat;
using ThoriumMod.Buffs;
using FargowiltasSouls.Content.Buffs.Masomode;

// TODO: this needs testing in mp with more than 1 person, but it should work (tm)

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class FallenPaladinEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Beige;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.FallenPaladinEnch = true;
            DLCPlayer.FallenPaladinEnchItem = Item;
        }

        public static readonly List<int> WhiteList = new() 
        {
            BuffID.Venom,
            BuffID.Blackout,
            BuffID.Bleeding,
            BuffID.BrokenArmor,
            BuffID.Burning,
            BuffID.Chilled,
            BuffID.Confused,
            BuffID.Cursed,
            BuffID.CursedInferno,
            BuffID.Darkness,
            BuffID.Electrified,
            BuffID.Rabies,
            BuffID.Frostburn,
            BuffID.Frostburn2,
            BuffID.Frozen,
            BuffID.Ichor,
            BuffID.OnFire,
            BuffID.OnFire3,
            BuffID.OgreSpit,
            BuffID.Poisoned,
            BuffID.ShadowFlame,
            BuffID.Silenced,
            BuffID.Slow,
            BuffID.Stoned,
            BuffID.Weak,
            BuffID.Webbed,

            ModContent.BuffType<Destabilized>(),
            ModContent.BuffType<GraniteSurge>(),
            ModContent.BuffType<Liquefied>(),
            ModContent.BuffType<Staggered>(),

            ModContent.BuffType<AnticoagulationBuff>(),
            ModContent.BuffType<AntisocialBuff>(),
            ModContent.BuffType<AtrophiedBuff>(),
            ModContent.BuffType<BerserkedBuff>(),
            ModContent.BuffType<ClippedWingsBuff>(),
            ModContent.BuffType<CrippledBuff>(),
            ModContent.BuffType<CurseoftheMoonBuff>(),
            ModContent.BuffType<DefenselessBuff>(),
            ModContent.BuffType<FlamesoftheUniverseBuff>(),
            ModContent.BuffType<GuiltyBuff>(),
            ModContent.BuffType<HypothermiaBuff>(),
            ModContent.BuffType<InfestedBuff>(),
            ModContent.BuffType<JammedBuff>(),
            ModContent.BuffType<LethargicBuff>(),
            ModContent.BuffType<LightningRodBuff>(),
            ModContent.BuffType<LivingWastelandBuff>(),
            ModContent.BuffType<MarkedforDeathBuff>(),
            ModContent.BuffType<MidasBuff>(),
            ModContent.BuffType<NanoInjectionBuff>(),
            ModContent.BuffType<OiledBuff>(),
            ModContent.BuffType<RottingBuff>(),
            ModContent.BuffType<SmiteBuff>(),
            ModContent.BuffType<ShadowflameBuff>(),
            ModContent.BuffType<Stunned>(),
            ModContent.BuffType<UnluckyBuff>(),
            ModContent.BuffType<UnstableBuff>(),
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

                bool flag = false;

                for (int buff = 0; buff < other.buffType.Length; buff++)
                {
                    int buffType = other.buffType[buff];

                    if (buffType <= 0) continue;

                    if (Main.debuff[buffType] && Items.Accessories.Enchantments.FallenPaladinEnchant.WhiteList.Contains(buffType))
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                {
                    ModPacket request = Mod.GetPacket();
                    request.Write((byte)FargowiltasCrossmod.PacketID.RequestFallenPaladinUsed);
                    request.Send();
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

        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
        }
    }
}