using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Bestiary;
using System.IO;
using CalamityMod.CalPlayer;
using Terraria.Audio;
using Terraria.Localization;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDesolation
{
    [JITWhenModsEnabled("CalamityMod")]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [AutoloadBossHead]
    public class DesolationHead : WormHead
    {
        public override string BossHeadTexture => "CalamityMod/NPCs/SunkenSea/SeaSerpent1";
        public override string Texture => "CalamityMod/NPCs/SunkenSea/SeaSerpent1";
        public override int BodyType => ModContent.NPCType<DesolationBody>();
        public override int TailType => ModContent.NPCType<DesolationTail>();

        public override void SetStaticDefaults()
        {
            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/SeaSerpent_Bestiary",
                Position = new Vector2(40f, 24f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 12f
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            //add more debuffs if it makes sense idk what else is needed (separate with comma)
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,

            });

            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 24;
            NPC.scale = 2;
            NPC.lifeMax = 800000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.value = Item.buyPrice(20);
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.damage = 110;
            NPC.netAlways = true;
            if (!Main.dedServ)
            {
                Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                    ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
                SceneEffectPriority = SceneEffectPriority.BossLow;
            }
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(NPC.lifeMax * bossAdjustment);
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ChampionofDesolation.BestiaryEntry"))
            });
        }
        public override void Init()
        {
            MinSegmentLength = 20;
            MaxSegmentLength = 20;
            CanFly = true;
            CommonWormInit(this);
        }
        internal static void CommonWormInit(Worm worm)
        {
            worm.MoveSpeed = 10f;
            worm.Acceleration = 0.5f;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {

        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {

        }
        public int squidTimer;
        public int phase;
        public override void AI()
        {
            int scaledDamage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(110);
            Player target = Main.player[NPC.target];
            if (NPC.velocity.X > 0)
            {
                NPC.spriteDirection = 1;
            }
            else
            {
                NPC.spriteDirection = -1;
            }
            for (int i = 0; i < Main.player.Length; i++)
            {
                Player p = Main.player[i];
                if (p != null && p.active && !p.dead)
                {
                    CalamityPlayer c = p.GetModPlayer<CalamityPlayer>();
                    if (p.Distance(NPC.Center) < 10000)
                    {
                        p.AddBuff(ModContent.BuffType<CalamityMod.Buffs.StatBuffs.AmidiasBlessing>(), 2);

                    }
                }
            }
            if (phase != 2 && NPC.GetLifePercent() <= 0.5f)
            {
                if (phase == 0)
                {
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.ai[1] = 0;
                    phase = 1;
                }
                NPC.ai[3]++;
                NPC.velocity = (target.Center - NPC.Center) / 30;
                if (NPC.ai[3] == 60)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<FargowiltasSouls.Content.Projectiles.GlowRing>(), 0, 0, Main.myPlayer, NPC.whoAmI, -10);
                    }
                    SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                }
                if (NPC.ai[3] == 120)
                {
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.ai[1] = 0;
                    phase = 2;
                }
                return;
            }
            squidTimer++;
            if (NPC.AnyNPCs(ModContent.NPCType<BabyColossalSquid>()))
            {
                squidTimer = 0;
            }
            if (squidTimer >= 600)
            {
                squidTimer = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BabyColossalSquid>(), 0, 0, NPC.whoAmI);
                }
            }
            if (NPC.ai[2] == 0)
            {
                NPC.ai[2] = 1;
            }
            NPC.ai[1]++;
            if (NPC.ai[1] < 100)
            {
                return;
            }

            if (NPC.ai[2] == 1)
            {
                SideDashPearls();
            }
            else if (NPC.ai[2] == 2)
            {
                AstralMines();
            }
            else if (NPC.ai[2] == 3)
            {
                CloudSpam();
            }
        }
        public void CloudSpam()
        {
            int scaledDamage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(210);
            Player target = Main.player[NPC.target];
            NPC.ai[3]++;
            NPC.rotation = NPC.AngleTo(target.Center + target.velocity * 15) + MathHelper.PiOver2;
            NPC.velocity = new Vector2(0, -3).RotatedBy(NPC.rotation);
            if (NPC.GetLifePercent() > 0.5f)
            {
                if (NPC.ai[3] % 5 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -1).RotatedBy(NPC.rotation + MathHelper.ToRadians(Main.rand.Next(-40, 40))) * Main.rand.Next(12, 20), ModContent.ProjectileType<SulphurCloudHostile>(), scaledDamage, 0, Main.myPlayer);
                    }
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
                }
            }
            else
            {
                if (NPC.ai[3] % 8 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -1).RotatedBy(NPC.rotation + MathHelper.ToRadians(Main.rand.Next(-40, 40))) * Main.rand.Next(12, 20), ModContent.ProjectileType<SulphurCloudHostile>(), scaledDamage, 0, Main.myPlayer);
                    }
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                }
                if (NPC.ai[3] % 50 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, ProjectileID.CultistBossLightningOrbArc, scaledDamage, 0, Main.myPlayer, NPC.Center.AngleTo(target.Center), Main.rand.Next(-10, 10));
                    }
                    SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Thunder_0") with { Volume = 0.75f }, NPC.Center);

                }
            }
            if (NPC.ai[3] == 80 && Main.netMode != NetmodeID.MultiplayerClient)
            {

                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PlagueScythe>(), scaledDamage, 0, Main.myPlayer, target.whoAmI);
            }
            if (NPC.ai[3] == 90 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PlagueScythe>(), scaledDamage, 0, Main.myPlayer, target.whoAmI);
            }
            if (NPC.ai[3] >= 100)
            {
                NPC.ai[2] = 1;
                NPC.ai[3] = 0;
                NPC.ai[1] = -100;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<PlagueScythe>(), scaledDamage, 0, Main.myPlayer, target.whoAmI);
            }
        }
        public Vector2 targetOffset;
        public void AstralMines()
        {
            int scaledDamage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(210);
            Player target = Main.player[NPC.target];
            if (NPC.GetLifePercent() > 0.5f)
            {
                NPC.ai[3]++;
                if (NPC.ai[3] % 15 == 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))) * 3, ModContent.ProjectileType<AstralMine>(), 0, 0, Main.myPlayer);
                    }
                    SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                }
                if (NPC.ai[3] > 70)
                {
                    NPC.ai[3] = 0;
                    NPC.ai[2] = 3;
                    NPC.ai[1] = 80;
                }
            }
            else
            {
                if (NPC.ai[3] == 0)
                {
                    NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 30f;
                    if (NPC.Distance(target.Center) < 200)
                    {
                        targetOffset = (NPC.Center - target.Center).SafeNormalize(Vector2.Zero) * 600;
                        NPC.ai[3]++;
                    }
                }
                if (NPC.ai[3] > 0)
                {
                    targetOffset = targetOffset.RotatedBy(MathHelper.ToRadians(5));
                    NPC.velocity = (target.Center + targetOffset - NPC.Center).SafeNormalize(Vector2.Zero) * 35f;
                    NPC.ai[3]++;
                }
                if (NPC.ai[3] % 15 == 0 && NPC.ai[3] != 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, targetOffset / 20, ModContent.ProjectileType<AstralMine>(), 0, 0, Main.myPlayer);
                    }
                    SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                }
                if (NPC.ai[3] >= 100)
                {
                    NPC.velocity /= 2;
                    NPC.ai[3] = 0;
                    NPC.ai[2] = 3;
                    NPC.ai[1] = 20;
                }
            }
        }
        Vector2 DashingVelocity;
        public void SideDashPearls()
        {
            int scaledDamage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(210);
            Player target = Main.player[NPC.target];
            if (NPC.ai[3] == 0)
            {
                DashingVelocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20;
                if (NPC.GetLifePercent() > 0.5f)
                {
                    if (Main.rand.NextBool())
                    {
                        DashingVelocity = DashingVelocity.RotatedBy(MathHelper.ToRadians(30));
                    }
                    else
                    {
                        DashingVelocity = DashingVelocity.RotatedBy(MathHelper.ToRadians(-30));
                    }

                }
            }
            NPC.ai[3]++;
            NPC.velocity = DashingVelocity;
            if (NPC.ai[3] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, DashingVelocity.RotatedBy(MathHelper.PiOver2) / 2f, ModContent.ProjectileType<CalamityMod.Projectiles.Enemy.PearlBurst>(), scaledDamage, 0, Main.myPlayer);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, DashingVelocity.RotatedBy(-MathHelper.PiOver2) / 2f, ModContent.ProjectileType<CalamityMod.Projectiles.Enemy.PearlBurst>(), scaledDamage, 0, Main.myPlayer);
            }
            if (NPC.ai[3] % 10 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item68, NPC.Center);
            }
            if (NPC.ai[3] >= 75)
            {
                if (NPC.GetLifePercent() <= 0.5f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -5; i < 6; i++)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, -DashingVelocity.RotatedBy(MathHelper.ToRadians(i * 20)) / 2f, ModContent.ProjectileType<CalamityMod.Projectiles.Enemy.PearlBurst>(), scaledDamage, 0, Main.myPlayer);
                        }
                    }
                    SoundEngine.PlaySound(SoundID.Item68, NPC.Center);
                }
                NPC.ai[3] = 0;
                NPC.ai[2] = 2;
                NPC.ai[1] = 0;
            }
        }
    }
    [JITWhenModsEnabled("CalamityMod")]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DesolationBody : WormBody
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);


            //add more debuffs if it makes sense idk what else is needed (separate with comma)
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,

            });
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override string Texture => "CalamityMod/NPCs/SunkenSea/SeaSerpent2";
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 16;
            NPC.scale = 2;
            NPC.lifeMax = 400000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.netAlways = true;

            NPC.value = Item.buyPrice(20);
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.damage = 110;
        }
        public override void Init()
        {
            DesolationHead.CommonWormInit(this);
        }
        public override bool CheckActive()
        {
            NPC head = null;
            if (NPC.realLife != -1)
            {
                head = Main.npc[NPC.realLife];
            }
            if (head == null || !head.active)
            {
                return true;
            }
            return false;
        }
        public override void AI()
        {
            base.AI();
            NPC head = null;
            if (NPC.realLife != -1)
            {
                head = Main.npc[NPC.realLife];
            }
            if (head == null || !head.active)
            {
                return;
            }

            if (NPC.oldPosition.X < NPC.position.X)
            {
                NPC.spriteDirection = 1;
            }
            else
            {
                NPC.spriteDirection = -1;
            }
            if (head.ai[2] == 2 && head.ai[3] > 0 && head.GetLifePercent() <= 0.5f)
            {
                Vector2 pivot = Main.npc[NPC.realLife].Center;
                pivot += Vector2.Normalize(Main.npc[NPC.realLife].velocity.RotatedBy(MathHelper.PiOver2 * 1)) * 300;
                if (NPC.Distance(pivot) < 300) //make sure body doesnt coil into the circling zone
                    NPC.Center = pivot + NPC.DirectionFrom(pivot) * 300;
            }
            //NPC next = Main.npc[(int)NPC.ai[0]];
            //if (head.ai[2] == 2 && head.ai[3] > 0 && head.GetLifePercent() <= 0.5f && next.Distance(Main.player[head.target].Center) < head.Distance(Main.player[head.target].Center) + 50)
            //{
            //    next.Center = NPC.Center + new Vector2(0, NPC.width).RotatedBy(NPC.rotation + MathHelper.ToRadians(-15));
            //}
        }
    }
    [JITWhenModsEnabled("CalamityMod")]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class DesolationTail : WormTail
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Hide = true // Hides this NPC from the Bestiary, useful for multi-part NPCs whom you only want one entry.
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);


            //add more debuffs if it makes sense idk what else is needed (separate with comma)
            NPCID.Sets.DebuffImmunitySets.Add(NPC.type, new Terraria.DataStructures.NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true,

            });
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override string Texture => "CalamityMod/NPCs/SunkenSea/SeaSerpent5";
        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 24;
            NPC.scale = 2;
            NPC.lifeMax = 400000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 20;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.netAlways = true;

            NPC.value = Item.buyPrice(20);
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.damage = 110;
        }
        public override void Init()
        {
            DesolationHead.CommonWormInit(this);
        }
        public override bool CheckActive()
        {
            NPC head = null;
            if (NPC.realLife != -1)
            {
                head = Main.npc[NPC.realLife];
            }
            if (head == null || !head.active)
            {
                return true;
            }
            return false;
        }
        public override void AI()
        {
            base.AI();
            NPC head = null;
            if (NPC.realLife != -1)
            {
                head = Main.npc[NPC.realLife];
            }
            if (head == null || !head.active)
            {
                return;
            }

            if (NPC.oldPosition.X < NPC.position.X)
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
