using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FargowiltasSouls;
using Terraria.DataStructures;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofDesolation
{
    [JITWhenModsEnabled("CalamityMod")]
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class BabyColossalSquid : ModNPC
    {
        public override string Texture => "CalamityMod/Projectiles/Summon/CalamariMinion";
        public override void SetDefaults()
        {

            NPC.lifeMax = 5000;
            NPC.width = 62;
            NPC.height = 66;
            NPC.damage = 110;
            Main.npcFrameCount[Type] = 5;
            NPC.frame.Height = 66;
            NPC.frame.Width = 62;
            NPC.scale = 1.5f;
            NPC.aiStyle = -1;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath28;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 15; i++)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<Tentacle>(), FargoSoulsUtil.ScaledProjectileDamage(210), 0, Main.myPlayer, NPC.whoAmI, MathHelper.ToRadians(i * 24));
            }
            base.OnSpawn(source);
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModLoader.GetMod("CalamityMod").Find<ModGore>("ColossalSquid").Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModLoader.GetMod("CalamityMod").Find<ModGore>("ColossalSquid" + Main.rand.Next(2, 5)).Type);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModLoader.GetMod("CalamityMod").Find<ModGore>("ColossalSquid" + Main.rand.Next(2, 5)).Type);
            }
        }
        public override void OnKill()
        {
            base.OnKill();
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 10)
            {
                NPC.frame.Y += NPC.frame.Height;
                if (NPC.frame.Y >= NPC.frame.Height * 5)
                {
                    NPC.frame.Y = 0;
                }
                NPC.frameCounter = 0;
            }
            base.FindFrame(frameHeight);
        }
        public override void AI()
        {
            NPC.TargetClosest();
            if (NPC.localAI[1] == 0)
            {
                NPC.localAI[1] = 0.1f;
            }
            NPC.ai[0] += NPC.localAI[1];
            NPC.position.Y += NPC.ai[0];
            if (NPC.ai[0] > 1)
            {
                NPC.localAI[1] = -0.05f;
            }
            if (NPC.ai[0] < -1)
            {
                NPC.localAI[1] = 0.05f;
            }
            NPC owner = Main.npc[(int)NPC.ai[1]];
            if (owner != null && owner.active && NPC.Distance(owner.Center) > 100)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, (owner.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, 0.03f);
            }
        }
    }
}
