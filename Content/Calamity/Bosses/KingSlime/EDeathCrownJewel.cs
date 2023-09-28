using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
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
    public class EDeathCrownJewel : EternideathNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<KingSlimeJewel>());
       
        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            if (npc.ai[0] == 59)
            {
                Player target = Main.player[npc.target];
                Vector2 totarget = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
                npc.ai[0] = 0;
                SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(MathHelper.ToRadians(20)) * 10, ModContent.ProjectileType<JewelProjectile>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(50), 0);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(MathHelper.ToRadians(-20)) * 10, ModContent.ProjectileType<JewelProjectile>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(50), 0);
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
