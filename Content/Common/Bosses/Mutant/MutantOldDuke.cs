using CalamityMod.Buffs.StatDebuffs;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using Terraria;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Common.Bosses.Mutant
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MutantOldDuke : MutantFishron
    {
        public override string Texture => "CalamityMod/NPCs/OldDuke/OldDuke";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Projectile.type] = 7;
        }
        public override bool PreAI()
        {
            return true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 60 * 10);
            base.OnHitPlayer(target, info);
        }
    }
}
