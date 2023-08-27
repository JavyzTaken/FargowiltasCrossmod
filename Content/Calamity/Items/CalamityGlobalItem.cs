
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Items
{
    [ExtendsFromMod("CalamityMod")]
    public partial class CalamityGlobalItem : GlobalItem
    {
        public override bool OnPickup(Item item, Player player)
        {
            CrossplayerCalamity crossplayer = player.GetModPlayer<CrossplayerCalamity>();
            if (crossplayer.Tarragon)
            {
                TarragonPickupEffect(item, player);
            }
            return base.OnPickup(item, player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            CrossplayerCalamity crossplayer = player.GetModPlayer<CrossplayerCalamity>();
            if (crossplayer.VictideSwimmin)
            {
                VictideShootEffect(item, player, source, position, velocity);
            }
            if (crossplayer.Mollusk)
            {
                crossplayer.MolluskClamShot(source, position, velocity, damage, knockback);
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
        public override bool? UseItem(Item item, Player player)
        {
            CrossplayerCalamity mp = player.GetModPlayer<CrossplayerCalamity>();
            if (mp.Gemtech)
            {
                GemTechUseEffect(item, player);
            }
            return base.UseItem(item, player);
        }
    }
}
