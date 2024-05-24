using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.NPCs.HiveMind;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity.Globals;
using FargowiltasCrossmod.Core.Common;
using FargowiltasSouls;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DankCreeperEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<DankCreeper>());
        public override bool InstancePerEntity => true;

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            base.HitEffect(npc, hit);
        }
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            base.SetDefaults(entity);
            entity.lifeMax *= 5;
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
            if (npc.ai[3] == 1)
                return;
            NPC owner = Main.npc[(int)npc.ai[0]];
            float maxRadians = MathHelper.Pi;
            if (owner.GetLifePercent() <= 0.9f)
            {
                maxRadians = MathHelper.TwoPi;
            }
            npc.ai[1] = Main.rand.NextFloat(0f, maxRadians);

        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
        }
        public override bool PreKill(NPC npc)
        {
            return false; // Prevent from spawning rainclouds on death
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            if (npc.ai[3] == 1)
                return true;
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
        public override bool SafePreAI(NPC npc)
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
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                npc.StrikeInstantKill();
                return false;
            }
            NPC owner = Main.npc[hiveMind];
            if (npc.ai[3] == 1 || owner.ai[1] != 0)
            {
                if (npc.ai[3] != 1)
                    npc.velocity *= 0.2f;
                npc.ai[3] = 1;
                npc.velocity -= npc.DirectionTo(target.Center);
                if (npc.Distance(target.Center) > 1200)
                    npc.StrikeInstantKill();
                return false;
            }
            if (npc.ai[2] == 0)
            {
                npc.ai[2] = Main.rand.Next(80, 90);
            }
            npc.ai[2]--;
            if (owner.GetGlobalNPC<HMEternity>().Phase >= 2 && npc.ai[2] == 1 && DLCUtils.HostCheck)
            {
                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, (target.Center - npc.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(MathF.PI * 0.4f) * -3, ModContent.ProjectileType<HMShadeNimbus>(), FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
                npc.ai[2] = 100;
            }
            npc.position += owner.velocity;
            Vector2 desiredPos = owner.Center + owner.DirectionTo(target.Center).RotatedByRandom(MathF.PI * 0.4f) * 120;
            npc.velocity = Vector2.Lerp(npc.velocity, npc.DirectionTo(desiredPos) * 10, 0.1f);
            for (int i = 0; i < Main.maxNPCs; i++) // force from colliding other creepers
            {
                NPC otherNPC = Main.npc[i];
                if (otherNPC.TypeAlive(npc.type) && otherNPC.Distance(npc.Center) < Math.Max(npc.width, npc.height))
                    npc.velocity -= 0.8f * npc.SafeDirectionTo(otherNPC.Center, Vector2.Zero);
            }
            float maxDif = MathF.PI * 0.3f;
            return false;
        }
        
    }
}
