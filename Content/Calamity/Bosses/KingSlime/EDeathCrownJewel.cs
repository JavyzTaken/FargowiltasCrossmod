﻿using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
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
    public class EDeathCrownJewel : EternityDeathBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<KingSlimeJewel>());
        public int Timer = 0;

        public override bool SafePreAI(NPC npc)
        {
            if (!npc.HasValidTarget)
            {
                return true;
            }
            npc.ai[0] = 0; //don't fire normal shots
            if (++Timer >= 180)
            {
                Player target = Main.player[npc.target];
                Vector2 totarget = (target.Center - npc.Center).SafeNormalize(Vector2.Zero);
                Timer = 0;
                SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(MathHelper.ToRadians(20)) * 10, ModContent.ProjectileType<JewelProjectile>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(50), 0);
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, totarget.RotatedBy(MathHelper.ToRadians(-20)) * 10, ModContent.ProjectileType<JewelProjectile>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(50), 0);
                }
            }
            return base.SafePreAI(npc);
        }
    }
}
