using CalamityMod.NPCs.HiveMind;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ID;
using FargowiltasSouls;
using FargowiltasSouls.Core.Systems;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DankCreeperEternity : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<DankCreeper>();
        }
        public override bool InstancePerEntity => true;

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            base.HitEffect(npc, hit);
        }
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            base.SetDefaults(entity);
            entity.lifeMax = 250;
        }
        public override void ApplyDifficultyAndPlayerScaling(NPC npc, int numPlayers, float balance, float bossAdjustment)
        {
            base.ApplyDifficultyAndPlayerScaling(npc, numPlayers, balance, bossAdjustment);
        }
        public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo)
        {

        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (!WorldSavingSystem.EternityMode) return;
            NPC owner = Main.npc[(int)npc.ai[0]];
            float maxRadians = MathHelper.Pi;
            if (owner.GetLifePercent() <= 0.9f)
            {
                maxRadians = MathHelper.TwoPi;
            }
            npc.ai[1] = Main.rand.NextFloat(0f, maxRadians);

        }
        public override void OnKill(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return;
            Player target = Main.player[npc.target];
            NPC owner = Main.npc[(int)npc.ai[0]];
            if (owner != null && owner.active && Main.netMode != NetmodeID.MultiplayerClient && owner.GetGlobalNPC<HMEternity>().phase < 2)
            Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, (target.Center + new Vector2(0, -400) - npc.Center) / 60, ModContent.ProjectileType<MovingCorruptCloud>(), FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            Asset<Texture2D> t = TextureAssets.Chain10;

            NPC owner = Main.npc[(int)npc.ai[0]];
            if (owner == null || !owner.active)
            {
                return true;
            }
            Vector2 pos = npc.Center + ((owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * 20);
            while (pos.Distance(owner.Center) > 20)
            {
                Main.EntitySpriteDraw(t.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), pos.AngleTo(owner.Center) + MathHelper.Pi / 2, t.Size() / 2, npc.scale, SpriteEffects.None);
                pos += (owner.Center - pos).SafeNormalize(Vector2.Zero) * 24;
            }
            return true;
        }
        
        public override void FindFrame(NPC npc, int frameHeight)
        {
            base.FindFrame(npc, frameHeight);
        }
        public override bool PreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode)
            {
                return true;
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.velocity.Y += 1;
                return false;
            }
            Player target = Main.player[npc.target];
            NPC owner = Main.npc[(int)npc.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                npc.StrikeInstantKill();
                return false;
            }
            if (owner.GetGlobalNPC<HMEternity>().phase >= 2 && Main.rand.NextBool(300) && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 7, ProjectileID.CultistBossFireBallClone, FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
            }
            npc.velocity = Vector2.Lerp(npc.velocity, (owner.Center + new Vector2(-120, 0).RotatedBy(npc.ai[1]) - npc.Center).SafeNormalize(Vector2.Zero) * 10, 0.05f);
            npc.ai[2]++;
            float maxRadians = MathHelper.Pi;
            if (owner.GetLifePercent() <= 0.9f)
            {
                maxRadians = MathHelper.TwoPi;
            }
            if (npc.ai[2] == 120)
            {
                npc.ai[2] = 0;
                npc.ai[1] = Main.rand.NextFloat(0f, maxRadians);
            }
            return false;
        }
    }
}
