using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod;
using FargowiltasCrossmod.Content.Calamity.Buffs;
using FargowiltasCrossmod.Content.Common.Projectiles;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Calamity.Systems;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FargowiltasCrossmod.Content.Common.Bosses.Abominationn
{
    public class AbominationnDLC : CalDLCEmodeExtraGlobalNPC
    {
        public override bool InstancePerEntity => true;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<AbomBoss>());
        public override GlobalNPC NewInstance(NPC target) //the cursed beast
        {
            return WorldSavingSystem.EternityMode && ExtraRequirements() ? base.NewInstance(target) : null;
        }
        public override bool ExtraRequirements()
        {
            return ShouldDoDLC;
        }

        private static bool Thorium => ModCompatibility.ThoriumMod.Loaded;
        private static bool Calamity => ModCompatibility.Calamity.Loaded;
        public static bool ShouldDoDLC
        {
            get => Calamity;
        }

        public enum DLCAttack
        {
            None = 0,
            Mauler
        };
        public enum Variant
        {
            Vanilla,
            Calamity
        }
        public DLCAttack DLCAttackChoice = 0;
        public int Timer = 0;
        public Vector2 LockVector1;
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write7BitEncodedInt((int)DLCAttackChoice);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.WriteVector2(LockVector1);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            DLCAttackChoice = (DLCAttack)binaryReader.Read7BitEncodedInt();
            Timer = binaryReader.Read7BitEncodedInt();
            LockVector1 = binaryReader.ReadVector2();
        }

        public override bool SafePreAI(NPC npc)
        {
            ref float attackChoice = ref npc.ai[0];

            bool ret = base.SafePreAI(npc);

            Player player = Main.player[npc.target];
            if (player == null || !player.active || player.dead)
            {
                return true;
            }

            if (attackChoice == 1) // interrupts scythe spread
            {
                int sharkEndTime = npc.localAI[3] > 1 ? 220 : 140;
                if (Timer < sharkEndTime)
                {
                    if (!AliveCheck(npc, player) || Phase2Check(npc))
                        return true;
                    Timer++;
                    int arcStart = 60;
                    int arcTime = 20;
                    float horizontalDir = -npc.HorizontalDirectionTo(LockVector1);
                    if (Timer <= 5)
                    {
                        LockVector1 = player.Center;
                        npc.netUpdate = true;
                    }
                    if (Timer == arcStart - 2)
                    {
                        SoundEngine.PlaySound(CalamityMod.NPCs.AcidRain.Mauler.RoarSound, npc.Center);
                    }
                    if (Timer >= arcStart && Timer % 1 == 0 && Timer <= arcStart + arcTime)
                    {
                        float arcProgress = (Timer - arcStart) / (float)arcTime;
                        Vector2 startAngle = (Vector2.UnitX * -horizontalDir) - Vector2.UnitY * 0.3f;
                        Vector2 endAngle = Vector2.UnitY + (Vector2.UnitX * horizontalDir) * 0.8f;
                        float angle = Vector2.Lerp(startAngle, endAngle, arcProgress).ToRotation();

                        if (DLCUtils.HostCheck)
                        {
                            int type = ModContent.ProjectileType<AbomMauler>();
                            Vector2 vel = angle.ToRotationVector2() * 3f;
                            vel *= Main.rand.NextFloat(0.7f, 1f);
                            float ai0 = npc.localAI[3] > 1 ? 1 : 0;
                            float ai2 = ai0 == 0 ? 0 : player.whoAmI;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, vel, type, FargoSoulsUtil.ScaledProjectileDamage(npc.defDamage), 0f, ai0: ai0, ai2: ai2);
                        }
                        
                    }
                    float posX = player.Center.X + horizontalDir * 650;
                    float posY = player.Center.Y - 500;
                    Vector2 desiredPos = new(posX, posY);
                    if (Timer < arcStart - 10)
                    {
                        Movement(npc, desiredPos, 2f);
                    }
                    else
                    {
                        npc.velocity *= 0.95f;
                    }
                    npc.direction = npc.spriteDirection = -(int)horizontalDir;
                    ret = false;
                }
            }
            else
            {
                Timer = 0;
            }
            if (!ret) // standard abom stuff in case we don't run normal AI
            {
                if (player.immune || player.hurtCooldowns[0] != 0 || player.hurtCooldowns[1] != 0)
                    npc.As<AbomBoss>().playerInvulTriggered = true;
            }
            return ret;
        }


        private bool AliveCheck(NPC NPC, Player player)
        {
            if ((!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 5000f) && NPC.localAI[3] > 0)
            {
                NPC.TargetClosest();
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 5000f)
                {
                    // let base handle despawn
                    return false;
                }
            }

            if (NPC.timeLeft < 600)
                NPC.timeLeft = 600;

            if (player.Center.Y / 16f > Main.worldSurface)
            {
                NPC.velocity.X *= 0.95f;
                NPC.velocity.Y -= 1f;
                if (NPC.velocity.Y < -32f)
                    NPC.velocity.Y = -32f;
                return false;
            }

            return true;
        }

        private bool Phase2Check(NPC npc)
        {
            if (npc.localAI[3] > 1)
                return false;

            if (npc.life < npc.lifeMax * (WorldSavingSystem.EternityMode ? 0.66 : 0.50) && Main.expertMode)
            {
                if (FargoSoulsUtil.HostCheck)
                {
                    npc.ai[0] = -1;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                    npc.ai[3] = 0;
                    npc.netUpdate = true;
                    FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                }
                return true;
            }
            return false;
        }
        private void Movement(NPC NPC, Vector2 targetPos, float speedModifier, bool fastX = true)
        {
            if (Math.Abs(NPC.Center.X - targetPos.X) > 5)
            {
                if (NPC.Center.X < targetPos.X)
                {
                    NPC.velocity.X += speedModifier;
                    if (NPC.velocity.X < 0)
                        NPC.velocity.X += speedModifier * (fastX ? 2 : 1);
                }
                else
                {
                    NPC.velocity.X -= speedModifier;
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X -= speedModifier * (fastX ? 2 : 1);
                }
            }
            if (NPC.Center.Y < targetPos.Y)
            {
                NPC.velocity.Y += speedModifier;
                if (NPC.velocity.Y < 0)
                    NPC.velocity.Y += speedModifier * 2;
            }
            else
            {
                NPC.velocity.Y -= speedModifier;
                if (NPC.velocity.Y > 0)
                    NPC.velocity.Y -= speedModifier * 2;
            }
            if (Math.Abs(NPC.velocity.X) > 24)
                NPC.velocity.X = 24 * Math.Sign(NPC.velocity.X);
            if (Math.Abs(NPC.velocity.Y) > 24)
                NPC.velocity.Y = 24 * Math.Sign(NPC.velocity.Y);
        }
    }
}
