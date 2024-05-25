
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Golem
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathGolemHead : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.GolemHeadFree;

        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            NPC golem = FargoSoulsUtil.NPCExists(NPC.golemBoss, NPCID.Golem);
            if (golem != null && golem.active)
            {
                NPC.damage = golem.damage;
            }
            GolemHead gol = NPC.GetGlobalNPC<GolemHead>();
            //Main.NewText(gol.AttackTimer);
            Player target = Main.player[NPC.target];
            if (gol != null && gol.AttackTimer == 260)
            {
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8, ProjectileID.InfernoHostileBolt, FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, ai0: target.Center.X, ai1: target.Center.Y);
                }
            }
            return true;
        }
    }
}
