using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using FargowiltasSouls.Core.AccessoryEffectSystem;
using FargowiltasSouls.Core.Toggler;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SpiritTrapperEnchant : BaseEnchant
    {
        public override Color nameColor => Color.LightBlue;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.AddEffect<SpiritTrapperEffect>(Item);
        }
    }

    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SpiritTrapperEffect : AccessoryEffect
    {
        public override Header ToggleHeader => Header.GetHeader<Core.Toggler.Content.HelheimHeader>();
        public override int ToggleItemType => ModContent.ItemType<SpiritTrapperEnchant>();

        public override void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            var DLCPlayer = player.ThoriumDLC();
            if (!DLCPlayer.soulEssenceHit)
            {
                DLCPlayer.soulEssenceHit = true;
                var thoriumPlayer = player.Thorium();
                int charge = player.ForceEffect<SpiritTrapperEffect>() ? 2 : 1;
                player.AddBuff(ModContent.BuffType<ThoriumMod.Buffs.Healer.SoulEssence>(), 1800, true, false);
                CombatText.NewText(target.Hitbox, new Color(100, 255, 200), charge, false, true);
                thoriumPlayer.soulEssence += charge;
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium.Items
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class SpiritTrapperGlobalItem : GlobalItem
    {
        public override void UseAnimation(Item item, Player player)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.soulEssenceHit = false;
        }
    }
}