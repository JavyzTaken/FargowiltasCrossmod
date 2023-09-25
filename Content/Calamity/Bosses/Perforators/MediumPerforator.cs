using CalamityMod.NPCs.Perforator;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasSouls;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.Perforators
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class MediumPerforator : GlobalNPC
    {
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            if (new List<int>() { ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorBodyMedium>(),
            ModContent.NPCType<PerforatorTailMedium>()}.Contains(entity.type))
            {
                entity.lifeMax = 25;
            }
        }
        public override void SpawnNPC(int npc, int tileX, int tileY)
        {
            base.SpawnNPC(npc, tileX, tileY);
        }

        public override void OnKill(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return;
            if (new List<int>() { ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorBodyMedium>(),
            ModContent.NPCType<PerforatorTailMedium>()}.Contains(npc.type))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)), ModContent.ProjectileType<IchorBlob>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
            }
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override bool PreAI(NPC npc)
        {
            return base.PreAI(npc);
        }
    }
}
