using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod("ThoriumMod")]
    public class NoviceClericEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.Yellow;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.NoviceClericEnch = true;
            DLCPlayer.NoviceClericEnchItem = Item;
        }

        int drawTimer = 0;
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var DLCPlayer = Main.LocalPlayer.GetModPlayer<CrossplayerThorium>();
            if (DLCPlayer.EbonEnch && DLCPlayer.NoviceClericEnch)
            {
                for (int j = 0; j < 12; j++)
                {
                    Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 1f;
                    float modifier = 0.25f + ((float)Math.Sin(drawTimer / 30f) / 2);
                    Color glowColor = Color.Lerp(Color.White with { A = 0 }, Color.Black with { A = 0 }, modifier) * 0.5f;

                    Texture2D texture = Terraria.GameContent.TextureAssets.Item[Item.type].Value;
                    Main.EntitySpriteDraw(texture, position + afterimageOffset, null, glowColor, 0, texture.Size() * 0.5f, Item.scale, SpriteEffects.None, 0f);
                }
            }
            drawTimer++;
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}
namespace FargowiltasCrossmod.Content.Thorium
{
    public partial class CrossplayerThorium
    {
        public void NoviceClericEffect()
        {
            if (!NoviceClericEnch)
            {
                NoviceClericCrosses = 0;
                NoviceClericTimer = 0;
                return;
            }

            NoviceClericTimer++;
            crossOrbitalRotation = Utils.RotatedBy(crossOrbitalRotation, -0.05, default);

            bool synergy = NoviceClericEnch && EbonEnch;
            int maxCrosses = synergy ? 15 : 5;

            if (NoviceClericCrosses < maxCrosses)
            {
                if (NoviceClericTimer > (synergy ? 180 : 300))
                {
                    NoviceClericCrosses++;
                    NoviceClericTimer = 0;
                }
            }
            else
            {
                NoviceClericTimer = 0;
            }
        }

        public void NoviceClericOnManaUse()
        {
            if (NoviceClericCrosses <= 0 || !NoviceClericEnch) return;

            NoviceClericCrosses--;
            bool synergy = NoviceClericEnch && EbonEnch;
            int burstNum = (synergy && NoviceClericCrosses > 0) ? 5 : 3;

            if (synergy && NoviceClericCrosses % 2 == 0 || !synergy)
            {
                // holy crux
                Vector2 vector = Utils.RotatedBy(Player.Center.DirectionTo(Main.MouseWorld), MathF.PI / burstNum) * 8f;
                for (int i = 0; i < burstNum; i++)
                {
                    vector = Utils.RotatedBy(vector, MathF.Tau / burstNum);
                    Projectile.NewProjectile(Player.GetSource_Accessory(NoviceClericEnchItem), Player.Center, vector, ModContent.ProjectileType<Projectiles.DLCShadowWispPro>(), 15, 2f, Player.whoAmI, 0f, 0f, 1f);
                }
            }
            else
            {
                // ebon crux
                Vector2 vector = Player.Center.DirectionTo(Main.MouseWorld) * 8f;
                for (int i = 0; i < burstNum; i++)
                {
                    vector = Utils.RotatedBy(vector, MathF.Tau / burstNum);
                    Projectile.NewProjectile(Player.GetSource_Accessory(EbonEnchItem), Player.Center, vector, ModContent.ProjectileType<Projectiles.DLCShadowWispPro>(), 15, 2f, Player.whoAmI, 0f, 0f, 0f);
                }
            }
        }
    }
}