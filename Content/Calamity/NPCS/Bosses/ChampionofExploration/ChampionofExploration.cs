using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using FargowiltasSouls;
using Terraria.Localization;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    //Boss for the force of exploration, initial creator: Shipmans Agla, complain to him if shits broken
    //Enchantments included (base attacks off): Wulfrum, Desert Prowler, Marnite, Aerospec
    //Intended Tier: Post-ml
    [AutoloadBossHead]
    public class ChampionofExploration : ModNPC
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/Valkyrie";
        public override string BossHeadTexture => "FargowiltasCrossmod/Content/Calamity/NPCS/Bosses/ChampionofExploration/ChampionofExploration_Head_Boss";
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
            NPC.width = 50;
            NPC.height = 50;
            NPC.scale = 2;
            NPC.lifeMax = 200000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath6;
            NPC.defense = 30;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.npcSlots = 10;
            NPC.value = Item.buyPrice(platinum: 20);
            NPC.SpawnWithHigherTime(30);
            NPC.aiStyle = -1;
            NPC.frame.Width = 86;
            NPC.frame.Height = 68;
            Main.npcFrameCount[NPC.type] = 4;
            NPC.damage = 110;
            NPC.color = new Color(255, 255, 255);

            if (!Main.dedServ)
            {
                Music = ModLoader.TryGetMod("FargowiltasMusic", out Mod musicMod)
                    ? MusicLoader.GetMusicSlot(musicMod, "Assets/Music/Champions") : MusicID.OtherworldlyBoss1;
                SceneEffectPriority = SceneEffectPriority.BossLow;
            }
        }
        //without this the hp will just be doubled in expert (vanilla doesnt do that for bosses)
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)/* tModPorter Note: bossLifeScale -> balance (bossAdjustment is different, see the docs for details) */
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
            NPC.damage = (int)(NPC.damage * balance);
        }
        //bestiary bg is sky, idk what the info should be
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement(Language.GetTextValue("Mods.FargowiltasCrossmod.NPCs.ChampionofExploration.BestiaryEntry"))
            });
        }
        //drop forces
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {

        }
        //do something only when spawning (such as dust for spawn effect)
        public override void OnSpawn(IEntitySource source)
        {

        }
        //for gores when i feel like it, or other stuff
        public override void OnKill()
        {

        }

        //special effects like drawing extra sprites or afterimages. I assume this will be used later.
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {


            //afterimage
            Main.instance.LoadNPC(NPC.type);
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            //texture.Frame(1, 4, 0, NPC.frame.Y);
            Vector2 drawOrigin = new Vector2(NPC.frame.Width / 2f, NPC.frame.Height / 2f);
            for (int k = 0; k < (int)drawLength; k++)
            {
                SpriteEffects dir = SpriteEffects.FlipHorizontally;
                if (NPC.spriteDirection == -1)
                {
                    dir = SpriteEffects.None;
                }
                Vector2 drawPos = NPC.oldPos[k] - screenPos + drawOrigin;
                Color color = NPC.GetAlpha(drawColor) * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                spriteBatch.Draw(texture, drawPos, new Rectangle(0, NPC.frame.Y, NPC.frame.Width, NPC.frame.Height), color, NPC.rotation, drawOrigin, NPC.scale, dir, 0);
            }

            return true;
        }
        //playing animation
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter == 5)
            {
                NPC.frame.Y += NPC.frame.Height;
                NPC.frameCounter = 0;
                if (NPC.frame.Y == NPC.frame.Height * 4)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
        //the ai (wow!)

        public int attackTimer = 0;
        public int attack = 0;
        public Player target;
        public int phase;
        public int lastAttack;
        public int targetDrawLength = 10;
        public float drawLength = 10f;
        public int transitionTimer;
        public int dashTimer = 0;
        public Vector2 offset = new Vector2(400, 0);
        public int turbulenceTimer;
        public int direction;
        bool[] used = new bool[4] { false, false, false, false };
        int currentLightning;
        Vector2 laserBaseVel = Vector2.Zero;
        //Send extra added fields for multiplayer sync
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(attackTimer);
            writer.Write7BitEncodedInt(attack);
            writer.Write7BitEncodedInt(target.whoAmI);
            writer.Write7BitEncodedInt(phase);
            writer.Write7BitEncodedInt(lastAttack);
            writer.Write7BitEncodedInt(targetDrawLength);
            writer.Write(drawLength);
            writer.Write7BitEncodedInt(transitionTimer);
            writer.Write7BitEncodedInt(dashTimer);
            writer.Write7BitEncodedInt(turbulenceTimer);
            writer.Write7BitEncodedInt(direction);
            writer.Write7BitEncodedInt(currentLightning);
            writer.WriteVector2(offset);
            for (int i = 0; i < used.Length; i++)
            {
                writer.Write(used[i]);
            }
        }
        //Recieve extra added fields for multiplayer sync
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            attackTimer = reader.Read7BitEncodedInt();
            attack = reader.Read7BitEncodedInt();
            target = Main.player[reader.Read7BitEncodedInt()];
            phase = reader.Read7BitEncodedInt();
            lastAttack = reader.Read7BitEncodedInt();
            targetDrawLength = reader.Read7BitEncodedInt();
            drawLength = reader.ReadSingle();
            transitionTimer = reader.Read7BitEncodedInt();
            dashTimer = reader.Read7BitEncodedInt();
            turbulenceTimer = reader.Read7BitEncodedInt();
            direction = reader.Read7BitEncodedInt();
            currentLightning = reader.Read7BitEncodedInt();
            offset = reader.ReadVector2();
            for (int i = 0; i < used.Length; i++)
            {
                used[i] = reader.ReadBoolean();
            }
        }
        public enum attackP1
        {
            None,
            Circle,
            Coins,
            Lightning,
            Wulfrum,
        }
        public override void AI()
        {

            Targetting();
            UpdateTrailLength();

            if (NPC.GetLifePercent() <= 0.3 && phase == 0)
            {
                PhaseTransition();
                return;
            }
            switch ((attackP1)attack)
            {
                case attackP1.None:
                    NoAttack();
                    targetDrawLength = 3;
                    break;
                case attackP1.Circle:
                    CircleAttack();

                    targetDrawLength = 7;
                    break;
                case attackP1.Coins:
                    CoinsAttack();
                    targetDrawLength = 4;
                    break;
                case attackP1.Lightning:
                    LightningAttack();

                    break;
                case attackP1.Wulfrum:
                    WulfrumAttack();

                    break;
            }
        }
        //targetting the player, giving the player as a variable, despawning if no player, turning to face the player
        public void Targetting()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            target = Main.player[NPC.target];
            if (target.dead)
            {
                NPC.velocity.Y += 0.4f;
                NPC.EncourageDespawn(10);
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
        //grow/shrink the afterimage trail when it changes (drawLength is used in predraw)

        public void UpdateTrailLength()
        {
            if (drawLength > NPC.oldPos.Length)
            {
                drawLength = NPC.oldPos.Length;
            }
            if (drawLength > targetDrawLength)
            {
                drawLength -= 0.5f;
            }
            else if (drawLength < targetDrawLength)
            {
                drawLength += 0.5f;
            }
        }
        //i tried to use attackTimer but it breaks

        public void PhaseTransition()
        {
            NPC.defense = 75;
            transitionTimer++;
            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.03f);
            if (transitionTimer == 60)
            {
                SoundEngine.PlaySound(SoundID.Roar, NPC.Center);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/WulfrumBastionActivate"), NPC.Center);
            }
            if (transitionTimer > 60)
            {
                NPC.color = Color.Lerp(NPC.color, new Color(197, 255, 159), 0.05f);
            }
            if (transitionTimer == 120)
            {
                attack = (int)attackP1.None;
                transitionTimer = 0;
                phase = 1;
                attackTimer = 0;
                dashTimer = 0;
                turbulenceTimer = 0;
            }
        }
        //No attack chosen (intermediate between attacks, dashes towards player)

        public void NoAttack()
        {
            NPC.damage = NPC.GetAttackDamage_ScaledByStrength(110);
            NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.03f);
            dashTimer++;
            if (dashTimer == 60)
            {
                attackTimer++;
            }
            if (dashTimer == 60 && attackTimer != 4)
            {
                NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX) * 37;
                if (phase == 1)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.ToRadians(i * 5)) * 12, ModContent.ProjectileType<ExplorationTurbulence>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0);


                        }
                    }
                    SoundEngine.PlaySound(SoundID.Item1);
                    NPC.velocity *= 1.25f;
                }
                dashTimer = 0;

            }

            if (attackTimer == 4 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                attackTimer = 0;
                attack = Main.rand.Next((int)attackP1.Circle, (int)attackP1.Wulfrum + 1);
                while (lastAttack == attack)
                {
                    attack = Main.rand.Next((int)attackP1.Circle, (int)attackP1.Wulfrum + 1);
                }
                lastAttack = attack;
                //attack = (int)attackP1.Lightning;
                dashTimer = 0;
                NPC.netUpdate = true;
            }
        }

        public void CircleAttack()
        {
            if (attackTimer == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                direction = 1;
                if (Main.rand.NextBool())
                {
                    direction = -1;
                }
                offset = new Vector2(400, 0);
                offset = offset.RotatedBy(target.AngleTo(NPC.Center));
                NPC.netUpdate = true;
            }
            attackTimer++;
            int rotationAmount = 3;
            if (phase == 1)
            {
                rotationAmount = 4;
            }
            if (direction == -1)
            {
                offset = offset.RotatedBy(MathHelper.ToRadians(rotationAmount));
            }
            else
            {
                offset = offset.RotatedBy(MathHelper.ToRadians(-rotationAmount));
            }
            Vector2 targetPos = target.Center + offset;
            Vector2 toTargetPos = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 toPlayer = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);

            if (attackTimer % 10 == 0 && (direction == -1 && NPC.Center.X > target.Center.X || direction == 1 && NPC.Center.X < target.Center.X) && attackTimer > 70)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 projPos = new Vector2(NPC.Center.X + Main.rand.Next(-300, 300), target.Center.Y - 800 + Main.rand.Next(-50, 50));
                        Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), projPos, (target.Center - projPos).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<AeroFeather>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0f);
                    }
                }
                SoundEngine.PlaySound(SoundID.Item102, NPC.Center);
            }
            if (NPC.Center.Y < target.Center.Y + 20 && NPC.Center.Y > target.Center.Y - 20 && turbulenceTimer == 0 && attackTimer > 70)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = -30; i < 31; i += 30)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (toPlayer * 7).RotatedBy(MathHelper.ToRadians(i)), ModContent.ProjectileType<ExplorationTurbulence>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0);

                    }
                }
                turbulenceTimer = 10;
                SoundEngine.PlaySound(SoundID.Item1);
            }

            if (turbulenceTimer > 0) turbulenceTimer--;

            if (attackTimer > 500)
            {
                attack = (int)attackP1.None;
                attackTimer = 0;
            }
            if (phase == 0)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, toTargetPos * 50, 0.05f);
            }
            else
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, toTargetPos * 60, 0.07f);
            }
        }
        public void CoinsAttack()
        {

            NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center + new Vector2(0, -300) - NPC.Center).SafeNormalize(Vector2.Zero) * 30, 0.05f);
            if (attackTimer == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = -750; i < 751; i += 250)
                {
                    Projectile proj2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center + new Vector2(-1000, i), Vector2.Zero, ProjectileID.SandnadoHostile, FargoSoulsUtil.ScaledProjectileDamage(210), 0f, Main.myPlayer);
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center + new Vector2(1000, i), Vector2.Zero, ProjectileID.SandnadoHostile, FargoSoulsUtil.ScaledProjectileDamage(210), 0f, Main.myPlayer);

                }
                if (phase == 1)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center + new Vector2(-500, 0), Vector2.Zero, ProjectileID.SandnadoHostile, NPC.damage / 2, 0f, Main.myPlayer);
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center + new Vector2(500, 0), Vector2.Zero, ProjectileID.SandnadoHostile, NPC.damage / 2, 0f, Main.myPlayer);
                }
                for (int i = -1000; i < 1001; i += 75)
                {
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center + new Vector2(i, -750), Vector2.Zero, ModContent.ProjectileType<ExplorationDust>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0, Main.myPlayer, 0, -100);

                    Projectile proj2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), target.Center + new Vector2(i, 750), Vector2.Zero, ModContent.ProjectileType<ExplorationDust>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0, Main.myPlayer, 0, -100);

                }
            }
            if (attackTimer % 5 == 0 && attackTimer <= 130 && attackTimer > 60)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -1).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-45, 45))) * Main.rand.Next(7, 15), ModContent.ProjectileType<ExplorationCoin>(), 0, 0, Main.myPlayer, 0);

                }
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/Ultrabling"), NPC.Center);

            }
            if (attackTimer == 180)
            {
                Projectile coin = null;
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile the = Main.projectile[i];
                    if (the.type == ModContent.ProjectileType<ExplorationCoin>() && (coin == null || the.Distance(NPC.Center) < coin.Distance(NPC.Center)))
                    {

                        coin = the;

                    }


                }
                if (coin != null)
                {
                    coin.velocity *= 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (coin.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, ModContent.ProjectileType<ExplorationShot>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0, Main.myPlayer, -1, -1);

                    }
                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/CrackshotColtShot"), NPC.Center);
                }
            }

            if (attackTimer >= 180)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    Projectile the = Main.projectile[i];
                    if (the.type == ModContent.ProjectileType<ExplorationCoin>())
                    {
                        the.ai[0] = 1;
                    }
                }

            }
            attackTimer++;
            if (attackTimer == 210)
            {
                attack = (int)attackP1.None;
                attackTimer = 0;
            }
        }
        public enum LightningPos
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            MAX = BottomRight
        }

        public void LightningAttack()
        {
            NPC.damage = 0;
            if (attackTimer == 0)
            {
                bool spawnSword = true;
                for (int i = 0; i < used.Length; i++)
                {
                    if (used[i] == true)
                    {
                        spawnSword = false;
                    }
                }

                if (spawnSword && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile sword = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<ExplorationBlade>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0, Main.myPlayer, 0, target.whoAmI);

                    if (phase == 1)
                    {
                        Projectile sword2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, -NPC.velocity, ModContent.ProjectileType<ExplorationBlade>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0, Main.myPlayer, 40, target.whoAmI);

                    }
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    currentLightning = Main.rand.Next(0, (int)LightningPos.MAX + 1);

                    while (used[currentLightning] == true)
                    {
                        currentLightning = Main.rand.Next(0, (int)LightningPos.MAX + 1);
                    }
                    NPC.netUpdate = true;
                }
                attackTimer++;

            }
            Vector2 targetPos = Vector2.Zero;
            Vector2 futureTarget = Vector2.Zero;
            Vector2 lightningOffset = Vector2.Zero;
            Vector2 sparkVelocity = Vector2.Zero;
            float lightningRot = 0;

            switch ((LightningPos)currentLightning)
            {
                case LightningPos.TopLeft:
                    targetPos = target.Center + new Vector2(-400, -400);
                    futureTarget = target.Center + new Vector2(400, -400);
                    lightningOffset = new Vector2(-300, 0);
                    sparkVelocity = new Vector2(0, 5);
                    lightningRot = 0;
                    break;
                case LightningPos.TopRight:
                    targetPos = target.Center + new Vector2(400, -400);
                    futureTarget = target.Center + new Vector2(400, 400);
                    lightningOffset = new Vector2(0, -300);
                    sparkVelocity = new Vector2(-5, 0);
                    lightningRot = MathHelper.ToRadians(90);
                    break;
                case LightningPos.BottomLeft:
                    targetPos = target.Center + new Vector2(-400, 400);
                    futureTarget = target.Center + new Vector2(-400, -400);
                    lightningOffset = new Vector2(0, 300);
                    sparkVelocity = new Vector2(5, 0);
                    lightningRot = MathHelper.ToRadians(270);
                    break;
                case LightningPos.BottomRight:
                    targetPos = target.Center + new Vector2(400, 400);
                    futureTarget = target.Center + new Vector2(-400, 400);
                    lightningOffset = new Vector2(300, 0);
                    sparkVelocity = new Vector2(0, -5);
                    lightningRot = MathHelper.ToRadians(180);
                    break;
            }

            attackTimer++;
            if (attackTimer >= 120)
            {
                if (attackTimer == 140 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile lightning = null;
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Main.projectile[i].type == ProjectileID.CultistBossLightningOrbArc && Main.projectile[i].active)
                        {
                            lightning = Main.projectile[i];
                        }
                    }
                    if (currentLightning == (int)LightningPos.TopLeft && lightning != null)
                    {
                        for (int i = 0; i < 360; i += 15)
                        {
                            Projectile spark = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(20, 0).RotatedBy(MathHelper.ToRadians(i)), ProjectileID.MartianTurretBolt, FargoSoulsUtil.ScaledProjectileDamage(210), 0f);
                        }
                    }
                    else if (currentLightning == (int)LightningPos.TopRight && lightning != null)
                    {
                        for (int i = 0; i < 360; i += 15)
                        {
                            Projectile spark = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, 20).RotatedBy(MathHelper.ToRadians(i)), ProjectileID.MartianTurretBolt, FargoSoulsUtil.ScaledProjectileDamage(210), 0f);
                        }
                    }
                    else if (currentLightning == (int)LightningPos.BottomRight && lightning != null)
                    {
                        for (int i = 0; i < 360; i += 15)
                        {
                            Projectile spark = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-20, 0).RotatedBy(MathHelper.ToRadians(i)), ProjectileID.MartianTurretBolt, FargoSoulsUtil.ScaledProjectileDamage(210), 0f);
                        }
                    }
                    else if (currentLightning == (int)LightningPos.BottomLeft && lightning != null)
                    {
                        for (int i = 0; i < 360; i += 15)
                        {
                            Projectile spark = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -20).RotatedBy(MathHelper.ToRadians(i)), ProjectileID.MartianTurretBolt, FargoSoulsUtil.ScaledProjectileDamage(210), 0f);
                        }
                    }

                }
                targetPos = futureTarget;

            }
            if (attackTimer == 120)
            {
                attackTimer++;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + lightningOffset, (NPC.Center - (NPC.Center + lightningOffset)).SafeNormalize(Vector2.Zero) * 10, ProjectileID.CultistBossLightningOrbArc, FargoSoulsUtil.ScaledProjectileDamage(210), 0f, Main.myPlayer, lightningRot, Main.rand.Next(-10, 10));
                    Projectile vortex = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + lightningOffset, Vector2.Zero, ProjectileID.VortexVortexLightning, 0, 0f, Main.myPlayer, 91);


                }
                SoundEngine.PlaySound(new SoundStyle("Terraria/Sounds/Thunder_0") with { Volume = 0.5f }, NPC.Center);
            }
            Vector2 lerpVel = (targetPos - NPC.Center).SafeNormalize(Vector2.Zero) * 30;
            NPC.velocity = Vector2.Lerp(NPC.velocity, lerpVel, 0.08f);
            if (attackTimer == 180)
            {
                attackTimer = 0;
                used[currentLightning] = true;
            }

            for (int i = 0; i < used.Length; i++)
            {
                if (used[i] == false)
                {
                    return;
                }
            }


            used = new bool[4] { false, false, false, false };
            attack = (int)attackP1.None;
            attackTimer = 0;
        }

        public void WulfrumAttack()
        {


            if (attackTimer == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/AbilitySounds/WulfrumBastionActivate"), NPC.Center);

            }
            if (attackTimer < 60)
            {
                laserBaseVel = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.ToRadians(90)) * 15;
                NPC.velocity = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * NPC.Distance(target.Center) / 30;
            }
            int sweepingTime = 230;
            if (phase == 1)
            {
                sweepingTime += 120;
            }
            if (attackTimer < sweepingTime && attackTimer > 60)
            {

                Vector2 vel = laserBaseVel;
                vel = vel.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-10, 10)));
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ProjectileID.SaucerLaser, NPC.damage / 2, 0);
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile scrap = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))) * Main.rand.Next(1, 40) / 4f, ModContent.ProjectileType<WulfrumScrap>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0);

                    }

                }

                NPC.velocity = Vector2.Lerp(NPC.velocity, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 3, 0.03f);
                if (attackTimer % 4 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item91, NPC.Center);
                }
            }
            attackTimer++;
            laserBaseVel = laserBaseVel.RotatedBy(MathHelper.ToRadians(-2.2f));
            if (attackTimer == 10 + sweepingTime)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC drone = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<ExplorationDrone>());

                    }
                }

            }
            else if (attackTimer == 20 + sweepingTime)
            {
                attackTimer = 0;
                attack = (int)attackP1.None;
            }
        }
    }
}
