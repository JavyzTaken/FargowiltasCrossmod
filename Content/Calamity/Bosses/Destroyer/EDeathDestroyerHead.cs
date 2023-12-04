using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Destroyer
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDestroyerHead : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(NPCID.TheDestroyer);

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget) return true;
            FargowiltasSouls.Content.Bosses.VanillaEternity.Destroyer destroyer = npc.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.Destroyer>();
            //Main.NewText(destroyer.AttackModeTimer + ", " + destroyer.SecondaryAttackTimer + ", " + npc.ai[2]);
            if (destroyer.InPhase2 && destroyer.SecondaryAttackTimer == 5 && destroyer.AttackModeTimer > 0 && destroyer.AttackModeTimer % 100 == 0 && !destroyer.IsCoiling)
            {
                if (DLCUtils.HostCheck)
                    for (int i = -2; i < 3; i++)
                    {
                        if (i != 0)
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(7 * i) - MathHelper.PiOver2) * 7, ProjectileID.DeathLaser, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(npc.damage / 2), 0);
                    }
            }
            return base.SafePreAI(npc);
        }
    }
}
