using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.KingSlime
{
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EDeathCrownJewel : CalDLCEDeathBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<KingSlimeJewelRuby>();
        public int Timer = 0;

        public override bool PreAI()
        {
            if (!NPC.HasValidTarget)
            {
                return true;
            }
            NPC.ai[0] = 0; //don't fire normal shots
            int slime = NPC.FindFirstNPC(NPCID.KingSlime);
            if (slime.IsWithinBounds(Main.maxNPCs) && Main.npc[slime] != null && Main.npc[slime].Alive())
            {
                if (Main.npc[slime].GetGlobalNPC<FargowiltasSouls.Content.Bosses.VanillaEternity.KingSlime>().DeathTimer > 0)
                    return true;
            }
            if (++Timer >= 180)
            {
                Player target = Main.player[NPC.target];
                Vector2 totarget = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                Timer = 0;
                SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, totarget.RotatedBy(MathHelper.ToRadians(20)) * 10, ModContent.ProjectileType<JewelProjectile>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(50), 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, totarget.RotatedBy(MathHelper.ToRadians(-20)) * 10, ModContent.ProjectileType<JewelProjectile>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(50), 0);
                }
            }
            return true;
        }
    }
}
