using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader; using FargowiltasCrossmod.Core;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using System;
using ReLogic.Content;
using Terraria.Localization;
using FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofAnnihilation;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofAnnihilation
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [AutoloadBossHead]
    public class ChampionofAnnihilation : ModNPC
    {

        public override void SetStaticDefaults()
        {

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCDebuffImmunityData debuffdata = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused,
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.BrimstoneFlames>(),
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.Dragonfire>(),
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.GodSlayerInferno>(),
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.HolyFlames>(),
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.HolyInferno>(),
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.MiracleBlight>(),
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.Shadowflame>(),
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.BanishingFire>(),
                    ModContent.BuffType<CalamityMod.Buffs.DamageOverTime.WeakBrimstoneFlames>(),
                    BuffID.OnFire,
                    BuffID.OnFire3,
                    BuffID.ShadowFlame,
                    BuffID.Frostburn,
                    BuffID.Frostburn2,
                    BuffID.CursedInferno,
                    BuffID.Burning,
                    BuffID.Daybreak

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
            NPC.width = 28;
            NPC.height = 46;
            NPC.HitSound = new SoundStyle("CalamityMod/Sounds/NPCHit/CryogenHit", 3);
            NPC.DeathSound = new SoundStyle("CalamityMod/Sounds/NPCKilled/CryogenDeath");
            NPC.lifeMax = 5200000;
            NPC.damage = 0;
            Main.npcFrameCount[Type] = 8;
            NPC.frame.Height = 28;
            NPC.frame.Width = 46;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(platinum: 20);
            NPC.SpawnWithHigherTime(30);
            NPC.scale = 2.5f;
            NPC.noGravity = true;

            if (!Main.dedServ)
            {
                Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                    ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
                SceneEffectPriority = SceneEffectPriority.BossLow;
            }
            NPC.noTileCollide = true;

        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<CalamityMod.Items.Potions.OmegaHealingPotion>();
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = 5200000;
            if (Main.expertMode)
                NPC.lifeMax += 500000;
            if (Main.masterMode)
            {
                NPC.lifeMax += 500000;
            }
            NPC.lifeMax += 2500000 * (numPlayers - 1);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ChampionofAnnihilation.BestiaryEntry"))
            });
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public Asset<Texture2D>[] gems = {ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechBlueGem"),
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechGreenGem"),
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechPinkGem"),
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechPurpleGem"),
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechRedGem"),
            ModContent.Request<Texture2D>("CalamityMod/Projectiles/Typeless/GemTechYellowGem") };
        public Vector2[] gemPostions = {new Vector2(2*MathHelper.Pi/6, 2*MathHelper.Pi/6),
        new Vector2(2*MathHelper.Pi/6*2, 2*MathHelper.Pi/6*2),
        new Vector2(2*MathHelper.Pi/6*3, 2*MathHelper.Pi/6*3),
        new Vector2(2*MathHelper.Pi/6*4, 2*MathHelper.Pi/6*4),
        new Vector2(2*MathHelper.Pi/6*5, 2*MathHelper.Pi/6*5),
        new Vector2(2*MathHelper.Pi/6*6, 2*MathHelper.Pi/6*6)};

        public float[] gemRotations = { 0.2f, 0.5f, 0.3f, 0.4f, 0.6f, 0.1f };
        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < gems.Length; i++)
            {
                if (Math.Cos(gemPostions[i].X) >= 0)
                {
                    spriteBatch.Draw(gems[i].Value, NPC.Center + new Vector2((float)Math.Sin(gemPostions[i].X) * 100, (float)Math.Sin(gemPostions[i].Y) * 10).RotatedBy(MathHelper.ToRadians(NPC.velocity.X)) - Main.screenPosition, null, drawColor, gemRotations[i], gems[i].Size() / 2, 2, SpriteEffects.None, 0);
                    gemPostions[i].X += 0.04f;
                    gemPostions[i].Y += 0.02f;
                    gemRotations[i] += MathHelper.ToRadians(1) * i / 2 + MathHelper.ToRadians(2);
                }
            }

        }
        public Vector2 oldPosition = Vector2.Zero;
        public float oldOpacity = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < gems.Length; i++)
            {
                if (Math.Cos(gemPostions[i].X) < 0)
                {
                    spriteBatch.Draw(gems[i].Value, NPC.Center + new Vector2((float)Math.Sin(gemPostions[i].X) * 100, (float)Math.Sin(gemPostions[i].Y) * 10).RotatedBy(MathHelper.ToRadians(NPC.velocity.X)) - Main.screenPosition, null, drawColor, gemRotations[i], gems[i].Size() / 2, 2, SpriteEffects.None, 0);
                    gemPostions[i].X += 0.04f;
                    gemPostions[i].Y += 0.02f;
                    gemRotations[i] += MathHelper.ToRadians(1) * i / 2 + MathHelper.ToRadians(2);
                }
            }
            if (oldOpacity > 0)
            {
                spriteBatch.Draw(TextureAssets.Npc[Type].Value, oldPosition - Main.screenPosition, new Rectangle(0, 0, 28, 46), drawColor * oldOpacity, 0, new Vector2(28, 46) / 2, NPC.scale, SpriteEffects.None, 0);
                oldOpacity -= 0.05f;
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frame.Y += NPC.frame.Height;
                NPC.frameCounter = 0;
                if (NPC.frame.Y > NPC.frame.Height * 7)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Mod cal = ModLoader.GetMod("CalamityMod");
                for (int i = 0; i < 10; i++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(0, Main.rand.Next(0, 7)).RotatedByRandom(MathHelper.TwoPi), cal.Find<ModGore>("CrawlerDiamond").Type);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(0, Main.rand.Next(0, 7)).RotatedByRandom(MathHelper.TwoPi), cal.Find<ModGore>("CrawlerDiamond2").Type);

                }
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(0, Main.rand.Next(0, 7)).RotatedByRandom(MathHelper.TwoPi), cal.Find<ModGore>("CrawlerAmber").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(0, Main.rand.Next(0, 7)).RotatedByRandom(MathHelper.TwoPi), cal.Find<ModGore>("CrawlerCrystal").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(0, Main.rand.Next(0, 7)).RotatedByRandom(MathHelper.TwoPi), cal.Find<ModGore>("CrawlerEmerald").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(0, Main.rand.Next(0, 7)).RotatedByRandom(MathHelper.TwoPi), cal.Find<ModGore>("CrawlerRuby").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(0, Main.rand.Next(0, 7)).RotatedByRandom(MathHelper.TwoPi), cal.Find<ModGore>("CrawlerSapphire").Type);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(oldPosition);

            writer.Write7BitEncodedInt(NPC.target);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            oldPosition = reader.ReadVector2();

            NPC.target = reader.Read7BitEncodedInt();
        }

        int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(400);
        public override void AI()
        {

            NPC.rotation = MathHelper.ToRadians(NPC.velocity.X);
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player target = Main.player[NPC.target];
            if (target.dead)
            {
                NPC.velocity.Y += 0.4f;
                NPC.EncourageDespawn(10);
                return;
            }
            if (1 - NPC.GetLifePercent() > NPC.localAI[0])
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(target.Center.X > NPC.Center.X ? 5 : -5, -10), ModContent.ProjectileType<GemProjectile>(), damage, Main.myPlayer);
                NPC.localAI[0] += 0.05f;
            }
            if (NPC.ai[0] == 0)
            {
                Idle();
            }
            else if (NPC.ai[0] == 1)
            {
                DemonAttacks();
            }
            else if (NPC.ai[0] == 2)
            {
                TpRocket();
            }
            else if (NPC.ai[0] == 3)
            {
                TwoPortals();
            }
            else if (NPC.ai[0] == 4)
            {
                LaserSweep();
            }
            else if (NPC.ai[0] == 5)
            {
                ThrowPortal();
            }
            else if (NPC.ai[0] == 6)
            {
                LaserRockets();
            }
            NPC.netAlways = true;
        }
        public void Teleport(Vector2 newPos)
        {
            oldPosition = NPC.Center;
            oldOpacity = 1;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.Center = newPos;
                NPC.netUpdate = true;
            }

            for (int i = 0; i < 50; i++)
            {
                Vector2 speed = new Vector2(0, Main.rand.NextFloat() * 10).RotateRandom(Math.PI * 2);
                Dust.NewDustDirect(NPC.Center, 0, 0, DustID.BubbleBurst_White, (int)speed.X, (int)speed.Y, Scale: 3).noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
        }
        public void Idle()
        {
            Player target = Main.player[NPC.target];
            NPC.ai[1]++;
            if (NPC.ai[1] == 1) ChooseNext();
            if (NPC.ai[1] % 30 == 0)
            {

                Teleport(target.Center + new Vector2(0, 300).RotateRandom(Math.PI * 2));


                Vector2 vel = new Vector2(5, -10);
                if (NPC.Center.X > target.Center.X) vel = new Vector2(-5, -10);
                //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<GemProjectile>(), damage, 0, Main.myPlayer);
            }

            if (NPC.ai[1] >= 0)
            {
                NPC.ai[1] = 0;

                NPC.ai[0] = NPC.ai[3];
                NPC.ai[3] = 0;
            }
        }
        public void ChooseNext()
        {
            Player target = Main.player[NPC.target];
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (target.Distance(NPC.Center) > 2000 || Math.Abs(target.velocity.X) > 15)
                {

                    NPC.ai[3] = Main.rand.Next(1, 3);
                    if (NPC.GetLifePercent() > 0.66) NPC.ai[3] = 2;
                    else if (NPC.GetLifePercent() > 0.33) NPC.ai[3] = 1;
                }
                else if (Math.Abs(target.Center.Y - NPC.Center.Y) > Math.Abs(target.Center.X - NPC.Center.X))
                {
                    NPC.ai[3] = Main.rand.Next(3, 5);
                    if (NPC.GetLifePercent() > 0.66) NPC.ai[3] = 4;
                    else if (NPC.GetLifePercent() > 0.33) NPC.ai[3] = 3;
                }
                else
                {
                    NPC.ai[3] = Main.rand.Next(5, 7);
                    if (NPC.GetLifePercent() > 0.66) NPC.ai[3] = 5;
                    else if (NPC.GetLifePercent() > 0.33) NPC.ai[3] = 6;
                }
                NPC.netUpdate = true;
            }

        }
        public void LaserRockets()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 0)
            {
                Teleport(target.Center + new Vector2(800, 0));
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -1200), Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), damage, 0, Main.myPlayer);
            }
            NPC.ai[1]++;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 20, 0.05f);
            if (NPC.ai[1] == 10)
            {
                Teleport(target.Center + new Vector2(-800, -400));
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -1200), Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), damage, 0, Main.myPlayer);
            }
            if (NPC.ai[1] % 20 == 0 && NPC.ai[1] <= 110 && NPC.ai[1] >= 10)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 12).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))), ModContent.ProjectileType<AnnihilationRocket>(), damage, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 12).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))), ModContent.ProjectileType<AnnihilationRocket>(), damage, 0, Main.myPlayer);
                }
            }
            if (NPC.ai[1] == 110)
            {
                Teleport(target.Center + new Vector2(-800, 400));
                NPC.velocity *= 0;
            }
            if (NPC.ai[1] % 20 == 0 && NPC.ai[1] > 110)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -12).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))), ModContent.ProjectileType<AnnihilationRocket>(), damage, 0, Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -12).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))), ModContent.ProjectileType<AnnihilationRocket>(), damage, 0, Main.myPlayer);
                }
            }
            if (NPC.ai[1] >= 210)
            {
                NPC.ai[1] = -200;
                NPC.ai[0] = 0;
                Teleport(target.Center + new Vector2(0, 400).RotatedByRandom(Math.PI * 2));
                NPC.velocity *= 0;
            }
        }
        public void TwoPortals()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 0)
            {
                Vector2 posOff = new Vector2(0, 400).RotatedByRandom(MathHelper.TwoPi);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + posOff, Vector2.Zero, ModContent.ProjectileType<CorvidPortal>(), damage, 0, Main.myPlayer, 3);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center - posOff, Vector2.Zero, ModContent.ProjectileType<CorvidPortal>(), damage, 0, Main.myPlayer, 3);
                }
                Teleport(Main.rand.NextBool() ? target.Center + posOff.RotatedBy(Main.rand.NextBool() ? MathHelper.ToRadians(20) : MathHelper.ToRadians(-20)) : target.Center - posOff.RotatedBy(Main.rand.NextBool() ? MathHelper.ToRadians(20) : MathHelper.ToRadians(-20)));
            }
            NPC.ai[1]++;
            if (NPC.ai[1] == 100 || NPC.ai[1] == 200)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center + target.velocity * 40 - NPC.Center).SafeNormalize(Vector2.Zero) * 20, ModContent.ProjectileType<CalamityMod.Projectiles.Enemy.FlameBurstHostile>(), damage, 0, Main.myPlayer);
            }
            if (NPC.ai[1] >= 300)
            {
                NPC.ai[1] = -30;
                NPC.ai[0] = 0;
            }
        }
        public void ThrowPortal()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 0)
            {
                Teleport(target.Center + new Vector2(500 * (Main.rand.NextBool() ? 1 : -1), 0));
            }
            if (NPC.ai[1] == 10)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(Math.PI) * 20, ModContent.ProjectileType<CorvidPortal>(), damage, 0, Main.myPlayer, 2.5f);
            }
            NPC.ai[1]++;

            if (NPC.ai[1] > 30 && NPC.ai[1] < 230 && NPC.ai[1] % 30 == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center + target.velocity * 20 - NPC.Center).SafeNormalize(Vector2.Zero) * 20, ModContent.ProjectileType<CalamityMod.Projectiles.Enemy.FlameBurstHostile>(), damage, 0, Main.myPlayer);
            }
            if (NPC.ai[1] == 250)
            {
                NPC.ai[1] = -60;
                NPC.ai[0] = 0;
            }
        }
        public void TpRocket()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.ai[2] = Main.rand.NextBool() ? 1 : -1;
                Teleport(target.Center + new Vector2(0, 400 * NPC.ai[2]));
            }
            NPC.ai[1]++;
            if (NPC.ai[1] == 5)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -30 * NPC.ai[2]).RotatedBy(MathHelper.ToRadians(60)), ModContent.ProjectileType<AnnihilationRocket>(), Main.myPlayer, 0, Main.myPlayer, 1);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -30 * NPC.ai[2]).RotatedBy(MathHelper.ToRadians(-60)), ModContent.ProjectileType<AnnihilationRocket>(), Main.myPlayer, 0, Main.myPlayer, 1);
                }
            }
            if (NPC.ai[1] == 30)
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = -60;
                NPC.ai[2] = 0;
            }
        }
        public void DemonAttacks()
        {
            Player target = Main.player[NPC.target];
            NPC.ai[1]++;
            if (NPC.ai[1] == 30)
            {
                if (target.velocity.X > 0)
                    Teleport(target.Center + new Vector2(400, 0));
                else
                    Teleport(target.Center + new Vector2(-400, 0));
                NPC.velocity *= 0;
            }
            if (NPC.ai[1] == 60)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), damage, 0, Main.myPlayer, 2);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), damage, 0, Main.myPlayer, 2, MathHelper.Pi);
                }
            }
            if (NPC.ai[1] == 180)
            {
                NPC.velocity *= 0;
                NPC.ai[1] = -60;
                NPC.ai[0] = 0;
            }
        }
        public void LaserSweep()
        {
            Player target = Main.player[NPC.target];
            if (NPC.ai[1] == 0)
            {
                Teleport(target.Center + new Vector2(-500 * (Main.rand.NextBool() ? 1 : -1), 0));
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), damage, 0, Main.myPlayer, 3, 0);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<AnnihilationLaser>(), damage, 0, Main.myPlayer, 3, MathHelper.Pi);
                }
            }
            NPC.ai[1]++;
            if (target.Center.X > NPC.Center.X)
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, 18, 0.02f);
            }
            else
            {
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, -18, 0.02f);
            }
            if (NPC.ai[1] % 60 == 0)
            {
                for (int i = -5; i < 5; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(NPC.velocity.X > 0 ? 20 : -20, 0).RotatedBy(MathHelper.ToRadians(i * 5)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.BrimstoneBarrage>(), damage, 0, Main.myPlayer);
                }
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneFireblastImpact"), NPC.Center);
            }
            if (NPC.ai[1] >= 150)
            {
                NPC.velocity.X = 0;
                NPC.ai[1] = -60;
                NPC.ai[0] = 0;
            }
        }

    }
}
