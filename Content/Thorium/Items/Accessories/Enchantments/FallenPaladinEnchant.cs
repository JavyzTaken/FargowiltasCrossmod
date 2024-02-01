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
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class FallenPaladinEnchant : BaseEnchant
    {
        public override Color nameColor => Color.Beige;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<FallenPaladinEffect>(Item);
        }

        public static readonly List<int> WhiteList = new() 
        {
            // status
            BuffID.Blackout,
            BuffID.Bleeding,
            BuffID.BrokenArmor,
            BuffID.Chilled,
            BuffID.Confused,
            BuffID.Cursed,
            BuffID.Darkness,
            BuffID.Rabies,
            BuffID.Frozen,
            BuffID.Ichor,
            BuffID.OgreSpit,
            BuffID.Silenced,
            BuffID.Slow,
            BuffID.Stoned,
            BuffID.Weak,
            BuffID.Webbed,

            ModContent.BuffType<Liquefied>(),
            ModContent.BuffType<Staggered>(),

            ModContent.BuffType<AntisocialBuff>(),
            ModContent.BuffType<AtrophiedBuff>(),
            ModContent.BuffType<BerserkedBuff>(),
            ModContent.BuffType<ClippedWingsBuff>(),
            ModContent.BuffType<CrippledBuff>(),
            ModContent.BuffType<DefenselessBuff>(),
            ModContent.BuffType<GuiltyBuff>(),
            ModContent.BuffType<HypothermiaBuff>(),
            ModContent.BuffType<JammedBuff>(),
            ModContent.BuffType<LethargicBuff>(),
            ModContent.BuffType<LightningRodBuff>(),
            ModContent.BuffType<LivingWastelandBuff>(),
            ModContent.BuffType<MarkedforDeathBuff>(),
            ModContent.BuffType<MidasBuff>(),
            ModContent.BuffType<OiledBuff>(),
            ModContent.BuffType<RottingBuff>(),
            ModContent.BuffType<SmiteBuff>(),
            ModContent.BuffType<Stunned>(),
            ModContent.BuffType<UnluckyBuff>(),
            ModContent.BuffType<UnstableBuff>(),

            // dot
            BuffID.ShadowFlame,
            BuffID.Poisoned,
            BuffID.OnFire3,
            BuffID.OnFire,
            BuffID.Frostburn2,
            BuffID.Frostburn,
            BuffID.Electrified,
            BuffID.CursedInferno,
            BuffID.Burning,
            BuffID.Venom,
            ModContent.BuffType<Destabilized>(),
            ModContent.BuffType<GraniteSurge>(),

            ModContent.BuffType<AnticoagulationBuff>(),
            ModContent.BuffType<CurseoftheMoonBuff>(),
            ModContent.BuffType<FlamesoftheUniverseBuff>(),
            ModContent.BuffType<InfestedBuff>(),
            ModContent.BuffType<NanoInjectionBuff>(),
            ModContent.BuffType<ShadowflameBuff>(),
        };
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class FallenPaladinEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.AlfheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<FallenPaladinEnchant>();
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void FallenPaladinKey()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient) return;
            if (Player.dead || !Player.active) return;

            bool flag = false;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player other = Main.player[i];

                if (i == Player.whoAmI || !other.active || other.dead) continue;

                for (int buff = 0; buff < other.buffType.Length; buff++)
                {
                    int buffType = other.buffType[buff];

                    if (buffType <= 0) continue;

                    if (Items.Accessories.Enchantments.FallenPaladinEnchant.WhiteList.Contains(buffType))
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag) break;
            }

            if (flag)
            {
                Core.Calamity.Systems.PacketManager.SendPacket<Core.Thorium.FallenPaladinPacket>();
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