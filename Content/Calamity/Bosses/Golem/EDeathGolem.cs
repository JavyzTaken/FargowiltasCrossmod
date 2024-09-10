
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Golem
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathGolem : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => NPCID.Golem;

        public override bool PreAI()
        {
            if (!NPC.HasValidTarget) return true;
            //Main.NewText(NPC.ai[0] + ", " + NPC.ai[1] + ", " + NPC.ai[2] + ", " + NPC.ai[3]);
            if (NPC.ai[0] == 0 && NPC.ai[1] == 0 && !NPC.AnyNPCs(NPCID.GolemHeadFree) && NPC.velocity.Y == 0)
            {
                if (DLCUtils.HostCheck)
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile fireball = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.BottomRight + new Vector2(0, -10), new Vector2(5, 0).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<BouncingFireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage / 2), 0);
                        Projectile fireball2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.BottomLeft + new Vector2(0, -10), new Vector2(-5, 0).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)), ModContent.ProjectileType<BouncingFireball>(), FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage / 2), 0);

                    }
            }
            return true;
        }
    }
}
