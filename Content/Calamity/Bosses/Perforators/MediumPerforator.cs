using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Content.Projectiles.Masomode;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MediumPerforator : CalDLCEmodeExtraGlobalNPC
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<PerforatorHeadMedium>(),
            ModContent.NPCType<PerforatorBodyMedium>(),
            ModContent.NPCType<PerforatorTailMedium>()
        );
        public Vector2 VelocityReal = Vector2.UnitY * 22;
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            entity.lifeMax = 5000;
            entity.Opacity = 1f;
        }
        public override void SpawnNPC(int npc, int tileX, int tileY)
        {
            base.SpawnNPC(npc, tileX, tileY);
        }
        public override void OnKill(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return;

            if (DLCUtils.HostCheck && npc.HasPlayerTarget)
            {
                float shotSpeed = Main.rand.NextFloat(7f, 16f);
                Vector2 shotDir = -Vector2.UnitY.RotatedByRandom(MathHelper.Pi / 3.2f);
                Vector2 vel = shotDir * shotSpeed;
                if (vel.Y < -6)
                    vel.Y *= 0.6f;
                int p = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, vel, ModContent.ProjectileType<IchorShot>(), FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0);
                if (p.IsWithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].extraUpdates = 1;
                    Main.projectile[p].netUpdate = true;
                }
            }
                
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override bool SafePreAI(NPC npc)
        {
            npc.netUpdate = true; //fuck you worm mp code
            
            if (Collision.SolidCollision(npc.position, npc.width, npc.height))
            {
                npc.StrikeInstantKill();
            }
            if (npc.type == ModContent.NPCType<PerforatorHeadMedium>())
            {
                npc.velocity = VelocityReal;
            }

            return true;
        }
        public override void SafePostAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<PerforatorHeadMedium>())
            {
                VelocityReal.Y += 0.75f;
                if (npc.HasPlayerTarget)
                {
                    Player player = Main.player[npc.target];
                    VelocityReal.X += npc.HorizontalDirectionTo(player.Center) * 0.3f;
                }
                //npc.velocity = VelocityReal;
            }
        }
        /*
        public void ManageWormStuff(NPC NPC)
        {
            NPC.realLife = -1;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    int totalSegments = 10;
                    NPC.ai[2] = totalSegments;
                    NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (NPC.width / 2)), (int)(NPC.position.Y + NPC.height), ModContent.NPCType<PerforatorBodyMedium>(), NPC.whoAmI);
                    Main.npc[(int)NPC.ai[0]].ai[1] = NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                // Splitting effect
                if (!Main.npc[(int)NPC.ai[1]].active && !Main.npc[(int)NPC.ai[0]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
                if (!Main.npc[(int)NPC.ai[0]].active)
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                    NPC.active = false;
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }

                if (!NPC.active && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
            }

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
        }
        */
    }
}
