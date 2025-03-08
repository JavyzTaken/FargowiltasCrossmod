using CalamityMod.Events;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.World;
using FargowiltasCrossmod.Core;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using CalamityMod;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasSouls;
using ReLogic.Content;
using CalamityMod.Projectiles.Summon;
using Terraria.Audio;
using CalamityMod.Projectiles.Boss;
using Terraria.ID;
using CalamityMod.NPCs.AcidRain;
using FargowiltasSouls.Content.Bosses.VanillaEternity;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.NPCs;
using Terraria.ModLoader.IO;
using System.IO;
using CalamityMod.Particles;
using FargowiltasSouls.Core.Systems;
using FargowiltasCrossmod.Core.Calamity;
using static log4net.Appender.RollingFileAppender;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.AquaticScourge
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class ASEternity : CalDLCEmodeBehavior
    {
        public static bool Enabled = true;
        public override bool IsLoadingEnabled(Mod mod) => Enabled;
        public override int NPCOverrideID => ModContent.NPCType<AquaticScourgeHead>();

    }
    public class ASPartsEternity : CalDLCEmodeExtraGlobalNPC
    {
        public override bool IsLoadingEnabled(Mod mod) => ASEternity.Enabled;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<AquaticScourgeBody>(), 
            ModContent.NPCType<AquaticScourgeHead>(), 
            ModContent.NPCType<AquaticScourgeBodyAlt>(), 
            ModContent.NPCType<AquaticScourgeTail>());

        public static readonly string TexturePath = "FargowiltasCrossmod/Assets/ExtraTextures/AquaticScourge/";
        int drawTimer = 0;
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            if (npc.type == ModContent.NPCType<AquaticScourgeHead>())
            {
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (npc.spriteDirection == 1)
                    spriteEffects = SpriteEffects.FlipHorizontally;

                
                Texture2D head = ModContent.Request<Texture2D>(TexturePath + "ASHead", AssetRequestMode.ImmediateLoad).Value;
                Texture2D left = ModContent.Request<Texture2D>(TexturePath + "ASJawLeft", AssetRequestMode.ImmediateLoad).Value;
                Texture2D right = ModContent.Request<Texture2D>(TexturePath + "ASJawRight", AssetRequestMode.ImmediateLoad).Value;
                Vector2 scaledDraw = new Vector2(TextureAssets.Npc[npc.type].Value.Width / 2, TextureAssets.Npc[npc.type].Value.Height / 2);

                Vector2 drawLocation = npc.Center - screenPos;
                drawLocation -= new Vector2(head.Width, head.Height) * npc.scale / 2f;
                drawLocation += scaledDraw * npc.scale + new Vector2(0f, npc.gfxOffY);
                Color color = npc.GetAlpha(drawColor);

                if (CalamityWorld.revenge || BossRushEvent.BossRushActive || Main.zenithWorld)
                {
                    if (npc.Calamity().newAI[3] > 300f)
                        color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp((npc.Calamity().newAI[3] - 300f) / 180f, 0f, 1f));
                    else if (npc.localAI[3] > 0f)
                        color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp(npc.localAI[3] / 90f, 0f, 1f));
                }

                float open = Math.Clamp(OpenMouth, 0, 1);
                Vector2 offset = new(-26, 12);
                float maxOpen = MathHelper.PiOver4 * 1f;

                if (npc.HasPlayerTarget && !Main.player[npc.target].Calamity().ZoneSulphur)
                {
                    for (int j = 0; j < 12; j++)
                    {
                        Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 2f * npc.scale;
                        Color glowColor = npc.GetAlpha(Color.Red with { A = 0 } * 0.9f);
                        // head
                        spriteBatch.Draw(head, npc.Center + afterimageOffset - screenPos, npc.frame, glowColor, npc.rotation, head.Size() / 2, 1, spriteEffects, 0);
                        // left
                        spriteBatch.Draw(left, drawLocation + afterimageOffset + offset.RotatedBy(npc.rotation), null, glowColor, npc.rotation - open * maxOpen, left.Size() / 2, npc.scale, SpriteEffects.None, 0);
                        offset.X *= -1;
                        // right
                        spriteBatch.Draw(right, drawLocation + afterimageOffset + offset.RotatedBy(npc.rotation), null, glowColor, npc.rotation + open * maxOpen, right.Size() / 2, npc.scale, SpriteEffects.None, 0);
                        offset.X *= -1;
                    }
                }
                // head
                spriteBatch.Draw(head, npc.Center - screenPos, npc.frame, color, npc.rotation, head.Size() / 2, 1, spriteEffects, 0);
                // left
                spriteBatch.Draw(left, drawLocation + offset.RotatedBy(npc.rotation), null, color, npc.rotation - open * maxOpen, left.Size() / 2, npc.scale, SpriteEffects.None, 0);
                offset.X *= -1;
                // right
                spriteBatch.Draw(right, drawLocation + offset.RotatedBy(npc.rotation), null, color, npc.rotation + open * maxOpen, right.Size() / 2, npc.scale, SpriteEffects.None, 0);
                return false;
            }
            else
            {
                if (npc.ai[2] < 0)
                    return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>())
            {
                if (npc.ai[2] == -1)
                {
                    return;
                }
                NPC head = Main.npc[(int)npc.ai[2]];
                

                if (CurrentHittable(npc))
                {
                    Texture2D gut = ModContent.Request<Texture2D>(TexturePath + "ASGut", AssetRequestMode.ImmediateLoad).Value;
                    int frames = 9;
                    int framePeriod = (int)(5 + 5f * head.GetLifePercent());
                    int frame = ++drawTimer / framePeriod;
                    frame %= frames;
                    int height = gut.Height / frames;
                    Rectangle rect = new(0, frame * height, gut.Width, height);
                    Vector2 origin = rect.Size() / 2;
                    spriteBatch.Draw(gut, npc.Center - screenPos, rect, drawColor, npc.rotation, origin, 1, SpriteEffects.None, 0);
                }


            }
        }
        public bool CurrentHittable(NPC npc)
        {
            if (npc.ai[2] < 0)
                return false;
            NPC head = Main.npc[(int)npc.ai[2]];
            ASPartsEternity ashead = head.GetGlobalNPC<ASPartsEternity>();
            return (head.GetLifePercent() > Phase2Percent && ashead.Hittable1 == npc.whoAmI) ||
                    (head.GetLifePercent() > Phase3Percent && ashead.Hittable2 == npc.whoAmI && head.GetLifePercent() <= Phase2Percent) ||
                    (ashead.Hittable3 == npc.whoAmI && head.GetLifePercent() <= Phase3Percent);
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return base.CanBeHitByProjectile(npc, projectile);
        }
        public override bool? CanCollideWithPlayerMeleeAttack(NPC npc, Player player, Item item, Rectangle meleeAttackHitbox)
        {
            return base.CanCollideWithPlayerMeleeAttack(npc, player, item, meleeAttackHitbox);
        }
        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            if (npc.type == ModContent.NPCType<AquaticScourgeHead>() && (npc.justHit || npc.life <= npc.lifeMax * 0.999 || BossRushEvent.BossRushActive || Main.getGoodWorld))
            {
                return true;
            }
            if ((npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>()) && npc.ai[2] >= 0) {
                NPC head = Main.npc[(int)npc.ai[2]];
                ASPartsEternity ashead = head.GetGlobalNPC<ASPartsEternity>();
                if ((head.GetLifePercent() > Phase2Percent && ashead.Hittable1 == npc.whoAmI) ||
                        (head.GetLifePercent() > Phase3Percent && ashead.Hittable2 == npc.whoAmI && head.GetLifePercent() <= Phase2Percent) ||
                        (ashead.Hittable3 == npc.whoAmI && head.GetLifePercent() <= Phase3Percent))
                {
                    return true;
                }
            }
            return base.DrawHealthBar(npc, hbPosition, ref scale, ref position);
        }
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            entity.lifeMax = (int)Math.Round(entity.lifeMax * 0.56f);
            entity.defense = 20;
            entity.Calamity().DR = 0;
            entity.buffImmune[BuffID.Darkness] = true;

            entity.damage = 110;
            if (BossRushEvent.BossRushActive) entity.damage = 300;
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitByProjectile(npc, projectile, ref modifiers);

            //if (ProjectileID.Sets.CultistIsResistantTo[projectile.type] && !FargoSoulsUtil.IsSummonDamage(projectile))
            //    modifiers.FinalDamage *= 0.8f;
        }
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Hittable);
            binaryWriter.Write(HasDoneSpinny);
            binaryWriter.Write7BitEncodedInt(SuckCooldown);
            binaryWriter.Write7BitEncodedInt(Hittable1);
            binaryWriter.Write7BitEncodedInt(Hittable2);
            binaryWriter.Write7BitEncodedInt(Hittable3);

            binaryWriter.Write7BitEncodedInt(FollowTimer);
            binaryWriter.Write7BitEncodedInt(RocksTimer);
            binaryWriter.Write7BitEncodedInt(SpikeTimer);
            binaryWriter.Write7BitEncodedInt(SpinTimer);
            binaryWriter.Write7BitEncodedInt(SuckTimer);

            binaryWriter.Write7BitEncodedInt(AttackChain);
            binaryWriter.Write7BitEncodedInt(AttackPart);

            binaryWriter.Write7BitEncodedInt(LastFewAttacks[0]);
            binaryWriter.Write7BitEncodedInt(LastFewAttacks[1]);

            binaryWriter.Write(FollowTurnSpeed);
            binaryWriter.Write(DashAttackTimer);
            binaryWriter.Write(StorePosition.X);
            binaryWriter.Write(StorePosition.Y);
            base.SendExtraAI(npc, bitWriter, binaryWriter);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            Hittable = binaryReader.ReadBoolean();
            HasDoneSpinny = binaryReader.ReadBoolean();

            SuckCooldown = binaryReader.Read7BitEncodedInt();
            Hittable1 = binaryReader.Read7BitEncodedInt();
            Hittable2 = binaryReader.Read7BitEncodedInt();
            Hittable3 = binaryReader.Read7BitEncodedInt();

            FollowTimer = binaryReader.Read7BitEncodedInt();
            RocksTimer = binaryReader.Read7BitEncodedInt();
            SpikeTimer = binaryReader.Read7BitEncodedInt();
            SpinTimer = binaryReader.Read7BitEncodedInt();
            SuckTimer = binaryReader.Read7BitEncodedInt();

            AttackChain = binaryReader.Read7BitEncodedInt();
            AttackPart = binaryReader.Read7BitEncodedInt();

            LastFewAttacks[0] = binaryReader.Read7BitEncodedInt();
            LastFewAttacks[1] = binaryReader.Read7BitEncodedInt();

            FollowTurnSpeed = binaryReader.ReadSingle();
            DashAttackTimer = binaryReader.ReadSingle();
            StorePosition.X = binaryReader.ReadSingle();
            StorePosition.Y = binaryReader.ReadSingle();

        }

        //segments
        public bool Hittable;

        //head
        public int Hittable1 = 0;
        public int Hittable2 = 0;
        public int Hittable3 = 0;

        public float FollowTurnSpeed = 0.4f;
        public static float FollowTurnSpeedMax => 0.5f;
        public int FollowTimer;


        public int AttackChain = 0;
        public int AttackPart = 0;

        public float DashAttackTimer = 0;
        public float OpenMouth = 0;
        public int RocksTimer = 0;
        public int SpikeTimer = 0;

        public int SuckTimer = 0;
        public int SpinTimer = 0;
        public Vector2 StorePosition = Vector2.Zero;

        public int[] LastFewAttacks = [-1, -1];
        public int SuckCooldown = 0;
        public bool HasDoneSpinny = false;

        public float Phase2Percent = 0.75f;
        public float Phase3Percent = 0.4f;
        public int ProjectileDamage = 120;

        public enum Attacks {
            Follow,
            RockDashDashRock,
            GasGasGasRockDash,
            LittleDashes,

            Transition1,

            LittleDashesHomingSpikes,
            SpikesThenGas,
            GasThenSpikes,
            CircleSpikes,
            BigSuck,

            Transition2,

        };
        
        
        public override bool SafePreAI(NPC npc)
        {
            if(npc.type == ModContent.NPCType<AquaticScourgeTail>())
            {
                npc.dontTakeDamage = true;
                npc.CalamityDLC().ImmuneToAllDebuffs = true;
            }
            //npc.ai[0] = 3;
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>())
            {
                if (npc.width > 32)
                {
                    npc.position = npc.Center;
                    npc.width = 32;
                    npc.height = 32;
                    npc.Center = npc.position;
                }
                if (npc.ai[2] == -1)
                {
                    npc.StrikeInstantKill();
                    return false;
                }
                NPC head = Main.npc[(int)npc.ai[2]];
                ASPartsEternity ashead = head.GetGlobalNPC<ASPartsEternity>();
                npc.dontTakeDamage = true;
                npc.CalamityDLC().ImmuneToAllDebuffs = true;

                if (CurrentHittable(npc))
                {
                    npc.dontTakeDamage = false;
                    npc.CalamityDLC().ImmuneToAllDebuffs = false;
                }
                //take damage if near a segment that can
                /*
                NPC prev = Main.npc[(int)npc.ai[1]];
                NPC next = Main.npc[(int)npc.ai[0]];
                NPC prevprev = Main.npc[(int)Main.npc[(int)npc.ai[1]].ai[1]];
                NPC nextnext = Main.npc[(int)Main.npc[(int)npc.ai[0]].ai[0]];
                if ((next != null && next.active && !next.dontTakeDamage && (ashead.Hittable1 == next.whoAmI || ashead.Hittable2 == next.whoAmI || ashead.Hittable3 == next.whoAmI)) ||
                    (prev != null && prev.active && !prev.dontTakeDamage && (ashead.Hittable1 == prev.whoAmI || ashead.Hittable2 == prev.whoAmI || ashead.Hittable3 == prev.whoAmI)) ||
                    (nextnext != null && nextnext.active && !nextnext.dontTakeDamage && (ashead.Hittable1 == nextnext.whoAmI || ashead.Hittable2 == nextnext.whoAmI || ashead.Hittable3 == nextnext.whoAmI)) ||
                    (prevprev != null && prevprev.active && !prevprev.dontTakeDamage && (ashead.Hittable1 == prevprev.whoAmI || ashead.Hittable2 == prevprev.whoAmI || ashead.Hittable3 == prevprev.whoAmI)))
                {
                    //npc.dontTakeDamage = false;
                    //npc.CalamityDLC().ImmuneToAllDebuffs = true; // still immune to debuffs!!
                }
                */
                //take damage if in non hostile phase
                if (!(head.justHit || head.life <= head.lifeMax * 0.999 || BossRushEvent.BossRushActive || Main.getGoodWorld))
                {
                    npc.dontTakeDamage = false;
                    npc.CalamityDLC().ImmuneToAllDebuffs = false;
                }
                //Main.NewText(head.GetLifePercent());
                if ((ashead.Hittable1 == npc.whoAmI && head.GetLifePercent() < Phase2Percent + 0.05f) ||
                    (ashead.Hittable2 == npc.whoAmI && head.GetLifePercent() < Phase3Percent + 0.05f))
                {
                    ashead.SuckCooldown = 0;
                    DestroyAllSegmentsBelowMe();
                    NPC.HitInfo info = new NPC.HitInfo();
                    info.Damage = (int)(head.lifeMax * 0.05f);
                    head.StrikeNPC(info, false, true);
                    ashead.AttackPart = 0;
                    ashead.DashAttackTimer = 0;
                    ashead.FollowTimer = 0;
                    ashead.RocksTimer = 0;
                    ashead.SuckTimer = 0;
                    ashead.SpikeTimer = 0;
                    ashead.SpinTimer = 0;
                    if (FargoSoulsUtil.HostCheck)
                    {
                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            if (Main.projectile[i].active && Main.projectile[i].type == ModContent.ProjectileType<AquaticSuck>() && Main.projectile[i].ai[0] == head.whoAmI)
                            {
                                Main.projectile[i].Kill();
                                NetMessage.SendData(MessageID.SyncProjectile, number: i);
                                break;
                            }
                        }
                    }
                    ashead.AttackChain = head.GetLifePercent() > Phase3Percent + 0.05f ? (int)Attacks.Transition1 : (int)Attacks.Transition2;
                }

                void DestroyAllSegmentsBelowMe()
                {
                    bool reachedTail = false;
                    int thing = npc.whoAmI;
                    while (!reachedTail)
                    {
                        if (Main.npc[thing].type != ModContent.NPCType<AquaticScourgeTail>())
                        {
                            var segment = Main.npc[thing];
                            segment.realLife = -1;
                            segment.ai[2] = -1;
                            thing = (int)segment.ai[0];
                        }
                        else
                        {
                            reachedTail = true;
                            Main.npc[thing].ai[1] = npc.ai[1];
                        }
                    }
                }
            }
            if (npc.type == ModContent.NPCType<AquaticScourgeHead>() && npc.Calamity().newAI[2] == 0 && npc.ai[0] == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int maxLength = 50;
                    int Previous = npc.whoAmI;
                    for (int segments = 0; segments < maxLength; segments++)
                    {
                        int next;
                        if (segments >= 0 && segments < maxLength - 1)
                        {
                            if (segments % 2 == 0)
                                next = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<AquaticScourgeBodyAlt>(), npc.whoAmI);
                            else
                                next = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<AquaticScourgeBody>(), npc.whoAmI);
                        }
                        else
                            next = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<AquaticScourgeTail>(), npc.whoAmI);

                        Main.npc[next].realLife = npc.whoAmI;
                        Main.npc[next].ai[2] = npc.whoAmI;
                        Main.npc[next].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = next;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, next);
                        Previous = next;
                    }
                }

                npc.Calamity().newAI[2] = 1f;
                return false;
            }
            if (npc.type == ModContent.NPCType<AquaticScourgeHead>() && (npc.justHit || npc.life <= npc.lifeMax * 0.999 || BossRushEvent.BossRushActive || Main.getGoodWorld))
            {
                npc.boss = true;
                npc.damage = npc.GetAttackDamage_ScaledByStrength(60);
                if (BossRushEvent.BossRushActive) npc.damage = npc.GetAttackDamage_ScaledByStrength(300);
                npc.TargetClosest();
                npc.dontTakeDamage = true;
                npc.CalamityDLC().ImmuneToAllDebuffs = true;
                //increases when angle is too different. increases turn rate when gets high enough. decreases over time when angle isnt too different

                //Main.NewText(npc.GetLifePercent());

                //max length is 40 in revengeance and 80 in death. i dont feel like changing the max length.
                int maxLength = 50;

                //get segments that will be hittable throughout the fight
                if (Hittable1 == 0)
                {
                    Hittable1 = FindSegment(2 * maxLength / 3);
                }
                if (Hittable2 == 0)
                {
                    Hittable2 = FindSegment(maxLength / 3);
                }
                if (Hittable3 == 0)
                {
                    Hittable3 = FindSegment(maxLength / 5);
                }

                if (npc.GetLifePercent() > Phase2Percent)
                {
                    Main.npc[Hittable1].GetGlobalNPC<ASPartsEternity>().Hittable = true;
                }
                else if (npc.GetLifePercent() > Phase3Percent)
                {
                    Main.npc[Hittable2].GetGlobalNPC<ASPartsEternity>().Hittable = true;
                }
                else
                {
                    Main.npc[Hittable3].GetGlobalNPC<ASPartsEternity>().Hittable = true;
                }

                int FindSegment(int howFarDown)
                {
                    NPC segment = npc;
                    for (int i = 0; i < howFarDown; i++)
                    {
                        NPC thing = Main.npc[(int)segment.ai[0]];
                        if (thing != null && thing.active && (thing.type == ModContent.NPCType<AquaticScourgeBody>() || thing.type == ModContent.NPCType<AquaticScourgeBodyAlt>()))
                        {
                            segment = thing;
                        }
                    }
                    return segment.whoAmI;
                }

                //Main.NewText(Vars[9]);
                SoundStyle DSroar = new("CalamityMod/Sounds/Custom/DesertScourgeRoar");
                SoundStyle Mroar = Mauler.RoarSound;




                Player target = null;
                if (npc.HasValidTarget)
                {
                    

                    target = Main.player[npc.target];
                    Vector2 targetPos = target.Center;


                    float angleDiff = MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(npc.velocity, npc.AngleTo(targetPos).ToRotationVector2()));
                    if (Math.Abs(angleDiff) > 10)
                    {
                        FollowTurnSpeed += 0.0065f;
                    }else if (FollowTurnSpeed > FollowTurnSpeedMax)
                    {
                        FollowTurnSpeed -= 0.01f;
                    }
                    if (SuckCooldown > 0)
                        SuckCooldown--;
                    
                    //Follow(10, aggroTimer);
                    switch ((Attacks)AttackChain)
                    {
                        case Attacks.RockDashDashRock:
                            if (AttackPart == 0)
                            {
                                RandomRocks(6, npc.AngleTo(target.Center), 40);
                            }
                            else if (AttackPart == 1 || AttackPart == 2)
                            {
                                Dash(25, 100);
                            }
                            else if (AttackPart == 3)
                            {
                                RandomRocks(28, MathHelper.ToRadians(-90), 50);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.GasGasGasRockDash:
                            if (AttackPart < 3)
                            {
                                Dash(25, 100, 1);
                            }
                            else if (AttackPart == 3)
                            {
                                RandomRocks(28, MathHelper.ToRadians(-90), 50);
                            }
                            else if (AttackPart == 4)
                            {
                                Dash(30, 140);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.LittleDashes:
                            if (AttackPart < 3)
                            {
                                Dash(30, 60);
                            }
                            else if (AttackPart == 3)
                            {
                                Dash(25, 100, 1);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.LittleDashesHomingSpikes:
                            if (AttackPart < 8 && AttackPart % 2 == 0)
                            {
                                Dash(30, 80);
                            }
                            else if (AttackPart < 8)
                            {
                                Spikes(5, 30, true, 25);
                            }
                            else if (AttackPart == 8)
                            {
                                Dash(25, 100);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.SpikesThenGas:
                            if (AttackPart == 0)
                            {
                                Dash(25, 80);
                            }
                            else if (AttackPart == 1)
                            {
                                Spikes(5, 40, false);
                            }
                            else if (AttackPart < 4)
                            {
                                Dash(25, 100, 1);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.GasThenSpikes:
                            if (AttackPart < 3)
                            {
                                Dash(25, 100, 1);
                            }
                            else if (AttackPart == 3)
                            {
                                Dash(25, 80);

                            }
                            else if (AttackPart == 4)
                            {
                                Spikes(5, 40, false);
                            }
                            else
                            {
                                AttackChain = (int)Attacks.Follow;
                                AttackPart = 0;
                            }
                            break;
                        case Attacks.BigSuck:
                            if (AttackPart == 0)
                            {
                                Follow(2, 4);
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, Vector2.Zero, ModContent.ProjectileType<AquaticSuck>(), 0, 0, -1, npc.whoAmI);
                                    NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                }
                                AttackPart++;
                            }
                            else if (AttackPart == 1)
                            {
                                SuckTimer++;
                                if (SuckTimer <= 60)
                                {
                                    OpenMouth = MathHelper.SmoothStep(0, 1, SuckTimer / 60f);
                                }
                                else
                                {
                                    Vector2 pull = target.AngleTo(npc.Center).ToRotationVector2() * 0.2f;
                                    target.velocity.X += pull.X;
                                    if (target.velocity.Y != 0)
                                        target.velocity.Y += pull.Y;
                                }
                                if (SuckTimer % 60 == 0 && SuckTimer <= 350 && FargoSoulsUtil.HostCheck)
                                {
                                    for (int i = -4; i < 5; i++)
                                    {
                                        Vector2 pos = npc.Center + npc.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(i*15 + Main.rand.NextFloat(-20, 21))) * (960 + npc.Distance(targetPos) + Main.rand.NextFloat(0, 50));
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), pos, pos.AngleTo(npc.Center).ToRotationVector2() * 4, ModContent.ProjectileType<SuckedRock>(), FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1, -1, npc.whoAmI, i == 0 ? 1 : 0);
                                        NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                    }
                                }
                                if (SuckTimer > 350 && SuckTimer <= 380)
                                {
                                    OpenMouth = MathHelper.SmoothStep(1, 0, (SuckTimer - 350) / 30f);
                                }
                                if (npc.Distance(targetPos) > 600)
                                {
                                    Follow(10, 1);
                                }
                                else
                                {
                                    Follow(3, 1);
                                }
                                if (SuckTimer >= 430)
                                {
                                    OpenMouth = MathHelper.SmoothStep(0, 1, (SuckTimer - 430f) / 20f);
                                }
                                if (SuckTimer >= 450)
                                {
                                    SuckTimer = 0;
                                    AttackPart++;
                                }
                            }else if (AttackPart == 2)
                            {
                                SpinAround(18, 3, 140);
                                if (SpinTimer % 20 == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-20, 20))) * Main.rand.NextFloat(10, 20), ModContent.ProjectileType<CrabBoulder>(), FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1);
                                            NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                        }
                                    }
                                }
                            }else if (AttackPart == 3)
                            {

                                if (npc.Distance(target.Center) > 500 && SuckTimer < 30)
                                {
                                    Follow(20, 3, 1.05f);
                                }
                                else if (SuckTimer < 50)
                                {
                                    targetPos = target.Center + target.velocity * 40;
                                    Follow(7, 5);
                                }
                                else
                                {
                                    Follow(4, 1);
                                }
                                    SuckTimer++;
                                if ( SuckTimer > 50 && SuckTimer <= 80 && SuckTimer % 4 == 0)
                                {
                                    SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(-10, 11))) * Main.rand.NextFloat(15, 20), ModContent.ProjectileType<SuckedRock>(), FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1, -1, npc.whoAmI, 3);
                                        NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                    }
                                }
                                if (SuckTimer >= 80)
                                {
                                    OpenMouth = MathHelper.SmoothStep(1, 0, (SuckTimer - 80f) / 20f);
                                }
                                if (SuckTimer > 100)
                                {
                                    SuckTimer = 0;
                                    AttackPart++;
                                }
                            }
                            else
                            {
                                AttackPart = 0;
                                AttackChain = (int)Attacks.Follow;
                                SuckCooldown = 60 * 55;
                                npc.netUpdate = true;
                            }
                            break;
                        case Attacks.CircleSpikes:
                            //Main.NewText(AttackPart);
                            if (!HasDoneSpinny)
                            {
                                HasDoneSpinny = true;
                                npc.netUpdate = true;
                            }
                            if (AttackPart == 0)
                            {
                                
                                Dash(30, 80);
                                
                            }
                            else if (AttackPart == 1)
                            {
                                SpinAround(20, -5, 260, 1.1f);
                                if (SpinTimer % 15 == 0)
                                {
                                    NPC n = npc;
                                    for (int i = 0; i < 10; i++)
                                    {
                                        n = Main.npc[(int)n.ai[0]];
                                        if (n == null || !n.active || (n.type != ModContent.NPCType<AquaticScourgeBody>() && n.type != ModContent.NPCType<AquaticScourgeBodyAlt>()))
                                        {
                                            break;
                                        }
                                        else if (n.type == ModContent.NPCType<AquaticScourgeBody>())
                                        {
                                            if (FargoSoulsUtil.HostCheck)
                                            {
                                                int type = ModContent.ProjectileType<Tooth>();
                                                int proj = Projectile.NewProjectile(n.GetSource_FromAI(), n.Center, n.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(90 + 50)) * 20, type, FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1, ai0: 25, ai1: 0, ai2: target.whoAmI);
                                                int proj2 = Projectile.NewProjectile(n.GetSource_FromAI(), n.Center, n.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(90 - 50)) * 20, type, FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1, ai0: 25, ai1: 0, ai2: target.whoAmI);
                                                NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                                NetMessage.SendData(MessageID.SyncProjectile, number: proj2);
                                            }
                                            SoundEngine.PlaySound(SoundID.Item17, npc.Center);
                                        }

                                    }
                                }
                            }else if (AttackPart == 2)
                            {
                                Dash(30, 80);
                            }
                            else
                            {
                                AttackPart = 0;
                                AttackChain = (int)Attacks.Follow;
                            }
                            break;
                        case Attacks.Transition1:
                            FollowTimer++;
                            npc.velocity *= 0.95f;
                            if (FollowTimer == 80)
                            {
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    for (int i = 0; i < 40; i++)
                                    {
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(2, 9), 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ToxicGas>(), FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1);
                                        NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                    }
                                }
                                SoundEngine.PlaySound(DSroar, npc.Center);
                            }
                            if (FollowTimer > 100)
                            {
                                FollowTimer = 200;
                                AttackChain = (int)Attacks.Follow;
                            }
                            
                            break;
                        case Attacks.Transition2:
                            FollowTimer++;
                            npc.velocity *= 0.95f;
                            if (FollowTimer == 80)
                            {
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    for (int i = 0; i < 60; i++)
                                    {
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(3, 11), 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ToxicGas>(), FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1);
                                        NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                    }
                                }
                                SoundEngine.PlaySound(DSroar, npc.Center);
                            }
                            if (FollowTimer > 100)
                            {
                                FollowTimer = 200;
                                AttackChain = (int)Attacks.Follow;
                            }
                            break;
                        case Attacks.Follow:
                            FollowTimer++;

                            int idleTime = WorldSavingSystem.MasochistModeReal ? 140 : 300;
                            if (npc.GetLifePercent() < Phase2Percent)
                                idleTime -= 40;
                            if (npc.GetLifePercent() < Phase3Percent)
                                idleTime -= 40;

                            if (npc.HasPlayerTarget && !target.Calamity().ZoneSulphur)
                                idleTime = 0;

                            if (FollowTimer > idleTime && npc.Distance(targetPos) < 800)
                            {
                                FollowTimer = 0;
                                AttackChain = LastFewAttacks[0];
                                if (FargoSoulsUtil.HostCheck)
                                {
                                    if (npc.GetLifePercent() < 0.15f && !HasDoneSpinny)
                                    {
                                        AttackChain = (int)Attacks.CircleSpikes;
                                    }
                                    else
                                    {
                                        while (LastFewAttacks[0] == AttackChain)
                                        {
                                            AttackChain = Main.rand.Next(1, 4);
                                            if (npc.GetLifePercent() <= Phase2Percent) AttackChain = Main.rand.Next(5, 8);
                                            if (npc.GetLifePercent() <= Phase3Percent) AttackChain = Main.rand.Next(5, 9);
                                        }
                                        
                                    }
                                    LastFewAttacks[1] = LastFewAttacks[0];
                                    LastFewAttacks[0] = AttackChain;
                                    npc.netUpdate = true;
                                }
                                if (((npc.GetLifePercent() < Phase2Percent && npc.GetLifePercent() > Phase3Percent) || npc.GetLifePercent() < Phase3Percent) && SuckCooldown <= 0)
                                {
                                    AttackChain = (int)Attacks.BigSuck;
                                    SuckCooldown = 60 * 55;
                                }
                                
                            }
                            //targetPos = npc.Center - npc.velocity.RotatedBy(MathHelper.ToRadians(10));
                            float speed = 10;
                            float dist = target.Distance(npc.Center);
                            float minDist = 200;
                            if (dist < minDist)
                            {
                                speed = 3f + 7f * (dist / minDist);
                            }
                            Follow(speed, FollowTurnSpeed);
                            break;
                    }

                    void RandomRocks(float amount, float angle, float spread)
                    {
                        RocksTimer++;
                        int telegraphTime = 40;
                        if (RocksTimer < telegraphTime)
                        {
                            if (RocksTimer < 5 && npc.Distance(target.Center) > 350)
                            {
                                RocksTimer = 0;
                                targetPos = target.Center;
                                Follow(16, 7, 1.05f);
                            }
                            else
                            {
                                targetPos = npc.Center + angle.ToRotationVector2();
                                Follow(5, 5, 1.05f);

                                if (RocksTimer > 10)
                                {
                                    float open = MathHelper.SmoothStep(0, 1, (RocksTimer - 10f) / telegraphTime);
                                    if (OpenMouth < open)
                                        OpenMouth = open;
                                }
                            }
                        }
                        else if (RocksTimer == telegraphTime)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                            SoundEngine.PlaySound(Mroar, npc.Center);

                            if (FargoSoulsUtil.HostCheck)
                            {
                                for (int i = 0; i < amount; i++)
                                {
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.rotation.ToRotationVector2().RotatedBy(-MathHelper.Pi / 2).RotatedByRandom(MathHelper.ToRadians(spread)) * Main.rand.NextFloat(10, 15), ModContent.ProjectileType<CalamityMod.Projectiles.Enemy.CrabBoulder>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 1);
                                    NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                }
                            }
                        }
                        if (RocksTimer > telegraphTime)
                        {
                            OpenMouth = MathHelper.SmoothStep(1, 0, MathF.Pow((RocksTimer - 40f) / 20f, 2f));
                        }
                        if (RocksTimer == telegraphTime + 20)
                        {
                            RocksTimer = 0;
                            AttackPart++;
                        }
                    }
                    void Dash(float speed, float time, int epicExtraFlag = 0)
                    {
                        DashAttackTimer++;

                        float accel = 1.06f;
                        float turnBonus = 1f;
                        if (npc.GetLifePercent() < Phase2Percent)
                        {
                            accel += 0.011f;
                            time *= 0.93f;
                        }
                            
                        if (npc.GetLifePercent() < Phase3Percent)
                        {
                            accel += 0.011f;
                            time *= 0.93f;
                        }

                        if (npc.HasPlayerTarget && !target.Calamity().ZoneSulphur)
                        {
                            time *= 0.8f;
                            accel += 0.06f;
                            turnBonus = 1.5f;
                        }
                            

                        if (DashAttackTimer == (int)(time * 0.8f) && epicExtraFlag == 1)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath13, npc.Center);
                            if (FargoSoulsUtil.HostCheck)
                            {
                                for (int i = 0; i < 20; i++)
                                {
                                    
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(Main.rand.NextFloat(2, 7), 0).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ToxicGas>(), FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1);
                                    NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                }
                            }
                        }

                        if (DashAttackTimer < time * 0.3f)
                        {
                            float open = MathHelper.SmoothStep(0, 1, DashAttackTimer / (time * 0.3f));
                            if (OpenMouth < open)
                                OpenMouth = open;
                            Follow(3, 9 * turnBonus, 1.05f);
                        }
                        else if (DashAttackTimer == (int)(time * 0.3f)+1)
                        {
                            
                            SoundEngine.PlaySound(Mroar, npc.Center);
                        }
                        else 
                        {
                            for (int i = -1; i < 2; i += 2)
                            {
                                Color color = new Color(100, (int)MathHelper.Lerp(120, 255, Main.rand.NextFloat(0.4f)) + Main.rand.Next(-50, 50), 50);
                                Vector2 vel = -npc.velocity.RotatedBy(MathHelper.PiOver4 * i);
                                Particle p = new TimedSmokeParticle(npc.Center + vel.SafeNormalize(Vector2.UnitY) * npc.width / 3, vel.RotatedByRandom(MathHelper.PiOver4 * 0.4f) * Main.rand.NextFloat(0.5f, 0.7f), Color.Gray, color, Main.rand.NextFloat(0.7f, 1.3f), 0.7f, 26);
                                GeneralParticleHandler.SpawnParticle(p);
                            }
                            Follow(speed, 0.8f * turnBonus, accel);
                        }

                        float mouthClose = 0.88f;
                        if (DashAttackTimer > time * 0.8f)
                        {
                            float close = (DashAttackTimer - time * mouthClose) / (time * (1 - mouthClose));
                            OpenMouth = MathHelper.SmoothStep(1, 0, close);
                        }

                        if (DashAttackTimer > time && npc.Distance(targetPos) > 500)
                        {
                            DashAttackTimer = 0;
                            //DashAttackState = 0;
                            AttackPart++;
                        }
                    }
                    void Spikes(float speed, float time, bool homing, int howFar = -1)
                    {
                        targetPos = npc.Center + npc.velocity * 10;
                        Follow(speed, 1);
                        int final = howFar == -1 ? maxLength : howFar;

                        SpikeTimer++;
                        if (SpikeTimer == (int)(time * 0.8f))
                        {
                            NPC n = npc;
                            int counter = 0;
                            for (int i = 0; i < final; i++)
                            {
                                counter++;
                                n = Main.npc[(int)n.ai[0]];
                                if (n == null || !n.active || (n.type != ModContent.NPCType<AquaticScourgeBody>() && n.type != ModContent.NPCType<AquaticScourgeBodyAlt>()))
                                {
                                    break;
                                }
                                else if (n.type == ModContent.NPCType<AquaticScourgeBody>() && counter % 4 == 0)
                                {
                                    if (FargoSoulsUtil.HostCheck)
                                    {
                                        int type = ModContent.ProjectileType<Tooth>();
                                        int proj = Projectile.NewProjectile(n.GetSource_FromAI(), n.Center, n.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(90 + 80)) * 20, type, FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1, ai0: 25, ai1: homing ? 1 : 0, ai2: target.whoAmI);
                                        int proj2 = Projectile.NewProjectile(n.GetSource_FromAI(), n.Center, n.rotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(90 - 80)) * 20, type, FargoSoulsUtil.ScaledProjectileDamage(ProjectileDamage), 1, ai0: 25, ai1: homing ? 1 : 0, ai2: target.whoAmI);
                                        NetMessage.SendData(MessageID.SyncProjectile, number: proj);
                                        NetMessage.SendData(MessageID.SyncProjectile, number: proj2);
                                    }
                                    SoundEngine.PlaySound(SoundID.Item17, npc.Center);
                                }
                                
                            }
                        }
                        if (SpikeTimer >= time)
                        {
                            SpikeTimer = 0;
                            AttackPart++;
                        }
                    }
                    void SpinAround(float maxSpeed, float turnSpeed, int time, float acceleration = 1.05f)
                    {
                        SpinTimer++;
                        if (npc.velocity.Length() < maxSpeed)
                        {
                            npc.velocity *= acceleration;
                        }
                        else
                        {
                            npc.velocity /= acceleration;
                        }
                        npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(turnSpeed));
                        if (SpinTimer >= time)
                        {
                            SpinTimer = 0;
                            AttackPart++;
                        }
                    }
                    
                    //Follow the targetPos variable
                    void Follow(float maxSpeed, float turnSpeed, float acceleration = 1.05f) {
                        

                        //speed up if too slow and vice versa
                        if (npc.velocity == Vector2.Zero)
                        {
                            npc.velocity = new Vector2(1, 1);
                        }
                        if (npc.velocity.Length() < maxSpeed)
                        {
                            npc.velocity *= acceleration;
                        }
                        else
                        {
                            npc.velocity /= acceleration;
                        }
                        angleDiff = MathHelper.ToDegrees(FargoSoulsUtil.RotationDifference(npc.velocity, npc.AngleTo(targetPos).ToRotationVector2()));
                        //turning to face the player
                        if (Math.Abs(angleDiff) > 0.5f )
                        {

                            
                            npc.velocity = npc.velocity.RotatedBy(MathHelper.ToRadians(angleDiff > 0 ? turnSpeed : -turnSpeed));

                        }
                    }

                }
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                return false;
            }
            //stops body segments from shooting spikes randomly from base calamity code
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>() || npc.type == ModContent.NPCType<AquaticScourgeTail>())
            {

                npc.localAI[0] = 0;
               
            }
            return base.SafePreAI(npc);
        }

        public override void SafePostAI(NPC npc)
        {
            if (npc.type == ModContent.NPCType<AquaticScourgeBody>() || npc.type == ModContent.NPCType<AquaticScourgeBodyAlt>())
            {
                if (CurrentHittable(npc))
                {
                    npc.position = npc.Center;
                    npc.width = 110;
                    npc.height = 110;
                    npc.Center = npc.position;
                }
            }
        }

    }
}
