using System;
using System.IO;
using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Common.Graphics.Particles;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeGodsEternity : EModeCalBehaviour
    {
        public static readonly SoundStyle ExitSound = new SoundStyle("CalamityMod/Sounds/Custom/SlimeGodExit", (SoundType)0);

        public bool Empowered = false;
        public int Timer = 0;
        public bool DidSpecial = false;
        public Vector2 CrimsonSlamPos = Vector2.Zero;
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<EbonianPaladin>(),
            ModContent.NPCType<CrimulanPaladin>()
        );

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(Empowered);
            binaryWriter.Write7BitEncodedInt(Timer);
            binaryWriter.Write(DidSpecial);
            binaryWriter.WriteVector2(CrimsonSlamPos);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            Empowered = binaryReader.ReadBoolean();
            Timer = binaryReader.Read7BitEncodedInt();
            DidSpecial = binaryReader.ReadBoolean();
            CrimsonSlamPos = binaryReader.ReadVector2();
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

            if (!Targeting())
                return false;

            NPC core = Main.npc[CalamityGlobalNPC.slimeGod];
            if (core != null && core.active && core.type == ModContent.NPCType<SlimeGodCore>())
            {
                if (Empowered)
                {
                    return EmpoweredAI(npc);
                }
            }
            return true;

            bool Targeting()
            {
                const float despawnRange = 5000f;
                Player p = Main.player[npc.target];
                if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                {
                    npc.TargetClosest();
                    p = Main.player[npc.target];
                    if (!p.active || p.dead || Vector2.Distance(npc.Center, p.Center) > despawnRange)
                    {
                        npc.noTileCollide = true;
                        if (npc.timeLeft > 30)
                            npc.timeLeft = 30;
                        npc.velocity.Y -= 1f;
                        if (npc.timeLeft == 1)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                FargoSoulsUtil.ClearHostileProjectiles(2, npc.whoAmI);
                            }
                        }
                        return false;
                    }
                }
                return true;
            }
        }

        public bool EmpoweredAI(NPC npc)
        {
            ref float calState = ref npc.ai[0];
            ref float calOtherTimer = ref npc.ai[1];
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
                            if (npc.type == ModContent.NPCType<CrimulanPaladin>())
                            {
                                if (DidSpecial) //reversed here so it doesn't start with it
                                {
                                    calState = 22;
                                    Timer = 0;
                                    DidSpecial = false;
                                    npc.netUpdate = true;

                                    return false;
                                }
                                else
                                {
                                    DidSpecial = true;
                                    npc.netUpdate = true;
                                }
                                NetSync(npc);
                            }
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
                            NetSync(npc);
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
                            NetSync(npc);
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
                            NetSync(npc);
                        }
                        else
                        {
                            calState = 2;
                            DidSpecial = false;
                            npc.netUpdate = true;
                            NetSync(npc);
                        }
                    }
                    return false;
                case 5: //for some reason, ebonian teleport anim is 6 while crimulean is 5
                    {
                        int type;
                        if (npc.type == ModContent.NPCType<EbonianPaladin>())
                        {
                            break;
                        }
                        else
                        {

                            type = ModContent.ProjectileType<SlimeHomingCrimuleanGlob>();
                        }
                        if (calOtherTimer % 3 == 0)
                        {
                            Vector2 dir = (-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2);
                            float speed = Main.rand.NextFloat(4, 6);
                            if (DLCUtils.HostCheck)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * npc.height / 2, dir * speed, type, npc.GetProjectileDamage(type), 2f, Main.myPlayer);
                            }
                        }
                    }
                    break;
                case 6:
                    {
                        int type;
                        if (npc.type == ModContent.NPCType<EbonianPaladin>())
                        {
                            type = ModContent.ProjectileType<SlimeHomingEbonianGlob>();
                        }
                        else
                        {
                            break;

                        }
                        if (calOtherTimer % 3 == 0)
                        {
                            Vector2 dir = (-Vector2.UnitY).RotatedByRandom(MathHelper.PiOver2);
                            float speed = Main.rand.NextFloat(4, 6);
                            if (DLCUtils.HostCheck)
                            {
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * npc.height / 2, dir * speed, type, npc.GetProjectileDamage(type), 2f, Main.myPlayer);
                            }
                        }
                    }
                    break;
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
            bool below = npc.position.Y + (float)npc.height >= player.position.Y;
            bool collision = Collision.SolidCollision(npc.position + npc.velocity, npc.width, npc.height);
            if (npc.ai[2] == 0f && below && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1) && !collision)
            {
                npc.ai[2] = 1f;
                npc.netUpdate = true;
                NetSync(npc);
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
            if (below || npc.velocity.Y <= 0f || collision)
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
                    NetSync(npc);
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
            bool below = npc.position.Y + (float)npc.height >= player.position.Y;
            bool collision = Collision.SolidCollision(npc.position + npc.velocity, npc.width, npc.height);
            if (npc.ai[2] == 0f && below && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1) && !collision)
            {
                npc.ai[2] = 1f;
                npc.netUpdate = true;
                NetSync(npc);
            }
            if ((below || npc.velocity.Y <= 0f) || collision)
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
                    NetSync(npc);
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
            Player player = Main.player[npc.target];
            if (Timer == 0)
            {
                SoundEngine.PlaySound(ExitSound with { Pitch = -0.3f }, npc.Center);
                FargowiltasSouls.Common.Graphics.Particles.Particle p = new ExpandingBloomParticle(npc.Center, Vector2.Zero, Color.Magenta, Vector2.One, Vector2.One * 60, 40, true, Color.Transparent);
                p.Spawn();
                bounces = 0;
            }
            if (Timer < 30)
            {
                npc.velocity.X *= 0.9f;
            }
            if (Timer == 60 && bounces < 3)
            {
                npc.velocity.Y = -30;
            }
            if (Timer > 60 && bounces < 3)
            {
                if (player != null && player.active && !player.dead && !player.ghost)
                {
                    float distX = player.Center.X - npc.Center.X;
                    float dirX = Math.Sign(distX);
                    const float xTracking = 0.4f;
                    npc.velocity.X += dirX * xTracking;
                }
                bool below = npc.Bottom.Y + npc.height > player.Center.Y;
                if (!below)
                {
                    npc.noTileCollide = true;
                }
                bool platforms = below;
                bool inTiles = false;
                const int extraPixelsBelow = 32;
                for (int x = 0; x < npc.width; x += 16)
                {
                    for (float y = 0; y < npc.height + extraPixelsBelow; y += 16)
                    {
                        Tile tile = Framing.GetTileSafely((int)(npc.position.X + x) / 16, (int)(npc.position.Y + y) / 16);
                        if ((tile.HasUnactuatedTile && platforms) && (Main.tileSolid[tile.TileType] || (Main.tileSolidTop[tile.TileType] && platforms)))
                        {
                            inTiles = true;
                            break;
                        }
                    }
                }
                if (inTiles && npc.velocity.Y >= 0)
                {
                    SoundEngine.PlaySound(in SlimeGodCore.BigShotSound, npc.Center);
                    if (DLCUtils.HostCheck)
                    {
                        float speed = 2f;
                        int type = ModContent.ProjectileType<UnstableEbonianGlob>();
                        int projectileDamage = npc.GetProjectileDamage(type);
                        const int GlobCount = 12;
                        for (int i = 0; i < GlobCount; i++)
                        {
                            Vector2 dir = Vector2.UnitY.RotatedBy(MathHelper.TwoPi * (float)i / GlobCount);
                            float projSpeed = speed + Main.rand.NextFloat(2.5f);
                            Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * npc.height / 2, dir * projSpeed, type, projectileDamage, 3f, Main.myPlayer);
                        }

                    }
                    npc.velocity.X /= 2f;

                    npc.velocity.Y = 0;
                    bounces++;
                    if (bounces >= 3)
                    {
                        npc.velocity.X = 0;
                    }
                    Timer = 55;
                    NetSync(npc);
                    npc.netUpdate = true;
                }

                npc.noGravity = true;
                npc.velocity.Y += 1f;
            }
            if (bounces >= 3 && Timer >= 90)
            {
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                Timer = 0;
                npc.netUpdate = true;
                NetSync(npc);
                return false;
            }
            Timer++;
            return false;
        }
        public bool CrimsonSpecial(NPC npc)
        {
            ref float slams = ref npc.ai[2];
            ref float slamAngle = ref npc.ai[1];

            Player player = Main.player[npc.target];
            const int TotalSlams = 3;

            npc.noGravity = slams < TotalSlams;

            if (Timer == 0)
            {
                SoundEngine.PlaySound(ExitSound with { Pitch = -0.3f }, npc.Center);
                FargowiltasSouls.Common.Graphics.Particles.Particle p = new ExpandingBloomParticle(npc.Center, Vector2.Zero, Color.Crimson, Vector2.One, Vector2.One * 60, 40, true, Color.Transparent);
                p.Spawn();
                slams = 0;
            }
            if (Timer < 30)
            {
                npc.velocity.X *= 0.9f;
            }
            //const int JumpSpeed = 30;
            const int StartTime = 60;
            if (Timer == StartTime && slams < TotalSlams)
            {
                const int JumpHeight = 900;
                CrimsonSlamPos = npc.Center - Vector2.UnitY * JumpHeight;
                Vector2 targetPos = player.Center + player.velocity * 45;
                slamAngle = CrimsonSlamPos.DirectionTo(targetPos).ToRotation();
                if (DLCUtils.HostCheck)
                {
                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center, slamAngle.ToRotationVector2(), ModContent.ProjectileType<SlamTelegraph>(), 0, 0, Main.myPlayer, ai1: npc.height * 1.2f);
                }
                npc.netUpdate = true;
                NetSync(npc);
            }
            if (Timer > StartTime && slams < TotalSlams)
            {
                const int TelegraphTime = StartTime + 45;
                if (Timer < TelegraphTime)
                {
                    npc.velocity.Y = (CrimsonSlamPos.Y - npc.Center.Y) / 25f;
                }
                else if (Timer == TelegraphTime)
                {
                    npc.velocity = slamAngle.ToRotationVector2() * 40;
                    npc.netUpdate = true;
                    NetSync(npc);
                }
                else
                {
                    bool below = npc.Bottom.Y > player.Center.Y;
                    if (!below)
                    {
                        npc.noTileCollide = true;
                    }
                    bool inTiles = Collision.SolidCollision(npc.position, npc.width, npc.height + 10);
                    if (inTiles || Timer > TelegraphTime + 55) //extra safety max time
                    {
                        SoundEngine.PlaySound(in SlimeGodCore.BigShotSound, npc.Center);
                        /*
                        if (DLCUtils.HostCheck)
                        {
                            float speed = 2f;
                            int type = ModContent.ProjectileType<UnstableEbonianGlob>();
                            int projectileDamage = npc.GetProjectileDamage(type);
                            const int GlobCount = 12;
                            for (int i = 0; i < GlobCount; i++)
                            {
                                Vector2 dir = Vector2.UnitY.RotatedBy(MathHelper.TwoPi * (float)i / GlobCount);
                                float projSpeed = speed + Main.rand.NextFloat(2.5f);
                                Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + dir * npc.height / 2, dir * projSpeed, type, projectileDamage, 3f, Main.myPlayer);
                            }

                        }
                        */
                        npc.velocity.X /= 6f;

                        npc.velocity.Y = -npc.velocity.Y / 2;
                        slams++;
                        Timer = 30;
                        npc.netUpdate = true;
                        NetSync(npc);
                    }

                }
            }
            if (slams >= TotalSlams && Timer >= 90)
            {
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                Timer = 0;
                npc.netUpdate = true;
                NetSync(npc);
                return false;
            }
            Timer++;
            return false;
        }
        /*
        public bool CrimsonSpecial(NPC npc)
        {
            ref float attackStep = ref npc.ai[2];
            Player player = Main.player[npc.target];
            if (Timer == 0)
            {
                SoundEngine.PlaySound(ExitSound with { Pitch = -0.3f }, npc.Center);
                FargowiltasSouls.Common.Graphics.Particles.Particle p = new ExpandingBloomParticle(npc.Center, Vector2.Zero, Color.Crimson, Vector2.One, Vector2.One * 60, 40, true, Color.Transparent);
                p.Spawn();
                attackStep = 0;
            }
            switch (attackStep)
            {
                case 0:
                    {
                        const int TelegraphTime = 40;
                        if (Timer == TelegraphTime && player != null && player.active && !player.dead && !player.ghost)
                        {
                            npc.velocity = npc.DirectionTo(player.Center) * 30;
                        }
                        if (Timer > TelegraphTime)
                        {
                            float dif = FargoSoulsUtil.RotationDifference(npc.velocity, npc.DirectionTo(player.Center));
                            npc.velocity = npc.velocity.RotatedBy(dif * Math.Max(dif, MathHelper.Pi / 80));
                            npc.noGravity = true;
                            const int minDistanceRequired = 400;
                            if (npc.Distance(player.Center) < minDistanceRequired || Timer > 120)
                            {
                                attackStep = 1;
                                Timer = 0;
                            }
                        }
                    }
                    break;
                case 1:
                    {
                        npc.noGravity = true;

                        float dif = FargoSoulsUtil.RotationDifference(npc.velocity, npc.DirectionTo(player.Center));
                        npc.velocity = npc.velocity.RotatedBy(dif * Math.Max(dif, MathHelper.Pi / 80));
                        npc.velocity *= 0.95f;
                        if (Timer == 5)
                        {
                            float randomRot = Main.rand.NextFloat(MathHelper.TwoPi);
                            const int Spikes = 12;
                            const float spikeRandomRot = (MathHelper.TwoPi / Spikes) / 10f;
                            if (DLCUtils.HostCheck)
                            {
                                for (int i = 0; i < Spikes; i++)
                                {
                                    Vector2 spikeDir = randomRot.ToRotationVector2().RotatedBy(MathHelper.TwoPi * (float)i / Spikes).RotatedByRandom(spikeRandomRot);
                                    float spikeDistance = npc.scale * MathHelper.Lerp(154f / 2, 100f / 2, (float)Math.Sin(spikeDir.ToRotation()));
                                    int type = ModContent.ProjectileType<CrimsonPaladinSpike>();
                                    Projectile.NewProjectile(npc.GetSource_FromThis(), npc.Center + spikeDir * spikeDistance, Vector2.Zero, type, npc.GetProjectileDamage(type), 2f, Main.myPlayer, spikeDir.ToRotation(), spikeDistance);
                                }
                            }
                                
                        }
                        if (Timer > 100)
                        {
                            npc.velocity = Vector2.Zero;
                            npc.ai[0] = 0f;
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                            npc.ai[3] = 0f;
                            Timer = 0;
                            npc.netUpdate = true;
                            return false;
                        }
                    }
                    break;
            }
            Timer++;
            return false;
        }
        */
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot) => npc.ai[0] != 2; //not while winding up slam
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
