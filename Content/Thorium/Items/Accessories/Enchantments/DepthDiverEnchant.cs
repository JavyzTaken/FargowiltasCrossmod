using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
	[ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class DepthDiverEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => new(11, 86, 255);
        protected override Color SynergyColor1 => Color.White; 
        protected override Color SynergyColor2 => Color.White;
        internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.DepthDiverEnchant && DLCPlayer.IcyEnch;
        internal override int SynergyEnch => ModContent.ItemType<IcyEnchant>();

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
                player.endurance += 0.15f;
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
                if (SynergyEffect(DepthDiverEnchantItem.type))
                {
                    Vector2 vector = Vector2.UnitY * 4;
                    for (int i = 0; i < 8; i++)
                    {
                        vector = vector.RotatedBy(i * MathHelper.TwoPi / 8);
                        Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_Accessory(DepthDiverEnchantItem), Player.Center + vector, vector, ProjectileID.IceSpike, 25, 1f, Player.whoAmI);
                        proj.friendly = true;
                        proj.hostile = false;
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