
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeGodsEternity : EModeCalBehaviour
    {
        public bool Empowered = false;
        public int Timer = 0;
        public bool DidSpecial = false;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<EbonianPaladin>(),
            ModContent.NPCType<CrimulanPaladin>()
        );

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Empowered);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.Write(DidSpecial);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            Empowered = binaryReader.ReadBoolean();
            Timer = binaryReader.Read7BitEncodedInt();
            DidSpecial = binaryReader.ReadBoolean();
        }
        
        //Slimes have much less health because of the phase and respawn mechanic
        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            npc.lifeMax /= 4;
        }
        //Slime the Core is attached to takes 50% damage
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (CalamityGlobalNPC.slimeGod < 0 || CalamityGlobalNPC.slimeGod >= Main.maxNPCs)
            {
                return;
            }
            NPC core = Main.npc[CalamityGlobalNPC.slimeGod];
            if (core != null && core.active && core.type == ModContent.NPCType<SlimeGodCore>())
            {
                if (Empowered)
                {
                    modifiers.FinalDamage /= 2;
                }
            }
            
        }
        //Slime the Core is attached to draws a glow aura
        
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (CalamityGlobalNPC.slimeGod < 0 || CalamityGlobalNPC.slimeGod >= Main.maxNPCs)
            {
                return true;
            }
            NPC core = Main.npc[CalamityGlobalNPC.slimeGod];
            if (core != null && core.active && core.type == ModContent.NPCType<SlimeGodCore>())
            {
                if (Empowered)
                {
                    //draw glow aura
                    for (int j = 0; j < 12; j++)
                    {
                        Texture2D texture = TextureAssets.Npc[npc.type].Value;

                        SpriteEffects effects = 0;
                        if (npc.spriteDirection == 1)
                        {
                            effects = (SpriteEffects)1;
                        }

                        Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 6f;
                        Color color = npc.type == ModContent.NPCType<EbonianPaladin>() ? Color.Purple : Color.Red;
                        Color glowColor = color with { A = 0 } * 0.7f;

                        Vector2 wackySlimeOffsetY = -Vector2.UnitY * npc.height * 0.125f;
                        Vector2 offset = afterimageOffset + wackySlimeOffsetY - screenPos + new Vector2(0f, npc.gfxOffY);

                        Main.EntitySpriteDraw(texture, npc.Center + offset, npc.frame, glowColor, npc.rotation, Utils.Size(npc.frame) / 2f, npc.scale, effects);
                    }
                }
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            if (CalamityGlobalNPC.slimeGod < 0 || CalamityGlobalNPC.slimeGod >= Main.maxNPCs)
            {
                return true;
            }
            NPC core = Main.npc[CalamityGlobalNPC.slimeGod];
            if (core != null && core.active && core.type == ModContent.NPCType<SlimeGodCore>())
            {
                if (Empowered)
                {
                    return EmpoweredAI(npc);
                }
            }
            return true;
        }
        public bool EmpoweredAI(NPC npc)
        {
            ref float calState = ref npc.ai[0];
            ref float calTimer = ref npc.ai[3];
            switch (calState)
            {
                case 2:
                    {
                        npc.noTileCollide = true;
                        npc.noGravity = true;
                        if (npc.velocity.X < 0f)
                        {
                            npc.direction = -1;
                        }
                        else
                        {
                            npc.direction = 1;
                        }

                        if (Timer == 1)
                        {
                            SoundEngine.PlaySound(SoundID.Item154, npc.Center);
                        }

                        npc.spriteDirection = npc.direction;
                        npc.TargetClosest();
                        Player player = Main.player[npc.target];
                        Vector2 desiredPos = player.Center;
                        desiredPos.Y -= 350f;
                        Vector2 toPos = desiredPos - npc.Center;
                        if (Math.Abs(npc.Center.X - desiredPos.X) < 40)
                        {
                            if (DLCUtils.HostCheck)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + Vector2.UnitY * npc.height / 2, Vector2.UnitY, ModContent.ProjectileType<SlamTelegraph>(), 0, 0, Main.myPlayer, ai1: npc.width * 1.2f);
                            }

                            npc.ai[1] = 0f;
                            npc.ai[2] = 1f;
                            calState = 2.05f;
                            Timer = 0;
                            npc.netUpdate = true;
                            return false;
                        }

                        toPos.Normalize();
                        const float distanceFactor = 0.02f;
                        float maxSpeed = 22f + npc.Distance(desiredPos) * distanceFactor;
                        float speed = maxSpeed * Math.Min(Timer / 60, 1);
                        toPos *= speed;
                        npc.velocity = toPos;
                        //npc.velocity = (npc.velocity * 5f + toPos) / 6f;
                        Timer++;
                    }
                    return false;
                case 2.05f:
                    {
                        npc.velocity.X *= 0.92f;
                        npc.velocity.Y = 0;
                        if (++Timer > 40)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, npc.Center);
                            npc.ai[1] = 0f;
                            npc.ai[0] = 2.1f;
                            npc.ai[2] = 0f;
                            npc.velocity = Vector2.UnitY * 30;
                            Timer = 0;
                            npc.netUpdate = true;
                        }
                    }
                    break;
                case 2.1f:
                    {
                        if (npc.type == ModContent.NPCType<EbonianPaladin>())
                        {
                            return CorruptionSlamAttack(npc);
                        }
                        else
                        {
                            return CrimsonSlamAttack(npc);
                        }
                    }
                case 3:
                    {
                        if (!DidSpecial)
                        {
                            calState = 22;
                            DidSpecial = true;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            calState = 2;
                            DidSpecial = false;
                            npc.netUpdate = true;
                        }
                    }
                    return false;
                case 22:
                    {
                        if (npc.type == ModContent.NPCType<EbonianPaladin>())
                        {
                            return CorruptionSpecial(npc);
                        }
                        else
                        {
                            return CrimsonSpecial(npc);
                        }
                    }
                default:
                    {
                        Timer = 0;
                    }
                    break;
            }
            return true;
        }
        public bool CorruptionSlamAttack(NPC npc)
        {
            ref int Timer = ref npc.GetGlobalNPC<SlimeGodsEternity>().Timer;
            Player player = Main.player[npc.target];
            if (player == null || !player.active || player.dead)
            {
                return true;
            }
            bool flag8 = npc.position.Y + (float)npc.height >= player.position.Y;
            if (npc.ai[2] == 0f && flag8 && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
            {
                npc.ai[2] = 1f;
                npc.netUpdate = true;
            }
            const int shootDelay = 5;
            if (++Timer % shootDelay == 0)
            {
                if (DLCUtils.HostCheck)
                {
                    float speed = 5.5f;
                    int type = ModContent.ProjectileType<UnstableEbonianGlob>();
                    int projectileDamage = npc.GetProjectileDamage(type);
                    for (int i = -1; i < 2; i++)
                    {
                        Vector2 dir = (-Vector2.UnitY).RotatedBy(MathHelper.PiOver2 * i);
                        Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * npc.height / 2, dir * speed, type, projectileDamage, 3f, Main.myPlayer);
                    }
                }
            }
            if (flag8 || npc.velocity.Y <= 0f)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] > 10f)
                {
                    SoundEngine.PlaySound(in SlimeGodCore.BigShotSound, npc.Center);


                    npc.localAI[2] = npc.ai[0] - 0.1f;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    Timer = 0;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 0f)
            {
                npc.noTileCollide = true;
            }

            npc.noGravity = true;
            npc.velocity.Y += 1f;
            float maxSpeed = 30f;
            if (npc.velocity.Y > maxSpeed)
            {
                npc.velocity.Y = maxSpeed;
            }
            return false;
        }
        public bool CrimsonSlamAttack(NPC npc)
        {
            ref int Timer = ref npc.GetGlobalNPC<SlimeGodsEternity>().Timer;
            Player player = Main.player[npc.target];
            if (player == null || !player.active || player.dead)
            {
                return true;
            }
            bool flag8 = npc.position.Y + (float)npc.height >= player.position.Y;
            if (npc.ai[2] == 0f && flag8 && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
            {
                npc.ai[2] = 1f;
                npc.netUpdate = true;
            }
            if (flag8 || npc.velocity.Y <= 0f)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] > 10f)
                {
                    SoundEngine.PlaySound(in SlimeGodCore.BigShotSound, npc.Center);
                    if (DLCUtils.HostCheck)
                    {
                        float speed = 2.6f;
                        int type = ModContent.ProjectileType<UnstableCrimulanGlob>();
                        int projectileDamage = npc.GetProjectileDamage(type);
                        const int GlobCount = 7;
                        for (int j = -1; j < 2; j += 2)
                        {
                            for (int i = -GlobCount; i < GlobCount; i++)
                            {
                                Vector2 dir = (Vector2.UnitY).RotatedBy(MathHelper.PiOver2 * j).RotatedBy(MathHelper.Pi * ((float)i / GlobCount / 3));
                                float projSpeed = speed + Main.rand.NextFloat(2.5f);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * npc.height / 2, dir * projSpeed, type, projectileDamage, 3f, Main.myPlayer);
                            }
                        }

                    }

                    npc.localAI[2] = npc.ai[0] - 0.1f;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    Timer = 0;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 0f)
            {
                npc.noTileCollide = true;
            }

            npc.noGravity = true;
            npc.velocity.Y += 1f;
            float maxSpeed = 30f;
            if (npc.velocity.Y > maxSpeed)
            {
                npc.velocity.Y = maxSpeed;
            }
            return false;
        }
        public bool CorruptionSpecial(NPC npc)
        {
            ref float bounces = ref npc.ai[2];
            if (Timer == 0)
            {
                FargowiltasSouls.Common.Graphics.Particles.Particle p = new ExpandingBloomParticle(npc.Center, Vector2.Zero, Color.Magenta, Vector2.One, Vector2.One * 60, 40, true, Color.Transparent);
                p.Spawn();
            }
            if (++Timer > 60)
            {
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                Timer = 0;
                npc.netUpdate = true;
            }
            return false;
        }
        public bool CrimsonSpecial(NPC npc)
        {
            if (++Timer > 60)
            {
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                Timer = 0;
                npc.netUpdate = true;
            }
            return false;
        }
        
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeGodMinionEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(SlimeGodCoreEternity.SlimesToKill.ToArray());
        public bool LobotomizeAndSuck = false;
        public int CoreNPCID = -1;
        public int Timer = 0;
        public float origScale = 1f;
        public override bool SafePreAI(NPC npc)
        {
            if (LobotomizeAndSuck)
            {
                const float suckTime = 90f;
                NPC core = Main.npc[CoreNPCID];
                if (core == null || !core.active || core.type != ModContent.NPCType<SlimeGodCore>())
                {
                    npc.active = false;
                    return false;
                }
                if (Timer == 0)
                {
                    origScale = npc.scale;
                }
                float modifier = 1 - (Timer / suckTime);
                npc.scale = origScale * modifier;
                npc.Opacity = modifier;
                npc.velocity = (core.Center - npc.Center) * (Timer / suckTime) * 0.25f;
                if (Timer >= suckTime)
                {
                    npc.active = false;
                }
                Timer++;
                return false;
            }
            return base.SafePreAI(npc);
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => !LobotomizeAndSuck;
    }
}
