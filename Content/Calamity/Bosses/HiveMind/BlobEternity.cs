using CalamityMod.NPCs.HiveMind;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ID;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Core.Systems;
using FargowiltasCrossmod.Core;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class BlobEternity : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == ModContent.NPCType<HiveBlob>() || entity.type == ModContent.NPCType<HiveBlob2>();
        }
        public override bool InstancePerEntity => true;

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            base.HitEffect(npc, hit);
        }
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
            
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
            if (owner != null && owner.active && owner.GetLifePercent() <= 0.9f)
            {
                maxRadians = MathHelper.TwoPi;
            }
            npc.ai[1] = Main.rand.NextFloat(0f, maxRadians);
        }
        public override void OnKill(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC owner = Main.npc[(int)npc.ai[0]];
                if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.HiveMind.HiveMind>()) return;
                int range = 20;
                if (owner.GetLifePercent() <= 0.9f) range = 40;
                int amount = 0;
                if (owner.GetGlobalNPC<HMEternity>().phase < 2) amount = 2;
                else if (Main.rand.NextBool()) amount = 1;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    for (int i = 0; i < amount; i++)
                        Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, new Vector2(0, -10).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-range, range))), ModContent.ProjectileType<OldDukeGore>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            Asset<Texture2D> t = TextureAssets.Chains[3];
            
            NPC owner = Main.npc[(int)npc.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                return true;
            }
            Vector2 pos = npc.Center + ((owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * 20);
            while (pos.Distance(owner.Center) > 20)
            {
                Main.EntitySpriteDraw(t.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), pos.AngleTo(owner.Center) + MathHelper.Pi/2, t.Size() / 2, npc.scale, SpriteEffects.None);
                pos += (owner.Center - pos).SafeNormalize(Vector2.Zero) * 30;
            }
            return true;
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            base.FindFrame(npc, frameHeight);
        }
        public override bool PreAI(NPC npc)
        {
            if (!FargowiltasSouls.Core.Systems.WorldSavingSystem.EternityMode) return true;

            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.velocity.Y += 1;
                return false;
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
            npc.velocity = Vector2.Lerp(npc.velocity, (owner.Center + new Vector2(-180, 0).RotatedBy(npc.ai[1]) - npc.Center).SafeNormalize(Vector2.Zero) * 10, 0.05f);
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
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero) * 7, ModContent.ProjectileType<CalamityMod.Projectiles.Boss.VileClot>(), FargowiltasSouls.FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
                }
            }

            return false;

        }
    }
}
