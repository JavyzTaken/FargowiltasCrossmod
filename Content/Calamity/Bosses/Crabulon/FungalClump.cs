using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Crabulon
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class FungalClump : ModNPC
    {
        public override string Texture => "CalamityMod/Projectiles/Typeless/FungalClumpMinion";
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.lifeMax = 1000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.defense = 5;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0;
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.frame.Height = 70;
            NPC.frame.Width = 50;
            Main.npcFrameCount[Type] = 6;
            DrawOffsetY = 20;
        }
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Hide = true
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {

        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {

        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 5; i++)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, Main.rand.Next(375, 378), 1.5f);
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npcLoot);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.ai[2] == 1)
            {
                Main.instance.LoadProjectile(ProjectileID.Arkhalis);
                Asset<Texture2D> slash = TextureAssets.Projectile[ProjectileID.Arkhalis];
                Player target = Main.player[NPC.target];
                NPC.localAI[0] += 0.5f;
                if (NPC.localAI[0] >= 28)
                {
                    NPC.localAI[0] = 0;
                }
                spriteBatch.Draw(slash.Value, NPC.Center - Main.screenPosition, new Rectangle(0, (int)NPC.localAI[0] * 64, 68, 64), new Color(250, 250, 250), NPC.AngleTo(target.Center), new Vector2(10, 32), NPC.scale * 1.4f, SpriteEffects.None, 0);
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);

        }

        public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
        {

            return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
        }
        public override void OnKill()
        {

        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > frameHeight * 5)
                {
                    NPC.frame.Y = 0;
                }
                NPC.frameCounter = 0;
            }
        }

        public override void AI()
        {

            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
                NPC.netUpdate = true;
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += 1;
                return;
            }
            Player target = Main.player[NPC.target];
            NPC owner = Main.npc[(int)NPC.ai[1]];
            Vector2 toplayer = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            if (Main.rand.NextBool(10))
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.GlowingMushroom, Alpha: 100).noGravity = true;
            Lighting.AddLight(NPC.Center, new Vector3(0.2f, 0.2f, 0.6f));

            NPC.velocity = Vector2.Lerp(NPC.velocity, toplayer * 4, 0.03f);
            NPC.rotation = MathHelper.ToRadians(NPC.velocity.X * 3);
            NPC.ai[0]++;
            if (NPC.ai[0] >= 300)
            {
                if (DLCUtils.HostCheck)
                    for (int i = 0; i < 20; i++)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, Main.rand.NextFloat() / 1.5f).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ShroomGas>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
                NPC.ai[0] = 0;
                SoundEngine.PlaySound(SoundID.Item16, NPC.Center);

                if (owner.GetLifePercent() < 0.55f)
                {
                    NPC.ai[2] = 1;
                    NPC.velocity = toplayer * 7;
                }
                NPC.netUpdate = true;
            }
            if (NPC.ai[0] % 100 == 0)
            {
                if (DLCUtils.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toplayer * 10, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.MushBomb>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0, Main.myPlayer);
            }
            Player player = Main.player[NPC.target];
            if (NPC.ai[2] == 1)
            {
                NPC.ai[3]++;
                if (NPC.ai[3] % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
                }
                if (NPC.ai[3] >= 60)
                {
                    NPC.ai[2] = 0;
                    NPC.ai[3] = 0;
                    NPC.netUpdate = true;
                }

            }
            if (NPC.ai[0] == 150)// && owner.GetLifePercent() < 0.25f)
            {
                if (DLCUtils.HostCheck)
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, toplayer.RotatedBy(MathHelper.ToRadians(Main.rand.Next(-20, 20))) * 4, ModContent.ProjectileType<FungusBall>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(NPC.defDamage), 0);
            }
        }
    }
}
