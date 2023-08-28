using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;
using FargowiltasSouls.Content.Projectiles;
using Terraria.Localization;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDevastation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ChampionofDevastation : ModNPC
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/DaedalusGolem";
        public override void SetStaticDefaults()
        {

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            //add more debuffs if it makes sense idk what else is needed (separate with comma)
            NPCDebuffImmunityData debuffdata = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffdata);
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 30;
            NPC.height = 50;
            Main.npcFrameCount[Type] = 16;
            NPC.frame.Height = 64;
            NPC.frame.Width = 66;
            NPC.lifeMax = 300000;
            NPC.HitSound = SoundID.NPCHit41;
            NPC.DeathSound = SoundID.NPCDeath43;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10;
            NPC.value = Item.buyPrice(platinum: 20);
            NPC.SpawnWithHigherTime(30);
            NPC.damage = 100;
            NPC.scale = 2.5f;
            NPC.noGravity = true;
            if (!Main.dedServ)
            {
                Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                    ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
                SceneEffectPriority = SceneEffectPriority.BossLow;
            }
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);

        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ChampionofDevastation.BestiaryEntry"))
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

        }
        public override void OnSpawn(IEntitySource source)
        {
            NPC.ai[1] = -300;
            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -600), Vector2.Zero, ModContent.ProjectileType<DarklightBarrier>(), 50, 0, Main.myPlayer, NPC.whoAmI);
        }
        public override void OnKill()
        {

        }
        public bool phase2 = false;
        public int nextAttack;
        public int[] angles = new int[20];
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(phase2);
            writer.Write7BitEncodedInt(nextAttack);
            for (int i = 0; i < angles.Length; i++)
            {
                writer.Write7BitEncodedInt(angles[i]);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            phase2 = reader.ReadBoolean();
            nextAttack = reader.Read7BitEncodedInt();
            for (int i = 0; i < angles.Length; i++)
            {
                angles[i] = reader.Read7BitEncodedInt();
            }
        }
        public override void FindFrame(int frameHeight)
        {
            if (NPC.velocity.X == 0)
            {
                NPC.frame.Y = 0;
            }
            if (NPC.velocity.X != 0)
            {
                NPC.frameCounter += 1 * Math.Abs(NPC.velocity.X);
                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y += NPC.frame.Height;
                    NPC.frameCounter = 0;

                }
                if (NPC.frame.Y > 10 * NPC.frame.Height)
                {
                    NPC.frame.Y = 6 * NPC.frame.Height;
                }
                if (NPC.frame.Y < 6 * NPC.frame.Height)
                {
                    NPC.frame.Y = 6 * NPC.frame.Height;
                }
            }
            if (NPC.velocity.Y != 0)
            {
                NPC.frame.Y = 5 * NPC.frame.Height;
            }
            if (NPC.ai[1] > 70 && NPC.ai[0] == 1 && NPC.ai[1] < 100)
            {
                NPC.frame.Y = NPC.frame.Height * 2;
                if (NPC.ai[1] % 3 == 0)
                {
                    NPC.frame.Y = NPC.frame.Height * 3;
                }
            }
            if (NPC.ai[0] == 2 && NPC.ai[1] < 300)
            {
                NPC.frame.Y = NPC.frame.Height * 11;
            }
            if (NPC.ai[0] == 3 && NPC.ai[1] > 0 && NPC.oldVelocity.Y <= 0 && NPC.velocity.Y >= 0 && NPC.ai[1] < 60)
            {
                NPC.frame.Y = NPC.frame.Height * 11;
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            SpriteEffects dir = SpriteEffects.FlipHorizontally;
            if (NPC.spriteDirection == -1)
            {
                dir = SpriteEffects.None;
            }
            Rectangle source = new Rectangle(0, NPC.frame.Y, NPC.frame.Width, NPC.frame.Height);
            spriteBatch.Draw(texture, NPC.Center + new Vector2(20 * NPC.spriteDirection, -10) - screenPos, source, drawColor, NPC.rotation, source.Size() / 2, NPC.scale, dir, 0);
            return false;
        }

        public override void AI()
        {
            Targetting();
            Player target = Main.player[NPC.target];
            if (NPC.Distance(target.Center) > 1000 && !(NPC.ai[0] == 1 && NPC.ai[1] < 70) && !(NPC.ai[0] == 2) && !(NPC.ai[0] == 3 && NPC.ai[1] < 60) && !(NPC.ai[1] < 0))
            {
                NPC.ai[3] = 1;

            }
            if (!WorldGen.SolidTile(NPC.Bottom.ToTileCoordinates()) && !TileID.Sets.Platforms[Main.tile[NPC.Bottom.ToTileCoordinates()].TileType] && NPC.velocity.Y < 10)
            {
                NPC.velocity.Y += 0.3f;
            }
            if (NPC.ai[3] == 1)
            {

                SlamTooFar();
            }
            if (NPC.ai[0] == 0)
            {

                walking();
            }
            else if (NPC.ai[0] == 1)
            {
                HollowKnight();
            }
            else if (NPC.ai[0] == 2)
            {
                Plantera();
            }
            else if (NPC.ai[0] == 3)
            {
                PBG();
            }
            if (NPC.GetLifePercent() <= 0.6f && !phase2)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, Main.myPlayer, NPC.whoAmI, -8);
                }
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                phase2 = true;
            }
        }

        public void PBG()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 0)
            {
                NPC.velocity = new Vector2(0, -15);
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(NPC.BottomLeft, NPC.width, 0, DustID.Smoke);
                    Dust.NewDust(NPC.BottomLeft, NPC.width, 0, DustID.FrostStaff);
                }
                SoundEngine.PlaySound(SoundID.Item14, NPC.Bottom);
                NPC.ai[1]++;
            }
            if (NPC.oldVelocity.Y <= 0 && NPC.velocity.Y >= 0 && NPC.ai[1] == 1)
            {
                NPC.ai[1]++;
            }
            if (NPC.ai[1] > 1 && NPC.ai[1] < 60)
            {

                if (NPC.ai[1] == 30)
                {
                    for (int i = 0; i < 13; i++)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(63 * NPC.spriteDirection, -12), new Vector2(10 * NPC.spriteDirection, 0).RotatedBy(MathHelper.ToRadians(i * 9f * NPC.spriteDirection)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.HiveBombGoliath>(), 50, 0, Main.myPlayer);
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PlagueSounds/PBGBarrageLaunch"), NPC.Center);

                    }
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PlaguebringerSmalliath>(), 50, 0, Main.myPlayer);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PlagueSounds/PBGAttackSwitch1"));
                }
                if (NPC.ai[1] == 45)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PlaguebringerSmalliath>(), 50, 0, Main.myPlayer);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PlagueSounds/PBGAttackSwitch1"));
                }
                if (NPC.ai[1] == 59)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PlaguebringerSmalliath>(), 50, 0, Main.myPlayer);
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/PlagueSounds/PBGAttackSwitch1"));
                }
                NPC.ai[1]++;
                NPC.velocity.Y = 0;
            }
            if (NPC.collideY && NPC.Bottom.Y > target.Center.Y && NPC.ai[1] == 60)
            {
                SoundEngine.PlaySound(SoundID.Item14, NPC.Bottom);
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.BottomLeft.X + Main.rand.Next(0, NPC.width), NPC.Bottom.Y), Vector2.Zero, GoreID.Smoke1);
                }
                if (phase2 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile fire = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 100), Vector2.Zero, ModContent.ProjectileType<FireChain>(), 0, 0, Main.myPlayer, 0, 150);
                    fire.spriteDirection = NPC.spriteDirection;

                }
                NPC.ai[1]++;
            }
            if (NPC.ai[1] > 60)
            {
                NPC.ai[1]++;
                if (NPC.ai[1] >= 250)
                {
                    NPC.ai[1] = 0;
                    NPC.ai[0] = 0;
                }
            }
        }
        public void Plantera()
        {
            Player target = Main.player[NPC.target];
            NPC.ai[1]++;
            if (NPC.ai[1] == 1)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0, 0);
                    dust.noGravity = true;
                    dust.scale = 3;
                }
                SoundEngine.PlaySound(SoundID.NPCDeath7, NPC.Center);
                NPC.Center = target.Center + new Vector2(target.velocity.X * 20, -475);
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0, 0);
                    dust.noGravity = true;
                    dust.scale = 3;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 20).RotatedBy(MathHelper.ToRadians(30)), ModContent.ProjectileType<PlantHook>(), 50, 0, Main.myPlayer, NPC.whoAmI);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 20).RotatedBy(MathHelper.ToRadians(-30)), ModContent.ProjectileType<PlantHook>(), 50, 0, Main.myPlayer, NPC.whoAmI);
                }

            }
            if (NPC.ai[1] < 300)
            {
                NPC.velocity = Vector2.Zero;
                if (NPC.ai[1] % 40 == 0 && NPC.ai[1] < 250 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int attack = Main.rand.Next(1, 4);
                    if (attack == 1)
                    {
                        LeftLeaves();
                    }
                    else if (attack == 2)
                    {
                        RightLeaves();
                    }
                    else if (attack == 3)
                    {
                        EvenLeaves();
                    }
                    NPC.netUpdate = true;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.ai[1] % 150 == 0)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(10, 0), ModContent.ProjectileType<ReaverChakram>(), 50, 0, Main.myPlayer, 200 / NPC.Distance(target.Center), NPC.Distance(target.Center) / 3.14f);
                    }
                    Vector2 sporePos = new Vector2(0, 1).RotatedBy(MathHelper.ToRadians(30 + Main.rand.Next(0, 300)));
                    Projectile sporeGas = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + sporePos * Main.rand.Next(0, 1000), Vector2.Zero, ProjectileID.SporeGas, 50, 0, Main.myPlayer);
                    sporeGas.friendly = false;
                    sporeGas.hostile = true;
                }
                bool outside = false;
                if (target.AngleFrom(NPC.Center) > MathHelper.ToRadians(120) && target.Center.Y > NPC.Center.Y)
                {
                    target.velocity.X = 10;
                    Dust.NewDust(target.position, target.width, target.height, DustID.TerraBlade);
                    outside = true;
                }
                else if (target.AngleFrom(NPC.Center) < MathHelper.ToRadians(60) && target.Center.Y > NPC.Center.Y)
                {
                    target.velocity.X = -10;
                    Dust.NewDust(target.position, target.width, target.height, DustID.TerraBlade);
                    outside = true;
                }
                else if (target.Center.Y < NPC.Center.Y)
                {
                    target.velocity.Y = 10;
                    Dust.NewDust(target.position, target.width, target.height, DustID.TerraBlade);
                    outside = true;
                }
                if (outside && NPC.ai[1] % 40 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile spore = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center, Vector2.Zero, ProjectileID.SporeGas, 50, 0, Main.myPlayer);
                    spore.friendly = false;
                    spore.hostile = true;
                }
            }
            if (NPC.ai[1] >= 300 && (!NPC.collideY || NPC.Bottom.Y < target.Center.Y))
            {
                NPC.velocity = new Vector2(0, 20);
            }
            if (NPC.ai[1] > 300 && NPC.collideY && NPC.Bottom.Y > target.Center.Y)
            {
                SoundEngine.PlaySound(SoundID.Item14, NPC.Bottom);
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.BottomLeft.X + Main.rand.Next(0, NPC.width), NPC.Bottom.Y), Vector2.Zero, GoreID.Smoke1);
                }
                if (phase2 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile fire = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 100), Vector2.Zero, ModContent.ProjectileType<FireChain>(), 0, 0, Main.myPlayer, 0, 100);
                    fire.spriteDirection = -1;
                    Projectile fire2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 100), Vector2.Zero, ModContent.ProjectileType<FireChain>(), 0, 0, Main.myPlayer, 0, 100);
                    fire2.spriteDirection = 1;

                }
                NPC.ai[1] = 0;
                NPC.ai[0] = 0;
            }
        }
        public void LeftLeaves()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 12; i++)
                {
                    Projectile leaf = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ReaverSpike>(), 50, 0, Main.myPlayer, MathHelper.ToRadians(120 - i * 3f));
                    leaf.friendly = false;
                    leaf.hostile = true;

                }
            }
            SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
        }
        public void RightLeaves()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 12; i++)
                {
                    Projectile leaf = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ReaverSpike>(), 50, 0, Main.myPlayer, MathHelper.ToRadians(60 + i * 3f));
                    leaf.friendly = false;
                    leaf.hostile = true;
                }
            }
            SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
        }
        public void EvenLeaves()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i <= 4; i++)
                {
                    Projectile leaf = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<ReaverSpike>(), 50, 0, Main.myPlayer, MathHelper.ToRadians(120 - i * 15f));
                    leaf.friendly = false;
                    leaf.hostile = true;

                }
            }
            SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

        }


        public void HollowKnight()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 0)
            {
                if (target.Center.X > NPC.Center.X)
                {
                    NPC.velocity = new Vector2(15, -10);
                }
                else
                {
                    NPC.velocity = new Vector2(-15, -10);
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(NPC.BottomLeft, NPC.width, 0, DustID.Smoke);
                    Dust.NewDust(NPC.BottomLeft, NPC.width, 0, DustID.FrostStaff);
                }
                SoundEngine.PlaySound(SoundID.Item14, NPC.Bottom);
            }
            if (NPC.collideY && NPC.ai[1] > 50)
            {
                if (phase2 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile fire = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 100), Vector2.Zero, ModContent.ProjectileType<FireChain>(), 0, 0, Main.myPlayer, 0, 150);
                    fire.spriteDirection = NPC.spriteDirection;
                }

                NPC.velocity = Vector2.Zero;
            }

            if (NPC.ai[1] == 70 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = -10; i < 10; i++)
                {

                    Vector2 pos = new Vector2(NPC.Center.X + i * 125, NPC.Center.Y - 1000);
                    float angle = MathHelper.ToRadians(Main.rand.Next(-5, 6) + 90);
                    Projectile graah = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), pos, Vector2.Zero, ModContent.ProjectileType<GlowLine>(), 0, 0, Main.myPlayer, 20, angle);
                    graah.scale = 0.5f;
                    angles[i + 10] = (int)MathHelper.ToDegrees(angle);
                }
                NPC.netUpdate = true;
            }
            if (NPC.ai[1] == 130)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = -10; i < 10; i++)
                    {
                        Vector2 pos = new Vector2(NPC.Center.X + i * 125, NPC.Center.Y - 1000);
                        float angle = MathHelper.ToRadians(angles[i + 10]);

                        Projectile lightning = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), pos, Vector2.One.RotatedBy(MathHelper.ToRadians(45)) * 2, ModContent.ProjectileType<CalamityMod.Projectiles.Summon.DaedalusLightning>(), 50, 0, 255, angle, Main.rand.Next(-10, 10));
                        lightning.ai[0] = angle;
                        lightning.ai[1] = Main.rand.Next(-10, 10);
                        lightning.friendly = false;
                        lightning.hostile = true;
                        lightning.minion = false;

                    }
                }
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Thunder_0") with { Volume = 0.75f }, NPC.Center);
            }

            if (NPC.ai[1] > 70 && NPC.ai[1] % 3 == 0 && NPC.ai[1] < 100 && Main.netMode != NetmodeID.MultiplayerClient)
            {

                Vector2 pos = NPC.Center + new Vector2(63 * NPC.spriteDirection, -12);
                Projectile orb = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), pos, (target.Center - pos).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-5, 6))) * 15, ModContent.ProjectileType<CalamityMod.Projectiles.Summon.DaedalusPellet>(), 50, 0);
                orb.hostile = true;
                orb.friendly = false;
                orb.minion = false;
            }
            NPC.ai[1]++;
            if (NPC.ai[1] >= 200)
            {
                NPC.ai[1] = 0;
                NPC.ai[0] = 0;
            }
        }
        public void SlamTooFar()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[2] == 0)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath7, target.Center);

            }
            if (NPC.ai[2] <= 10)
            {
                NPC.Center = target.Center + new Vector2(target.velocity.X * 18, -600);
            }
            else
            {
                NPC.velocity.Y = 30;
                NPC.velocity.X = 0;
            }
            NPC.ai[2]++;
            if (NPC.collideY && NPC.Center.Y > target.Center.Y - 100)
            {
                SoundEngine.PlaySound(SoundID.Item14, NPC.Bottom);
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGore(NPC.GetSource_FromAI(), new Vector2(NPC.BottomLeft.X + Main.rand.Next(0, NPC.width), NPC.Bottom.Y), Vector2.Zero, GoreID.Smoke1);
                }
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                return;
            }

        }
        public void walking()
        {
            int TimeLeft = 200;
            if (Main.expertMode)
            {
                TimeLeft = 120;
            }
            if (FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode)
            {
                TimeLeft = 60;
            }
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                nextAttack = Main.rand.Next(1, 4);
                NPC.netUpdate = true;
            }

            if (target.Center.X > NPC.Center.X)
            {
                if (NPC.velocity.X < 5)
                {
                    NPC.velocity.X += 0.25f;
                }
                else if (NPC.velocity.X > 5)
                {
                    NPC.velocity.X -= 0.25f;
                }

            }
            else if (target.Center.X < NPC.Center.X)
            {
                if (NPC.velocity.X < -5)
                {
                    NPC.velocity.X += 0.25f;
                }
                else if (NPC.velocity.X > -5)
                {
                    NPC.velocity.X -= 0.25f;
                }
            }

            if (NPC.collideX)
            {
                NPC.velocity.Y = -10;
            }

            if (NPC.velocity.Y < 0 && NPC.oldVelocity.Y >= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(NPC.BottomLeft, NPC.width, 0, DustID.Smoke);
                    Dust.NewDust(NPC.BottomLeft, NPC.width, 0, DustID.FrostStaff);
                }
                SoundEngine.PlaySound(SoundID.Item14, NPC.Bottom);
            }
            if (NPC.ai[1] == TimeLeft - 60)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (nextAttack == 1)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, Main.myPlayer, NPC.whoAmI, -10);
                    }
                    if (nextAttack == 2)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, Main.myPlayer, NPC.whoAmI, -9);
                    }
                    if (nextAttack == 3)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<GlowRing>(), 0, 0, Main.myPlayer, NPC.whoAmI, -11);
                    }
                }
                SoundEngine.PlaySound(SoundID.ForceRoarPitched, NPC.Center);
            }
            NPC.ai[1]++;
            if (NPC.ai[1] >= TimeLeft)
            {
                NPC.ai[0] = nextAttack;
                NPC.ai[1] = 0;
            }

        }
        public void Targetting()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player target = Main.player[NPC.target];
            if (target.dead)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Vortex, 0, 0);
                    dust.noGravity = true;
                    dust.scale = 3;
                }
                SoundEngine.PlaySound(SoundID.NPCDeath7, NPC.Center);
                NPC.active = false;
                return;
            }
            if (target.Center.X > NPC.Center.X)
            {
                NPC.spriteDirection = 1;
            }
            else
            {
                NPC.spriteDirection = -1;
            }
        }
    }
}
