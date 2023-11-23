using FargowiltasSouls.Content.Items.Accessories.Souls;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Souls
{
    public class ThoriumSoul : BaseSoul
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 24));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
    }
}