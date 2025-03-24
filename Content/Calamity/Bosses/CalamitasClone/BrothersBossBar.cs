using CalamityMod.NPCs;
using CalamityMod.NPCs.CalClone;
using FargowiltasCrossmod.Core.Calamity;
using FargowiltasSouls;
using FargowiltasSouls.Content.Bosses.AbomBoss;
using FargowiltasSouls.Content.Bosses.DeviBoss;
using FargowiltasSouls.Content.Bosses.MutantBoss;
using FargowiltasSouls.Core.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Drawing;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.CalamitasClone
{
    public class BrothersBossBar : ModBossBar
    {
        private int bossHeadIndex = -1;
        private float maxHP = 0;
        public override Asset<Texture2D> GetIconTexture(ref Microsoft.Xna.Framework.Rectangle? iconFrame)
        {
            if (bossHeadIndex != -1)
            {
                return TextureAssets.NpcHeadBoss[bossHeadIndex];
            }
            return null;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, NPC npc, ref BossBarDrawParams drawParams)
        {
            return true;
        }

        public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax)
        {
            NPC npc = Main.npc[info.npcIndexToAimAt];
            if (npc.townNPC || !npc.active)
                return false;

            life = npc.life;
            lifeMax = npc.lifeMax;

            var otherNPC = FargoSoulsUtil.NPCExists(CalamityGlobalNPC.cataclysm, ModContent.NPCType<Cataclysm>());
            if (otherNPC != null)
            {
                life += otherNPC.life;
                lifeMax += otherNPC.lifeMax;
            }
            otherNPC = FargoSoulsUtil.NPCExists(CalamityGlobalNPC.catastrophe, ModContent.NPCType<Catastrophe>());
            if (otherNPC != null)
            {
                life += otherNPC.life;
                lifeMax += otherNPC.lifeMax;
            }

            if (maxHP > lifeMax)
                lifeMax = maxHP;
            else
                maxHP = lifeMax;

            bossHeadIndex = npc.GetBossHeadTextureIndex();
            return true;
        }
    }
}
