using Terraria;
using Terraria.ModLoader;
using FargowiltasSouls.Core.Globals;
using ThoriumMod.NPCs.BossTheGrandThunderBird;
using FargowiltasSouls.Core.NPCMatching;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace FargowiltasCrossmod.Content.Thorium.EternityMode.Enemy
{
    [ExtendsFromMod(Core.ModCompatibility.ThoriumMod.Name)]
    public class GFBHatling : EModeNPCBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<StormHatchling>());

        const int OrbAttSpeed = 100;
        const int WaitTime = 60;
        int WaitingTimer;
        Vector2? NextPosition = null;
        Vector2 LastPosition;

        public override void SetDefaults(NPC npc)
        {
            npc.damage = 10;
            npc.lifeMax = 80;
            npc.life = 80;
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            WaitingTimer = 0;
            base.OnSpawn(npc, source);
        }

        const float MinDistSQFromPlayer = 36864f;
        const float MinDistSQFromLastPos = 16384f;
        private Vector2 GetNextPosition(Player target, NPC npc)
        {
            Vector2 vec = new(Main.rand.NextFloat(target.Center.X - Main.ScreenSize.X / 3, target.Center.X + Main.ScreenSize.X / 3),
                              Main.rand.NextFloat(target.Center.Y - Main.ScreenSize.Y / 2, target.Center.Y));
            if (vec.DistanceSQ(target.Center) <= MinDistSQFromPlayer || vec.DistanceSQ(npc.Center) <= MinDistSQFromLastPos) return GetNextPosition(target, npc);
            Vector2 closestPointToPlr = Vector2.Dot(target.Center - npc.Center, vec - npc.Center) / (vec - npc.Center).LengthSquared() * (vec - npc.Center);
            if (closestPointToPlr.X > (vec - npc.Center).X) return vec;
            if ((target.Center - npc.Center).LengthSquared() <= MinDistSQFromPlayer) return GetNextPosition(target, npc);
            return vec;
        }

        public override bool SafePreAI(NPC npc)
        {
            if (!Main.npc[(int)npc.ai[0]].active)
            {
                npc.life = 0;
                return false;
            }
            if (WaitingTimer > 0)
            {
                WaitingTimer--;
                return false;
            }

            if (NextPosition.HasValue && npc.Center.DistanceSQ(NextPosition.Value) > 64f)
            {
                // go towards chosen location
            }
            else if (npc.HasPlayerTarget && npc.HasValidTarget)
            {
                // choose new location if the attack is not finished
                LastPosition = npc.Center;
                NextPosition = GetNextPosition(Main.player[npc.target], npc);
                npc.velocity = (NextPosition.Value - LastPosition) / OrbAttSpeed;

                // spawn projectile
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<Projectiles.KluexOrb>(), 16, 3f, Main.myPlayer, Projectiles.KluexOrb.GFBOrb);
                WaitingTimer = WaitTime;
            }
            npc.spriteDirection = npc.velocity.X > 0 ? 1 : -1;

            return false;
        }
    }
}
