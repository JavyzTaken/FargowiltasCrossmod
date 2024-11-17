using CalamityMod;
using CalamityMod.NPCs;
using CalamityMod.NPCs.HiveMind;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Calamity;
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
    public class DankCreeperEternity : CalDLCEmodeBehavior
    {
        public override int NPCOverrideID => ModContent.NPCType<DankCreeper>();

        public override void SetDefaults()
        {
            if (!WorldSavingSystem.EternityMode) return;
            NPC.lifeMax *= 5;
        }
        public override void OnSpawn(IEntitySource source)
        {
            if (!WorldSavingSystem.EternityMode) return;
            if (NPC.ai[3] == 1)
                return;
            NPC owner = Main.npc[(int)NPC.ai[0]];
            float maxRadians = MathHelper.Pi;
            if (owner.GetLifePercent() <= 0.9f)
            {
                maxRadians = MathHelper.TwoPi;
            }
            NPC.ai[1] = Main.rand.NextFloat(0f, maxRadians);

        }
        public override bool PreKill()
        {
            return false; // Prevent from spawning rainclouds on death
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!WorldSavingSystem.EternityMode) return true;
            if (NPC.ai[3] == 1)
                return true;
            Asset<Texture2D> t = TextureAssets.Chain10;

            NPC owner = Main.npc[(int)NPC.ai[0]];
            if (owner == null || !owner.active)
            {
                return true;
            }
            Vector2 pos = NPC.Center + ((owner.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 20);
            while (pos.Distance(owner.Center) > 20)
            {
                Main.EntitySpriteDraw(t.Value, pos - Main.screenPosition, null, Lighting.GetColor(pos.ToTileCoordinates()), pos.AngleTo(owner.Center) + MathHelper.Pi / 2, t.Size() / 2, NPC.scale, SpriteEffects.None);
                pos += (owner.Center - pos).SafeNormalize(Vector2.Zero) * 24;
            }
            return true;
        }

        public override bool PreAI()
        {
            if (!WorldSavingSystem.EternityMode)
            {
                return true;
            }

            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            if (NPC.target < 0 || Main.player[NPC.target] == null || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.velocity.Y += 1;
                return false;
            }
            Player target = Main.player[NPC.target];
            int hiveMind = CalamityGlobalNPC.hiveMind;
            if (hiveMind < 0 || !Main.npc[hiveMind].TypeAlive<CalamityMod.NPCs.HiveMind.HiveMind>())
            {
                NPC.StrikeInstantKill();
                return false;
            }
            NPC owner = Main.npc[hiveMind];
            if (NPC.ai[3] == 1 || owner.ai[1] != 0)
            {
                if (NPC.ai[3] != 1)
                    NPC.velocity *= 0.2f;
                NPC.ai[3] = 1;
                NPC.velocity -= NPC.DirectionTo(target.Center);
                if (NPC.Distance(target.Center) > 1200)
                    NPC.StrikeInstantKill();
                return false;
            }
            if (NPC.ai[2] == 0)
            {
                NPC.ai[2] = Main.rand.Next(80, 90);
            }
            NPC.ai[2]--;
            if (owner.GetDLCBehavior<HMEternity>().Phase >= 2 && NPC.ai[2] == 1 && DLCUtils.HostCheck)
            {
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).SafeNormalize(Vector2.Zero).RotatedByRandom(MathF.PI * 0.4f) * -3, ModContent.ProjectileType<BrainMassProjectile>(), FargoSoulsUtil.ScaledProjectileDamage(owner.defDamage), 0);
                NPC.ai[2] = 100;
            }
            NPC.position += owner.velocity;
            Vector2 desiredPos = owner.Center + owner.DirectionTo(target.Center).RotatedByRandom(MathF.PI * 0.4f) * 120;
            NPC.velocity = FargoSoulsUtil.SmartAccel(NPC.Center, desiredPos, NPC.velocity, 4f, 4f);
            //NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(desiredPos) * 10, 0.1f);
            for (int i = 0; i < Main.maxNPCs; i++) // force from colliding other creepers
            {
                NPC otherNPC = Main.npc[i];
                if (otherNPC.TypeAlive(NPC.type) && otherNPC.Distance(NPC.Center) < Math.Max(NPC.width, NPC.height))
                    NPC.velocity -= 0.8f * NPC.SafeDirectionTo(otherNPC.Center, Vector2.Zero);
            }
            float maxDif = MathF.PI * 0.3f;
            return false;
        }
        
    }
}
