using FargowiltasSouls.Content.Items;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class MynaAccessory : SoulsItem
    {
        public override string Texture => "FargowiltasSouls/Content/Items/Placeholder";
        public override bool Eternity => true;

        public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.MynaAccessory = true;
        }
    }
}
