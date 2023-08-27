using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Calamity.Projectiles;


namespace FargowiltasCrossmod.Content.Calamity.Buffs
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class LifeShell : ModBuff
    {
        public override string Texture => "CalamityMod/Buffs/Summon/SilvaCrystalBuff";
        public override void SetStaticDefaults()
        {

            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        private bool shattered;
        public override void Update(Player player, ref int buffIndex)
        {
            Projectile crystal = null;
            shattered = false;
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<LargeSilvaCrystal>() && Main.projectile[i].owner == player.whoAmI)
                {
                    crystal = Main.projectile[i];
                }
            }
            if (crystal != null)
            {
                if (crystal.ai[0] == 0)
                {
                    player.statDefense += 30;
                    player.lifeRegen += 7;
                    player.moveSpeed -= 1;
                    player.jumpSpeedBoost -= 2;
                    player.wingAccRunSpeed -= 1;
                }
                else
                {
                    player.GetDamage(DamageClass.Generic) += 0.4f;
                    player.statDefense -= 15;
                    shattered = true;
                }
            }
            if (player.ownedProjectileCounts[ModContent.ProjectileType<LargeSilvaCrystal>()] < 1)
            {
                player.DelBuff(buffIndex);
                buffIndex--;

            }
            else
            {
                player.buffTime[buffIndex] = 18000;
            }

        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            if (shattered)
            {
                tip = Language.GetTextValue("Mods.FargowiltasCrossmod.Buffs.LifeShell.ShatteredDescription");
            }
            else
            {
                tip = Language.GetTextValue("Mods.FargowiltasCrossmod.Buffs.LifeShell.Description");
            }

        }
    }
}
