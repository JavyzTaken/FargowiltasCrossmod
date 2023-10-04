
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using FargowiltasCrossmod.Core;
using FargowiltasCrossmod.Core.Utils;
using FargowiltasSouls;
using FargowiltasSouls.Core.Globals;
using FargowiltasSouls.Core.NPCMatching;
using FargowiltasSouls.Core.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.SlimeGod
{
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class SlimeGodsEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchTypeRange(
            ModContent.NPCType<EbonianPaladin>(),
            ModContent.NPCType<CrimulanPaladin>()
        );
        //Slimes have much less health because of the phase and respawn mechanic
        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            npc.lifeMax /= 4;
        }
        //Slime the Core is attached to takes 50% damage
        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            NPC core = Main.npc[CalamityGlobalNPC.slimeGod];
            if (core != null && core.active && core.type == ModContent.NPCType<SlimeGodCore>())
            {
                if (core.TryGetGlobalNPC(out SlimeGodCoreEternity emodeCore) && emodeCore.AttachedSlime == npc.whoAmI)
                {
                    modifiers.FinalDamage /= 2;
                }
            }
        }
        //Slime the Core is attached to draws a glow aura
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPC core = Main.npc[CalamityGlobalNPC.slimeGod];
            if (core != null && core.active && core.type == ModContent.NPCType<SlimeGodCore>())
            {
                if (core.TryGetGlobalNPC(out SlimeGodCoreEternity emodeCore) && emodeCore.AttachedSlime == npc.whoAmI)
                {
                    //draw glow aura
                    for (int j = 0; j < 12; j++)
                    {
                        Texture2D texture = TextureAssets.Npc[npc.type].Value;

                        SpriteEffects effects = 0;
                        if (npc.spriteDirection == 1)
                        {
                            effects = (SpriteEffects)1;
                        }

                        Vector2 afterimageOffset = (MathHelper.TwoPi * j / 12f).ToRotationVector2() * 6f;
                        Color color = npc.type == ModContent.NPCType<EbonianPaladin>() ? Color.Purple : Color.Red;
                        Color glowColor = color with { A = 0 } * 0.7f;

                        Vector2 wackySlimeOffsetY = -Vector2.UnitY * npc.height * 0.125f;
                        Vector2 offset = afterimageOffset + wackySlimeOffsetY - screenPos + new Vector2(0f, npc.gfxOffY);

                        Main.EntitySpriteDraw(texture, npc.Center + offset, npc.frame, glowColor, npc.rotation, Utils.Size(npc.frame) / 2f, npc.scale, effects);
                    }
                }
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }
    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class EbonianPaladinEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<EbonianPaladin>());
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override bool SafePreAI(NPC npc)
        {
            return true;
            if (!WorldSavingSystem.EternityMode) return true;
            #region Passives and Variables
            CalamityGlobalNPC.slimeGodPurple = npc.whoAmI;
            NPC core = Main.npc[CalamityGlobalNPC.slimeGod];
            #endregion
            return false;
        }

    }
    [JITWhenModsEnabled(ModCompatibility.Calamity.Name)]
    [ExtendsFromMod(ModCompatibility.Calamity.Name)]
    public class CrimuleanPaladinEternity : EModeCalBehaviour
    {
        public override NPCMatcher CreateMatcher() => new NPCMatcher().MatchType(ModContent.NPCType<CrimulanPaladin>());
        public override void SetDefaults(NPC entity)
        {
            base.SetDefaults(entity);
        }
        public override bool SafePreAI(NPC npc)
        {
            return true;
            if (!WorldSavingSystem.EternityMode) return true;
            #region Passives and Variables
            CalamityGlobalNPC.slimeGodRed = npc.whoAmI;
            NPC core = Main.npc[CalamityGlobalNPC.slimeGod];
            #endregion
            return false;
        }
    }
}
