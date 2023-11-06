using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DepthDiverEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.MediumBlue;

		public override bool IsLoadingEnabled(Mod mod) => !ModContent.GetInstance<Core.ThoriumConfig>().HideWIPThorium;

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.ThoriumDLC();
            DLCPlayer.DepthDiverEnchant = true;
            DLCPlayer.DepthDiverEnchantItem = Item;

			DepthDiverItemEffect(player);
		}

		public static void DepthDiverItemEffect(Player player)
        {
            if (player.ThoriumDLC().DepthBubble > 0)
            {
                player.GetDamage(player.ProcessDamageTypeFromHeldItem()) += 0.10f;
                player.statDefense += 12;
                player.dripping = true;
            }
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        internal bool lastWet;
        internal int DepthBubble;
        public void DepthDiverEffect()
        {
            if (!DepthDiverEnchant)
            {
                DepthBubble = 0;
                return;
            }

            if (!Player.wet && lastWet)
            {
                DepthBubble = 1;
            }

            lastWet = Player.wet;
        }

        public void DepthDiverHit()
        {
            if (DepthBubble > 0)
            {
                DepthBubble = 0;
                if (IcyEnch)
                {
                    Vector2 vector = Vector2.UnitY * 4;
                    for (int i = 0; i < 8; i++)
                    {
                        vector = vector.RotatedBy(i * MathHelper.TwoPi / 8);
                        Projectile.NewProjectile(Player.GetSource_Accessory(DepthDiverEnchantItem), Player.Center + vector, vector, ProjectileID.IceSpike, 25, 1f, Player.whoAmI);
                    }
                }

                for (int i = 0; i < 12; i++)
                {
                    Dust.NewDustPerfect(Main.rand.NextVector2CircularEdge(Player.width, Player.height), DustID.Water);
                }
            }
        }
    }
}