using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;
using System.Collections.Generic;
using Terraria.GameContent.Bestiary;
using System.IO;
using CalamityMod;
using Terraria.Localization;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExaltation
{
    
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ChampionofExaltation : ModNPC
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/FieryDraconid";
        public override void SetStaticDefaults()
        {

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            //add more debuffs if it makes sense idk what else is needed (separate with comma)
            NPCDebuffImmunityData debuffdata = new NPCDebuffImmunityData
            {
                ImmuneToAllBuffsThatAreNotWhips = true
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffdata);
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.width = 100;
            NPC.height = 50;
            Main.npcFrameCount[Type] = 10;
            NPC.frame.Height = 176;
            NPC.frame.Width = 206;
            NPC.lifeMax = 4000000;
            NPC.HitSound = SoundID.DD2_BetsyHurt;
            NPC.DeathSound = SoundID.DD2_BetsyDeath;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.npcSlots = 10;
            NPC.value = Item.buyPrice(platinum: 20);
            NPC.SpawnWithHigherTime(30);
            NPC.damage = 100;
            NPC.scale = 2f;
            NPC.noGravity = true;
            NPC.netAlways = true;

            NPC.noTileCollide = true;
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
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frame.Y += NPC.frame.Height;
                NPC.frameCounter = 0;
                if (NPC.frame.Y > 5 * NPC.frame.Height && NPC.localAI[0] == 0)
                {
                    NPC.frame.Y = 0;
                }
                else if (NPC.frame.Y > 9 * NPC.frame.Height && NPC.localAI[0] == 1)
                {
                    NPC.frame.Y = 5 * NPC.frame.Height;
                }
            }
            if (NPC.localAI[0] == 1 && NPC.frame.Y < NPC.frame.Height * 6)
            {
                NPC.frame.Y = NPC.frame.Height * 6;
            }
            else if (NPC.localAI[0] == 0 && NPC.frame.Y >= NPC.frame.Height * 6)
            {
                NPC.frame.Y = NPC.frame.Height * 0;
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheUnderworld,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ChampionofExaltation.BestiaryEntry"))
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

        }
        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void OnKill()
        {

        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            //writer.Write7BitEncodedInt(barrierTime);
            //writer.Write(barrierRot);
            //writer.WriteVector2(dashLocation);
            //writer.WriteVector2(previousLocation);
            //writer.Write(NPC.localAI[0]);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //barrierTime = reader.Read7BitEncodedInt();
            //barrierRot = reader.ReadSingle();
            //dashLocation = reader.ReadVector2();
            //previousLocation = reader.ReadVector2();
            //NPC.localAI[0] = reader.ReadSingle();
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            base.HitEffect(hit);
        }
        public int barrierTime;
        public float barrierRot;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (barrierTime > 0)
            {
                Asset<Texture2D> barrier = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Healing/SilvaOrb");
                for (int i = 0; i < 100; i++)
                {
                    spriteBatch.Draw(barrier.Value, NPC.Center + new Vector2(0, 1500).RotatedBy(barrierRot + MathHelper.ToRadians(i * 3.6f)) - Main.screenPosition, null, new Color(255, 255, 255, 100) * 0.9f, barrierRot * 2 + i, barrier.Size() / 2, 4, SpriteEffects.None, 0);
                }
            }
            Player target = Main.player[NPC.target];
            SpriteEffects dir = SpriteEffects.FlipHorizontally;
            int mult = 1;
            int mult2 = 1;
            if (NPC.spriteDirection == -1)
            {
                if (NPC.rotation == 0)
                {
                    dir = SpriteEffects.None;
                    mult2 = -1;
                }
                else
                {
                    mult = -1;
                    dir = SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
                }
            }
            if (NPC.ai[0] == 6 && NPC.ai[1] != 0 && target != null && target.active && NPC.ai[1] < 100)
            {
                Vector2 toTarget = target.Center - NPC.Center;
                Vector2 targetLoc = NPC.Center + (toTarget + toTarget.SafeNormalize(Vector2.Zero) * 600).RotatedBy(MathHelper.ToRadians(20));
                Asset<Texture2D> laserTelegraph = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/LaserWallTelegraphBeam");
                spriteBatch.Draw(laserTelegraph.Value, NPC.Center - Main.screenPosition, null, Color.Cyan * (NPC.ai[1] / 100f), NPC.AngleTo(targetLoc), new Vector2(0, laserTelegraph.Height() / 2), new Vector2(5, 3), SpriteEffects.None, 0);
            }

            Asset<Texture2D> texture = TextureAssets.Npc[Type];
            Vector2 position = NPC.Center - new Vector2(75 * mult2, 25 * mult).RotatedBy(NPC.rotation) - Main.screenPosition;
            Rectangle source = new Rectangle(0, NPC.frame.Y, NPC.frame.Width, NPC.frame.Height);


            if (NPC.ai[0] == 6 && NPC.ai[1] >= 100 && NPC.ai[1] <= 160)
            {
                Vector2 beginToEnd = previousLocation - dashLocation;
                for (int i = 0; i < 20; i++)
                {
                    Main.EntitySpriteDraw(texture.Value, position + (beginToEnd - beginToEnd.SafeNormalize(Vector2.Zero) * i * beginToEnd.Length() / 20), source, new Color(drawColor.R, drawColor.G, drawColor.B, 205) * (1 - (NPC.ai[1] - 100f) / (60f * (i / 20f))), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, dir, 0);
                }
            }
            spriteBatch.Draw(texture.Value, position, source, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, dir, 0);
            return false;
        }
        public override bool CheckActive()
        {
            Targetting();
            Player target = Main.player[NPC.target];
            if (target == null || !target.active || target.dead)
            {
                return true;
            }
            return false;
        }

        public override void AI()
        {

            //NPC.ai[0] = 6;
            Targetting();
            int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310);
            Player target = Main.player[NPC.target];
            if (target == null || !target.active || target.dead)
            {
                NPC.velocity.Y += -1;
                return;
            }
            //NPC.rotation = NPC.AngleTo(target.Center);
            if (NPC.ai[0] == 0)
            {
                Tarragon();
                barrierRot += MathHelper.ToRadians(1);
                if (target.Distance(NPC.Center) > 1500)
                {
                    target.position += (NPC.Center - target.Center).SafeNormalize(Vector2.Zero) * 3;
                    target.velocity = Vector2.Zero;

                    Dust.NewDust(target.position, target.width, target.height, DustID.TerraBlade);
                }
            }
            else if (NPC.ai[0] == 2)
            {
                Bloodflare();
            }
            else if (NPC.ai[0] == 4)
            {
                Silva();
            }
            else if (NPC.ai[0] == 6)
            {
                GodSlayer();
            }
            else
            {
                Auric();
            }
            barrierTime--;
        }
        public void Tarragon()
        {
            int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310);
            Player target = Main.player[NPC.target];
            NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 1, 0.03f);
            barrierTime = 120;
            //NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 10, 0.03f);

            NPC.ai[1]++;
            if (NPC.ai[1] > 100 && NPC.ai[1] < 160 && NPC.ai[1] % 3 == 0)
            {
                //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20, ModContent.ProjectileType<Biofussilade>(), damage, 0, Main.myPlayer);
                //SoundEngine.PlaySound(SoundID.Item33, NPC.Center);
            }
            if (NPC.ai[1] % 120 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TarragonRoot>(), damage, 0, Main.myPlayer, 40, MathHelper.ToRadians(i * 72));
                }

            }
            if (NPC.ai[1] == 260)
            {
                NPC.ai[1] = 0;
            }
            if (NPC.GetLifePercent() <= 0.75f)
            {

                AuricTransition(4);
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<TarragonRoot>())
                    {
                        Main.projectile[i].Kill();
                    }
                }
            }
        }
        public void Bloodflare()
        {

            int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310);
            Player target = Main.player[NPC.target];

            if (NPC.ai[1] == 0)
            {
                if (NPC.ai[2] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float angle = MathHelper.ToRadians(360 / 5 * i);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 500).RotatedBy(angle), Vector2.Zero, ModContent.ProjectileType<PhantomOrb>(), damage, 0, Main.myPlayer, target.whoAmI, angle);
                    }
                }
                NPC.ai[2]++;
                NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, 0.03f);
                if (NPC.ai[2] < 100 && NPC.ai[2] > 40 && NPC.ai[2] % 4 == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCHit36, NPC.Center);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 10).RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360))), ModContent.ProjectileType<PhantomSpirit>(), damage, 0, Main.myPlayer, NPC.whoAmI);

                }
                if (NPC.ai[2] > 300)
                {
                    NPC.ai[2] = 0;
                    NPC.ai[1]++;

                }

            }
            if (NPC.ai[1] > 0 && NPC.ai[1] < 4)
            {
                NPC.velocity = Vector2.Zero;
                if (NPC.ai[2] == 0)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.RedTorch, Scale: 5f);
                        dust.noGravity = true;
                        dust.velocity *= new Vector2(0, Main.rand.Next(1, 40)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360)));
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.Center = target.Center + new Vector2(0, 300).RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360)));
                        NPC.localAI[0] = 0;
                        NPC.rotation = NPC.AngleTo(target.Center) + MathHelper.PiOver4;
                        NPC.ai[3] = NPC.AngleTo(target.Center);
                        NPC.netUpdate = true;
                    }

                    SoundEngine.PlaySound(SoundID.Item69, NPC.Center);
                    for (int i = 0; i < 100; i++)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.RedTorch, Scale: 5f);
                        dust.noGravity = true;
                        dust.velocity *= new Vector2(0, Main.rand.Next(1, 40)).RotatedBy(MathHelper.ToRadians(Main.rand.Next(0, 360)));
                    }
                }
                NPC.ai[2] += 1f / 40f;

                if (NPC.ai[2] > 1)
                {

                    NPC.localAI[0] = 1;
                    NPC.rotation = (NPC.ai[3] + MathHelper.PiOver4).AngleLerp(NPC.ai[3] - MathHelper.PiOver4, NPC.ai[2] - 1);
                    if (Main.rand.NextBool(2))
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(50, 0).RotatedBy(NPC.rotation), ModContent.ProjectileType<Bloodfire>(), damage, 0, Main.myPlayer);
                        SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, NPC.Center);
                    }
                }
                else
                {
                    NPC.Center = target.Center + new Vector2(-300, 0).RotatedBy(NPC.ai[3]);
                }
                if (NPC.ai[2] >= 2)
                {
                    NPC.ai[1]++;
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.rotation = 0;
                    NPC.localAI[0] = 0;
                }

            }
            if (NPC.ai[1] == 4)
            {
                NPC.ai[1] = 0;
            }
            if (NPC.GetLifePercent() <= 0.5f)
            {
                AuricTransition(6);
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<PhantomOrb>())
                    {
                        Main.projectile[i].Kill();
                    }
                }
            }
        }
        public void Silva()
        {
            int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310);
            Player target = Main.player[NPC.target];

            NPC.ai[1]++;
            if (NPC.ai[1] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                float randomOffset = MathHelper.ToRadians(Main.rand.Next(0, 360));
                for (int i = 0; i < 6; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<SilvaCrystal>(), damage, 0, Main.myPlayer, NPC.target, MathHelper.ToRadians(60 * i) + randomOffset);
                }
            }
            if (NPC.ai[1] >= 300 && NPC.ai[1] < 360)
            {
                if (NPC.ai[1] == 300)
                {
                    NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 25;
                    SoundEngine.PlaySound(SoundID.DD2_BetsyScream, NPC.Center);
                    NPC.rotation = NPC.AngleTo(target.Center);
                    NPC.localAI[0] = 1;
                }
                if (NPC.velocity.X > 0)
                {
                    NPC.spriteDirection = 1;
                }
                else
                {
                    NPC.spriteDirection = -1;
                }
                if (NPC.ai[1] % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, -NPC.velocity.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-40, 40))) * 0.25f, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.RedLightningFeather>(), damage, 0, Main.myPlayer);
                }
                return;
            }
            NPC.rotation = 0;
            NPC.localAI[0] = 0;
            NPC.velocity /= 1.1f;
            if (NPC.ai[1] == 400)
            {

                NPC.ai[1] = 0;
            }

            if (NPC.GetLifePercent() <= 0.25f)
            {
                AuricTransition(10);
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Main.projectile[i].type == ModContent.ProjectileType<SilvaCrystal>())
                    {
                        Main.projectile[i].Kill();
                    }
                }
            }
        }
        Vector2 dashLocation;
        Vector2 previousLocation;
        public void GodSlayer()
        {
            int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310);
            Player target = Main.player[NPC.target];
            NPC.velocity = Vector2.Zero;
            NPC.ai[1]++;
            if (NPC.ai[1] == 100)
            {
                previousLocation = NPC.Center;
                Vector2 toTarget = target.Center - NPC.Center;
                dashLocation = NPC.Center + (toTarget + toTarget.SafeNormalize(Vector2.Zero) * 600).RotatedBy(MathHelper.ToRadians(20));
                NPC.Center = dashLocation;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/DevourerAttack"), NPC.Center);
                int distance = (int)(dashLocation - previousLocation).Length();
                for (int i = 0; i < (dashLocation - previousLocation).Length(); i++)
                {
                    Dust dust = Main.dust[Dust.NewDust(NPC.Center - (dashLocation - previousLocation).SafeNormalize(Vector2.Zero) * i - new Vector2(NPC.width, NPC.height) / 2, NPC.width, NPC.height, DustID.VenomStaff, newColor: new Color(250, 250, 250, 170), Scale: 2)];
                    dust.noGravity = true;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 5).RotatedBy(MathHelper.ToRadians(i * 36)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.DoGFire>(), damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(i * 36 + 9)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.DoGFire>(), damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 8).RotatedBy(MathHelper.ToRadians(i * 36 - 9)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.DoGFire>(), damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 6f).RotatedBy(MathHelper.ToRadians(i * 36f + 18f)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.DoGFire>(), damage, 0, Main.myPlayer);
                        //Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 6.5f).RotatedBy(MathHelper.ToRadians(i * 36f)), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.DoGFire>(), damage, 0, Main.myPlayer);
                    }
                }
            }

            if (NPC.ai[1] == 200)
            {
                NPC.ai[1] = 0;
                Vector2 offset = new Vector2(0, 1100);
                Vector2 offset2;

                if (Main.rand.NextBool())
                {
                    offset = offset.RotatedBy(MathHelper.ToRadians(22.5f));
                    offset2 = offset.RotatedBy(MathHelper.ToRadians(-45));
                }
                else
                {
                    offset = -offset.RotatedBy(MathHelper.ToRadians(-22.5f));
                    offset2 = offset.RotatedBy(MathHelper.ToRadians(45));
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = -20; i <= 20; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + offset + offset.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * i * 100, -offset.SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.DoGDeath>(), damage, 0, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), target.Center + offset2 + offset2.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * i * 100, -offset2.SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.DoGDeath>(), damage, 0, Main.myPlayer);
                    }
                }
                SoundEngine.PlaySound(SoundID.Item12, target.Center);
            }
            if (NPC.GetLifePercent() <= 0.01f)
            {
                AuricTransition(13);
            }
        }
        public void Auric()
        {
            int damage = FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(310);
            Player target = Main.player[NPC.target];
            NPC.reflectsProjectiles = true;
            NPC.defense = 9999;
            NPC.localAI[0] = 0;
            NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 8, 0.03f);
            NPC.ai[1]++;
            if (NPC.ai[1] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(15, 0).RotatedBy(MathHelper.TwoPi / 5f * i + MathHelper.ToRadians(NPC.ai[1])), ModContent.ProjectileType<CalamityMod.Projectiles.Boss.FlareDust2>(), damage, 0, Main.myPlayer);
                }
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<AuricSoul>()))
            {
                NPC.reflectsProjectiles = false;
                NPC.defense = 100;
                NPC.ai[0]++;
                NPC.ai[1] = 0;
                NPC.ai[2] = 0;
                NPC.ai[3] = 0;
                NPC.DR_NERD(0);
            }
        }
        public void AuricTransition(int numSouls)
        {
            NPC.ai[1] = 0;
            NPC.ai[2] = 0;
            NPC.ai[3] = 0;
            NPC.localAI[0] = 0;
            NPC.rotation = 0;
            NPC.DR_NERD(1);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < numSouls; i++)
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AuricSoul>(), 0, NPC.whoAmI);


                }
            }
            SoundEngine.PlaySound(SoundID.Item92, NPC.Center);
            NPC.ai[0]++;
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
                NPC.velocity.Y += 0.5f;
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
