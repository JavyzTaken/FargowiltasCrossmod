using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using FargowiltasSouls;

namespace FargowiltasCrossmod.Content.Calamity.NPCS.Bosses.ChampionofExploration
{
    
    [ExtendsFromMod(ModCompatibility.Calamity.Name)] [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    public class ExplorationDrone : ModNPC
    {
        public override string Texture => "CalamityMod/NPCs/NormalNPCs/WulfrumDrone";
        public override void SetStaticDefaults()
        {
            NPCDebuffImmunityData debuffdata = new NPCDebuffImmunityData
            {
                SpecificallyImmuneTo = new int[]
                {
                    BuffID.Confused
                }
            };
            NPCID.Sets.DebuffImmunitySets.Add(Type, debuffdata);
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 30;
            NPC.lifeMax = 5000;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = CommonCalamitySounds.WulfrumNPCDeathSound;
            Main.npcFrameCount[Type] = 6;
            NPC.frame.Width = 32;
            NPC.frame.Height = 204 / 6;
            NPC.frame.Y = 3;
            NPC.noGravity = true;
            NPC.noTileCollide = true;

        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 5)
            {
                NPC.frame.Y += NPC.frame.Height;
                NPC.frameCounter = 0;
                if (NPC.frame.Y >= NPC.frame.Height * 6)
                {
                    NPC.frame.Y = NPC.frame.Height * 3;
                }
            }
        }
        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && !Main.dedServ)
            {
                Mod cal = ModLoader.GetMod("CalamityMod");
                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center, NPC.velocity, cal.Find<ModGore>("WulfrumDroneGore1").Type);
                Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center, NPC.velocity, cal.Find<ModGore>("WulfrumDroneGore2").Type);
                int amount = Main.rand.Next(1, 4);
                for (int i = 0; i < amount; i++)
                {
                    Gore.NewGoreDirect(NPC.GetSource_Death(), NPC.Center, NPC.velocity, cal.Find<ModGore>("WulfrumEnemyGore" + Main.rand.Next(1, 11)).Type);
                }
            }
        }
        public override void OnKill()
        {

        }
        public override void AI()
        {
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            if (NPC.ai[1] == 0)
            {
                NPC.ai[0] = MathHelper.ToRadians(Main.rand.Next(0, 360));
            }
            NPC.velocity = Vector2.Lerp(NPC.velocity, (player.Center + new Vector2(0, 300).RotatedBy(NPC.ai[0]) - NPC.Center).SafeNormalize(Vector2.Zero) * 30, 0.05f);
            NPC.ai[1]++;
            if (NPC.ai[1] == 200)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 10, ProjectileID.SaucerLaser, FargoSoulsUtil.ScaledProjectileDamage(210), 0);
                }
                NPC.ai[1] = 1;
                SoundEngine.PlaySound(SoundID.Item12, NPC.Center);
            }
            if (player.Center.X < NPC.Center.X)
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
