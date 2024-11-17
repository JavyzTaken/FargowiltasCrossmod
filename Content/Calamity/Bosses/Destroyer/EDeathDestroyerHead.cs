using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Destroyer
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathDestroyerHead : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.TheDestroyer;

        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            FargowiltasSouls.Content.Bosses.VanillaEternity.Destroyer destroyer = NPC.GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.Destroyer>();
            //Main.NewText(destroyer.AttackModeTimer + ", " + destroyer.SecondaryAttackTimer + ", " + NPC.ai[2]);
            if (destroyer.InPhase2 && destroyer.SecondaryAttackTimer == 5 && destroyer.AttackModeTimer > 0 && destroyer.AttackModeTimer % 100 == 0 && !destroyer.IsCoiling)
            {
                if (DLCUtils.HostCheck)
                    for (int i = -2; i < 3; i++)
                    {
                        if (i != 0)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(7 * i) - MathHelper.PiOver2) * 7, ProjectileID.DeathLaser, FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage / 2), 0);
                    }
            }
            return true;
        }
    }
}
