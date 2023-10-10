using CalamityMod.NPCs.HiveMind;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Terraria.GameContent;
using CalamityMod.Projectiles.Boss;
using FargowiltasSouls.Core.Systems;
using FargowiltasSouls;
using Terraria.ID;
using FargowiltasCrossmod.Core;
using CalamityMod.Events;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasCrossmod.Core.Utils;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.HiveMind
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class DarkHeartEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<DarkHeart>());

        public override void HitEffect(NPC npc, NPC.HitInfo hit)
        {
            if (!WorldSavingSystem.EternityMode) return;
            if (npc.life <= 0 && NPC.AnyNPCs(ModContent.NPCType<CalamityMod.NPCs.HiveMind.HiveMind>())) npc.life = 1;
        }
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            base.SetDefaults(entity);
            entity.scale = 2.5f;
            entity.noTileCollide = true;
            entity.Opacity = 0;
            entity.knockBackResist = 0;
            if (BossRushEvent.BossRushActive)
            {
                entity.lifeMax = 5000000;
            }
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
            Vector2 pos = npc.Center + ((owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * 20);
            while (pos.Distance(owner.Center) > 20)
            {
                Main.EntitySpriteDraw(t.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()) * npc.Opacity, pos.AngleTo(owner.Center) + MathHelper.Pi / 2, t.Size() / 2, 1, SpriteEffects.None);
                pos += (owner.Center - pos).SafeNormalize(Vector2.Zero) * 30;
            }
            Vector2 off2 = new Vector2(45, 20);
            Vector2 pos2 = npc.Center + ((owner.Center - npc.Center + off2).SafeNormalize(Vector2.Zero) * 20);
            
            for (int i = 0; i < npc.Distance(owner.Center + off2)/30; i++)
            {
                Main.EntitySpriteDraw(t.Value, pos2 + ((owner.Center + off2 - pos2).SafeNormalize(Vector2.Zero) * 30 * i) - Main.screenPosition, null, Lighting.GetColor(pos2.ToTileCoordinates()) * npc.Opacity, pos2.AngleTo(owner.Center + off2) + MathHelper.Pi / 2, t.Size() / 2, 1, SpriteEffects.None);
            }
            Vector2 off3 = new Vector2(-45, 20);
            Vector2 pos3 = npc.Center + ((owner.Center - npc.Center + off3).SafeNormalize(Vector2.Zero) * 20);
            for (int i = 0; i < npc.Distance(owner.Center + off3) / 30; i++)
            {
                Main.EntitySpriteDraw(t.Value, pos3 + (owner.Center + off3 - pos3).SafeNormalize(Vector2.Zero) * 30*i - Main.screenPosition, null, Lighting.GetColor(pos3.ToTileCoordinates()) * npc.Opacity, pos3.AngleTo(owner.Center + off3) + MathHelper.Pi / 2, t.Size() / 2, 1, SpriteEffects.None);
               
            }
            return true;
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            base.FindFrame(npc, frameHeight);
        }
        public override bool SafePreAI(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
                NetSync(npc);
            }
            if (npc.target < 0 || Main.player[npc.target] == null || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.velocity.Y += 1;
                return false;
            }
            Player target = Main.player[npc.target];
            NPC owner = Main.npc[(int)npc.ai[0]];
            if (owner == null || !owner.active || owner.type != ModContent.NPCType <CalamityMod.NPCs.HiveMind. HiveMind>())
            {
                npc.realLife = -1;
                npc.StrikeInstantKill();
            };
            npc.realLife = (int)npc.ai[0];
            if (owner.ai[1] == 1)
            {
                HMEternity ownerAI = owner.GetGlobalNPC<HMEternity>();
                npc.dontTakeDamage = false;
                npc.velocity.Y += 0.2f;
                if (npc.Distance(owner.Center) >= 150)
                {
                    npc.velocity = Vector2.Lerp(npc.velocity, (owner.Center - npc.Center).SafeNormalize(Vector2.Zero)*20, 0.1f);
                }
                npc.Opacity += 0.1f;
                npc.ai[1]++;
                int ownerAttack = ownerAI.attackCycle[(int)owner.ai[2]];
                if (Main.rand.NextBool(30) && ownerAI.phase <= 3 && ownerAttack != 1) //not during accelerating dash
                {
                    if (DLCUtils.HostCheck)
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(0, -10).RotatedBy(MathHelper.ToRadians(Main.rand.Next(-30, 30))), ModContent.ProjectileType<OldDukeSummonDrop>(), FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0, ai0:1);
                }
                if (npc.ai[1] >= 60)
                {
                    npc.ai[1] = 0;
                    if (DLCUtils.HostCheck)
                        for (int i = 0; i < 6; i++)
                        {
                            Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, new Vector2(7, 0).RotatedBy(MathHelper.ToRadians(i * (360 / 6))), ModContent.ProjectileType<VileClot>(), FargoSoulsUtil.ScaledProjectileDamage(owner.damage), 0);
                        }
                    NetSync(npc);
                }
            }
            else
            {
                npc.Opacity -= 0.01f;
                npc.dontTakeDamage = true;
                npc.velocity = (owner.Center - npc.Center).SafeNormalize(Vector2.Zero) * 5;
            }
            return false;
        }
    }
}
