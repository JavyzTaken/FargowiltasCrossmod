using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Content.Items.Accessories.Enchantments;
using Microsoft.Xna.Framework;

namespace FargowiltasCrossmod.Content.Thorium.Items.Accessories.Enchantments
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TitanEnchant : BaseEnchant
    {
        protected override Color nameColor => Color.SteelBlue;

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<CrossplayerThorium>().TitanEnch = true;
        }
    }
}

namespace FargowiltasCrossmod.Content.Thorium.Projectiles
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class TitanGlobalProj : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile proj, bool lateInstantiation)
        {
            return proj.active && proj.friendly && proj.damage > 0;
        }

        const float EffectMult = 0.3f;
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[projectile.owner];
            if (player.GetModPlayer<CrossplayerThorium>().TitanEnch)
            {
                target.defense -= (player.statDefense) * EffectMult;
            }
        }
    }
}