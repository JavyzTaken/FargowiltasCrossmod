using Terraria;
using Terraria.ModLoader;
using FargowiltasCrossmod.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(ModCompatibility.ThoriumMod.Name)]
    public class NoviceClericEnchant : BaseSynergyEnchant
    {
        protected override Color nameColor => Color.Yellow;
        internal override bool SynergyActive(CrossplayerThorium DLCPlayer) => DLCPlayer.NoviceClericEnchItem == Item && DLCPlayer.EbonEnch;

        protected override Color SynergyColor1 => Color.White with { A = 0 };
        protected override Color SynergyColor2 => Color.Purple with { A = 0 };
        internal override int SynergyEnch => ModContent.ItemType<EbonEnchant>();

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var DLCPlayer = player.GetModPlayer<CrossplayerThorium>();
            DLCPlayer.NoviceClericEnch = true;
            DLCPlayer.NoviceClericEnchItem = Item;
        }

        //public override void Load()
        //{
        //    new EnchantSynergy(
        //        new Func<CrossplayerThorium, bool>((CrossplayerThorium DLCPlayer) => DLCPlayer.NoviceClericEnch && DLCPlayer.EbonEnch),
        //        new Action<CrossplayerThorium>((CrossplayerThorium DLCPlayer) => DLCPlayer.clericEbonSynergy = true)
        //        ).Register();
        //}
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

            int maxCrosses = SynergyEffect(NoviceClericEnchItem.type) ? 8 : 5;

            if (NoviceClericCrosses < maxCrosses)
            {
                if (NoviceClericTimer > (SynergyEffect(NoviceClericEnchItem.type) ? 180 : 300))
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
            bool synergy = SynergyEffect(NoviceClericEnchItem.type);
            int burstNum = (synergy && NoviceClericCrosses > 0) ? 5 : 3;

            if ((synergy && NoviceClericCrosses % 2 == 0) || !synergy)
            {
                // holy crux
                Vector2 vector = Utils.RotatedBy(Player.Center.DirectionTo(Main.MouseWorld), MathF.PI / burstNum) * 8f;
                for (int i = 0; i < burstNum; i++)
                {
                    vector = Utils.RotatedBy(vector, MathF.Tau / burstNum);
                    Projectile.NewProjectile(Player.GetSource_Accessory(NoviceClericEnchItem), Player.Center, vector, ModContent.ProjectileType<Projectiles.DLCShadowWispPro>(), 0, 0, Player.whoAmI, 0f, 0f, 1f);
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