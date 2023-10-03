using CalamityMod.NPCs.Perforator;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
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
    public class MediumPerforator : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<PerforatorHeadMedium>(),
            ModContent.NPCType<PerforatorBodyMedium>(),
            ModContent.NPCType<PerforatorTailMedium>()
        );
        public override void SetDefaults(NPC entity)
        {
            if (!WorldSavingSystem.EternityMode) return;
            entity.lifeMax = 50;
        }
        public override void SpawnNPC(int npc, int tileX, int tileY)
        {
            base.SpawnNPC(npc, tileX, tileY);
        }

        public override void OnKill(NPC npc)
        {
            if (!WorldSavingSystem.EternityMode) return;

            if (DLCUtils.HostCheck)
                Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f)), ModContent.ProjectileType<IchorBlob>(), FargoSoulsUtil.ScaledProjectileDamage(npc.damage), 0);
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
        public override bool SafePreAI(NPC npc)
        {
            npc.netUpdate = true; //fuck you worm mp code
            return base.SafePreAI(npc);
        }
    }
}
